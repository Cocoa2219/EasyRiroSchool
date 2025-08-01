﻿using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasyRiroSchool.API.Exceptions;
using EasyRiroSchool.Models;
using EasyRiroSchool.Models.Authentication;
using EasyRiroSchool.Models.Deserialization;
using EasyRiroSchool.Models.Deserialization.Items;
using HtmlAgilityPack;

namespace EasyRiroSchool.API;

/// <summary>
/// Represents the Riro School API client.
/// </summary>
public class RiroSchool
{
    static RiroSchool()
    {
        var riroItemTypes = typeof(RiroSchool).Assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(RiroItem)));

        foreach (var type in riroItemTypes)
        {
            var attribute = type.GetCustomAttribute<RiroItemAttribute>();
            if (attribute != null)
            {
                _itemTypes[type] = attribute;
            }
        }
    }

    public RiroSchool()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://kdd.riroschool.kr/")
        };
        Token = string.Empty;
    }

    private static readonly Dictionary<Type, RiroItemAttribute> _itemTypes = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
    };

    public string Token { get; private set; }

    private readonly HttpClient _client;

    /// <summary>
    /// Logs in to the Riro School API using the provided ID and password.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <exception cref="RiroLoginException">Thrown when the login fails.</exception>
    public async Task LoginAsync(string id, string password)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "ajax.php");
        message.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        message.Headers.Add("Origin", "https://kdd.riroschool.kr");
        message.Headers.Add("Referer", "https://kdd.riroschool.kr/user.php?action=signin");
        message.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "app", "user" },
            { "userType", "1" },
            { "mode", "login" },
            { "id", id },
            { "pw", password },
            { "deeplink", "" },
            { "redirect_link", "" }
        });
        message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
        {
            CharSet = "UTF-8"
        };

        var response = await _client.SendAsync(message);

        if (!response.IsSuccessStatusCode)
            throw new RiroLoginException(response.ReasonPhrase ??
                                         "Login request failed with code " + response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, _jsonOptions);

        if (loginResponse == null) throw new RiroLoginException("Login response is null.");

        if (loginResponse.Code != "000") throw new RiroLoginException(loginResponse.Message);

        Token = loginResponse.Token;
    }

    /// <summary>
    /// Retrieves a table of items from the Riro School API based on the provided DbInfo.
    /// </summary>
    /// <param name="info">The <see cref="DbInfo"/> containing the database ID and category.</param>
    /// <typeparam name="T">The type of items to retrieve, which must inherit from RiroItem.</typeparam>
    /// <returns>A <see cref="RiroTableList{T}"/> containing the items retrieved from the API.</returns>
    /// <exception cref="RiroApiException">Thrown when the API request fails or the response is invalid.</exception>
    public async Task<RiroTableList<T>> GetTableAsync<T>(DbInfo info) where T : RiroItem, new()
    {
        if (string.IsNullOrEmpty(Token))
            throw new RiroApiException("You must login before accessing the API.");

        if (!_itemTypes.TryGetValue(typeof(T), out var itemAttribute))
            throw new RiroApiException($"Type {typeof(T).Name} is not registered in the API.");

        var path = itemAttribute.Path;

        var list = new RiroTableList<T>();
        var iterations = info.Count / 20 + (info.Count % 20 > 0 ? 1 : 0);

        for (var i = 0; i < iterations; i++)
        {
            var message = new HttpRequestMessage(HttpMethod.Get,
                $"{path}.php?db=" + (int)info.Id + "&cate=" + info.Category + "&t_doc=" + info.Category + $"&page={i + 1}");
            message.Headers.Add("Cookie", "cookie_token=" + Token);

            Console.WriteLine($"Requesting table: {path}.php?db={(int)info.Id}&cate={info.Category}&t_doc={info.Category}");

            var response = await _client.SendAsync(message);
            if (!response.IsSuccessStatusCode)
                throw new RiroApiException("Failed to retrieve table: " + response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(content);

            var rdBoard = document.DocumentNode.SelectSingleNode("//div[@class='rd_board']");
            var table = rdBoard?.SelectSingleNode(".//table")
                        ?? throw new RiroApiException("Table not found in the response.");

            var rows = table.SelectNodes(".//tr")
                       ?? throw new RiroApiException("No data found in the table.");

            if (rows.Count < 2)
                throw new RiroApiException("Table must contain header and at least one data row.");

            list.Append(rows.Skip(1));
        }

        if (list.Count == 0)
            throw new RiroApiException("No data found.");

        return list;
    }

    public async Task<RiroCalendar> GetCalendarAsync(int year, int month)
    {
        if (string.IsNullOrEmpty(Token))
            throw new RiroApiException("You must login before accessing the API.");

        var message = new HttpRequestMessage(HttpMethod.Get,
            $"school_schedule.php?db=2301&year={year}&month={month}");
        message.Headers.Add("Cookie", "cookie_token=" + Token);

        var response = await _client.SendAsync(message);
        if (!response.IsSuccessStatusCode)
            throw new RiroApiException("Failed to retrieve calendar: " + response.ReasonPhrase);

        var content = await response.Content.ReadAsStringAsync();
        var document = new HtmlDocument();
        document.LoadHtml(content);

        var table = document.DocumentNode.SelectSingleNode("//table[@class='calendar_table']")
                     ?? throw new RiroApiException("Calendar table not found in the response.");

        var rows = table.SelectNodes(".//tr");

        if (rows == null || rows.Count < 2)
            throw new RiroApiException("Calendar table must contain header and at least one data row.");

        var calendar = new RiroCalendar(rows.Skip(1));
        if (calendar.Count == 0)
            throw new RiroApiException("No calendar data found.");

        return calendar;
    }

    // public async Task<RiroTableList<BoardItem>> GetBoardMessageAsync(DbInfo info)
    // {
    //     var message = new HttpRequestMessage(HttpMethod.Get, "board.php?db=" + (int)info.Id + "&cate=" + info.Category);
    //     message.Headers.Add("Cookie", "cookie_token=" + Token);
    //
    //     var response = await _client.SendAsync(message);
    //
    //     if (!response.IsSuccessStatusCode)
    //         throw new RiroApiException("Failed to retrieve board messages: " + response.ReasonPhrase);
    //
    //     var content = await response.Content.ReadAsStringAsync();
    //     var document = new HtmlDocument();
    //     document.LoadHtml(content);
    //
    //     var rdBoard = document.DocumentNode.SelectSingleNode("//div[@class='rd_board']");
    //     var table = rdBoard?.SelectSingleNode(".//table")
    //                 ?? throw new RiroApiException("Portfolio table not found in the response.");
    //
    //     var rows = table.SelectNodes(".//tr")
    //                ?? throw new RiroApiException("No data found in the portfolio table.");
    //
    //     if (rows.Count < 2)
    //         throw new RiroApiException("Portfolio table must contain header and at least one data row.");
    //
    //     var boardList = new RiroTableList<BoardItem>(
    //         rows
    //     );
    //
    //     if (boardList.Count == 0)
    //         throw new RiroApiException("No board messages found.");
    //
    //     return boardList;
    // }
}
using EasyRiroSchool.API;
using EasyRiroSchool.API.Exceptions;
using EasyRiroSchool.API.Models.Deserialization;
using EasyRiroSchool.Models;
using EasyRiroSchool.Models.Deserialization;

namespace EasyRiroSchool.Test;

public class LoginTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task LoginTest()
    {
        var riroschool = new RiroSchool();

        try
        {
            await riroschool.LoginAsync("cocoa.2324a@gmail.com", "cocoa2219!");

            Assert.That(riroschool.Token, Is.Not.Empty, "Token should not be empty after login.");
            Console.WriteLine($"Login successful. Token: {riroschool.Token}");
        }
        catch (Exception e)
        {
            Assert.Fail("Login failed: " + e.Message);
        }
    }

    [Test]
    public async Task LoginTestWithInvalidCredentials()
    {
        var riroschool = new RiroSchool();

        try
        {
            await riroschool.LoginAsync("invalid_user", "invalid_password");
            Assert.Fail("Login should have failed with invalid credentials.");
        }
        catch (RiroLoginException e)
        {
            Assert.That(e.Message, Is.Not.Empty, "Exception message should not be empty.");
            Console.WriteLine($"Expected failure: {e.Message}");
        }
    }

    [Test]
    public async Task GetPortfolio()
    {
        var riroschool = new RiroSchool();

        try
        {
            await riroschool.LoginAsync("cocoa.2324a@gmail.com", "cocoa2219!");

            Assert.That(riroschool.Token, Is.Not.Empty, "Token should not be empty after login.");
            Console.WriteLine($"Login successful. Token: {riroschool.Token}");
        }
        catch (Exception e)
        {
            Assert.Fail("Login failed: " + e.Message);
        }

        try
        {
            var portfolioList = await riroschool.GetTableAsync<PortfolioItem>(new DbInfo(DbId.CurricularActivity));
            Assert.That(portfolioList, Is.Not.Null, "Portfolio should not be null.");

            foreach (var item in portfolioList)
            {
                Console.WriteLine(item.ToString());
            }
        }
        catch (RiroApiException e)
        {
            Assert.Fail("Failed to retrieve portfolio: " + e.Message);
        }
    }

    [Test]
    public async Task GetBoard()
    {
        var riroschool = new RiroSchool();

        try
        {
            await riroschool.LoginAsync("cocoa.2324a@gmail.com", "cocoa2219!");

            Assert.That(riroschool.Token, Is.Not.Empty, "Token should not be empty after login.");
            Console.WriteLine($"Login successful. Token: {riroschool.Token}");
        }
        catch (Exception e)
        {
            Assert.Fail("Login failed: " + e.Message);
        }

        try
        {
            var boardList = await riroschool.GetTableAsync<BoardItem>(new DbInfo(DbId.Announcement, count: 41));
            Assert.That(boardList, Is.Not.Null, "Board should not be null.");

            foreach (var item in boardList)
            {
                Console.WriteLine(item.ToString());
            }
        }
        catch (RiroApiException e)
        {
            Assert.Fail("Failed to retrieve board: " + e.Message);
        }
    }
}
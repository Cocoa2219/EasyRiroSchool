using System.Text.Json.Serialization;

namespace EasyRiroSchool.Models.Authentication;

internal class LoginResponse
{
    public LoginResponse()
    {
    }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("cid")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public DataObject Data { get; set; } = new DataObject();

    public class DataObject
    {
        public DataObject()
        {
        }

        [JsonPropertyName("lock")]
        public bool Locked { get; set; } = false;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("set_app")]
        public object[] SetApp { get; set; } = [];
    }
}
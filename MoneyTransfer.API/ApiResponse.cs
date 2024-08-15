using System.Text.Json.Serialization;

namespace MoneyTransfer.API
{
    public class ApiResponse
    {
        public string? StatusCode { get; set; }
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // Data null ise JSON'da gösterme
        public object? Data { get; set; }
        public bool IsSucceed { get; set; }
    }
}

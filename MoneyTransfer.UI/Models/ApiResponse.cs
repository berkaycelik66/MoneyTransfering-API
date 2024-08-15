namespace MoneyTransfer.UI.Models
{
    public class ApiResponse
    {
        public string? StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsSucceed { get; set; }
    }
}

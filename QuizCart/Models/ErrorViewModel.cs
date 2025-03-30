namespace QuizCart.Models.ViewModels
{
    public class ErrorViewModel
    {
        public List<string> Errors { get; set; } = new();

        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

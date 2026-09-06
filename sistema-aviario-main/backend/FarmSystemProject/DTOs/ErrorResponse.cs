namespace FarmSystemProject.DTOs;

public class ErrorResponse
{
    public DateTime Timestamp { get; set; }
    public int Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string Path { get; set; } = string.Empty;
}
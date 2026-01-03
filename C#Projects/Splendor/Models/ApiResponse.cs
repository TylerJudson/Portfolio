namespace Splendor.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int? ErrorCode { get; set; }

        public static ApiResponse<T> Ok(T data) => new ApiResponse<T>
        {
            Success = true,
            Data = data
        };

        public static ApiResponse<T> Fail(string message, int? code = null) => new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = message,
            ErrorCode = code
        };
    }
}

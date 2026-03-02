namespace DNAAnalysis.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;
        }

        public ApiResponse(IEnumerable<string> errors, string? message = null)
        {
            Success = false;
            Message = message;
            Errors = errors;
            Data = default;
        }
    }
}
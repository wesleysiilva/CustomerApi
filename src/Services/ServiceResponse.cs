namespace CustomerApi.Services
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "Success";
        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T data) => new ServiceResponse<T> { IsSuccess = true, Data = data, Message = "Success" };
        public static ServiceResponse<T> Fail(string message) => new ServiceResponse<T> { IsSuccess = false, Message = message };
    }
}

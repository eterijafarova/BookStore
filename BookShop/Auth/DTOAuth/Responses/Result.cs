namespace BookShop.Auth.DTOAuth.Responses
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }  // Заменено init на set
        public T? Data { get; set; }  // Заменено init на set
        public string? Message { get; set; }  // Заменено init на set

        public Result(bool isSuccess, T? data, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }

        // Статический метод для успешного результата
        public static Result<T> Success(T data, string? message = null) => new Result<T>(true, data, message);

        // Статический метод для ошибки
        public static Result<T> Error(T? data, string message) => new Result<T>(false, data, message);
    }
}
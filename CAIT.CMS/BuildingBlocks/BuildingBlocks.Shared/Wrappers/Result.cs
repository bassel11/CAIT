namespace BuildingBlocks.Shared.Wrappers
{
    public class Result<T>
    {
        public bool Succeeded { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T> { Succeeded = true, Data = data, Message = message };
        }
    }

    public class Result : Result<object>
    {
        public static Result Success(string message)
        {
            return new Result { Succeeded = true, Data = null, Message = message };
        }
    }
}

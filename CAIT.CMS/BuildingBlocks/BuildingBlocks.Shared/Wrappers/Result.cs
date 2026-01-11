namespace BuildingBlocks.Shared.Wrappers
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }

        public Result() { }

        protected Result(bool succeeded, string message, List<string>? errors, T? data)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
            Data = data;
        }

        public static Result<T> Success(T data, string message = "Operation Succeeded")
        {
            return new Result<T>(true, message, null, data);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, "Operation Failed", new List<string> { error }, default);
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, "Operation Failed", errors, default);
        }
    }

    public class Result : Result<object>
    {
        public Result() { }
        public static new Result Success(string message = "Operation Succeeded")
        {
            return new Result { Succeeded = true, Message = message, Data = null, Errors = null };
        }

        public static new Result Failure(string error)
        {
            return new Result { Succeeded = false, Message = "Operation Failed", Errors = new List<string> { error }, Data = null };
        }

        public static new Result Failure(List<string> errors)
        {
            return new Result { Succeeded = false, Message = "Operation Failed", Errors = errors, Data = null };
        }
    }
}
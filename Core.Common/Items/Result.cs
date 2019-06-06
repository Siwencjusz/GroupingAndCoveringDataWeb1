using System;

namespace Core.Common.Items
{
    public class Result<T>
    {
        public T Value { get; }
        public string Error { get; }

        public Result(T value)
        {
            Value = value;
        }
        public Result(string message)
        {
            Error = message;
        }
        public Result(Exception ex)
        {
            Error =  ex.Message;
        }

        public bool HasErrors()
        {
            return !string.IsNullOrEmpty(Error);
        }
        public bool HasValue()
        {
            return Value != null;
        }
        public void GetError()
        {
            throw new Exception(Error);
        }
    }

}


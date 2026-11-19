using System;
using System.Net;

namespace Example.BuildingBlocks.Exceptions
{
    public class BusinessException : Exception
    {
        public int Status { get; }

        public BusinessException(string message)
            : base(message) { }

        public BusinessException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            Status = (int)statusCode;
        }

        public BusinessException(string message, int statusCode)
            : base(message)
        {
            Status = statusCode;
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

namespace Diarios.Api.Util.CustomException
{
    public class DiarioCustomException : Exception
    {
        public int httpStatusCode = 500;

        public DiarioCustomException()
        { }

        public DiarioCustomException(string message) : base(message)
        { }

        public DiarioCustomException(string message, Exception? innerException) : base(message, innerException)
        { }

        public DiarioCustomException(int statusCode)
        {
            this.httpStatusCode = statusCode;
        }

        public DiarioCustomException(int statusCode, string message) : base(message)
        {
            this.httpStatusCode = statusCode;
        }

        public DiarioCustomException(int statusCode, string message, Exception? innerException) : base(message, innerException)
        {
            this.httpStatusCode = statusCode;
        }
    }
}

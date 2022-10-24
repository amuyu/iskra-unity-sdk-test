namespace Iskra
{
    public class Result
    {
        public bool status;
        public string data;
        public Error error;

        public Result(bool status, string data, Error error)
        {
            this.status = status;
            this.data = data;
            this.error = error;
        }

        public static Result EmptyData(bool status) => new Result(status, null, null);

        public static Result Error(Error error) => new Result(false, null, error);

    }
}
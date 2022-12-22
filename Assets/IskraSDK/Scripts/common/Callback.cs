namespace Iskra.Common
{
    public delegate void ErrorCallback(Error error);
    public delegate void DataErrorCallback<T>(T t, Error error);
}
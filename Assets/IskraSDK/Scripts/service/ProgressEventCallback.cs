namespace Iskra.Service
{
    public class ProgressEventCallback
    {
        public enum Type
        {
            OnStart,
            OnFinish,
            OnClose
        }

        public delegate void ProgressDelegate(Type type, Result result);
    }
}
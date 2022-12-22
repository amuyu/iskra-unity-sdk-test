using System.Runtime.CompilerServices;

namespace Iskra.Common
{
    public static class Logger
    {
        private static bool isDebug = false;

        public static void EnableLog()
        {
            isDebug = true;
        }

        public static void Debug(object message, object classObj,
            [CallerMemberName] string methodName = "")
        {
            if (isDebug == false)
            {
                return;
            }
            var logMessage = MakeLog(message, classObj, methodName);
            UnityEngine.Debug.Log(logMessage);
        }
        
        public static void Error(object message, object classObj,
            [CallerMemberName] string methodName = "")
        {
            var logMessage = MakeLog(message, classObj, methodName);
            UnityEngine.Debug.LogError(logMessage);
        }

        private static string MakeLog(object message, object classObj, string methodName)
        {
            return string.Format("[{0}::{1}] {2}", classObj.GetType().Name, methodName, message);
        }
    }
}
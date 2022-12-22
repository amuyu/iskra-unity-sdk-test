using System;

namespace Iskra
{
    [Serializable]
    public class Error
    {
        public string code;
        public string message;

        public override string ToString()
        {
            return code + ":" + message;
        }
    }
}
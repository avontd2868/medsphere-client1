using System;

namespace ClinSchd.Infrastructure
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class InputValidationException : Exception
    {
        public InputValidationException()
        {
        }

        public InputValidationException(string message) : base(message)
        {
        }

        public InputValidationException(string message, Exception inner) : base(message, inner)
        {
        }

#if !SILVERLIGHT
        protected InputValidationException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}

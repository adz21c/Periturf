using System;
using System.Runtime.Serialization;

namespace Periturf.Verify
{
    /// <summary>
    /// Indicates verification failed.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class VerificationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationFailedException"/> class.
        /// </summary>
        public VerificationFailedException() : base("Verification failed.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public VerificationFailedException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected VerificationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}

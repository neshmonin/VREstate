using System;
using System.Collections.Generic;
using System.Text;


namespace PayPal.Exception
{
    public class MissingCredentialException : System.Exception
    {
		/// <summary>
		/// Represents errors that occur during application execution
		/// </summary>
		public MissingCredentialException() : base()
		{}


		/// <summary>
		/// Represents errors that occur during application execution
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public MissingCredentialException(string message): base(message)
		{
		}


		/// <summary>
		/// Represents errors that occur during application execution
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		/// <param name="cause">The exception that is the cause of the current exception</param>
        public MissingCredentialException(string message, System.Exception cause)
            : base(message, cause)
		{
		}
	} // MissingCredentialException
}

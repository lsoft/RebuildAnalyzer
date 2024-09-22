using System;
using System.Collections.Generic;
using System.Text;

namespace RebuildAnalyzer
{
    /// <summary>
    /// Exception from repository analyzer;
    /// </summary>
    public sealed class RepositoryAnalyzeException : Exception
    {
        public RepositoryAnalyzeException(
            string message,
            Exception? innerException
            ) : base(message, innerException)
        {
        }

        public string GetResultErrorMessage()
        {
            if (InnerException is null)
            {
                return Message;
            }

            return GetResultErrorMessage(
                InnerException,
                "== " + Message
                );
        }

        public string GetResultErrorMessage(
            Exception excp,
            string previousMessages
            )
        {
            if (InnerException is null)
            {
                return previousMessages;
            }

            return GetResultErrorMessage(
                excp.InnerException,
                "--> " + previousMessages + Environment.NewLine
                );
        }
    }
}

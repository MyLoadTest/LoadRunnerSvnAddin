using System;
using System.Linq;
using ICSharpCode.Core;
using SharpSvn;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class SvnClientException : Exception
    {
        private readonly SvnErrorCode _errorCode;

        internal SvnClientException(SvnException ex) : base(ex.Message, ex)
        {
            this._errorCode = ex.SvnErrorCode;
            LoggingService.Debug(ex);
        }

        /// <summary>
        /// Gets the inner exception of the exception being wrapped.
        /// </summary>
        public Exception GetInnerException()
        {
            return InnerException.InnerException;
        }

        public bool IsKnownError(KnownError knownError)
        {
            return (int)_errorCode == (int)knownError;
        }
    }
}
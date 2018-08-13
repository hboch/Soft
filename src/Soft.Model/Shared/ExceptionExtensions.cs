using System;

namespace Soft.Model.Shared
{
    /// <summary>
    /// Extensin to determine InnerExecption-Messages
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Recursive determination of the InnerExecption-Messages of an Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Cocatenated InnerException-Messages separated by " --> "</returns>
        public static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }
}

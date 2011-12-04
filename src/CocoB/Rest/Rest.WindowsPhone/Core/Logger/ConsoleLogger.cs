/*
 * ConsoleLogger.cs
 * 
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/4/2011 1:20:01 AM
 *
 */

using System;

namespace CocoB.Rest.WindowsPhone.Core.Logger
{
    internal class ConsoleLogger : Logger
    {
        
        #region Constructors

        internal ConsoleLogger(string className)
            : base(className)
        {
        }

        #endregion

        #region Overrides of Logger

        /// <summary>
        /// Log debug messages. These messages will only be logged in debug mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log message format parameters. </param>
        public override void Debug(string message, params object[] args)
        {
            Print(LogType.DEBUG, message, args);
        }

        /// <summary>
        /// Logs information messages. These message will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public override void Info(string message, params object[] args)
        {
            Print(LogType.INFO, message, args);
        }

        /// <summary>
        /// Logs warning messages. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public override void Warn(string message, params object[] args)
        {
            Print(LogType.WARN, message, args);
        }

        /// <summary>
        /// Logs Exceptions. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="e"> Exception object. </param>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public override void Exception(Exception e, string message, params object[] args)
        {
            var appenedWithExceptionInfo =
                message +
                Environment.NewLine +
                "EXCEPTION INFO" +
                Environment.NewLine +
                e;

            Print(LogType.EXCEPTION, appenedWithExceptionInfo, args);
        }

        /// <summary>
        /// Logs error messages. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public override void Error(string message, params object[] args)
        {
            Print(LogType.ERROR, message, args);
        }

        private void Print(LogType type, string message, object [] args)
        {
            var formattedMessage = Format(message, args);
            var logEntry = CreateLogEntry(type, formattedMessage);
            System.Diagnostics.Debug.WriteLine(logEntry);
        }

        #endregion
    }
}

/*
 * Logger.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 11/20/2010 1:18:30 PM
 *
 */

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CocoB.Rest.WindowsPhone.Core.Logger
{
    internal enum LogType
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        EXCEPTION
    }

    internal abstract class Logger
    {

        private const int DEFAULT_LOG_LENGTH = 5000;

        private readonly string _className;
        private readonly int _maxLogLength;

        protected Logger(string className, int maxLogLength = DEFAULT_LOG_LENGTH)
        {
            _className = className;
            _maxLogLength = maxLogLength;
        }

        /// <summary>
        /// Log debug messages. These messages will only be logged in debug mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log message format parameters. </param>
        public abstract void Debug(string message, params object[] args);

        /// <summary>
        /// Logs information messages. These message will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public abstract void Info(string message, params object[] args);

        /// <summary>
        /// Logs warning messages. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public abstract void Warn(string message, params object[] args);

        /// <summary>
        /// Logs Exceptions. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="e"> Exception object. </param>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public abstract void Exception(Exception e, string message, params object[] args);

        /// <summary>
        /// Logs error messages. These messages will be logged in debug and release mode.
        /// </summary>
        /// <param name="message"> Log message. </param>
        /// <param name="args"> Log Message format parameters. </param>
        public abstract void Error(string message, params object[] args);

        /// <summary>
        ///   Initialize a new instance of type PMLogger that takes the name of the class that created this logger and a maximum log messages' length of 512 characters.
        /// </summary>
        /// <returns> Logger interface. </returns>
        public static Logger GetCurrentClassLogger()
        {
            string className = GetCallerClassName();
            return new ConsoleLogger(className);
        }

        private static string GetCallerClassName()
        {
            var trace = new StackTrace();
            return trace.GetFrame(2).GetMethod().ReflectedType.Name;
        }

        protected static string Format(string message, object[] args)
        {
            if (args != null && args.Length != 0)
            {
                return string.Format(message, args);
            }

            return message;
        }

        protected LogEntry CreateLogEntry(LogType type, string message)
        {
            var threadName = "<no name>";
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                threadName = Thread.CurrentThread.Name;
            }

            return new LogEntry(
                type, 
                DateTime.Now, 
                _className, 
                threadName,
                Thread.CurrentThread.ManagedThreadId,
                message,
                _maxLogLength
                );

        }

        protected class LogEntry
        {

            #region Constructors

            public LogEntry(
                LogType type,
                DateTime time,
                string className,
                string threadName,
                int threadID,
                string message,
                int maxLogLength)
            {
                Type = type;
                Time = time;
                ClassName = className;
                ThreadName = threadName;
                ThreadID = threadID;
                Message = message;
                MaxLogLength = maxLogLength;
            }

            #endregion

            #region Properties

            public LogType Type { get; private set; }

            public DateTime Time { get; private set; }

            public string ClassName { get; private set; }

            public string ThreadName { get; private set; }

            public int ThreadID { get; private set; }

            public string Message { get; private set; }

            public int MaxLogLength { get; set; }

            #endregion

            #region Methods

            public override string ToString()
            {
                var buffer = new StringBuilder();
                buffer.Append(Type);
                buffer.Append(" : ");
                buffer.Append(Time);
                buffer.Append(" : ");
                buffer.Append(ClassName);
                buffer.Append(" : ");
                buffer.Append(ThreadName);
                buffer.Append(" : ");
                buffer.Append(ThreadID);
                buffer.Append(" : ");

                string truncatedMessage;
                if (Message.Length > MaxLogLength)
                {
                    truncatedMessage = Message.Substring(0, MaxLogLength);
                    truncatedMessage += "\n[TRUNCATED]";
                }
                else
                {
                    truncatedMessage = Message;
                }

                buffer.Append(truncatedMessage);

                return buffer.ToString();
            }

            #endregion

        }
    }
}
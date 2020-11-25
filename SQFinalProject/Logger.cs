using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject
{
    /// 
    /// \class <b>Logger</b>
    ///
    /// \brief The static logger class to log specific application events to an external log file
    ///
    /// \author <i>Chris Lemon</i>
    ///
    public static class Logger
    {
        static public string path { get; set; }
        private static readonly object obj = new object();

        /// \brief Creates a string out of a message from anywhere in the application and log it into an external file
        /// \details <b>Details</b>
        /// Takes a string as a parameter, adds a date/time to it and appends it to an external txt file.  If the file doesn't exist, it creates the file.
        /// If the file can't be created it throws a log into the Windows Logger
        /// \param - msg - <b>string</b> - The message that needs to be logged
        /// 
        /// \return <b>Nothing</b>
        /// 
        public static void Log(string msg)
        {
            string timeStamp = DateTime.UtcNow.ToString();
            string logString = timeStamp + " - " + msg + "\n";
            lock (obj)
            {
                try
                {
                    File.AppendAllText(path, logString);
                }
                catch (IOException e)
                {
                    EventLog serverEventLog = new EventLog();
                    if (!EventLog.SourceExists("ChatServerEventSource"))
                    {
                        EventLog.CreateEventSource("ChatServerEventSource", "ChatServerEvents");
                    }
                    serverEventLog.Source = "ChatSeverEventSource";
                    serverEventLog.Log = "ChatServerEvents";
                    serverEventLog.WriteEntry(e.Message);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQFinalProject
{
    /// 
    /// \class Logger
    ///
    /// \brief The static logger class to log specific application events to an external log file.  It handles exceptions by catch any exception and 
    /// returning a fail value of 1 to whichever method called it
    ///
    /// \author <i>Nick Byam</i>
    ///
    public static class Logger
    {
        static public string path { get; set; } //!<path to write log file to
        private static readonly object obj = new object(); //!<object to lock writing if another thread is using the write method

        /// \brief Creates a string out of a message from anywhere in the application and log it into an external file
        /// \details <b>Details</b>
        /// Takes a string as a parameter, adds a date/time to it and appends it to an external txt file.  If the file doesn't exist, it creates the file.
        /// If the file can't be created it throws a log into the Windows Logger
        /// \param - msg - <b>string</b> - The message that needs to be logged
        /// 
        /// \return <b>Nothing</b>
        /// 
        public static int Log(string msg)
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
                    MessageBox.Show(e.Message, "Logger Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 1;
                }
            }
            return 0;
        }


        /// \brief A method that reads from the log
        /// \details <b>Details</b>
        /// A method that reads from the log to display on a screen in the admin panel. Creates a message box if the path can't be found
        /// \param - <b>Nothing</b>
        /// \returns - logFile - <b>string</b> - A string containing the contents of the log
        /// 
        public static string ReadLog()
        {
            string logFile;
            lock (obj)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        logFile = sr.ReadToEnd();
                    }
                }
                catch (IOException e)
                {
                    MessageBox.Show(e.Message, "Logger Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logFile = null;
                }
            }
            return logFile;
        }
    }
}

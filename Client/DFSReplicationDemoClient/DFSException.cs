using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DFSReplicationDemoClient
{
    /// <summary>
    /// Delegate for error handling
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">DFS exception event arguments</param>
    public delegate void DFSExceptionEventHandler(object sender, DFSExceptionEventArgs e);

    /// <summary>
    /// DFS event arguments
    /// </summary>
    public class DFSExceptionEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }
    }

    /// <summary>
    /// DFS error handler with Singleton instance
    /// and triggerable event.
    /// </summary>
    public class ErrorHandler
    {
        #region declaration
        private static ErrorHandler instance = null;
        private static object syncLock = new object();
        #endregion

        public event DFSExceptionEventHandler ExceptionOccured = null;

        #region methods
        /// <summary>
        /// Get the ErrorHandler instance
        /// 
        /// Support multithreaded applications through 'Double checked locking' 
        /// pattern which (once the instance exists) avoids locking each
        /// time the method is invoked
        /// </summary>
        /// <return>Singleton instance</return>
        public static ErrorHandler GetErrorHandler()
        {
            if (instance == null)
            {
#if TEST
#else
                Monitor.Enter(syncLock);
#endif
                
                if (instance == null)
                {
                    instance = new ErrorHandler();
                }

#if TEST
#else
                Monitor.Exit(syncLock);
#endif
            }

            return instance;
        }

        /// <summary>
        /// Initialization with console messages
        /// </summary>
        public static void InitConsoleMessages()
        {
            ErrorHandler handler = ErrorHandler.GetErrorHandler();

            handler.ExceptionOccured += new DFSExceptionEventHandler(handler.ShowConsoleMessage);
        }

        /// <summary>
        /// Raise error via DFS error handling
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="errorMessage">error message</param>
        /// <param name="infoMessage">info message</param>
        public void RaiseError(object sender, string errorMessage, string infoMessage = "")
        {
            if (this.ExceptionOccured != null)
            {
                this.ExceptionOccured(sender,
                                      new DFSExceptionEventArgs
                                      {
                                          ErrorMessage = errorMessage,
                                          InfoMessage = infoMessage
                                      });
            }
        }

        /// <summary>
        /// DFS error handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event arguments - error message, info message</param>
        protected void ShowConsoleMessage(object sender, DFSExceptionEventArgs e)
        {
            string message = "";

            if (e != null)
            {
                if (!String.IsNullOrEmpty(e.InfoMessage))
                    message += e.InfoMessage;

                if (!String.IsNullOrEmpty(e.ErrorMessage))
                {
                    if (!String.IsNullOrEmpty(message))
                        message += Environment.NewLine;

                    message += e.ErrorMessage;
                }

                if (!String.IsNullOrEmpty(message))
                    Console.WriteLine(message);
            }
        }
        #endregion
    }
}

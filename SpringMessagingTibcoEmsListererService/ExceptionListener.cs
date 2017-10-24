// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionListener.cs" company="Craig">
//   MIT
// </copyright>
// <summary>
//   Defines the ExceptionListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SpringMessagingTibcoEmsListenerService
{
    using System;

    using TIBCO.EMS;

    /// <inheritdoc />
    /// <summary>
    /// The exception listener used to asynchonously detect problems with connections, 
    /// like network partition events or when the Tibco EMS Server is shut down.
    /// </summary>
    public class ExceptionListener : IExceptionListener
    {
        /// <summary>
        /// The logger we will use to log
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionListener"/> class.
        /// </summary>
        public ExceptionListener()
        {
            Logger.Info("ExceptionListener.cs created.");
        }

        /// <inheritdoc />
        /// <summary>
        /// The on exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void OnException(EMSException exception)
        {
            Logger.Debug($"OnException : {exception.Message}");
            if (exception.LinkedException != null)
            {
                Logger.Debug($"OnException Linked Exception error msg  : {exception.LinkedException.Message}");
            }

            if (exception.InnerException != null)
            {
                Logger.Debug($"OnException InnerException : {exception.InnerException.Message}");
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmsListenerService.cs" company="Craig">
//   MIT
// </copyright>
// <summary>
//   Defines the EmsListenerService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SpringMessagingTibcoEmsListenerService
{
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Timers;

    /// <inheritdoc />
    /// <summary>
    /// The ems listener service.
    /// </summary>
    public partial class EmsListenerService : ServiceBase
    {
        #region Fields

        /// <summary>
        /// The logger we will use to log
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The consumer.
        /// </summary>
        private readonly ConsumerListener consumer = new ConsumerListener();
        
        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SpringMessagingTibcoEmsListererService.EmsListenerService" /> class.
        /// </summary>
        public EmsListenerService()
        {
            this.InitializeComponent();
            this.InitializeComponent();
            this.eventLog1 = new EventLog();
            if (!EventLog.SourceExists("EmsServiceListnerService"))
            {
                EventLog.CreateEventSource("EmsServiceListnerService", "EmsServiceListnerServiceLog");
            }

            this.eventLog1.Source = "EmsServiceListnerService";
            this.eventLog1.Log = "EmsServiceListnerServiceLog";

            // Set up a timer to trigger every minute.  
            var timer = new Timer { Interval = 30000 }; // 30 seconds  
            timer.Elapsed += this.OnTimer;
            timer.Start();
        }

        /// <summary>
        /// The on timer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            this.eventLog1.WriteEntry("OnTimer Event", EventLogEntryType.Information);
            Logger.Info("OnTimer Event");
        }

        /// <inheritdoc />
        /// <summary>
        /// The on start.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnStart(string[] args)
        {
            this.eventLog1.WriteEntry("OnStart Event", EventLogEntryType.Information);
            Logger.Info("OnStart Event");

            // Run the consumer so we can capture inbound messages from Tibco EMS
            this.consumer.Run();
        }

        /// <inheritdoc />
        /// <summary>
        /// The on stop.
        /// </summary>
        protected override void OnStop()
        {
            this.eventLog1.WriteEntry("OnStop Event", EventLogEntryType.Information);
            Logger.Info("OnStop Event");
        }

        /// <inheritdoc />
        /// <summary>
        /// The on continue.
        /// </summary>
        protected override void OnContinue()
        {
            this.eventLog1.WriteEntry("OnContinue Event", EventLogEntryType.Warning);
            Logger.Info("OnContinue Event");
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsumerListener.cs" company="Craig">
//   MIT
// </copyright>
// <summary>
//   The listener consumer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SpringMessagingTibcoEmsListenerService
{
    using System;
    using System.Configuration;

    using Spring.Messaging.Ems.Listener;

    using TIBCO.EMS;

    /// <summary>
    /// The listener consumer used to listen and consume messages from a Tibco EMS Queue.  Implemented by Craig Nicholson
    /// </summary>
    public class ConsumerListener
    {
        // TODO: Most of these fields need to be passed into the class so the class is reusable.
        #region Fields

        /// <summary>
        /// The AckMode sets whether how we treat incoming messages.  Messages need to be acknowledge so they message will
        /// be remove from the queue once the message is delivered.
        /// </summary>
        private const int AckMode = Session.AUTO_ACKNOWLEDGE;

        /// <summary>
        /// The logger we will use to log
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The target host name is the name of the server or endpoint without the url and port number.
        /// Examples {localhost, etss-appdev, tibco.test01.amu.ssnsgs.net}
        /// </summary>
        // ReSharper disable once StyleCop.SA1650
        private static readonly string TargetHostName = ConfigurationManager.AppSettings["TargetHostName"];

        /// <summary>
        /// The Uri, the endpoint for the Tibco EMS server
        /// Examples {localhost,tcp://10.86.1.31:7222,tcp://etss-appdev:7222, ssl://tibco.test01.amu.ssnsgs.net:7243}
        /// </summary>
        // ReSharper disable once StyleCop.SA1650
        private static readonly string Uri = ConfigurationManager.AppSettings["Uri"];

        /// <summary>
        /// Use SSL is a flag to indicate we will use SSL to encrypt the network traffic.
        /// </summary>
        private static readonly bool UseSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["Ssl"]);

        /// <summary>
        /// The destination or queue name we are listening for messages.
        /// </summary>
        private static readonly string DestinationName = ConfigurationManager.AppSettings["DestinationName"];

        /// <summary>
        /// The User if required for Tibco EMS.
        /// </summary>
        private static readonly string User = ConfigurationManager.AppSettings["User"];

        /// <summary>
        /// The Password if required for Tibco EMS.
        /// </summary>
        private static readonly string Pwd = ConfigurationManager.AppSettings["Pwd"];

        /// <summary>
        /// The SSL client certificate file path to the file.
        /// </summary>
        private static readonly string SslClientCert = ConfigurationManager.AppSettings["SslClientCert"];

        /// <summary>
        /// The SSL client certificate password.
        /// </summary>
        private static readonly string SslClientCertPassword = ConfigurationManager.AppSettings["SslClientCertPassword"];

        /// <summary>
        /// The max recovery time in days.
        /// </summary>
        private static readonly int MaxRecoveryTimeInDays = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRecoveryTimeInDays"]);

        /// <summary>
        /// The connection factory used to setup the connection to the Tibco EMS server.
        /// </summary>
        private static Spring.Messaging.Ems.Common.EmsConnectionFactory connectionFactory;

        /// <summary>
        /// The listener container which will listen as long as the ConsumerListener object exits
        /// </summary>
        private readonly SimpleMessageListenerContainer listenerContainer = new SimpleMessageListenerContainer();

        #endregion

        /// <summary>
        /// Listen for messages and consume when messages are received.
        /// </summary>
        public void Run()
        {
            try
            {
                if (string.IsNullOrEmpty(Uri))
                {
                    throw new ArgumentException("Uri can not be empty");
                }

                Logger.Info($"Uri : {Uri}");

                connectionFactory = new Spring.Messaging.Ems.Common.EmsConnectionFactory(Uri);
                if (!string.IsNullOrEmpty(User))
                {
                    connectionFactory.UserName = User;
                    Logger.Info($"User : {User}");
                }

                if (!string.IsNullOrEmpty(Pwd))
                {
                    connectionFactory.UserPassword = Pwd;
                    Logger.Info($"Pwd : {Pwd}");
                }

                if (!string.IsNullOrEmpty(TargetHostName))
                {
                    connectionFactory.TargetHostName = TargetHostName;
                    Logger.Info($"TargetHostName : {TargetHostName}");
                }

                if (UseSsl)
                {
                    if (!System.IO.File.Exists(SslClientCert))
                    {
                        throw new Exception("SslClientCert File is missing");
                    }

                    // Load client certificate from a file
                    var clientCert = new System.Security.Cryptography.X509Certificates.X509Certificate2(SslClientCert, SslClientCertPassword);
                    Logger.Info($"Certificate File:{SslClientCert}");
                    Logger.Info($"Certificate Subject:{clientCert.Subject}");
                    Logger.Info($"Certificate Public Key:{clientCert.PublicKey.Key}");
                    Logger.Info($"Certificate Has private key?:{(clientCert.HasPrivateKey ? "Yes" : "No")}");
                    var sslStore = new EMSSSLFileStoreInfo();
                    sslStore.SetSSLClientIdentity(clientCert);
                    connectionFactory.NativeConnectionFactory.SetCertificateStoreType(EMSSSLStoreType.EMSSSL_STORE_TYPE_FILE, sslStore);
                }

                // Check if we have established a connection since the TargetHostName, Uri, User, or Pwd could be incorrect
                if (connectionFactory == null)
                {
                    throw new Exception("connectionFactory has not been initialized");
                }

                connectionFactory.ClientID = "This needs to be set right";
                Logger.Debug("connectionFactory initialized");
                try
                {
                    Logger.Debug($"Destination - {DestinationName}");

                    this.listenerContainer.ConnectionFactory = connectionFactory;
                    this.listenerContainer.DestinationName = DestinationName;
                    this.listenerContainer.ConcurrentConsumers = 1;
                    this.listenerContainer.PubSubDomain = false;
                    this.listenerContainer.MessageListener = new MessageListener();
                    this.listenerContainer.ExceptionListener = new ExceptionListener();
                    this.listenerContainer.MaxRecoveryTime = new TimeSpan(MaxRecoveryTimeInDays, 0, 0, 0);
                    this.listenerContainer.RecoveryInterval = new TimeSpan(0, 0, 0, 10, 0); // set to 10 Minutes  
                    this.listenerContainer.AcceptMessagesWhileStopping = false;
                    this.listenerContainer.SessionAcknowledgeMode = AckMode;
                    this.listenerContainer.AfterPropertiesSet();
                    if (this.listenerContainer.IsRunning)
                    {
                        Logger.Debug("Listener IsRunning.");
                    }

                }
                catch (EMSException e)
                {
                    Logger.Error($"EMSException : {e.Message}");

                    if (e.LinkedException != null)
                    {
                        Logger.Error($"EMSException Linked Exception error msg : {e.LinkedException.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception : {ex.Message}");
                if (ex.InnerException != null)
                {
                    Logger.Error($"InnerException : {ex.InnerException.Message}");
                }
            }
        }
    }
}

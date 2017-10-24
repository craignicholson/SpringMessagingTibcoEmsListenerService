// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageListener.cs" company="Craig">
//   MIT
// </copyright>
// <summary>
//   Defines the MessageListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SpringMessagingTibcoEmsListenerService
{
    using System;
    using System.IO;

    using Spring.Messaging.Ems.Common;
    using Spring.Messaging.Ems.Listener;

    using TIBCO.EMS;

    /// <inheritdoc />
    /// <summary>
    /// The Listener implementation for Spring.Messaging.Ems.Listener
    /// </summary>
    public class MessageListener : ISessionAwareMessageListener
    {
        /// <summary>
        /// The logger we will use to log
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListener"/> class.
        /// </summary>
        public MessageListener()
        {
            Logger.Info("MessageListener.cs created.");
        }

        /// <inheritdoc />
        /// <summary>
        /// On Message Handler, runs when the consumer receives a message from the Tibco EMS Queue
        /// </summary>
        /// <param name="message">Tibco EMS Message</param>
        /// <param name="session">Spring.Messaging.Ems.Common.ISession when event occurs.</param>
        public void OnMessage(Message message, ISession session)
        {
            Logger.Info("OnMessage Event Raised");
            try
            {
                var xml = string.Empty;
                var ssnJobIdToken = message.GetStringProperty("SSN_JOB_ID_TOKEN");
                var ssnJobStatusToken = message.GetStringProperty("SSN_JOB_STATUS_TOKEN");  // Two states {Failure and Completed}
                Logger.Info($"ssnJobIdToken :{ssnJobIdToken}");
                Logger.Info($"ssnJobStatusToken :{ssnJobStatusToken}");
                Logger.Info($"MessageId :{message.MessageID}");
                Logger.Info($"CorrelationID :{message.CorrelationID}");
                Logger.Info($"DeliveryTime :{message.DeliveryTime}");

                switch (message)
                {
                    case TextMessage textMessage:
                        xml = textMessage.Text;
                        Logger.Info($"TextMessage.Text\n{textMessage.Text}");
                        break;
                    case BytesMessage byteMessage:
                        Logger.Info($"BytesMessage.BodyLength : {byteMessage.BodyLength}");
                        break;
                    case MapMessage mapMessage:
                        Logger.Info($"MapMessage.FieldCount : {mapMessage.FieldCount}");
                        break;
                    case StreamMessage streamMessage:
                        Logger.Info($"StreamMessage.FieldCount : {streamMessage.FieldCount}");
                        break;
                    case ObjectMessage objectMessage:
                        Logger.Info($"ObjectMessage.MessageID : {objectMessage.MessageID}");
                        break;
                }

                var fileName = DateTime.Now.ToString("yyyyMMddhhmmssFFF") + "_JobId"  + ".xml";
                File.WriteAllText(fileName, xml);
                message.Acknowledge();
            }
            catch (Exception e)
            {
                // If we are unable to successfully pass a message or write the message
                // to disk set an expiration date on the missing for one day.
                // Note, if we have 1000s of failures leaving the message on the queue will
                // be a problem.
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var expiryTime = (DateTime.UtcNow.AddSeconds(86400) - unixEpoch).TotalMilliseconds;
                message.Expiration = (long)expiryTime;
                Logger.Error(e.Message);
                throw;
            }

            Logger.Info("OnMessage processed message");
        }
    }
}

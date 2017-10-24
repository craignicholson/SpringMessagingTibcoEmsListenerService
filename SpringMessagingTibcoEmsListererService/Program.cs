// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Craig">
//   MIT
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SpringMessagingTibcoEmsListenerService
{
    using System.ServiceProcess;

    /// <summary>
    /// The EMS Listener Service
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var servicesToRun = new ServiceBase[]
                                              {
                                                  new EmsListenerService()
                                              };
            ServiceBase.Run(servicesToRun);
        }
    }
}

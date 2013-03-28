using System;
using System.Threading;

namespace Vre.Server
{
    public class ServiceRunner
    {
        private static bool _error = false;
        public static string Status { get { if (_error) return "Error."; else return StartupShutdown.Status; } }
        public static string[] Listeners { get { return StartupShutdown.Listeners; } }
        public static string ExtendedStatus = string.Empty;

        public static void Start()        
        {
            try
            {
                StartupShutdown.PerformStartup(true);
            }
            catch (Exception ex)
            {
                _error = true;
                ExtendedStatus = Utilities.ExplodeException(ex);
            }                
        }

        public static void Stop()
        {
            try
            {
                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name = "SvcDown#" + Thread.CurrentThread.ManagedThreadId.ToString();
                StartupShutdown.PerformShutdown();
            }
            catch (Exception ex)
            {
                _error = true;
                ExtendedStatus = Utilities.ExplodeException(ex);
            }
        }
    }
}
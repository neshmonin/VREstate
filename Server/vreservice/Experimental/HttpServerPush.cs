using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Vre.Server.Experimental
{
    internal class HttpServerPush
    {
        public static void StartServerPushThread(HttpListenerContext ctx)
        {
            new Thread(holdOffThread) { IsBackground = true }.Start(ctx);
        }

        private static void holdOffThread(object param)
        {
            HttpListenerContext ctx = param as HttpListenerContext;
            if (null == ctx) return;

            Thread.CurrentThread.Name = "HttpPush#" + Thread.CurrentThread.ManagedThreadId.ToString();

            int delay;
            if (!int.TryParse(ctx.Request.QueryString.Get("delay"), out delay)) delay = 5;
            string status = ctx.Request.QueryString.Get("status");
            if (string.IsNullOrEmpty(status)) status = "none";

            Thread.Sleep(delay * 1000);

            byte[] update = Encoding.UTF8.GetBytes("{\"status\":\"" + status + "\"}");
            try 
            { 
                ctx.Response.OutputStream.Write(update, 0, update.Length);
                ctx.Response.Close();
            }
            catch (Exception e) 
            { 
                ServiceInstances.Logger.Error("{0}", e); 
            }
        }
    }
}
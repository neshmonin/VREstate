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
            int status = 0;

            do
            {
                status++;
                Thread.Sleep(5000 + 5000 * status);

                byte[] update = Encoding.UTF8.GetBytes("{\"status\":" + status.ToString() + "}");
                try { ctx.Response.OutputStream.Write(update, 0, update.Length); }
                catch { break; }
            } while (true);
        }
    }
}
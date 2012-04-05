using System;
using System.Net;

namespace Vre.Server.HttpService
{
    internal class Statistics
    {
        public static string GetBrowserId(HttpListenerContext ctx)
        {
            Cookie browserId = ctx.Request.Cookies["a"];

            if (null == browserId)
            {
                browserId = new Cookie("a", Guid.NewGuid().ToString(), "/", ".3dcondox.com");
                browserId.Expires = DateTime.Now.AddYears(10);
                ctx.Response.Cookies.Add(browserId);
            }

            return browserId.Value;
        }

    }
}
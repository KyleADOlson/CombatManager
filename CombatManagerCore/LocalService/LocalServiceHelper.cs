using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public static class LocalServiceHelper
    {
        public static string RequestHeader(this EmbedIO.IHttpContext con, string header)
        {
            return con.Request.Headers[header];
        }

        public static bool HasRequestHeader(this EmbedIO.IHttpContext con, string header)
        {
            return con.Request.Headers[header] != null;
        }

    }
}

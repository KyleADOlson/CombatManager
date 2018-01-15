using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Service
{
    public static class ServiceInfo
    {
        public const string BaseURL = "https://ruwot9a45g.execute-api.us-west-2.amazonaws.com/prod/";

        public static String FunctionUrl(String function)
        {
            return BaseURL + function;
        }
    }
}

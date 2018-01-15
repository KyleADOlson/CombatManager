using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using CombatManager.Http;
using System.Web;

namespace CombatManager.Service
{
    public static class LoginService
    {
        

        public static async Task<TokenData> LoginAsync(string accessToken)
        {

            return await HttpUtil.JsonGet<TokenData>(ServiceInfo.BaseURL + "login?access_token=" + HttpUtility.UrlEncode(accessToken));
           
        }
        public static async Task<TokenStatusData> TokenStatusAsync(string token)
        {

            return await HttpUtil.JsonGet<TokenStatusData>(ServiceInfo.BaseURL + "validate-token?token=" + HttpUtility.UrlEncode(token));
            
        }
    }
}

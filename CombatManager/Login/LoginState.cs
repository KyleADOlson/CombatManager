using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CombatManager.WinUtil;
using CombatManager.Service;

namespace CombatManager.Login
{
    class LoginState : SimpleNotifyClass
    {
        string token;
        string service;
        string id;
       
        public void UpdateLogin(String token, string service, string id)
        {
            this.token = token;
            this.service = service;
            this.id = id;
            Notify("Token");
            Notify("Service");
            Notify("Id");
            Notify("LoggedIn");
            Save();
        }
        
        public String Token
        {
            get => token;
            set
            {
                if (token != value)
                {
                    token = value;
                    Notify("Token");
                    Notify("LoggedIn");
                }
            }
        }
        public String Service
        {
            get => service;
            set
            {
                if (service != value)
                {
                    service = value;
                    Notify("Service");
                }
            }
        }
        public String Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    Notify("Id");
                }
            }
        }

        public Boolean LoggedIn
        {
            get => token != null && token != "";
        }


        public async Task<bool> LoadAsync()
        {

            var key = RegistryHelper.LoadCU("Login");

            token = key.LoadString("Token");
            if (token == "")
            {
                token = null;
            }
            service = key.LoadString("Service");
            id = key.LoadString("Id");

            TokenStatusData data = await LoginService.TokenStatusAsync(token);
            if (data != null)
            {
                if (!data.valid)
                {
                    UpdateLogin(null, null, null);
                }
                else
                {
                    UpdateLogin(token, data.service, data.loginid);
                    return true;
                }
            }
            else
            {
            }
            return false;


        }

        public void Save()
        {
            var key = RegistryHelper.LoadCU("Login");

            key.SaveString("Token", token==null?"":token);
            key.SaveString("Service", service==null?"":service);
            key.SaveString("Id", id==null?"":id);
        }
    }
}

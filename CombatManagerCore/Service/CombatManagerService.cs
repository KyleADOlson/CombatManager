using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CombatManager.Http;
using Newtonsoft.Json;

namespace CombatManager.Service
{
    public class CombatManagerService
    {
        

        static CombatManagerService service;

        public static CombatManagerService Service
        {
            get
            {
                if (service == null)
                {
                    service = new CombatManagerService();
                }
                return service;
            }
        }

        public bool Initialized
        {
            get => Token != null;
        }

        public async Task<bool> IsValidLogin()
        {
            if (!Initialized)
            {
                return false;
            }
            return (await LoginService.TokenStatusAsync(Token)).valid;
        }

        public CombatManagerService()
        {


        }

        public String Token { get; set; }

        public async Task<ServiceResponse> SaveCombatState(CombatState combat_state, String name, String stateid)
        {
            var data = new { token = Token, stateid, name, combat_state};

            String datast = JsonConvert.SerializeObject(data,
                                     Formatting.Indented,
                                     new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ServiceResponse resp =  await HttpUtil.JsonPost<ServiceResponse>(ServiceInfo.FunctionUrl("save-combat-state"), data : data);

            DebugLogger.WriteLine("Result: " + resp.succeeded + " " + resp.error);

            return resp;
        }

        public async Task<CombatStateServiceResponse> LoadCombatState(String stateid)
        {
            var message = new { stateid, token=Token };

            return await HttpUtil.JsonPost<CombatStateServiceResponse>(
                ServiceInfo.FunctionUrl("load-combat-state"), data:message);
        }

        public async Task<CombatStateArrayServiceResponse> ListCombatStates()
        {
            var message = new { token = Token };
            return await HttpUtil.JsonPost<CombatStateArrayServiceResponse>(
                ServiceInfo.FunctionUrl("list-combat-states"), data:message);
        }

        public async Task<CombatStateServiceResponse> DeleteCombatState(String stateid)
        {
            var message = new { stateid, token = Token };

            return await HttpUtil.JsonPost<CombatStateServiceResponse>(
                ServiceInfo.FunctionUrl("delete-combat-state"), data: message);
        }

        public async Task<ServiceResponse> SaveMonster(String monsterid, Monster monster)
        {
            var message = new
            {
               monsterid, monster, token = Token };

            return await HttpUtil.JsonPost<CombatStateServiceResponse>(
                ServiceInfo.FunctionUrl("save-monster"), data: message);

        }
        public async Task<MonsterListResponse> ListMonsters()
        {
            var message = new { token = Token };

            return await HttpUtil.JsonPost<MonsterListResponse>(
                ServiceInfo.FunctionUrl("list-monsters"), data: message);

        }

        public async Task<MonsterResponse> LoadMonster(String monsterid)
        {

            var message = new {  monsterid, token = Token };

            return await HttpUtil.JsonPost<MonsterResponse>(
                ServiceInfo.FunctionUrl("load-monster"), data: message);
        }
    }
}

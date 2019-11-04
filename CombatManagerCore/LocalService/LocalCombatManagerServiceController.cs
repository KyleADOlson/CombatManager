using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

#pragma warning disable 618

namespace CombatManager.LocalService
{
    public class LocalCombatManagerServiceController : WebApiController
    {
        CombatState state;
        LocalCombatManagerService.ActionCallback actionCallback;
        Action saveCallback;
        LocalCombatManagerService service;

        public LocalCombatManagerServiceController(IHttpContext context, CombatState state, LocalCombatManagerService service, LocalCombatManagerService.ActionCallback actionCallback, Action saveCallback)
            : base(context)
        {
            this.state = state;
            this.service = service;
            this.actionCallback = actionCallback;
            this.saveCallback = saveCallback;
        }

        private class ResultHandler
        {
            public ResultHandler()
            {
                Code = System.Net.HttpStatusCode.BadRequest;
            }

            public object Data { get; set; }
            public bool Failed { get; set; }

            public System.Net.HttpStatusCode Code { get; set; }

        }

        public Character.HPMode HPMode { get; set; }

        public string Passcode { get; set; }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/state")]
        public async Task<bool> GetCombatState()
        {
            return await TakeAction((res) =>
            {
                res.Data = state.ToRemote();
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/next")]
        public async Task<bool> CombatNext()
        {
            return await TakeAction((res) =>
            {
                state.MoveNext();
                saveCallback();
                res.Data = state.ToRemote();
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/prev")]
        public async Task<bool> CombatPrev()
        {

            return await TakeAction((res) =>
                {
                    state.MovePrevious();
                    saveCallback();
                    res.Data = state.ToRemote();
                });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/rollinit")]
        public async Task<bool> CombatRollInit()
        {
            return await TakeAction((res) =>
                {
                    state.RollInitiative();
                    state.SortCombatList();
                    saveCallback();
                    res.Data = state.ToRemote();
                }
                );

        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/details/{charid}")]
        public async Task<bool> GetCharacterDetails(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {
                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/moveupcharacter/{charid}")]
        public async Task<bool> MoveCharacterUp(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.MoveUpCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/movedowncharacter/{charid}")]
        public async Task<bool> MoveDownCharacter(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.MoveDownCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/deletecharacter/{charid}")]
        public async Task<bool> DeleteCharacter(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.RemoveCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/ready/{charid}")]
        public async Task<bool> ReadyCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsReadying = true;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/unready/{charid}")]
        public async Task<bool> UnreadyCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsReadying = false;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/delay/{charid}")]
        public async Task<bool> DelayCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsDelaying = true;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/undelay/{charid}")]
        public async Task<bool> UndelayCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsDelaying = false;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/actnow/{charid}")]
        public async Task<bool> CharacterActNow(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                state.CharacterActNow(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }


        [WebApiHandler(HttpVerbs.Get, "/api/character/changehp/{charid}/{amount}")]
        public async Task<bool> ChangeHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.HP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/changemaxhp/{charid}/{amount}")]
        public async Task<bool> ChangeMaxHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.MaxHP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/changetemporaryhp/{charid}/{amount}")]
        public async Task<bool> ChangeTemporaryHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.TemporaryHP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }


        [WebApiHandler(HttpVerbs.Get, "/api/character/changenonlethaldamage/{charid}/{amount}")]
        public async Task<bool> ChangeNonlethalDamage(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.NonlethalDamage += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/hide/{charid}/{state}")]
        public async Task<bool> HideCharacter(string charid, bool state)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsHidden = state;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/idle/{charid}/{state}")]
        public async Task<bool> IdleCharacter(string charid, bool state)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsIdle = state;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }


        [WebApiHandler(HttpVerbs.Post, "/api/character/addcondition")]
        public async Task<bool> AddCondition()
        {
            return await TakeCharacterPostAction<AddConditionRequest>((res, data, ch) =>
            {
                Condition c = Condition.ByName(data.Name);
                if (c == null)
                {
                    res.Failed = true;
                    return;
                }
                ActiveCondition ac = new ActiveCondition();
                ac.Condition = c;
                ac.InitiativeCount = state.CurrentInitiativeCount;
                ac.Turns = data.Turns;
                ch.Monster.AddCondition(ac);
                saveCallback();

                res.Data = ch.ToRemote();

            });
        }


        [WebApiHandler(HttpVerbs.Post, "/api/character/removecondition")]
        public async Task<bool> RemoveCondition()
        {
            return await TakeCharacterPostAction<RemoveConditionRequest>((res, data, ch) =>
            {
                ch.RemoveConditionByName(data.Name);
                saveCallback();

                res.Data = ch.ToRemote();

            });
        }

        [WebApiHandler(HttpVerbs.Post, "/api/monster/list")]
        public async Task<bool> ListMonsters()
        {
            return await TakePostAction<MonsterListRequest>((res, data) =>
            {
                int? maxCR =  Monster.TryGetCRChartInt(data.MaxCR);
                int? minCR = Monster.TryGetCRChartInt(data.MinCR);
                res.Data= LocalRemoteConverter.CreateRemoteMonsterList(m =>
                {
                    int? crInt = m.IntCR;


                    try
                    {
                        return (data.Name.IsEmptyOrNull() || m.Name.ToUpper().Contains(data.Name.ToUpper()))
                        && (data.IsCustom == null || m.IsCustom == data.IsCustom)
                        && (data.IsNPC == null || m.NPC == data.IsNPC)
                        && (crInt.IsNullOrBetweenInclusive(minCR, maxCR)
                        );
                    }
                    catch (Exception ex)
                    {
                        if (ex != null)
                        {

                        }
                        return false;
                    }
                    
                });

            });
        }

        [WebApiHandler(HttpVerbs.Post, "/api/monster/get")]
        public async Task<bool> GetMonster()
        {

            return await TakePostAction<MonsterRequest>((res, data) =>
            {
                 res.Data = Monster.ByID(data.IsCustom, data.ID).ToRemote();
                 
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/monster/getregular/{id}")]
        public async Task<bool> GetRegularMonster(int id)
        {
            return await TakeAction( (res) =>
            {
                res.Data = Monster.ByDetailsID(id).ToRemote();
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/ui/bringtofront")]
        public async Task<bool> BringToFront()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.BringToFront);
                return await Ok(new { res = true });
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/ui/minimize")]
        public async Task<bool> Minimize()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.Minimize);
                return await Ok(new { res = true });
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/ui/goto/{place}")]
        public async Task<bool> UIGoto(string place)
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.Goto, place);
                return await Ok(new { res = true });
            });
        }

        [WebApiHandler(HttpVerbs.Get, "/api/ui/showcombatlist")]
        public async Task<bool> ShowCombatList()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.ShowCombatListWindow);
                return await Ok(new { res = true });
            });
        }


        [WebApiHandler(HttpVerbs.Get, "/api/ui/hidecombatlist")]
        public async Task<bool> HideCombatList()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.HideCombatListWindow);
                return await Ok(new { res = true });
            });
        }



        [WebApiHandler(HttpVerbs.Get, "/api/monster/getcustom/{id}")]
        public async Task<bool> GetCustomMonster(int id)
        {
            return await TakeAction((res) =>
            {
                res.Data = Monster.ByDBLoaderID(id).ToRemote();
            });
        }

        [WebApiHandler(HttpVerbs.Post, "/api/monster/fromlist")]
        public async Task<bool> GetMonsters()
        {
            return await TakePostAction<MonstersRequest>((res, data) =>
            {
                RemoteMonsterList list = new RemoteMonsterList();
                list.Monsters = new List<RemoteMonster>();
                if (data.Monsters != null)
                {
                    foreach (var mr in data.Monsters)
                    {
                        Monster m = Monster.ByID(mr.IsCustom, mr.ID);
                        list.Monsters.Add(m.ToRemote());
                    }
                }
                res.Data = list;
            });
        }

        [WebApiHandler(HttpVerbs.Post, "/api/monster/add")]
        public async Task<bool> AddMonster()
        {
            return await TakePostAction<MonsterAddRequest>((res, data) =>
            {

                foreach (var mr in data.Monsters)
                {
                    Monster m = Monster.ByID(mr.IsCustom, mr.ID);
                    if (m != null)
                    {
                        Character ch = state.AddMonster(m, HPMode, data.IsMonster);
                        if (!data.Name.IsEmptyOrNull())
                        {
                            ch.Name = data.Name;
                        }
                        res.Data = ch.ToRemote();

                    }
                }
                saveCallback();
            });
        }

        

        private async Task<bool> Precheck(Func<Task<bool>> function)
        {
            if (!Passcode.IsEmptyOrNull())
            {
                if (!HttpContext.HasRequestHeader("passcode"))
                {
                    return await InternalServerError(new ArgumentException(), System.Net.HttpStatusCode.Forbidden);
                }
                else
                {
                    string matchcode = HttpContext.RequestHeader("Passcode");
                    if (matchcode != Passcode)
                    {
                        return await InternalServerError(new ArgumentException(), System.Net.HttpStatusCode.Forbidden);

                    }
                }
            }
            return await function();
        }


        private async Task<bool> TakeAction(Action<ResultHandler> resAction)
        {
            return await Precheck(async () =>
            {
                try
                {
                    ResultHandler res = new ResultHandler();


                    actionCallback(() =>
                    {
                        try
                        {
                            resAction(res);
                        }
                        catch (Exception)
                        {
                            res.Failed = true;
                        }

                    });
                    if (res.Failed)
                    {

                        return await InternalServerError(new ArgumentException(), res.Code);
                    }
                    else
                    {
                        return await Ok(res.Data);
                    }
                }
                catch (Exception ex)
                {
                    return await InternalServerError(ex);
                }
            });
        }

        private async Task<bool> TakeCharacterAction(string charid, Action<ResultHandler, Character> handler)
        {
            return await TakeAction((res) =>
            {
                Guid id;

                Character ch = null;

                if (charid == "current")

                {
                    id = Guid.Empty;
                }
                else
                {
                    if (!Guid.TryParse(charid, out id))
                    {
                        res.Failed = true;
                        return;
                    }
                }


                if (id == Guid.Empty)
                {
                    ch = state.CurrentCharacter;
                }
                else
                {
                    ch = state.GetCharacterByID(id);
                }
                if (ch != null)
                {
                    try
                    {
                        handler(res, ch);
                    }
                    catch
                    {
                        res.Failed = true;
                    }
                }
                else
                {
                    res.Failed = true;
                }

            });
        }

        private async Task<bool> TakePostAction<T>(Action<ResultHandler, T> resAction) where T : class
        {

            return await Precheck(async () =>
            {
                try
                {
                    ResultHandler res = new ResultHandler();
                    T data = await HttpContext.ParseJsonAsync<T>();

                    if (data == null)
                    {
                        res.Failed = true;
                    }
                    else
                    {

                        actionCallback(() =>
                        {
                            try
                            {
                                resAction(res, data);
                            }
                            catch (Exception)
                            {
                                res.Failed = true;
                            }

                        });
                    }
                    if (res.Failed)
                    {

                        return await InternalServerError(new ArgumentException(), res.Code);
                    }
                    else
                    {
                        return await Ok(res.Data);
                    }

                }
                catch (Exception ex)
                {
                    return await InternalServerError(ex);
                }
            });
        }

        private async Task<bool> TakeCharacterPostAction<T>(Action<ResultHandler, T, Character> resAction) where T : CharacterRequest
        {
            return await TakePostAction<T>((res, data) =>
            {

                Character ch = state.GetCharacterByID(data.ID);
                if (ch != null)
                {
                    try
                    {
                        resAction(res, data, ch);
                    }
                    catch
                    {
                        res.Failed = true;
                    }
                }
                else
                {
                    res.Failed = true;
                }

            });
        }

       

    }

}

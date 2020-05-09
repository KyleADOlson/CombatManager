using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;

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

        [Route(HttpVerbs.Get, "/combat/state")]
        public async Task<object> GetCombatState()
        {
            return await TakeAction((res) =>
            {
                res.Data = state.ToRemote();
            });
        }

        [Route(HttpVerbs.Get, "/combat/next")]
        public async Task<object> CombatNext()
        {
            return await TakeAction((res) =>
            {
                state.MoveNext();
                saveCallback();
                res.Data = state.ToRemote();
            });
        }

        [Route(HttpVerbs.Get, "/combat/prev")]
        public async Task<object> CombatPrev()
        {

            return await TakeAction((res) =>
                {
                    state.MovePrevious();
                    saveCallback();
                    res.Data = state.ToRemote();
                });
        }

        [Route(HttpVerbs.Get, "/combat/rollinit")]
        public async Task<object> CombatRollInit()
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

        [Route(HttpVerbs.Get, "/character/details/{charid}")]
        public async Task<object> GetCharacterDetails(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {
                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/moveupcharacter/{charid}")]
        public async Task<object> MoveCharacterUp(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.MoveUpCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/movedowncharacter/{charid}")]
        public async Task<object> MoveDownCharacter(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.MoveDownCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/deletecharacter/{charid}")]
        public async Task<object> DeleteCharacter(string charid)
        {

            return await TakeCharacterAction(charid, (res, ch) =>
            {

                state.RemoveCharacter(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/ready/{charid}")]
        public async Task<object> ReadyCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsReadying = true;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/unready/{charid}")]
        public async Task<object> UnreadyCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsReadying = false;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/delay/{charid}")]
        public async Task<object> DelayCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsDelaying = true;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/undelay/{charid}")]
        public async Task<object> UndelayCharacter(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsDelaying = false;
                saveCallback();
                res.Data = state.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/combat/actnow/{charid}")]
        public async Task<object> CharacterActNow(string charid)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                state.CharacterActNow(ch);
                saveCallback();
                res.Data = state.ToRemote();

            });
        }


        [Route(HttpVerbs.Get, "/character/changehp/{charid}/{amount}")]
        public async Task<object> ChangeHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.HP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/character/changemaxhp/{charid}/{amount}")]
        public async Task<object> ChangeMaxHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.MaxHP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/character/changetemporaryhp/{charid}/{amount}")]
        public async Task<object> ChangeTemporaryHP(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.TemporaryHP += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }


        [Route(HttpVerbs.Get, "/character/changenonlethaldamage/{charid}/{amount}")]
        public async Task<object> ChangeNonlethalDamage(string charid, int amount)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.Adjuster.NonlethalDamage += amount;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/character/hide/{charid}/{state}")]
        public async Task<object> HideCharacter(string charid, bool state)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsHidden = state;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Get, "/character/idle/{charid}/{state}")]
        public async Task<object> IdleCharacter(string charid, bool state)
        {
            return await TakeCharacterAction(charid, (res, ch) =>
            {
                ch.IsIdle = state;
                saveCallback();
                res.Data = ch.ToRemote();

            });
        }


        [Route(HttpVerbs.Post, "/character/addcondition")]
        public async Task<object> AddCondition()
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


        [Route(HttpVerbs.Post, "/character/removecondition")]
        public async Task<object> RemoveCondition()
        {
            return await TakeCharacterPostAction<RemoveConditionRequest>((res, data, ch) =>
            {
                ch.RemoveConditionByName(data.Name);
                saveCallback();

                res.Data = ch.ToRemote();

            });
        }

        [Route(HttpVerbs.Post, "/monster/list")]
        public async Task<object> ListMonsters()
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

        [Route(HttpVerbs.Post, "/monster/get")]
        public async Task<object> GetMonster()
        {

            return await TakePostAction<MonsterRequest>((res, data) =>
            {
                 res.Data = Monster.ByID(data.IsCustom, data.ID).ToRemote();
                 
            });
        }

        [Route(HttpVerbs.Post, "/feat/get")]
        public async Task<object> GetFeat()
        {
            return await TakePostAction<FeatRequest>((res, data) =>
            {
                res.Data = Feat.ByID(data.IsCustom, data.ID).ToRemote();

            });
        }

        [Route(HttpVerbs.Post, "/spell/get")]
        public async Task<object> GetSpell()
        {
            return await TakePostAction<SpellRequest>((res, data) =>
            {
                res.Data = Spell.ByID(data.IsCustom, data.ID).ToRemote();

            });
        }

        [Route(HttpVerbs.Post, "/magicitem/get")]
        public async Task<object> GetMagicItem()
        {
            return await TakePostAction<MagicItemRequest>((res, data) =>
            {
                res.Data = MagicItem.ByID(data.IsCustom, data.ID).ToRemote();

            });
        }


        [Route(HttpVerbs.Get, "/monster/getregular/{id}")]
        public async Task<object> GetRegularMonster(int id)
        {
            return await TakeAction( (res) =>
            {
                res.Data = Monster.ByDetailsID(id).ToRemote();
            });
        }

        [Route(HttpVerbs.Get, "/ui/bringtofront")]
        public async Task<object> BringToFront()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.BringToFront);
                return await Task.FromResult(new { res = true });
            });
        }

        [Route(HttpVerbs.Get, "/ui/minimize")]
        public async Task<object> Minimize()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.Minimize);
                return await Task.FromResult(new { res = true });
            });
        }

        [Route(HttpVerbs.Get, "/ui/goto/{place}")]
        public async Task<object> UIGoto(string place)
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.Goto, place);
                return await Task.FromResult(new { res = true });
            });
        }

        [Route(HttpVerbs.Get, "/ui/showcombatlist")]
        public async Task<object> ShowCombatList()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.ShowCombatListWindow);
                return await Task.FromResult(new { res = true });
            });
        }


        [Route(HttpVerbs.Get, "/ui/hidecombatlist")]
        public async Task<object> HideCombatList()
        {

            return await Precheck(async () =>
            {
                service.TakeUIAction(LocalCombatManagerService.UIAction.HideCombatListWindow);
                return await Task.FromResult(new { res = true });
            });
        }



        [Route(HttpVerbs.Get, "/monster/getcustom/{id}")]
        public async Task<object> GetCustomMonster(int id)
        {
            return await TakeAction((res) =>
            {
                res.Data = Monster.ByDBLoaderID(id).ToRemote();
            });
        }

        [Route(HttpVerbs.Post, "/monster/fromlist")]
        public async Task<object> GetMonsters()
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

        [Route(HttpVerbs.Post, "/monster/add")]
        public async Task<object> AddMonster()
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

        

        private async Task<object> Precheck(Func<Task<object>> function)
        {
            if (!Passcode.IsEmptyOrNull())
            {
                if (!HttpContext.HasRequestHeader("passcode"))
                {
                    throw HttpException.Forbidden();                }
                else
                {
                    string matchcode = HttpContext.RequestHeader("Passcode");
                    if (matchcode != Passcode)
                    {
                        throw HttpException.Forbidden();
                    }
                }
            }
            return await function();
        }


        private async Task<object> TakeAction(Action<ResultHandler> resAction)
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
                        throw new HttpException(res.Code);
                    }
                    else
                    {
                        return await Task.FromResult(res.Data);
                    }
                }
                catch (Exception ex)
                {
                    throw new HttpException(System.Net.HttpStatusCode.InternalServerError, ex.ToString());
                }
            });
        }

        private async Task<object> TakeCharacterAction(string charid, Action<ResultHandler, Character> handler)
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

        private async Task<object> TakePostAction<T>(Action<ResultHandler, T> resAction) where T : class
        {

            return await Precheck(async () =>
            {
                try
                {
                    ResultHandler res = new ResultHandler();
                    T data = await HttpContext.GetRequestDataAsync<T>();

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
                        throw new HttpException(res.Code, new ArgumentException().ToString());
                    }
                    else
                    {
                        return res.Data;
                    }

                }
                catch (Exception ex)
                {
                    throw new HttpException(System.Net.HttpStatusCode.InternalServerError, ex.ToString());
                }
            });
        }

        private async Task<object> TakeCharacterPostAction<T>(Action<ResultHandler, T, Character> resAction) where T : CharacterRequest
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

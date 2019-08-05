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


        public LocalCombatManagerServiceController(IHttpContext context, CombatState state, LocalCombatManagerService.ActionCallback actionCallback)
            : base(context)
        {
            this.state = state;
            this.actionCallback = actionCallback;
        }

        private class ResultHandler
        {
            public object Data { get; set; }
            public bool Failed { get; set; }
            
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/state")]
        public async Task<bool> GetCombatState()
        {
            try
            {
                ResultHandler res = new ResultHandler();

                actionCallback(() =>
                {
                    res.Data = CreateRemoteCombatState(state);
                });

                return await Ok(res.Data);
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/next")]
        public async Task<bool> CombatNext()
        {
            try
            {
                ResultHandler res = new ResultHandler();

                actionCallback(() =>
                {
                    state.MoveNext();
                    res.Data = CreateRemoteCombatState(state);
                });
                return await Ok(res.Data);
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/prev")]
        public async Task<bool> CombatPrev()
        {
            try
            {
                ResultHandler res = new ResultHandler();

                actionCallback(() =>
                {
                    state.MovePrevious();
                    res.Data = CreateRemoteCombatState(state);
                });
                return await Ok(res.Data);
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        [WebApiHandler(HttpVerbs.Get, "/api/combat/rollinit")]
        public async Task<bool> CombatRollInit()
        {
            try
            {
                ResultHandler res = new ResultHandler();

                actionCallback(() =>
                {
                    state.RollInitiative();
                    state.SortCombatList();
                    res.Data = CreateRemoteCombatState(state);
                });
                return await Ok(res.Data);
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        [WebApiHandler(HttpVerbs.Get, "/api/character/details/{ids}")]
        public async Task<bool> CombatRollInit(String ids)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(ids, out id))
                {

                    ResultHandler res = new ResultHandler();

                    actionCallback(() =>
                    {
                        Character ch = state.GetCharacterByID(id);
                        if (ch != null)
                        {
                            res.Data = CreateRemoteCharacter(ch);
                        }
                        else
                        {
                            res.Failed = true;
                        }

                    });
                    if (res.Failed)
                    {

                        return await InternalServerError(new ArgumentException(), System.Net.HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        return await Ok(res.Data);
                    }
                }
                else

                {
                    return await InternalServerError(new ArgumentException(), System.Net.HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        public RemoteCombatState CreateRemoteCombatState(CombatState state)
        {
            RemoteCombatState remoteState = new RemoteCombatState();
            remoteState.CR = state.CR;
            remoteState.Round = state.Round;
            remoteState.XP = state.XP;
            remoteState.RulesSystem = state.RulesSystemInt;
            remoteState.CurrentInitiativeCount = CreateRemoteInitiativeCount(state.CurrentInitiativeCount);
            remoteState.CurrentCharacterID = state.CurrentCharacterID;
            remoteState.CombatList = new List<RemoteCharacterInitState>();

            foreach (Character character in state.CombatList)
            {
                remoteState.CombatList.Add(CreateRemoteCharacterInitState(character));
            }

            return remoteState;

        }


        public RemoteInitiativeCount CreateRemoteInitiativeCount(InitiativeCount count)
        {
            if (count == null)

            {
                return null;
            }

            RemoteInitiativeCount remoteCount = new RemoteInitiativeCount();

            remoteCount.Base = count.Base;
            remoteCount.Dex = count.Dex;
            remoteCount.Tiebreaker = count.Tiebreaker;

            return remoteCount;
        }

        public RemoteCharacterInitState CreateRemoteCharacterInitState(Character character)
        {
            RemoteCharacterInitState initState = new RemoteCharacterInitState();

            initState.ID = character.ID;
            initState.InitiativeCount = CreateRemoteInitiativeCount(character.InitiativeCount);
            initState.Name = character.Name;
   

            return initState;
        }

        public RemoteCharacter CreateRemoteCharacter(Character character)
        {
            if (character == null)
            {
                return null;
            }

            RemoteCharacter remoteCharacter = new RemoteCharacter();

            remoteCharacter.ID = character.ID;
            remoteCharacter.Name = character.Name;
            remoteCharacter.HP = character.HP;
            remoteCharacter.MaxHP = character.MaxHP;
            remoteCharacter.NonlethalDamage = character.NonlethalDamage;
            remoteCharacter.TemporaryHP = character.TemporaryHP;
            remoteCharacter.Notes = character.Notes;
            remoteCharacter.IsMonster = character.IsMonster;
            remoteCharacter.IsReadying = character.IsReadying;
            remoteCharacter.IsDelaying = character.IsDelaying;
            remoteCharacter.Color = character.Color;
            remoteCharacter.IsActive = character.IsActive;
            remoteCharacter.IsIdle = character.IsIdle;
            remoteCharacter.InitiativeCount = CreateRemoteInitiativeCount(character.InitiativeCount);
            remoteCharacter.InitiativeRolled = character.InitiativeRolled;
                remoteCharacter.InitiativeLeader = character.InitiativeLeader?.InitiativeLeaderID;

            remoteCharacter.InitiativeFollowers = new List<Guid>();
            foreach (Character c in character.InitiativeFollowers)
            {
                remoteCharacter.InitiativeFollowers.Add(c.ID);

            }
            remoteCharacter.Monster = CreateRemoteMonster(character.Monster);
            return remoteCharacter;
        }

        public RemoteMonster CreateRemoteMonster(Monster monster)
        {
            RemoteMonster remoteMonster = new RemoteMonster();

            if (monster == null)
            {
                return null;
            }

            remoteMonster.Name = monster.Name;
            remoteMonster.CR = monster.CR;
            remoteMonster.XP = monster.XP;
            remoteMonster.Race = monster.Race;
            remoteMonster.ClassName = monster.Class;
            remoteMonster.Alignment = monster.Alignment;
            remoteMonster.Size = monster.Size;
            remoteMonster.Type = monster.Type;
            remoteMonster.SubType = monster.SubType;
            remoteMonster.Init = monster.Init;
            remoteMonster.DualInit = monster.DualInit;
            remoteMonster.Senses = monster.Senses;
            remoteMonster.AC = monster.FullAC;
            remoteMonster.ACMods = monster.AC_Mods;
            remoteMonster.HP = monster.HP;
            remoteMonster.HDText = monster.HD;
            remoteMonster.HD = CreateRemoteDieRoll(monster.Adjuster.HD);
            remoteMonster.Fort = monster.Fort ;
            remoteMonster.Ref = monster.Ref;
            remoteMonster.Will = monster.Will;
            remoteMonster.SaveMods = monster.Save_Mods;
            remoteMonster.Resist = monster.Resist;
            remoteMonster.DR = monster.DR;
            remoteMonster.SR = monster.SR;
            remoteMonster.Speed = monster.Speed;
            remoteMonster.Melee = monster.Melee;
            remoteMonster.Ranged = monster.Ranged;
            remoteMonster.Space = monster.Adjuster.Space;
            remoteMonster.Reach = monster.Adjuster.Reach;
            remoteMonster.SpecialAttacks = monster.SpecialAttacks;
            remoteMonster.SpellLikeAbilities = monster.SpellLikeAbilities;
            remoteMonster.Strength = monster.Strength;
            remoteMonster.Dexterity = monster.Dexterity;
            remoteMonster.Constitution = monster.Constitution;
            remoteMonster.Intelligence = monster.Intelligence;
            remoteMonster.Wisdom = monster.Wisdom;
            remoteMonster.Charisma = monster.Charisma;
            remoteMonster.BaseAtk = monster.BaseAtk;
            remoteMonster.CMB = monster.CMB_Numeric;
            remoteMonster.CMD = monster.CMD_Numeric;
            remoteMonster.Feats = monster.Feats;
            remoteMonster.Skills = monster.Skills;
            remoteMonster.RacialMods = monster.RacialMods;
            remoteMonster.Languages = monster.Languages;
            remoteMonster.SQ = monster.SQ;
            remoteMonster.Environment = monster.Environment;
            remoteMonster.Organization = monster.Organization;
            remoteMonster.Treasure = monster.Treasure;
            remoteMonster.DescriptionVisual = monster.Description_Visual;
            remoteMonster.Group = monster.Group;
            remoteMonster.Source = monster.Source;
            remoteMonster.IsTemplate = monster.IsTemplate;
            remoteMonster.SpecialAbilities = monster.SpecialAbilities;
            remoteMonster.Description = monster.Description;
            remoteMonster.FullText = monster.FullText;
            remoteMonster.Gender = monster.Gender;
            remoteMonster.Bloodline = monster.Bloodline;
            remoteMonster.ProhibitedSchools = monster.ProhibitedSchools;
            remoteMonster.BeforeCombat = monster.BeforeCombat;
            remoteMonster.DuringCombat = monster.DuringCombat;
            remoteMonster.Morale = monster.Morale;
            remoteMonster.Gear = monster.Gear;
            remoteMonster.OtherGear = monster.OtherGear;
            remoteMonster.Vulnerability = monster.Vulnerability;
            remoteMonster.Note = monster.Note;
            remoteMonster.CharacterFlag = monster.CharacterFlag;
            remoteMonster.CompanionFlag = monster.CompanionFlag;
            remoteMonster.FlySpeed = monster.Adjuster.FlySpeed;
            remoteMonster.ClimbSpeed = monster.Adjuster.ClimbSpeed;
            remoteMonster.BurrowSpeed = monster.Adjuster.BurrowSpeed;
            remoteMonster.SwimSpeed = monster.Adjuster.SwimSpeed;
            remoteMonster.LandSpeed = monster.Adjuster.LandSpeed;
            remoteMonster.TemplatesApplied = monster.TemplatesApplied;
            remoteMonster.OffenseNote = monster.OffenseNote;
            remoteMonster.BaseStatistics = monster.BaseStatistics;
            remoteMonster.SpellsPrepared = monster.SpellsPrepared;
            remoteMonster.SpellDomains = monster.SpellDomains;
            remoteMonster.Aura = monster.Aura;
            remoteMonster.DefensiveAbilities = monster.DefensiveAbilities;
            remoteMonster.Immune = monster.Immune;
            remoteMonster.HPMods = monster.HP_Mods;
            remoteMonster.SpellsKnown = monster.SpellsKnown;
            remoteMonster.Weaknesses = monster.Weaknesses;
            remoteMonster.SpeedMod = monster.Speed_Mod;
            remoteMonster.MonsterSource = monster.MonsterSource;
            remoteMonster.ExtractsPrepared = monster.ExtractsPrepared;
            remoteMonster.AgeCategory = monster.AgeCategory;
            remoteMonster.DontUseRacialHD = monster.DontUseRacialHD;
            remoteMonster.VariantParent = monster.VariantParent;
            remoteMonster.NPC = monster.NPC;
            remoteMonster.MR = monster.MR;
            remoteMonster.Mythic = monster.Mythic;

            remoteMonster.TouchAC = monster.TouchAC;
            remoteMonster.FlatFootedAC = monster.FlatFootedAC;
            remoteMonster.NaturalArmor = monster.NaturalArmor;
            remoteMonster.Shield = monster.Shield;
            remoteMonster.Armor = monster.Armor;
            remoteMonster.Dodge = monster.Dodge;
            remoteMonster.Deflection = monster.Deflection;
            remoteMonster.ActiveConditions = new List<RemoteActiveCondition>();
            foreach (var ac in monster.ActiveConditions)
            {
                remoteMonster.ActiveConditions.Add(CreateRemoteActiveCondition(ac));

            }

            return remoteMonster;
        }

        public RemoteDieRoll CreateRemoteDieRoll(DieRoll roll)
        {
            if (roll == null)
            {
                return null;
            }

            RemoteDieRoll remoteRoll = new RemoteDieRoll();
            remoteRoll.Dice = new List<RemoteDie>();
            remoteRoll.Mod = roll.mod;
            remoteRoll.Fraction = roll.fraction;
            foreach (DieStep step in roll.AllRolls)
            {
                remoteRoll.Dice.Add(CreateRemoteDie(step));
            }
            return remoteRoll;
        }

        public RemoteDie CreateRemoteDie(DieStep step)
        {
            if (step == null)
            {
                return null;
            }

            RemoteDie die = new RemoteDie();
            die.Die = step.Die;
            die.Count = step.Count;
            return die;
        }

        public RemoteRollResult CreateRemoteRollResult(RollResult rollResult)
        {
            if (rollResult == null)
            {
                return null;
            }

            RemoteRollResult remoteResult = new RemoteRollResult();

            remoteResult.Total = rollResult.Total;
            remoteResult.Mod = rollResult.Mod;
            remoteResult.Rolls = new List<RemoteDieResult>();
            foreach (DieResult roll in rollResult.Rolls)
            {
                remoteResult.Rolls.Add(CreateRemoteDieResult(roll));
            }

            return remoteResult;

        }

        public RemoteDieResult CreateRemoteDieResult(DieResult result)
        {
            if (result == null)
            {
                return null;
            }

            return new RemoteDieResult() { Die = result.Die, Result = result.Result };


        }

        public RemoteActiveCondition CreateRemoteActiveCondition(ActiveCondition activeCondition)
        {
            if (activeCondition == null)
            {
                return null;
            }

            RemoteActiveCondition remoteActiveCondition = new RemoteActiveCondition();
            remoteActiveCondition.Conditon = CreateRemoteCondition(activeCondition.Condition);
            remoteActiveCondition.Details = activeCondition.Details;
            remoteActiveCondition.InitiativeCount = CreateRemoteInitiativeCount(activeCondition.InitiativeCount);
            remoteActiveCondition.Turns = activeCondition.Turns;

            return remoteActiveCondition;

        }

        public RemoteConditon CreateRemoteCondition(Condition condition)
        {
            if (condition == null)
            {
                return null;
            }

            RemoteConditon remoteCondition = new RemoteConditon();
            remoteCondition.Name = condition.Name;
            remoteCondition.Text = condition.Text;
            remoteCondition.Image = condition.Image;
            remoteCondition.Spell = CreateRemoteSpell(condition.Spell);
            remoteCondition.Affliction = CreateRemoteAffliction(condition.Affliction);
            remoteCondition.Bonus = CreateRemoteBonus(condition.Bonus);
            remoteCondition.Custom = condition.Custom;


            return remoteCondition;
        }

        public RemoteBonus CreateRemoteBonus(ConditionBonus bonus)
        {
            if (bonus == null)
            {
                return null;
            }

            

            RemoteBonus remoteBonus = new RemoteBonus();

            remoteBonus.Str = bonus.Str;
            remoteBonus.Dex = bonus.Dex;
            remoteBonus.Con = bonus.Con;
            remoteBonus.Int = bonus.Int;
            remoteBonus.Wis = bonus.Wis;
            remoteBonus.Cha = bonus.Cha;
            remoteBonus.StrSkill = bonus.StrSkill;
            remoteBonus.DexSkill = bonus.DexSkill;
            remoteBonus.ConSkill = bonus.ConSkill;
            remoteBonus.IntSkill = bonus.IntSkill;
            remoteBonus.WisSkill = bonus.WisSkill;
            remoteBonus.ChaSkill = bonus.ChaSkill;
            remoteBonus.Dodge = bonus.Dodge;
            remoteBonus.Armor = bonus.Armor;
            remoteBonus.Shield = bonus.Shield;
            remoteBonus.NaturalArmor = bonus.NaturalArmor;
            remoteBonus.Deflection = bonus.Deflection;
            remoteBonus.AC = bonus.AC;
            remoteBonus.Initiative = bonus.Initiative;
            remoteBonus.AllAttack = bonus.AllAttack;
            remoteBonus.MeleeAttack = bonus.MeleeAttack;
            remoteBonus.RangedAttack = bonus.RangedAttack;
            remoteBonus.AttackDamage = bonus.AttackDamage;
            remoteBonus.MeleeDamage = bonus.MeleeDamage;
            remoteBonus.RangedDamage = bonus.RangedDamage;
            remoteBonus.Perception = bonus.Perception;
            remoteBonus.LoseDex = bonus.LoseDex;
            remoteBonus.Size = bonus.Size;
            remoteBonus.Fort = bonus.Fort;
            remoteBonus.Ref = bonus.Ref;
            remoteBonus.Will = bonus.Will;
            remoteBonus.AllSaves = bonus.AllSaves;
            remoteBonus.AllSkills = bonus.AllSkills;
            remoteBonus.CMB = bonus.CMB;
            remoteBonus.CMD = bonus.CMD;
            remoteBonus.StrZero = bonus.StrZero;
            remoteBonus.DexZero = bonus.DexZero;
            remoteBonus.HP = bonus.HP;
            

            return remoteBonus;

        }

        public RemoteAffliction CreateRemoteAffliction(Affliction affliction)
        {
            if (affliction == null)
            {
                return null;
            }

            RemoteAffliction remoteAffliction = new RemoteAffliction();

            remoteAffliction.Name = affliction.Name;
            remoteAffliction.Type = affliction.Type;
            remoteAffliction.Cause = affliction.Cause;
            remoteAffliction.SaveType = affliction.SaveType;
            remoteAffliction.Save = affliction.Save;
            remoteAffliction.Onset = CreateRemoteDieRoll(affliction.Onset);
            remoteAffliction.OnsetUnit = affliction.OnsetUnit;
            remoteAffliction.Immediate = affliction.Immediate;
            remoteAffliction.Frequency = affliction.Frequency;
            remoteAffliction.FrequencyUnit = affliction.FrequencyUnit;
            remoteAffliction.Limit = affliction.Limit;
            remoteAffliction.LimitUnit = affliction.LimitUnit;
            remoteAffliction.SpecialEffectName = affliction.SpecialEffectName;
            remoteAffliction.SpecialEffectTime = CreateRemoteDieRoll(affliction.SpecialEffectTime);
            remoteAffliction.SpecialEffectUnit = affliction.SpecialEffectUnit;
            remoteAffliction.OtherEffect = affliction.OtherEffect;
            remoteAffliction.Once = affliction.Once;
            remoteAffliction.DamageDie = CreateRemoteDieRoll(affliction.DamageDie);
            remoteAffliction.DamageType = affliction.DamageType;
            remoteAffliction.IsDamageDrain = affliction.IsDamageDrain;
            remoteAffliction.SecondaryDamageDie = CreateRemoteDieRoll(affliction.SecondaryDamageDie);
            remoteAffliction.SecondaryDamageType = affliction.SecondaryDamageType;
            remoteAffliction.IsSecondaryDamageDrain = affliction.IsSecondaryDamageDrain;
            remoteAffliction.DamageExtra = affliction.DamageExtra;
            remoteAffliction.Cure = affliction.Cure;
            remoteAffliction.Details = affliction.Details;
            remoteAffliction.Cost = affliction.Cost;

            return remoteAffliction;
        }

        RemoteSpell CreateRemoteSpell(Spell spell)
        {
            if (spell == null)
            {
                return null;
            }

            RemoteSpell remoteSpell = new RemoteSpell();


            remoteSpell.Name = spell.name;
            remoteSpell.School = spell.school;
            remoteSpell.Subschool = spell.subschool;
            remoteSpell.Descriptor = spell.descriptor;
            remoteSpell.SpellLevel = spell.spell_level;
            remoteSpell.CastingTime = spell.casting_time;
            remoteSpell.Components = spell.components;
            remoteSpell.CostlyComponents = spell.costly_components;
            remoteSpell.Range = spell.range;
            remoteSpell.Targets = spell.targets;
            remoteSpell.Effect = spell.effect;
            remoteSpell.Dismissible = spell.dismissible;
            remoteSpell.Area = spell.area;
            remoteSpell.Duration = spell.duration;
            remoteSpell.Shapeable = spell.shapeable;
            remoteSpell.SavingThrow = spell.saving_throw;
            remoteSpell.SpellResistance = spell.spell_resistence;
            remoteSpell.Description = spell.description;
            remoteSpell.DescriptionFormated = spell.description_formated;
            remoteSpell.Source = spell.source;
            remoteSpell.FullText = spell.full_text;
            remoteSpell.Verbal = spell.verbal;
            remoteSpell.Somatic = spell.somatic;
            remoteSpell.Material = spell.material;
            remoteSpell.Focus = spell.focus;
            remoteSpell.DivineFocus = spell.divine_focus;
            remoteSpell.Sor = spell.sor;
            remoteSpell.Wiz = spell.wiz;
            remoteSpell.Cleric = spell.cleric;
            remoteSpell.Druid = spell.druid;
            remoteSpell.Ranger = spell.ranger;
            remoteSpell.Bard = spell.bard;
            remoteSpell.Paladin = spell.paladin;
            remoteSpell.Alchemist = spell.alchemist;
            remoteSpell.Summoner = spell.summoner;
            remoteSpell.Witch = spell.witch;
            remoteSpell.Inquisitor = spell.inquisitor;
            remoteSpell.Oracle = spell.oracle;
            remoteSpell.Antipaladin = spell.antipaladin;
            remoteSpell.Assassin = spell.assassin;
            remoteSpell.Adept = spell.adept;
            remoteSpell.RedMantisAssassin = spell.red_mantis_assassin;
            remoteSpell.Magus = spell.magus;
            remoteSpell.URL = spell.URL;
            remoteSpell.SLA_Level = spell.SLA_Level;
            remoteSpell.PreparationTime = spell.preparation_time;
            remoteSpell.Duplicated = spell.duplicated;
            remoteSpell.Acid = spell.acid;
            remoteSpell.Air = spell.air;
            remoteSpell.Chaotic = spell.chaotic;
            remoteSpell.Cold = spell.cold;
            remoteSpell.Curse = spell.curse;
            remoteSpell.Darkness = spell.darkness;
            remoteSpell.Death = spell.death;
            remoteSpell.Disease = spell.disease;
            remoteSpell.Earth = spell.earth;
            remoteSpell.Electricity = spell.electricity;
            remoteSpell.Emotion = spell.emotion;
            remoteSpell.Evil = spell.evil;
            remoteSpell.Fear = spell.fear;
            remoteSpell.Fire = spell.fire;
            remoteSpell.Force = spell.force;
            remoteSpell.Good = spell.good;
            remoteSpell.Language = spell.language;
            remoteSpell.Lawful = spell.lawful;
            remoteSpell.Light = spell.light;
            remoteSpell.MindAffecting = spell.mind_affecting;
            remoteSpell.Pain = spell.pain;
            remoteSpell.Poison = spell.poison;
            remoteSpell.Shadow = spell.shadow;
            remoteSpell.Sonic = spell.sonic;
            remoteSpell.Water = spell.water;

            return remoteSpell;
        }
    }

}

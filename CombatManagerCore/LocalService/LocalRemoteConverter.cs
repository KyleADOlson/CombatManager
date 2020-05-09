using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public static class LocalRemoteConverter
    {
        public static RemoteCombatState ToRemote(this CombatState state)
        {
            RemoteCombatState remoteState = new RemoteCombatState();
            remoteState.CR = state.CR;
            remoteState.Round = state.Round;
            remoteState.XP = state.XP;
            remoteState.RulesSystem = state.RulesSystemInt;
            if (state.CurrentInitiativeCount != null)
            {
                remoteState.CurrentInitiativeCount = state.CurrentInitiativeCount.ToRemote();
            }
            remoteState.CurrentCharacterID = state.CurrentCharacterID;
            remoteState.CombatList = new List<RemoteCharacterInitState>();

            foreach (Character character in state.CombatList)
            {
                var v = character.ToRemoteInit();
                if (v != null)
                {
                    remoteState.CombatList.Add(v);
                }
            }

            return remoteState;

        }


        public static RemoteInitiativeCount ToRemote( this InitiativeCount count)
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

        public static RemoteCharacterInitState ToRemoteInit(this Character character)
        {
            if (character == null)
            {
                return null;
            }
            RemoteCharacterInitState initState = new RemoteCharacterInitState();

            initState.ID = character.ID;
            if (character.InitiativeCount != null)
            {
                initState.InitiativeCount = character.InitiativeCount.ToRemote();
            }
            initState.Name = character.Name;
            initState.HP = character.HP;
            initState.MaxHP = character.MaxHP;
            initState.IsMonster = character.IsMonster;
            initState.IsHidden = character.IsHidden;
            initState.IsActive = character.IsActive;
            if (character.Monster != null && character.Monster.ActiveConditions != null && character.Monster.ActiveConditions.Count > 0)
            {
                initState.ActiveConditions = new List<RemoteActiveCondition>();
                foreach (ActiveCondition c in character.Monster.ActiveConditions)
                {
                    if (c != null)
                    {
                        initState.ActiveConditions.Add(c.ToRemote());
                    }
                }
            }


            return initState;
        }

        public static RemoteCharacter ToRemote(this Character character)
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
            remoteCharacter.InitiativeCount = character.InitiativeCount.ToRemote();
            remoteCharacter.InitiativeRolled = character.InitiativeRolled;
            remoteCharacter.InitiativeLeader = character.InitiativeLeader?.InitiativeLeaderID;

            remoteCharacter.InitiativeFollowers = new List<Guid>();
            foreach (Character c in character.InitiativeFollowers)
            {
                remoteCharacter.InitiativeFollowers.Add(c.ID);

            }
            remoteCharacter.Monster = character.Monster.ToRemote();
            return remoteCharacter;
        }

        public static RemoteMonster ToRemote(this Monster monster)
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
            remoteMonster.SubType = monster.Adjuster.Subtype;
            remoteMonster.Init = monster.Init;
            remoteMonster.DualInit = monster.DualInit;
            remoteMonster.Senses = monster.Senses;
            remoteMonster.AC = monster.FullAC;
            remoteMonster.ACMods = monster.AC_Mods;
            remoteMonster.HP = monster.HP;
            remoteMonster.HDText = monster.HD;
            remoteMonster.HD = monster.Adjuster.HD.ToRemote();
            remoteMonster.Fort = monster.Fort;
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
                remoteMonster.ActiveConditions.Add(ac.ToRemote());

            }

            return remoteMonster;
        }

        public static RemoteFeat ToRemote(this Feat feat)
        {

            if (feat == null)
            {
                return null;
            }
            RemoteFeat remoteFeat = new RemoteFeat();

            remoteFeat.Name = feat.Name;
            remoteFeat.AltName = feat.AltName;
            remoteFeat.Type = feat.Type;
            remoteFeat.Prerequistites = feat.Prerequistites;
            remoteFeat.Summary = feat.Summary;
            remoteFeat.Source = feat.Source;
            remoteFeat.System = feat.System;
            remoteFeat.License = feat.License;
            remoteFeat.URL = feat.URL;
            remoteFeat.Detail = feat.Detail;
            remoteFeat.Benefit = feat.Benefit;
            remoteFeat.Normal = feat.Normal;
            remoteFeat.Special = feat.Special;
            remoteFeat.DBLoaderID = feat.DBLoaderID;
            remoteFeat.ID = feat.Id;

            return remoteFeat;

        }

        public static RemoteMagicItem ToRemote(this MagicItem magicItem)
        {
            if (magicItem == null)
            {
                return null;
            }
            RemoteMagicItem remoteMagicItem = new RemoteMagicItem();
            remoteMagicItem.DetailsID = magicItem.DetailsID;
            remoteMagicItem.Name = magicItem.Name;
            remoteMagicItem.Aura = magicItem.Aura;
            remoteMagicItem.CL = magicItem.CL;
            remoteMagicItem.Slot = magicItem.Slot;
            remoteMagicItem.Price = magicItem.Price;
            remoteMagicItem.Weight = magicItem.Weight;
            remoteMagicItem.Description = magicItem.Description;
            remoteMagicItem.Requirements = magicItem.Requirements;
            remoteMagicItem.Cost = magicItem.Cost;
            remoteMagicItem.Group = magicItem.Group;
            remoteMagicItem.Source = magicItem.Source;
            remoteMagicItem.FullText = magicItem.FullText;
            remoteMagicItem.Destruction = magicItem.Destruction;
            remoteMagicItem.MinorArtifactFlag = magicItem.MinorArtifactFlag;
            remoteMagicItem.MajorArtifactFlag = magicItem.MajorArtifactFlag;
            remoteMagicItem.Abjuration = magicItem.Abjuration;
            remoteMagicItem.Conjuration = magicItem.Conjuration;
            remoteMagicItem.Divination = magicItem.Divination;
            remoteMagicItem.Enchantment = magicItem.Enchantment;
            remoteMagicItem.Evocation = magicItem.Evocation;
            remoteMagicItem.Necromancy = magicItem.Necromancy;
            remoteMagicItem.Transmutation = magicItem.Transmutation;
            remoteMagicItem.AuraStrength = magicItem.AuraStrength;
            remoteMagicItem.WeightValue = magicItem.WeightValue;
            remoteMagicItem.PriceValue = magicItem.PriceValue;
            remoteMagicItem.CostValue = magicItem.CostValue;
            remoteMagicItem.AL = magicItem.AL;
            remoteMagicItem.Int = magicItem.Int;
            remoteMagicItem.Wis = magicItem.Wis;
            remoteMagicItem.Cha = magicItem.Cha;
            remoteMagicItem.Ego = magicItem.Ego;
            remoteMagicItem.Communication = magicItem.Communication;
            remoteMagicItem.Senses = magicItem.Senses;
            remoteMagicItem.Powers = magicItem.Powers;
            remoteMagicItem.MagicItems = magicItem.MagicItems;
            remoteMagicItem.DescHTML = magicItem.DescHTML;
            remoteMagicItem.Mythic = magicItem.Mythic;
            remoteMagicItem.LegendaryWeapon = magicItem.LegendaryWeapon;

            return remoteMagicItem;
        }

        public static RemoteDieRoll ToRemote(this DieRoll roll)
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
                remoteRoll.Dice.Add(step.ToRemote());
            }
            return remoteRoll;
        }

        public static RemoteDie ToRemote(this DieStep step)
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

        public static RemoteRollResult ToRemote(this RollResult rollResult)
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
                remoteResult.Rolls.Add(roll.ToRemote());
            }

            return remoteResult;

        }

        public static RemoteDieResult ToRemote(this DieResult result)
        {
            if (result == null)
            {
                return null;
            }

            return new RemoteDieResult() { Die = result.Die, Result = result.Result };


        }

        public static RemoteActiveCondition ToRemote(this ActiveCondition activeCondition)
        {
            if (activeCondition == null)
            {
                return null;
            }

            RemoteActiveCondition remoteActiveCondition = new RemoteActiveCondition();
            remoteActiveCondition.Conditon = activeCondition.Condition.ToRemote();
            remoteActiveCondition.Details = activeCondition.Details;
            remoteActiveCondition.InitiativeCount = activeCondition.InitiativeCount.ToRemote();
            remoteActiveCondition.Turns = activeCondition.Turns;

            return remoteActiveCondition;

        }

        public static RemoteConditon ToRemote(this Condition condition)
        {
            if (condition == null)
            {
                return null;
            }

            RemoteConditon remoteCondition = new RemoteConditon();
            remoteCondition.Name = condition.Name;
            remoteCondition.Text = condition.Text;
            remoteCondition.Image = condition.Image;
            remoteCondition.Spell = condition.Spell.ToRemote();
            remoteCondition.Affliction = condition.Affliction.ToRemote();
            remoteCondition.Bonus = condition.Bonus.ToRemote();
            remoteCondition.Custom = condition.Custom;


            return remoteCondition;
        }

        public static RemoteBonus ToRemote(this ConditionBonus bonus)
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

        public static RemoteAffliction ToRemote(this Affliction affliction)
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
            remoteAffliction.Onset = affliction.Onset.ToRemote(); ;
            remoteAffliction.OnsetUnit = affliction.OnsetUnit;
            remoteAffliction.Immediate = affliction.Immediate;
            remoteAffliction.Frequency = affliction.Frequency;
            remoteAffliction.FrequencyUnit = affliction.FrequencyUnit;
            remoteAffliction.Limit = affliction.Limit;
            remoteAffliction.LimitUnit = affliction.LimitUnit;
            remoteAffliction.SpecialEffectName = affliction.SpecialEffectName;
            remoteAffliction.SpecialEffectTime = affliction.SpecialEffectTime.ToRemote() ;
            remoteAffliction.SpecialEffectUnit = affliction.SpecialEffectUnit;
            remoteAffliction.OtherEffect = affliction.OtherEffect;
            remoteAffliction.Once = affliction.Once;
            remoteAffliction.DamageDie = affliction.DamageDie.ToRemote();
            remoteAffliction.DamageType = affliction.DamageType;
            remoteAffliction.IsDamageDrain = affliction.IsDamageDrain;
            remoteAffliction.SecondaryDamageDie = affliction.SecondaryDamageDie.ToRemote();
            remoteAffliction.SecondaryDamageType = affliction.SecondaryDamageType;
            remoteAffliction.IsSecondaryDamageDrain = affliction.IsSecondaryDamageDrain;
            remoteAffliction.DamageExtra = affliction.DamageExtra;
            remoteAffliction.Cure = affliction.Cure;
            remoteAffliction.Details = affliction.Details;
            remoteAffliction.Cost = affliction.Cost;

            return remoteAffliction;
        }

        public static RemoteSpell ToRemote(this Spell spell)
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

        public delegate bool MonsterFilter(Monster monster);

        public static RemoteDBListing CreateRemoteMonsterList(MonsterFilter filter)
        {
            RemoteDBListing listing = new RemoteDBListing();
            listing.Items = new List<RemoteDBItem>();
            foreach (Monster ms in from m in Monster.Monsters where filter(m) select m)
            {
                listing.Items.Add(ms.ToDBItem());
            }

            return listing;
        }

        public static RemoteDBItem ToDBItem(this Monster monster)
        {
            RemoteDBItem item = new RemoteDBItem();
            item.Name = monster.Name;
            item.ID = monster.IsCustom ? monster.DBLoaderID : monster.DetailsID;
            item.IsCustom = monster.IsCustom;

            return item;
        }
    }
}

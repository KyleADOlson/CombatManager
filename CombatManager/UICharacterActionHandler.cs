using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    public enum CharacterAction
    {
        MeleeAttack = 0,
        RangedAttack = 1,
        Save = 2,
        Skill = 3,
        ApplyCondition = 4,
        EditAttacks = 5,
        EditFeats = 6,
        EditMenu = 7,
        EditMonster = 8,
        CopyToCustom = 9,
        ConditionMenu = 10,
        DamageHeal = 11,
        RollInitiative = 12,
        UnlinkFromGroup = 13,
        GroupAll = 14,
        Clone = 15,
        Delay = 16,
        Ready = 17,
        Clear = 18,
        ActNow = 19,
        Idle = 20,
        WakeUp = 21,
        Hide = 22,
        Unhide = 23,
        Delete = 24,
        NextTurn = 25,
        PreviousTurn = 26

    }



    public static class UICharacterActionHandler
    {
        public class ActionData
        {
            public ActionData() { }
            public ActionData(Character primary, List<Character> characters, CharacterAction action, String subtype, MainWindow parent)
            {
                Primary = primary;
                Characters = characters;
                Action = action;
                Subtype = subtype;
                Parent = parent;
            }


            public Character Primary { get; set; }
            public List<Character> Characters { get; set; }
            public CharacterAction Action { get; set; }
            public String Subtype { get; set; }
            public MainWindow Parent { get; set; }

            public bool HasPrimary
            {
                get
                {
                    return Primary != null;
                }
            }

            public bool HasCharacters
            {
                get
                {
                    return Characters != null && Characters.Count > 0;
                }
            }

            public void ActForEach(Action<Character, ActionData> action)
            {
                if (HasCharacters)
                {
                    foreach (Character c in Characters)
                    {
                        action(c, this);
                    }
                }
            }

            public  void ActForEach(Action<Character> action)
            {

                if (HasCharacters)
                {
                    foreach (Character c in Characters)
                    {
                        action(c);
                    }
                }
            }

            public void ActOnAll(Action<List<Character>> action)
            {

                if (HasCharacters)
                {
                    action(Characters);
                }

            }

            public void ActOnPrimary(Action<Character> action)
            {

                if (Primary != null)
                {
                    action(Primary);
                }
                

            }


        }

        public static void HandleAction(Character primary, List<Character> characters, CharacterAction action, String subtype, MainWindow parent)
        {
            ActionData data = new ActionData(primary, characters, action, subtype, parent);

            switch (action)
            {
                case CharacterAction.MeleeAttack:
                    HandleMeleeAttack(data);

                    break;
                case CharacterAction.RangedAttack:
                    HandleRangedAttack(data);

                    break;
                case CharacterAction.Save:
                    HandleSave(data);
                    break;
                case CharacterAction.Skill:
                    HandleSkill(data);
                    break;
                case CharacterAction.ApplyCondition:
                    HandleApplyCondition(data);
                    break;
                case CharacterAction.EditAttacks:
                    HandleEditAttacks(data);
                    break;
                case CharacterAction.EditFeats:
                    HandleEditFeats(data);
                    break;
                case CharacterAction.EditMenu:
                    break;
                case CharacterAction.EditMonster:
                    HandleEditMonster(data);
                    break;
                case CharacterAction.CopyToCustom:
                    HandleCopyToCustom(data);
                    break;
                case CharacterAction.ConditionMenu:
                    HandleConditionMenu(data);
                break;
                case CharacterAction.DamageHeal:
                    HandleDamageHeal(data);
                break;
                case CharacterAction.RollInitiative:
                    HandleRollInitiative(data);
                break;
                case CharacterAction.UnlinkFromGroup:
                    HandleUnlinkFromGroup(data);
                break;
                case CharacterAction.GroupAll:
                    HandleGroupAll(data);
                break;
                case CharacterAction.Clone:
                    HandleClone(data);
                break;
                case CharacterAction.Delay:
                    HandleDelay(data);
                break;
                case CharacterAction.Ready:
                    HandleReady(data);
                break;
                case CharacterAction.Clear:
                    HandleClear(data);
                break;
                case CharacterAction.ActNow:
                    HandleActNow(data);
                break; 
                case CharacterAction.Idle:
                    HandleIdle(data);
                break;
                case CharacterAction.WakeUp:
                    HandleWakeUp(data);
                    break;
                case CharacterAction.Delete:
                HandleDelete(data);
                break;
                case CharacterAction.NextTurn:
                    HandleNextTurn(data);
                    break;
                case CharacterAction.PreviousTurn:
                    HandlePreviousTurn(data);
                    break;
                case CharacterAction.Hide:
                    HandleHide(data);
                    break;
                case CharacterAction.Unhide:
                default:
                    HandleUnhide(data);
                    break;
            }

        }   

        private static void HandleMeleeAttack(ActionData data)
        {
            data.ActForEach(data.Parent.RollMeleeAttackCharacter);
        }

        private static void HandleRangedAttack(ActionData data)
        {
            data.ActForEach(data.Parent.RollRangedAttackCharacter);
        }

        private static void HandleSave(ActionData data)
        {
            if (data.HasCharacters)
            {
                Monster.SaveType st;
                switch (data.Subtype)
                {
                    case "Fort":

                        st = Monster.SaveType.Fort;
                        break;
                    case "Ref":

                        st = Monster.SaveType.Ref;
                        break;
                    case "Will":
                    default:
                        st = Monster.SaveType.Will;
                        break;
                }
                data.Parent.RollSave(data.Characters, st);
                
            }
        }

        private static void HandleSkill(ActionData data)
        {
            if (data.HasCharacters)
            {
                data.Parent.RollSkillCheck(data.Characters, data.Subtype, null);
            }
        }

        private static void HandleApplyCondition(ActionData data)
        {
            if (data.HasCharacters)
            {
                foreach (Character c in data.Characters)
                {
                    c.AddConditionByName(data.Subtype);
                }
            }
        }

        private static void HandleEditAttacks(ActionData data)
        {
            if (data.Primary != null)
            {
                data.Parent.EditAttacks(data.Primary);
            }
        }

        private static void HandleEditFeats(ActionData data)
        {
            if (data.Primary != null)
            {
                data.Parent.EditFeats(data.Primary);
            }
        }

        private static void HandleEditMonster(ActionData data)
        {
            data.ActOnPrimary(data.Parent.EditMonster);
        }

        private static void HandleCopyToCustom(ActionData data)
        {
            data.ActForEach(data.Parent.MakeCharacterCustomMonster);
        }


        private static void HandleConditionMenu(ActionData data)
        {

            data.ActOnPrimary(data.Parent.ShowConditionMenu);
        }
        private static void HandleDamageHeal(ActionData data)
        {

            data.ActOnPrimary(data.Parent.DamageHealDialogCharacter);
        }
        private static void HandleNotesd(ActionData data)
        {

        }
        private static void HandleRollInitiative(ActionData data)
        {
            data.Parent.RollInitiativeAndSort();
        }
        private static void HandleUnlinkFromGroup(ActionData data)
        {
            data.ActOnAll(data.Parent.UnlinkCharacterList);
        }
        private static void HandleGroupAll(ActionData data)
        {
            data.ActOnAll(data.Parent.GroupCharacterList);
        }

        private static void HandleClone(ActionData data)
        {
            data.ActOnAll(data.Parent.CloneCharacterList);
        }

        private static void HandleDelete(ActionData data)
        {
            data.ActOnAll(data.Parent.DeleteCharacterList);
        }

        private static void HandleDelay(ActionData data)
        {
            data.ActOnPrimary(data.Parent.DelayCharacter);
        }

        private static void HandleReady(ActionData data)
        {
            data.ActOnPrimary(data.Parent.ReadyCharacter);
        }

        private static void HandleClear(ActionData data)
        {
            data.ActOnPrimary(data.Parent.ClearCharacter);
        }

        private static void HandleActNow(ActionData data)
        {
            data.ActOnPrimary(data.Parent.ActNowCharacter);
        }

        private static void HandleIdle(ActionData data)
        {
            data.ActOnAll(data.Parent.IdleCharacterList);
        }

        private static void HandleWakeUp(ActionData data)
        {
            data.ActOnAll(data.Parent.WakeUpCharacterList);
        }


        private static void HandleHide(ActionData data)
        {
            data.ActOnAll(data.Parent.HideCharacterList);
        }


        private static void HandleUnhide(ActionData data)
        {
            data.ActOnAll(data.Parent.UnhideCharacterList);
        }

        private static void HandlePreviousTurn(ActionData data)
        {
            data.Parent.PreviousTurn();
        }


        private static void HandleNextTurn(ActionData data)
        {
            data.Parent.NextTurn();
        }



        public static string Description(this CharacterAction action)
        {
            switch (action)
            {
                case CharacterAction.EditAttacks:
                    return "Edit Attacks";
                case CharacterAction.EditFeats:
                    return "Edit Feats";
                case CharacterAction.EditMenu:
                    return "Show Edit Menu";
                case CharacterAction.EditMonster:
                    return "Edit Monster";
                case CharacterAction.CopyToCustom:
                    return "Copy to Custom Monsters";
                case CharacterAction.ConditionMenu:
                    return "Show Condition Menu";
                case CharacterAction.DamageHeal:
                    return "Show Damage/Heal Dialog";
                case CharacterAction.RollInitiative:
                    return "Roll Initiative";
                case CharacterAction.UnlinkFromGroup:
                    return "Unlink From Group";
                case CharacterAction.GroupAll:
                    return "Group All";
                case CharacterAction.Clone:
                    return "Clone";
                case CharacterAction.Delay:
                    return "Delay";
                case CharacterAction.Ready:
                    return "Ready";
                case CharacterAction.Delete:
                    return "Delete";
                case CharacterAction.Clear:
                    return "Clear Initiative Effects";
                case CharacterAction.ActNow:
                    return "ActNow";
                case CharacterAction.Idle:
                    return "Idle";
                case CharacterAction.WakeUp:
                    return "Wake Up";
                case CharacterAction.Hide:
                    return "Hide";
                case CharacterAction.Unhide:
                    return "Unhide";
                case CharacterAction.MeleeAttack:
                    return "Roll Melee Attack";
                case CharacterAction.RangedAttack:
                    return "Roll Ranged Attack";
                case CharacterAction.Save:
                    return "Roll Save";
                case CharacterAction.Skill:
                    return "Roll Skill";
                case CharacterAction.ApplyCondition:
                    return "Apply Condition";
                case CharacterAction.NextTurn:
                    return "Next Turn";
                case CharacterAction.PreviousTurn:
                    return "Previous Turn";
                default:
                    return "";
            }
        }
    }
}

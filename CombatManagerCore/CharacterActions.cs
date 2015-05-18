/*
 *  CharacterActions.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CombatManager;

namespace CombatManager
{
	
	public enum CharacterActionType
	{
		None = 0,
		EditAttacks,
		EditFeats,
		EditMonster,
		CopyToCustom,
		MakeIdle,
		RemoveIdle,
		MakeHidden,
		RemoveHidden,
		AddConditon,
		DamageHeal,
		ApplyAffliction,
		EditNotes,
		RollInitiative,
		LinkInitiative,
		UnlinkInitiative,
		Clone,
		MoveToParty,
		MoveToMonsters,
		Delete,
		MoveUpInitiative,
		MoveDownInitiative,
		MoveBeforeInitiative,
		MoveAfterInitiative,
		Delay,
		Ready,
        ActNow,
        RollAttackSet,
        RollAttack,
        RollSave,
        RollSkill

		
	}

	
	public enum CharacterActionResult
	{
		None,
		NeedConditionDialog,
		NeedAttacksDialog,
		NeedNotesDialog,
		NeedMonsterEditorDialog,
        RollAttack,
        RollAttackSet,
        RollSave,
        RollSkill
	}
	
	public class CharacterActionItem
	{
		public string Name;
		public CharacterActionType Action;
		public object Tag;
		public List<CharacterActionItem> SubItems;
		public string Icon;
		
		public CharacterActionItem() 
		{
			
		}
		public CharacterActionItem(string name, string icon) 
		{
			Name = name;
			Icon = icon;
		}
		
		public CharacterActionItem(string name, string icon, CharacterActionType action)
		{
			Name = name;
			Icon = icon;
			Action = action;
		}
		
		public CharacterActionItem(string name, string icon, CharacterActionType action, object tag)
		{
			Name = name;
			Icon = icon;
			Action = action;
			Tag = tag;
		}
		
		
		public CharacterActionItem(string name, string icon, List<CharacterActionItem> subitems)
		{
			Name = name;
			Icon = icon;
			SubItems = subitems;
		}
	}
	
	public static class 
        CharacterActions
	{
		
		
        public static List<CharacterActionItem> GetActions(Character ch, Character selCh)
        {
            return GetActions(ch, selCh, null);

        }

        public static List<CharacterActionItem> GetActions(Character ch, Character selCh, List<Character> nearCharacters)
		{
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			
			items.Add(new CharacterActionItem("Edit", "pencil", CharacterActionType.EditMonster));
			//items.Add(new CharacterActionItem("Copy to Custom", "import"));
			if (!ch.IsIdle)
			{
				
				items.Add(new CharacterActionItem("Make Idle", "zzz", CharacterActionType.MakeIdle));
			}
			else
			{
				
				items.Add(new CharacterActionItem("Remove Idle", "zzz", CharacterActionType.RemoveIdle));
			}
			/*if (!ch.IsHidden)
			{
				
				items.Add(new CharacterActionItem("Make Hidden", "blind", CharacterActionType.MakeHidden));
			}
			else
			{
				
				items.Add(new CharacterActionItem("Remove Hidden", "blind", CharacterActionType.RemoveHidden));
			}*/
			
			items.Add(new CharacterActionItem());
			CharacterActionItem conditionsItem = new CharacterActionItem("Add Condition", "clock", GetConditionItems(ch));
			items.Add(conditionsItem);
			//items.Add(new CharacterActionItem("Apply Affliction", "lightning"));
			items.Add(new CharacterActionItem("Notes", "notes", CharacterActionType.EditNotes));
			
			items.Add(new CharacterActionItem());
			items.Add(new CharacterActionItem("Roll", "d20", GetRollItems(ch)));
            items.Add(new CharacterActionItem("Initiative", "sort", GetInitiativeItems(ch, selCh, nearCharacters)));
			items.Add(new CharacterActionItem("Clone", "clone", CharacterActionType.Clone));
			if (ch.IsMonster)
			{
				items.Add(new CharacterActionItem("Move to Party", "prev", CharacterActionType.MoveToParty));
			}
			else
			{
				items.Add(new CharacterActionItem("Move to Monsters", "next", CharacterActionType.MoveToMonsters));
			}
			items.Add(new CharacterActionItem());
			items.Add(new CharacterActionItem("Delete", "delete", CharacterActionType.Delete));
			
			return items;
			
			
		}
		
		public static List<CharacterActionItem> GetEditItems(Character ch)
		{
			
			
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			
			items.Add(new CharacterActionItem("Attacks", "sword", CharacterActionType.EditAttacks));
			items.Add(new CharacterActionItem("Feats", "star", CharacterActionType.EditFeats));
			items.Add(new CharacterActionItem("Monster Editor", "monster", CharacterActionType.EditMonster));
		
		
			return items;
		}    

        public static List<CharacterActionItem> GetRollItems(Character ch)
        {
            
            
            List<CharacterActionItem> items = new List<CharacterActionItem>();
            
            items.AddRange(GetAttackItems(ch));

            items.Add (new CharacterActionItem());

            items.AddRange(GetSaveItems(ch));

            items.Add (new CharacterActionItem());

            items.AddRange (GetSkillItems(ch));
        
        
            return items;
        }

        public static List<CharacterActionItem> GetSaveItems(Character ch)
        {
            
            List<CharacterActionItem> items = new List<CharacterActionItem>();
            items.Add(new CharacterActionItem("Fort", "d20p", CharacterActionType.RollSave, Monster.SaveType.Fort));
            items.Add(new CharacterActionItem("Ref", "d20p", CharacterActionType.RollSave, Monster.SaveType.Ref));
            items.Add(new CharacterActionItem("Will", "d20p", CharacterActionType.RollSave, Monster.SaveType.Will));
            return items;
        
        }

        public static List<CharacterActionItem> GetAttackItems(Character ch)
        {
          
            List<CharacterActionItem> items = new List<CharacterActionItem>();

            foreach (AttackSet atkSet in ch.Monster.MeleeAttacks)
            {
                CharacterActionItem item = new CharacterActionItem(atkSet.ToString(), "sword", CharacterActionType.RollAttackSet, atkSet);
                items.Add(item);
            }

            foreach (Attack atk in ch.Monster.RangedAttacks)
            {
                CharacterActionItem item = new CharacterActionItem(atk.ToString(), "bow", CharacterActionType.RollAttack, atk);
                items.Add (item);
            }
            
            return items;

        }

        public static List<CharacterActionItem> GetSkillItems(Character ch)
        {
            
            List<CharacterActionItem> items = new List<CharacterActionItem>();

            foreach (string skill in from a in Monster.SkillsList orderby a.Key select a.Key)
            {             
                Monster.SkillInfo info = Monster.SkillsDetails[skill];


                if (info.Subtypes != null)
                {
                    List<CharacterActionItem> subitems = new List<CharacterActionItem>();
                    foreach (string subtype in info.Subtypes)
                    {
                        subitems.Add (new CharacterActionItem(subtype, "d20p", CharacterActionType.RollSkill, 
                                                              new Tuple<string, string>(skill, subtype)));
                    }
                    var ci = new CharacterActionItem(skill, "d20p", subitems);
                    items.Add (ci);

                }
                else
                {
                    items.Add(new CharacterActionItem(skill, "d20p", CharacterActionType.RollSkill, new Tuple<string, string>(skill, null)));
                }
            }


            return items;
        }
		
        public static List<CharacterActionItem> GetInitiativeItems(Character ch, Character selectedChar, List<Character> nearCharacters)
		{
			
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			
			items.Add(new CharacterActionItem("Move Up", "arrowup", CharacterActionType.MoveUpInitiative));
			items.Add(new CharacterActionItem("Move Down", "arrowdown", CharacterActionType.MoveDownInitiative));
			if (selectedChar != null && selectedChar != ch && selectedChar.InitiativeLeader == null)
			{
				items.Add(new CharacterActionItem("Move Before " + selectedChar.Name, "arrowsup", CharacterActionType.MoveBeforeInitiative, selectedChar));
				items.Add(new CharacterActionItem("Move After " + selectedChar.Name, "arrowsdown", CharacterActionType.MoveAfterInitiative, selectedChar));
	
			}
            items.Add(new CharacterActionItem());
            items.Add(new CharacterActionItem("Roll", "d20p", CharacterActionType.RollInitiative));
			items.Add(new CharacterActionItem());		
			items.Add(new CharacterActionItem("Ready", "target", CharacterActionType.Ready));
			items.Add(new CharacterActionItem("Delay", "hourglass", CharacterActionType.Delay));
            items.Add(new CharacterActionItem("Act Now", "next", CharacterActionType.ActNow));

            if (ch.InitiativeLeader != null)
            {
                items.Add(new CharacterActionItem());
                items.Add(new CharacterActionItem("Unlink Initiative", "link", CharacterActionType.UnlinkInitiative));
            }
            else if (selectedChar != null && selectedChar != ch && selectedChar.InitiativeLeader == null)
            {
                items.Add(new CharacterActionItem());
                items.Add(new CharacterActionItem("Link to " + selectedChar.Name, 
                    "link", CharacterActionType.LinkInitiative, selectedChar));
            }
            else if (nearCharacters != null)
            {
                List<CharacterActionItem> newItems = new List<CharacterActionItem>();

                foreach (Character nearChar in nearCharacters)
                {
                    if (nearChar != ch && nearChar.InitiativeLeader == null)
                    {

                        newItems.Add(new CharacterActionItem("Link to " + nearChar.Name, 
                            "link", CharacterActionType.LinkInitiative, nearChar));
                    }
                }

                if (newItems.Count > 0)
                {
                    items.Add(new CharacterActionItem());
                    items.AddRange(newItems);
                }

            }

		
		
			return items;
		}
		
		public static List<CharacterActionItem> GetConditionItems(Character ch)
		{
			
			List<CharacterActionItem> items = new List<CharacterActionItem>();
            items.Add(new CharacterActionItem("Other...", null, CharacterActionType.AddConditon));
            

			if (Condition.FavoriteConditions.Count > 0)
			{
                items.Add(new CharacterActionItem());
				foreach (FavoriteCondition fc in Condition.FavoriteConditions)
				{
					Condition c = Condition.FromFavorite(fc);
					
					items.Add(new CharacterActionItem(c.Name, c.Image, CharacterActionType.AddConditon, c));
				}
			}
			
			if (Condition.RecentConditions.Count > 0)
			{
                items.Add(new CharacterActionItem());
				foreach (var fc in Condition.RecentConditions)
				{
					Condition c = Condition.FromFavorite(fc);
					
					items.Add(new CharacterActionItem(c.Name, c.Image, CharacterActionType.AddConditon, c));
				}
			}
			

			return items;
		}
		
		public static CharacterActionResult TakeAction(CombatState state, CharacterActionType action, Character primaryChar, List<Character> allChars, object param)
		{
			CharacterActionResult res = CharacterActionResult.None;
			
			switch (action)
			{
			case CharacterActionType.MakeIdle:
				foreach (Character ch in allChars)
				{
					ch.IsIdle = true;
				}
				state.FilterList();
				break;
			case CharacterActionType.RemoveIdle:
				foreach (Character ch in allChars)
				{
					ch.IsIdle = false;
				}
				state.FilterList();
				break;
			case CharacterActionType.Clone:
				foreach (Character ch in allChars)
				{
					state.CloneCharacter(ch);
				}
				break;
			case CharacterActionType.Delete:
				foreach (Character ch in allChars)
				{
					state.RemoveCharacter(ch);
				}
				break;
			case CharacterActionType.MoveToMonsters:
				foreach (Character ch in allChars)
				{
					if (!ch.IsMonster)
					{
						state.RemoveCharacter(ch);
						ch.IsMonster = true;
						state.AddCharacter(ch);
					}
					
				}
				state.FilterList();
				break;
			case CharacterActionType.MoveToParty:
				foreach (Character ch in allChars)
				{
					if (ch.IsMonster)
					{
						state.RemoveCharacter(ch);
						ch.IsMonster = false;
						state.AddCharacter(ch);
					}
					
					
				}
				state.FilterList();
				break;
			case CharacterActionType.MoveUpInitiative:
				state.MoveUpCharacter(primaryChar);
				break;
			case CharacterActionType.MoveDownInitiative:
				state.MoveDownCharacter(primaryChar);
				break;
			case CharacterActionType.MoveAfterInitiative:
				state.MoveCharacterAfter(primaryChar, (Character)param);
				break;
			case CharacterActionType.MoveBeforeInitiative:
				state.MoveCharacterBefore(primaryChar, (Character)param);
				break;
            case CharacterActionType.RollInitiative:
                state.RollIndividualInitiative(primaryChar);
                state.SortCombatList(false, false);
                break;
            case CharacterActionType.Ready:
                primaryChar.IsReadying = !primaryChar.IsReadying;
                if (primaryChar.IsReadying && primaryChar.IsDelaying)
                {
                    primaryChar.IsDelaying = false;
                }
                break;
            case CharacterActionType.Delay:
                primaryChar.IsDelaying = !primaryChar.IsDelaying;
                if (primaryChar.IsReadying && primaryChar.IsDelaying)
                {
                    primaryChar.IsReadying = false;
                }
                break;
            case CharacterActionType.ActNow:
                if (primaryChar.IsIdle)
                {
                    primaryChar.IsIdle = false;
                }
                state.CharacterActNow(primaryChar);
                break;
            case CharacterActionType.LinkInitiative:
                Character targetChar = (Character)param;
                if (primaryChar != targetChar  && targetChar.InitiativeLeader == null)
                {
                    state.LinkInitiative(primaryChar, (Character)param);
                }
                break;
            case CharacterActionType.UnlinkInitiative:
                state.UnlinkLeader(primaryChar);
                break;
			case CharacterActionType.AddConditon:	
				if (param != null)
				{
					Condition c = (Condition)param;
					foreach (Character ch in allChars)
					{
						ActiveCondition a = new ActiveCondition();
						a.Condition = c;
						ch.Stats.AddCondition(a);
						Condition.PushRecentCondition(a.Condition);
					}
				}
				else
				{
					res = CharacterActionResult.NeedConditionDialog;
				}
				break;
			case CharacterActionType.EditNotes:
				res = CharacterActionResult.NeedNotesDialog;
				break;
			case CharacterActionType.EditMonster:
				res = CharacterActionResult.NeedMonsterEditorDialog;
				break;
            case CharacterActionType.RollAttack:
                res = CharacterActionResult.RollAttack;
                break;
            case CharacterActionType.RollAttackSet:
                res = CharacterActionResult.RollAttackSet;
                break;
            case CharacterActionType.RollSave:
                res = CharacterActionResult.RollSave;
                break;
            case CharacterActionType.RollSkill:
                res = CharacterActionResult.RollSkill;
                break;
			}
			return res;
		}
	}
}


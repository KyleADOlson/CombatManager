using System;
using System.Collections.Generic;
using CombatManager;

namespace CombatManagerMono
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
		Ready
		
	}
	
	public enum CharacterActionResult
	{
		None,
		NeedConditionDialog,
		NeedAttacksDialog,
		NeedNotesDialog,
		NeedMonsterEditorDialog,
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
	
	public static class CharacterActions
	{
		
		
		public static List<CharacterActionItem> GetActions(Character ch)
		{
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			
			items.Add(new CharacterActionItem("Edit", "pencil", GetEditItems(ch)));
			items.Add(new CharacterActionItem("Copy to Custom", "import"));
			if (!ch.IsIdle)
			{
				
				items.Add(new CharacterActionItem("Make Idle", "zzz", CharacterActionType.MakeIdle));
			}
			else
			{
				
				items.Add(new CharacterActionItem("Remove Idle", "zzz", CharacterActionType.RemoveIdle));
			}
			if (!ch.IsHidden)
			{
				
				items.Add(new CharacterActionItem("Make Hidden", "blind", CharacterActionType.MakeHidden));
			}
			else
			{
				
				items.Add(new CharacterActionItem("Remove Hidden", "blind", CharacterActionType.RemoveHidden));
			}
			
			items.Add(new CharacterActionItem());
			CharacterActionItem conditionsItem = new CharacterActionItem("Add Condition", "clock", GetConditionItems(ch));
			items.Add(conditionsItem);
			items.Add(new CharacterActionItem("Apply Affliction", "lightning"));
			items.Add(new CharacterActionItem("Notes", "notes", CharacterActionType.EditNotes));
			
			items.Add(new CharacterActionItem());
			items.Add(new CharacterActionItem("Roll", "d20"));
			items.Add(new CharacterActionItem("Initiative", "sort", GetInitiativeItems(ch, null)));
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
		
		public static List<CharacterActionItem> GetInitiativeItems(Character ch, Character selectedChar)
		{
			
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			
			items.Add(new CharacterActionItem("Move Up", "arrowup", CharacterActionType.MoveUpInitiative));
			items.Add(new CharacterActionItem("Move Down", "arrowdown", CharacterActionType.MoveDownInitiative));
			if (selectedChar != null && selectedChar != ch)
			{
				items.Add(new CharacterActionItem("Move Before " + selectedChar.Name, "arrowup", CharacterActionType.MoveBeforeInitiative, selectedChar));
				items.Add(new CharacterActionItem("Move After" + selectedChar.Name, "arrowdown", CharacterActionType.MoveAfterInitiative, selectedChar));
	
			}
			items.Add(new CharacterActionItem());		
			items.Add(new CharacterActionItem("Ready", "target", CharacterActionType.Ready));
			items.Add(new CharacterActionItem("Delay", "hourglass", CharacterActionType.Delay));
		
		
			return items;
		}
		
		public static List<CharacterActionItem> GetConditionItems(Character ch)
		{
			
			List<CharacterActionItem> items = new List<CharacterActionItem>();
			if (Condition.FavoriteConditions.Count > 0)
			{
				foreach (FavoriteCondition fc in Condition.FavoriteConditions)
				{
					Condition c = Condition.FromFavorite(fc);
					
					items.Add(new CharacterActionItem(c.Name, c.Image, CharacterActionType.AddConditon, c));
				}
				items.Add(new CharacterActionItem());
			}
			
			if (Condition.RecentConditions.Count > 0)
			{
				foreach (var fc in Condition.RecentConditions)
				{
					Condition c = Condition.FromFavorite(fc);
					
					items.Add(new CharacterActionItem(c.Name, c.Image, CharacterActionType.AddConditon, c));
				}
				items.Add(new CharacterActionItem());
			}
			
			items.Add(new CharacterActionItem("Other...", null, CharacterActionType.AddConditon));

		
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
			}
			return res;
		}
	}
}


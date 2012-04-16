using System;
using System.Collections.Generic;
using System.Drawing;
using CombatManager;
namespace CombatManagerMono
{
	public class SpellsTab: LookupTab<Spell>
	{
		
		
		ButtonStringPopover classFilterPopover;
		ButtonStringPopover levelFilterPopover;
		ButtonStringPopover schoolFilterPopover;
		
		GradientButton classFilterButton;
		GradientButton levelFilterButton;
		GradientButton schoolFilterButton;
		
		string classFilter = null;	
		int? levelFilter;
		string schoolFilter = null;
		
		const string AllClassesText = "All Classes";
		const string AllLevelsText = "All Levels";
		const string AllSchoolsText = "All Schools";
		
		public SpellsTab (CombatState state) : base (state)
		{
			BuildFilters();
		}

		protected override bool ItemFilter (Spell item)
		{
			return SchoolFilterMatch(item) && ClassFilterMatch(item) && LevelFilterMatch(item);
		}
		
		private bool SchoolFilterMatch(Spell item)
		{
			return schoolFilter == null || 
				String.Compare(item.school.Trim(), schoolFilter, true) == 0;
		}
		
		
		private bool LevelFilterMatch(Spell item)
		{
			if (levelFilter == null)
			{
				return true;	
			}
			else if (classFilter == null)
			{
				return item.IsLevel(levelFilter.Value);
			}
			else
			{
				return item.LevelForClass(CharacterClass.GetEnum(classFilter)) == levelFilter;	
			}
		}
		
		private bool ClassFilterMatch(Spell item)
		{
			if (classFilter == null)
			{
				return true;
			}
			else
			{
				return item.LevelForClass(CharacterClass.GetEnum(classFilter)) != null;
			}
		}

		protected override string ItemFilterText (Spell item)
		{
			return item.Name;
		}

		protected override string SortText (Spell item)
		{
			return item.Name;
		}

		protected override string DisplayText (Spell item)
		{
			return item.Name;
		}

		protected override string ItemHtml (Spell item)
		{
			
			return SpellHtmlCreator.CreateHtml(item);
		}

		protected override bool CompareItems (Spell item1, Spell item2)
		{
			return item1 == item2;
		}

		protected override System.Collections.Generic.IEnumerable<Spell> ItemsSource 
		{
			get 
			{
				return Spell.Spells;
			}
		}
		
		private void BuildFilters()
		{
			float locX = 0;
			float locY = 5;
			float bHeight = 30;
			float marginX = 10;
			
			GradientButton b;
			
			
			//class filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new RectangleF(locX, locY, 100, bHeight);
			locX += b.Frame.Width + marginX;
			b.SetText(AllClassesText);
			classFilterPopover = new ButtonStringPopover(b);
			classFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllClassesText, Tag=null});
			List<String> classList = new List<String>() {"Alchemist", "Antipaladin", "Bard", "Cleric", "Druid", 
				"Inquisitor", "Magus", "Oracle",  "Paladin", "Ranger", "Sorcerer", "Summoner", "Witch", "Wizard"};
			
			foreach (String s in classList)
			{
				classFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = s, Tag=s});
			}
			
			classFilterPopover.SetButtonText = true;
			classFilterPopover.ItemClicked += HandleClassFilterPopoverItemClicked;
			classFilterButton = b;
			
			FilterView.AddSubview(b);
			
			
			//level filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new RectangleF(locX, locY, 80, bHeight);
			locX += b.Frame.Width + marginX;
			b.SetText(AllLevelsText);
			levelFilterPopover = new ButtonStringPopover(b);
			levelFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllLevelsText, Tag=null});
			
			for (int i=0; i<=9; i++)
			{
				
				levelFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = i.PastTense() + " Level", Tag=i});
			}
			
			levelFilterPopover.SetButtonText = true;
			levelFilterPopover.ItemClicked += HandleLevelFilterPopoverItemClicked;
			levelFilterButton = b;
			
			FilterView.AddSubview(b);
			
			
			//school filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new RectangleF(locX, locY, 100, bHeight);
			locX += b.Frame.Width + marginX;
			b.SetText(AllSchoolsText);
			schoolFilterPopover = new ButtonStringPopover(b);
			schoolFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllSchoolsText, Tag=null});
			foreach (string school in Spell.Schools)
            {
               
				schoolFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = school, Tag=school});
			}
			schoolFilterPopover.SetButtonText = true;
			schoolFilterPopover.ItemClicked += HandleSchoolFilterPopoverItemClicked;
			schoolFilterButton = b;
			
			FilterView.AddSubview(b);
		}

		void HandleSchoolFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			schoolFilter = (string)e.Tag;
			Filter();
		}

		void HandleLevelFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			levelFilter = (int?)e.Tag;
			Filter();
		}

		void HandleClassFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			classFilter = (string)e.Tag;
			Filter();
		}
		protected override void ResetButtonClicked ()
		{
			schoolFilter = null;	
			levelFilter = null; 
			classFilter = null;
			
			schoolFilterButton.SetText(AllSchoolsText);
			levelFilterButton.SetText(AllLevelsText);
			classFilterButton.SetText(AllClassesText);
			
			filterField.Text = "";
			Filter();
		}
	}
}


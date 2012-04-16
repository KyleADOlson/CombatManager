using System;
using CombatManager;
using System.Drawing;
namespace CombatManagerMono
{
	public class FeatsTab: LookupTab<Feat>
	{
		ButtonStringPopover typeFilterPopover;
		
		GradientButton typeFilterButton;
		
		string typeFilter = null;	
		
		const string AllTypesText = "All Types";
		
		public FeatsTab (CombatState state) : base (state)
		{
			BuildFilters();
		}

		protected override bool ItemFilter (Feat item)
		{
			return typeFilter == null || String.Compare(item.Type.Trim(), typeFilter, true) == 0;
		}
		
		protected override string ItemFilterText (Feat item)
		{
			return item.Name;
		}

		protected override string SortText (Feat item)
		{
			return item.Name;
		}

		protected override string DisplayText (Feat item)
		{
			return item.Name;
		}

		protected override string ItemHtml (Feat item)
		{
			return FeatHtmlCreator.CreateHtml(item);
		}

		protected override bool CompareItems (Feat item1, Feat item2)
		{
			return item1 == item2;
		}

		protected override System.Collections.Generic.IEnumerable<Feat> ItemsSource 
		{
			get 
			{
				return Feat.Feats;
			}
		}
		
		private void BuildFilters()
		{
			float locX = 0;
			float locY = 5;
			float bHeight = 30;
			float marginX = 10;
			
			GradientButton b;
			
			
			//type filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new RectangleF(locX, locY, 100, bHeight);
			locX += b.Frame.Width + marginX;
			b.SetText(AllTypesText);
			typeFilterPopover = new ButtonStringPopover(b);
			typeFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllTypesText, Tag=null});
			
			foreach (String s in Feat.FeatTypes)
			{
				typeFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = s, Tag=s});
			}
			typeFilterPopover.SetButtonText = true;
			typeFilterPopover.ItemClicked += HandleTypeFilterPopoverItemClicked;;
			typeFilterButton = b;
			
			FilterView.AddSubview(b);
		}

		void HandleTypeFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			typeFilter = (string) e.Tag;
			
			Filter();
		}
		
		protected override void ResetButtonClicked ()
		{
			typeFilter = null;
			typeFilterButton.SetText(AllTypesText);
			filterField.Text = "";
			Filter();
		}
	}
}


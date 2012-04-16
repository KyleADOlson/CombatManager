using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.Text.RegularExpressions;

namespace CombatManagerMono
{
	
	
	public class ConditionViewEventArgs : EventArgs
	{
		public ConditionViewEventArgs()
		{
		}
		
		
		public Condition Condition{get; set;}
	}
	
	
	
	public delegate void ConditionViewEvent (object sender, ConditionViewEventArgs args);
	
	
	public partial class ConditionViewController : UIViewController
	{
		List<GradientButton> _TabButtons;
		
		static int _SelectedButton = 0;
		
		
		List<Condition> _VisibleConditions = new List<Condition>();
		
		Condition _SelectedCondition;
		
		public ConditionViewEvent ConditionApplied;
		
		
		//loads the ConditionViewController.xib file and connects it to this object
		public ConditionViewController () : base ("ConditionViewController", null)
		{
			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_TabButtons = new List<GradientButton>() {ConditionsTab,SpellsTab,AfflictionsTab,
				CustomTab, FavoritesTab};
			UpdateButtons();
			
			StyleBar(TopView);
			StyleBar(BottomView);
			
			StyleButton(ApplyButton);
			StyleButton(CloseButton);
			
			TitleView.CornerRadius = 0;
			
			RebuildConditionList();
			
			 SelectionTable.Delegate = new ViewDelegate(this);
			SelectionTable.DataSource = new ViewDataSource(this);
			
			FilterText.AllEditingEvents += HandleFilterTextAllEditingEvents;
			
			UpdateConditionDisplay();
		}

		void HandleFilterTextAllEditingEvents (object sender, EventArgs e)
		{
			RebuildConditionList();
			SelectionTable.ReloadData();
		}
		
		partial void StepperValueChanged (MonoTouch.UIKit.UIStepper sender)
		{
			 UpdateDuration();
		}
		
		partial void TabButtonClicked (UIButton sender)
		{
			if (_SelectedButton != sender.Tag)
			{
				_SelectedButton = sender.Tag;
				UpdateButtons();
				SelectionTable.ReloadData();
				
				
				RebuildConditionList();
				SelectionTable.ReloadData();
			}
			
		}
		
		private void UpdateButtons()
		{
			
			foreach (GradientButton b in _TabButtons)
			{
				StyleTabButton(b, b.Tag == _SelectedButton);
			}
			
		}
		
		private void UpdateDuration()
		{
			if (DurationStepper.Value == 0)
			{
				DurationLabel.Text = "N/A";
			}
			else
			{
				DurationLabel.Text = ((int)DurationStepper.Value).ToString();	
			}
		}
		
		private void StyleButton(GradientButton button)
		{
			button.CornerRadius = 0;
			button.TitleLabel.TextColor = UIColor.White;
		}
		
		private void StyleTabButton(GradientButton button, bool selected)
		{
			button.Border = 0;
			
			
			List<UIColor> colors;
			
			if (selected)
			{
				colors = new List<UIColor> { UIExtensions.ARGBColor(0x11FFFFFF),  UIExtensions.ARGBColor(0x88FFFFFF)};
			}
			else
			{
				
				colors = new List<UIColor> {UIColor.Clear, UIColor.Clear};
			}
			
			button.Gradient = new GradientHelper(colors, new List<float>{0.0f, 1.0f});
			button.CornerRadius = 2.0f;
		}
		
		private void StyleBar(GradientView view)
		{
			view.CornerRadius = 0;
			view.Border = 0;
			
			List<UIColor> colors = new List<UIColor> {UIExtensions.ARGBColor(0xFF000000), UIExtensions.ARGBColor(0xFF080808), UIExtensions.ARGBColor(0xFF202020),  UIExtensions.ARGBColor(0xFF202020)};
			
			view.Gradient = new GradientHelper(colors, new List<float>{0.0f, 0.5f, 0.5f, 1.0f});
		}
		
		private void RebuildConditionList()
		{
			_VisibleConditions = new List<Condition>();
			
           	foreach (Condition c in Condition.Conditions)
			{
				if (ConditionViewFilter(c))
				{
					_VisibleConditions.Add(c);
				}
			}
			
			
           	foreach (Condition c in Condition.CustomConditions)
			{
				if (ConditionViewFilter(c))
				{
					_VisibleConditions.Add(c);
				}
			}

            _VisibleConditions.Sort((a, b) => String.Compare(a.Name, b.Name, true));
            
		

        }

        bool ConditionViewFilter(Condition c)
        {

            return ConditionViewTypeFilter(c) && ConditionViewTextFilter(c);
        }


        bool ConditionViewTextFilter(Condition c)
        {
			string text = this.FilterText.Text.Trim();			
			
            if (text == null || text.Length == 0)
            {
                return true;
            }
            else
            {
                return new Regex(Regex.Escape(text.Trim()), RegexOptions.IgnoreCase).Match(c.Name).Success;

            }
        }


        bool ConditionViewTypeFilter(Condition c)
        {
            bool showingSpells = _SelectedButton == 1;
			bool showingConditions = _SelectedButton == 0;
			bool showingAfflictions = _SelectedButton == 2;
            bool showingCustom = _SelectedButton == 3;
            bool showingFavorites = _SelectedButton == 4;

            if (showingFavorites)
            {
                return Condition.FavoriteConditions.FirstOrDefault(a => String.Compare(a.Name, c.Name, true ) == 0
                    && a.Type == c.Type) != null;                      
            }
            else if (c.Custom)
            {
                return showingCustom;
            }
			else if (c.Spell != null)
			{
                return showingSpells;
			}
			else if (c.Affliction != null)
			{
				return showingAfflictions;
			}
			else
			{
				return showingConditions;
			}
        }
		
		public Condition SelectedCondition
		{
			get
			{
				return _SelectedCondition;
			}
		}
		
		partial void ApplyButtonClicked (UIButton sender)
		{
			if (_SelectedCondition != null && ConditionApplied != null)
			{
				ConditionApplied(this, new ConditionViewEventArgs() {Condition = _SelectedCondition});
			}
		}
		
		public static string ConditionHTML(Condition c)
		{
          	if (c != null)
            {
                if (c.Spell != null)
                {
                    return SpellHtmlCreator.CreateHtml(c.Spell);

                }
                else if (c.Affliction != null)
                {

                    return GenericHtmlCreator.CreateHtml(c.Affliction.Name, c.Affliction.Text);
                }
                else
                {

                 	return GenericHtmlCreator.CreateHtml(c.Name, c.Text);
                }
            }
			return "";
		}
		
		private void UpdateConditionDisplay()
		{
			if (_SelectedCondition == null)
			{
				ConditionDetailWebView.LoadHtmlString("", new NSUrl("http://localhost/"));
			}
			else
			{
	
				ConditionDetailWebView.LoadHtmlString(ConditionHTML(_SelectedCondition), new NSUrl("http://localhost/"));
			}
		}
		
		partial void CloseButtonClicked(NSObject sender)
		{
			this.View.RemoveFromSuperview();
		}
		
				
		private class ViewDataSource : UITableViewDataSource
		{
			ConditionViewController state;
			public ViewDataSource(ConditionViewController state)	
			{
				this.state = state;
				
				
			}
			
			public override int RowsInSection (UITableView tableView, int section)
			{
				return state._VisibleConditions.Count ;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("ConditionViewCell");
				
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, "ConditionViewCell");
				}
				
				cell.TextLabel.Text = state._VisibleConditions[indexPath.Row].Name;
				cell.TextLabel.Font = UIFont.SystemFontOfSize(15);
	
				return cell;			
			}
			public ConditionViewController ListView
			{
				get
				{
					return state;
				}
			}

			
		}
		
		private class ViewDelegate : UITableViewDelegate
		{
			ConditionViewController state;
			public ViewDelegate(ConditionViewController state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Row < state._VisibleConditions.Count)
				{
					state._SelectedCondition = state._VisibleConditions[indexPath.Row];
					state.UpdateConditionDisplay();
				}
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 26;
			}
			public ConditionViewController ListView
			{
				get
				{
					return state;
				}
			}
		}
		
	
		
	}
}

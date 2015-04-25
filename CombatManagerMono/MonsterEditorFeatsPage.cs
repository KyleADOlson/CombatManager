/*
 *  MonsterEditorFeatsPage.cs
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

using UIKit;
using CoreGraphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using System.Collections.ObjectModel;
using CombatManager;

namespace CombatManagerMono
{
	public partial class MonsterEditorFeatsPage : MonsterEditorPage
	{
        
        ObservableCollection<ParsedFeat> _CurrentFeats = new ObservableCollection<ParsedFeat>();
        List<Feat> _FilteredFeats = new List<Feat>();
        List<Feat> _SortedFeats = new List<Feat>();
        TextBoxDialog _TextDialog;

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorFeatsPage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorFeatsPage_iPhone" : "MonsterEditorFeatsPage_iPad", null)
		{
            _SortedFeats = new List<Feat>(Feat.Feats);
            _SortedFeats.Sort((a, b)=>a.Name.CompareTo(b.Name));

		}

        public void FilterFeats()
        {
            if (FilterTextView.Text.Length == 0)
            {
                _FilteredFeats = new List<Feat>(_SortedFeats);
            }
            else
            {
                _FilteredFeats = new List<Feat>(
                    from f in _SortedFeats 
                    where f.Name.ToUpper().Contains(FilterTextView.Text.ToUpper()) 
                    select f);
            }
            AvailableFeatsView.ReloadData();


        }
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
            CurrentFeatsView.DataSource = new CurrentViewDataSource(this);
            CurrentFeatsView.Delegate = new CurrentViewDelegate(this);
            AvailableFeatsView.DataSource = new AvailableViewDataSource(this);
            AvailableFeatsView.Delegate = new AvailableViewDelegate(this);

            FilterTextView.AllEditingEvents += (sender, e) => FilterFeats();

            foreach ( string f in CurrentMonster.FeatsList)
            {
                ParsedFeat p = new ParsedFeat(f);
                _CurrentFeats.Add(p);
            }

            FilterFeats();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. this.myOutlet = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone)
			{
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else
			{
				return true;
			}
		}

        private void AddClicked(Feat feat)
        {
            string name = feat.Name;

            if (feat.AltName != null && feat.AltName.Length > 0)
            {
                name = feat.AltName;
            }

            if (!CurrentMonster.FeatsList.Contains(name))
            {
                CurrentMonster.AddFeat(name);
                _CurrentFeats.Add(new ParsedFeat(name));
                CurrentFeatsView.ReloadData();
            }
           
        }


        private class CurrentViewDataSource : UITableViewDataSource
        {
            MonsterEditorFeatsPage state;
            public CurrentViewDataSource(MonsterEditorFeatsPage state) 
            {
                this.state = state;
                
                
            }
            
            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return state._CurrentFeats.Count;
            }
            
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("CurrentFeatsViewCell");
                
                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "CurrentFeatsViewCell");
                }
            
                ParsedFeat f = state._CurrentFeats[indexPath.Row];

                cell.TextLabel.Text = f.Name;

                cell.TextLabel.Font = UIFont.SystemFontOfSize(15);

                GradientButton b = new GradientButton();
                b.Frame = new CGRect(0, 3, 160, 22);
                b.TitleLabel.Font = UIFont.SystemFontOfSize(15);
                CMStyles.TextFieldStyle(b);
                b.SetText (f.Choice.NullToEmpty());
                b.TouchUpInside += (sender, e) => state.FeatTextClicked(f);
                UIView view = new UIView();
                UIButton x = new UIButton();
                UIImage redx = UIImage.FromFile("Images/External/redx.png");
                x.SetImage(redx, UIControlState.Normal);
                x.Frame = new CGRect(165, 0, redx.Size.Width, redx.Size.Height);
                x.TouchUpInside += (sender, e) => state.DeleteButtonClicked(f);

                view.Bounds = new CGRect(0, 0, x.Frame.Right, x.Frame.Bottom);
                view.Add(b);
                view.Add(x);


                cell.AccessoryView = view;
                
    
                return cell;            
            }

            
        }

        private void FeatTextClicked(ParsedFeat f)
        {
            _TextDialog = new TextBoxDialog();
            _TextDialog.Title = "Feat Options";
            _TextDialog.Value = f.Choice;
            _TextDialog.SingleLine = true;
            _TextDialog.OKClicked += (sender, e) => 
            {
                CurrentMonster.FeatsList.Remove(f.Text);
                f.Choice = _TextDialog.Value;
                CurrentMonster.FeatsList.Add(f.Text);
                RebuildFeats();
            };
            DialogParent.Add(_TextDialog.View);
        }

        private void DeleteButtonClicked(ParsedFeat f)
        {
            CurrentMonster.FeatsList.Remove(f.Text);
            RebuildFeats();
        }

        private void RebuildFeats()
        {
            _CurrentFeats.Clear();
            foreach ( string f in CurrentMonster.FeatsList)
            {
                ParsedFeat p = new ParsedFeat(f);
                _CurrentFeats.Add(p);
            }
            CurrentFeatsView.ReloadData();
        }

        private class CurrentViewDelegate : UITableViewDelegate
        {
            MonsterEditorFeatsPage state;
            public CurrentViewDelegate(MonsterEditorFeatsPage state) 
            {
                this.state = state;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
            }
            
            public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
            {

            }
            
            public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return 28;
            }           

            public MonsterEditorFeatsPage ListView
            {
                get
                {
                    return state;
                }
            }
        }

        private class AvailableViewDataSource : UITableViewDataSource
        {
            MonsterEditorFeatsPage state;
            public AvailableViewDataSource(MonsterEditorFeatsPage state) 
            {
                this.state = state;
                
                
            }
            
            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return state._FilteredFeats.Count;
            }
            
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("AvailableFeatsViewCell");
                
                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "AvailableFeatsViewCell");
                }
            
                Feat f =  state._FilteredFeats[indexPath.Row];

                GradientButton addButton = new GradientButton();
                addButton.Bounds = new CGRect(0, 0, 100, 24);
                addButton.SetText("Add");
                addButton.SetImage(UIExtensions.GetSmallIcon("prev"), UIControlState.Normal);
                addButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5);
                addButton.TouchUpInside += (sender, e) => state.AddClicked(f);
                addButton.TitleLabel.Font = UIFont.SystemFontOfSize(15);
                addButton.CornerRadius = 3.0f;
                cell.AccessoryView = addButton;

                cell.TextLabel.Text = f.Name;
                cell.TextLabel.Font = UIFont.SystemFontOfSize(15);
                
    
                return cell;            
            }

            
        }

        
        
        private class AvailableViewDelegate : UITableViewDelegate
        {
            MonsterEditorFeatsPage state;
            public AvailableViewDelegate(MonsterEditorFeatsPage state) 
            {
                this.state = state;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
            }
            
            public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
            {

            }
            
            public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return 26;
            }
            public MonsterEditorFeatsPage ListView
            {
                get
                {
                    return state;
                }
            }
        }
	}
}


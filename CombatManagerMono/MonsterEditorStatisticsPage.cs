/*
 *  MonsterEditorStatisticsPage.cs
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
using CombatManager;

namespace CombatManagerMono
{
	public partial class MonsterEditorStatisticsPage : MonsterEditorPage
	{
        List<GradientView> _AreaViews;
        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();
        List<String> _SelectableSkills = new List<string>();

        List<SkillValue> _CurrentSkills = new List<SkillValue>();

        ButtonStringPopover _SkillDetailPopover;
        TextBoxDialog _TBDialog;
        string _DetailText = "";

        Dictionary<int, ButtonPropertyManager> _SkillModManagers = new Dictionary<int, ButtonPropertyManager>();


        
        static List<String> _OptionsSkills = new List<string>(new string[]
            {"Craft", "Knowledge", "Perform", "Profession"});

        string _SelectedSkill;

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorStatisticsPage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorStatisticsPage_iPhone" : "MonsterEditorStatisticsPage_iPad", null)
		{
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
			
            _AreaViews = new List<GradientView>() {BaseView, ModView, DescriptionView, SkillsView};
            foreach (GradientView v in _AreaViews)
            {
                StylePanel(v);
            }



            ButtonPropertyManager pm;
            pm = new ButtonPropertyManager(BaseAtkButton, DialogParent, CurrentMonster, "BaseAtk") {Title = "Base Attack Bonus"};
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(CMBButton, DialogParent, CurrentMonster, "CMB_Numeric") {Title ="CMD"};
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(CMDButton, DialogParent, CurrentMonster, "CMD_Numeric") {Title="CMB"};
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(RacialModsButton, DialogParent, CurrentMonster, "RacialMods");
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(AuraButton, DialogParent, CurrentMonster, "Aura");
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(SQButton, DialogParent, CurrentMonster, "SQ") {Multiline = true, Title = "Special Qualities"};
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(LanguagesButton, DialogParent, CurrentMonster, "Languages") { Multiline = true};
            _Managers.Add(pm);
            pm = new ButtonPropertyManager(GearButton, DialogParent, CurrentMonster, "Gear") { Multiline = true};
            _Managers.Add(pm);

            CMStyles.TextFieldStyle(SkillDetailButton);
            SkillDetailButton.TouchUpInside += HandleSkillDetailClicked;

            _SkillDetailPopover = new ButtonStringPopover(SkillDetailSelectButton);
            _SkillDetailPopover.WillShowPopover += HandleWillShowDetailPopover;
            _SkillDetailPopover.ItemClicked += HandleSkillDetailItemClicked;


            SkillDetailSelectButton.SetImage(UIExtensions.GetSmallIcon("prev"), UIControlState.Normal);

            foreach (GradientButton b in from x in _Managers select x.Button)
            {
                CMStyles.TextFieldStyle(b, 15f);
            }

            AvailableSkillsTable.DataSource = new AvailableViewDataSource(this);
            AvailableSkillsTable.Delegate = new AvailableViewDelegate(this);
            AvailableSkillsTable.RowHeight = 24;

            KnownSkillsTable.DataSource = new KnownViewDataSource(this);
            KnownSkillsTable.Delegate = new KnownViewDelegate(this);
            KnownSkillsTable.RowHeight = 24;

            UpdateSelectableSkills();

            AddSkillButton.TouchUpInside += AddSkillButtonClicked;
		}

        void AddSkillButtonClicked (object sender, EventArgs e)
        {
            SkillValue v = new SkillValue(_SelectedSkill);

            if (_OptionsSkills.Contains(_SelectedSkill))
            {
                v.Subtype = _DetailText.Trim().ToLower();
            }

            if (!CurrentMonster.SkillValueDictionary.ContainsKey(v.FullName))
            {
                CurrentMonster.AddOrChangeSkill(v.Name, v.Subtype, 0);
                UpdateSelectableSkills();
            }
        }

        void HandleSkillDetailItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            _DetailText = (string)e.Tag;
            SkillDetailButton.SetText(_DetailText);
            UpdateSkillButtons();
        }

        void HandleWillShowDetailPopover (object sender, WillShowPopoverEventArgs e)
        {
            if (_SelectedSkill.IsEmptyOrNull())
            {
                e.Cancel = true;
            }
            else
            {
                _SkillDetailPopover.Items.Clear();
                Monster.SkillInfo det = Monster.SkillsDetails[_SelectedSkill];

                foreach (var v in det.Subtypes)
                {
                    _SkillDetailPopover.Items.Add(new ButtonStringPopoverItem()
                                                  {Text = v, Tag= v});
                }
            }
        }

        void HandleSkillDetailClicked (object sender, EventArgs e)
        {
            _TBDialog = new TextBoxDialog();
            _TBDialog.HeaderText = "Skill Detail";
            _TBDialog.SingleLine = true;
            _TBDialog.Value = _DetailText;
            _TBDialog.OKClicked += delegate
            {
                _DetailText = _TBDialog.Value;
                SkillDetailButton.SetText(_DetailText);

            };
            DialogParent.Add (_TBDialog.View);
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

        public void UpdateSelectableSkills()
        {
            _SelectableSkills.Clear();
            _SelectableSkills.AddRange(from x in Monster.SkillsList.Keys where (SelectableSkillsFilter(x)) select x);
            AvailableSkillsTable.ReloadData();
            _CurrentSkills = new List<SkillValue>(from x in CurrentMonster.SkillValueList orderby x.FullName select x);
            KnownSkillsTable.ReloadData();
            UpdateSkillButtons();
        }

        public bool SelectableSkillsFilter(object ob)
        {
            string skill = (string)ob;

            if (_OptionsSkills.Contains(skill))
            {
                return true;
            }
            else
            {
                return !CurrentMonster.SkillValueDictionary.ContainsKey(skill);
            }
        }

        private void UpdateSkillButtons()
        {

            NSIndexPath ip = AvailableSkillsTable.IndexPathForSelectedRow;
            if (ip == null)
            {
                _SelectedSkill = null;
            }
            else
            {
                _SelectedSkill = _SelectableSkills[ip.Row];
            }

            AddSkillButton.Enabled = (_SelectedSkill != null);

            SkillDetailButton.Hidden = !_OptionsSkills.Contains(_SelectedSkill);
            SkillDetailSelectButton.Hidden = SkillDetailButton.Hidden;

            AddSkillButton.Enabled =
                _SelectedSkill != null &&
                    (!_DetailText.IsEmptyOrNull() || 
                     !_OptionsSkills.Contains(_SelectedSkill));

        }

        private class AvailableViewDataSource : UITableViewDataSource
        {
            MonsterEditorStatisticsPage state;
            public AvailableViewDataSource(MonsterEditorStatisticsPage state) 
            {
                this.state = state;
            }
            
            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return state._SelectableSkills.Count;
            }
            
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("AvailableViewDataSourceCell");
                
                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "AvailableViewDataSourceCell");
                }
                
                cell.TextLabel.Text = state._SelectableSkills[indexPath.Row];
                cell.TextLabel.Font = UIFont.SystemFontOfSize(16f);
                cell.Data = state._SelectableSkills[indexPath.Row];
                
                return cell;            
            }

          
        }
        
        
        private class AvailableViewDelegate : UITableViewDelegate
        {
            MonsterEditorStatisticsPage state;
            public AvailableViewDelegate(MonsterEditorStatisticsPage state)   
            {
                this.state = state;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                state.UpdateSkillButtons();
            }
            
            public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
            {
                state.UpdateSkillButtons();
            }
            
            public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return 24;
            }
        }

        private class KnownViewDataSource : UITableViewDataSource
        {
            MonsterEditorStatisticsPage state;
            public KnownViewDataSource(MonsterEditorStatisticsPage state) 
            {
                this.state = state;
            }
            
            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return state._CurrentSkills.Count;
            }
            
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("AvailableViewDataSourceCell");
                
                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "AvailableViewDataSourceCell");
                }

                SkillValue v = state._CurrentSkills[indexPath.Row];

                
                cell.TextLabel.Text = v.FullName;
                cell.TextLabel.Font = UIFont.SystemFontOfSize(16f);
                cell.Data = v;

                GradientButton b = new GradientButton();
                var man  = new ButtonPropertyManager(b, state.DialogParent, v, "Mod");
                man.FormatDelegate = a => ((int)a).PlusFormat();
                state._SkillModManagers[indexPath.Row] = man;
                b.SetSize(65, 22);
                cell.AccessoryView = b;

                
                return cell;            
            }

            public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                SkillValue sv = state._CurrentSkills[indexPath.Row];
                state.CurrentMonster.SkillValueDictionary.Remove(sv.FullName);
                state.CurrentMonster.UpdateSkillValueList();
                state.UpdateSelectableSkills();
            }

          
        }
        
        
        private class KnownViewDelegate : UITableViewDelegate
        {
            MonsterEditorStatisticsPage state;
            public KnownViewDelegate(MonsterEditorStatisticsPage state)   
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
                return 24;
            }

            
            public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public MonsterEditorStatisticsPage State
            {
                get
                {
                    return state;
                }
            }

        }
	}
}


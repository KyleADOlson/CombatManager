/*
 *  ImportExportDialog.cs
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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Foundation;
using CombatManager;

namespace CombatManagerMono
{
    public class ImportExportDialogEventArgs
    {
        public ExportData Data{get; set;}
    }

    public delegate void ImportExportDialogEventHandler(object sender, ImportExportDialogEventArgs e);

    public partial class ImportExportDialog : UIViewController
    {
        ExportData data;

        ViewDataSource viewDataSource;
        ViewDelegate viewDelegate;

        int _SelectedIndex = -1;
        int selectedList;

        public event ImportExportDialogEventHandler ImportExportComplete;

        public bool importMode;

        List<List<ChecklistItem>> lists;

        public ImportExportDialog (ExportData data) : this(data, true)
        {

        }

        public ImportExportDialog (ExportData data, bool importMode) : base ("ImportExportDialog", null)
        {
            this.importMode = importMode;
            this.data = data;

            BuildLists();


            SetupDataSource();

        }

        void SetupDataSource()
        {
            _SelectedIndex = Math.Min(_SelectedIndex, lists[selectedList].Count - 1);


            viewDelegate = new ViewDelegate(this);
            viewDataSource = new ViewDataSource(this);
        }

        void BuildLists()
        {
            lists = new List<List<ChecklistItem>>();

            for (int i=0; i<4; i++)
            {
                lists.Add(new List<ChecklistItem>());
            }

            foreach (Monster m in data.Monsters)
            {
                lists[0].Add(new ChecklistItem(false, m.Name));
            }
            foreach (Spell s in data.Spells)
            {
                lists[1].Add(new ChecklistItem(false, s.Name));
            }
            foreach (Feat f in data.Feats)
            {
                lists[2].Add(new ChecklistItem(false, f.Name));
            }
            foreach (Condition c in data.Conditions)
            {
                lists[3].Add(new ChecklistItem(false, c.Name));
            }
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

            if (View.Bounds.Height > View.Bounds.Width)
            {
                View.SetSize((float)View.Bounds.Height, (float)View.Bounds.Width);
            }


            ButtonView.CornerRadius = 0;
            ButtonView.Border = 0;
            ButtonView.Gradient = new GradientHelper(0x0.UIColor());
            ButtonView.BackgroundColor = UIColor.Clear;

            TitleView.CornerRadius = 0;
            TitleView.Border = 0;
            TitleView.Gradient = new GradientHelper(0x0.UIColor());
            TitleView.BackgroundColor = UIColor.Clear;

            if (!importMode)
            {
                TitleLabel.Text = "Select Items to Export";
            }

            CMStyles.StyleBasicPanel(BackgroundView);

            OKButton.StyleStandardButton();
            CancelButton.StyleStandardButton();

            BackgroundButton.TouchUpInside += (sender, e) => {
                this.View.RemoveFromSuperview();
            };


            SelectAllButton.TouchUpInside += (sender, e) => {
                foreach (ChecklistItem item in lists[selectedList])
                {
                    item.IsChecked = true;
                }
                FileTableView.ReloadData();
            };

            FileTableView.Delegate = viewDelegate;
            FileTableView.DataSource = viewDataSource;

            _SelectedIndex = -1;
            UpdateOK();


        }

        void HandleFileNameTextAllEditingEvents (object sender, EventArgs e)
        {
            UpdateOK();
        }


        void HandleExistsAlertClicked (object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 1)
            {
                this.View.RemoveFromSuperview();
                SendSaveEvent();
            }
        }

        private void SendSaveEvent()
        {
            ImportExportDialogEventArgs ea = new ImportExportDialogEventArgs();
            //ea.Files = new string[] {_SaveFile};
            ImportExportComplete(this, ea);
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
            return true;
        }

        partial void CancelButton_TouchUpInside(GradientButton sender)
        {

            this.View.RemoveFromSuperview();
        }

        partial void OKClicked(GradientButton sender)
        {            
            ImportExportComplete?.Invoke(this, new ImportExportDialogEventArgs(){Data = Results});
            this.View.RemoveFromSuperview();
        }

        public void UpdateOK()
        {

        }

        partial void ListSelectionViewChanged(UISegmentedControl sender)
        {
            selectedList = (int)ListSelectionView.SelectedSegment;
             
            FileTableView.ReloadData();
        }

        ExportData Results
        {
            get
            {
                ExportData outputData = new ExportData();

                for (int i=0; i<lists[0].Count; i++)
                {
                    if (lists[0][i].IsChecked)
                    {
                        outputData.Monsters.Add(data.Monsters[i]);
                    }
                }
                for (int i=0; i<lists[1].Count; i++)
                {
                    if (lists[1][i].IsChecked)
                    {
                        outputData.Spells.Add(data.Spells[i]);
                    }
                }
                for (int i=0; i<lists[2].Count; i++)
                {
                    if (lists[2][i].IsChecked)
                    {
                        outputData.Feats.Add(data.Feats[i]);
                    }
                }
                for (int i=0; i<lists[3].Count; i++)
                {
                    if (lists[3][i].IsChecked)
                    {
                        outputData.Conditions.Add(data.Conditions[i]);
                    }
                }
                return outputData;
            }
        }

        public class ChecklistItem
        {
            public ChecklistItem(){}
            public ChecklistItem(bool isChecked, string text)
            {
                Text = text;
                IsChecked = isChecked;
            }

            public string Text {get; set;}
            public bool IsChecked {get; set;}
        }


        private class ViewDataSource : UITableViewDataSource
        {

            UIImage uncheckedImage = UIImage.FromFile("Images/External/CheckBoxUnchecked.png");
            UIImage checkedImage = UIImage.FromFile("Images/External/CheckBox.png");

            ImportExportDialog state;
            public ViewDataSource(ImportExportDialog state) 
            {
                this.state = state;


            }

            private List<ChecklistItem> SelectedList
            {
                get
                {
                    return state.lists[state.selectedList];
                }
            }
                        

            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return SelectedList.Count;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {

                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("ImportExportDialogCell");

                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "ImportExportDialogCell");
                }
                cell.ImageView.Image = SelectedList[indexPath.Row].IsChecked?checkedImage:uncheckedImage;

                cell.Data =  SelectedList[indexPath.Row];




                cell.TextLabel.Text = SelectedList[indexPath.Row].Text;



                return cell;            
            }


        }


        private class ViewDelegate : UITableViewDelegate
        {
            ImportExportDialog state;
            public ViewDelegate(ImportExportDialog state)   
            {
                this.state = state;
            }

            private List<ChecklistItem> SelectedList
            {
                get
                {
                    return state.lists[state.selectedList];
                }
            }

            private ChecklistItem SelectedItem
            {
                get
                {
                    return SelectedList[state._SelectedIndex];
                }
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                if (state != null)
                {
                    state._SelectedIndex = indexPath.Row;
                    SelectedItem.IsChecked = !SelectedItem.IsChecked;
                    tableView.ReloadData();

                    state.UpdateOK();
                }
            }

            public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
            {
                if (state != null)
                {
                    state.UpdateOK();
                }

            }



            public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return 28;
            }

            public ImportExportDialog ListView
            {
                get
                {
                    return state;
                }
            }
        }
    }
}


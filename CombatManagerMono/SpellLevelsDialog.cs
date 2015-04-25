
using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CombatManager;
using System.Collections.Generic;

namespace CombatManagerMono
{
    public partial class SpellLevelsDialog : StandardDialogView
    {
        Spell _spell;

        public SpellLevelsDialog(Spell spell): base(UserInterfaceIdiomIsPhone?"SpellLevelsDialog_iPhone":"SpellLevelsDialog_iPad", null)
        {
            _spell = spell;
        }
             

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }


        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            BackgroundView.BackgroundColor = UIColor.Clear;
            BackgroundView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
            BackgroundView.Border = 2.0f;
            BackgroundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);


            LevelsTable.DataSource = new ViewDataSource(this);
            LevelsTable.Delegate = new ViewDelegate(this);

        }


        partial void OkButtonClicked(NSObject sender)
        {
            HandleOK();
        }

        partial void CancelButtonClicked(NSObject sender)
        {
            HandleCancel();

        }

        partial void AddButtonClicked(NSObject sender)
        {

        }

        public void SpellLevelClicked(Spell.SpellAdjuster.LevelAdjusterInfo info)
        {


        }

        public void DeleteLevelClicked(Spell.SpellAdjuster.LevelAdjusterInfo info)
        {
            _spell.Adjuster.Levels.Remove(info);
            LevelsTable.ReloadData();
        }

        private class ViewDataSource : UITableViewDataSource
        {
            SpellLevelsDialog state;

            public ViewDataSource(SpellLevelsDialog state)  
            {
                this.state = state;


            }

            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return state._spell.Adjuster.Levels.Count;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("CurrentFeatsViewCell");

                if (cell == null)
                {
                    cell = new DataListViewCell (UITableViewCellStyle.Default, "CurrentFeatsViewCell");
                }


                Spell.SpellAdjuster.LevelAdjusterInfo info = state._spell.Adjuster.Levels[indexPath.Row];


                cell.TextLabel.Text = info.Class;

                cell.TextLabel.Font = UIFont.SystemFontOfSize(15);

                GradientButton b = new GradientButton();
                b.Frame = new CGRect(0, 3, 160, 22);
                b.TitleLabel.Font = UIFont.SystemFontOfSize(15);
                CMStyles.TextFieldStyle(b);
                b.SetText(info.Class);
                b.TouchUpInside += (sender, e) => state.SpellLevelClicked(info);
                UIView view = new UIView();
                UIButton x = new UIButton();
                UIImage redx = UIImage.FromFile("Images/External/redx.png");
                x.SetImage(redx, UIControlState.Normal);
                x.Frame = new CGRect(165, 0, redx.Size.Width, redx.Size.Height);
                x.TouchUpInside += (sender, e) => state.DeleteLevelClicked(info);

                view.Bounds = new CGRect(0, 0, x.Frame.Right, x.Frame.Bottom);
                view.Add(b);
                view.Add(x);


                cell.AccessoryView = view;


                return cell; 
   
            }

            public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                Spell.SpellAdjuster.LevelAdjusterInfo info = state._spell.Adjuster.Levels[indexPath.Row];
                state._spell.Adjuster.Levels.Remove(info);
            }


        }

        private class ViewDelegate : UITableViewDelegate
        {
            SpellLevelsDialog state;


            public ViewDelegate(SpellLevelsDialog state)  
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
                return 30;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

        }

    }
}


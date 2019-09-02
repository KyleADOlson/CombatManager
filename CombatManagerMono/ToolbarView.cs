/*
 *  ToolbarView.cs
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
using Foundation;
using UIKit;
using CoreGraphics;
using CombatManager;

namespace CombatManagerMono
{
	public class ToolbarView : UIView
	{
		
		public delegate void ToolbarClickEventHandler(object sender, int button);
		
		List<GradientButton> buttons = new List<GradientButton>();
		
		
		public event ToolbarClickEventHandler ButtonClicked;
		
		GradientButton clickedButton = null;

        GradientButton _AboutButton;
        GradientButton _SettingsButton;

        ButtonStringPopover settingsPopover;

        ImportExportDialog ieDialog;

        ButtonStringPopoverItem serverItem;
		
		public ToolbarView ()
		{
			
			BackgroundColor = new UIColor(1, 0, 0, 0);
			
			List<String> names = new List<String>() {"Combat", "Monsters", "Feats", "Spells", "Rules", "Treasure"};
			List<String> images = new List<String>() {"sword", "monster", "star", "scroll", "book", "treasure"};

			float pos = 0;
			float buttonWidth = 110;
			float buttonGap = -1;
			int i=0;
			foreach (string s in names)
			{
				GradientButton b = new GradientButton();
				b.Frame = (new CGRect(pos, 0, buttonWidth, 50));
				b.SetImage(UIExtensions.GetSmallIcon(images[i]), UIControlState.Normal);

				b.Border = 1;
                b.CornerRadii = new float[] {4, 16, 0, 0};
				
				b.SetTitle(s, UIControlState.Normal);
				UIEdgeInsets si = b.ImageEdgeInsets;
				si.Right += 10;
				b.ImageEdgeInsets = si;
				
				pos += buttonWidth + buttonGap;
				buttons.Add(b);
				b.Tag = i;
				b.TouchUpInside += HandleBTouchUpInside;
				i++;
				
				
				AddSubview(b);
			}
			
			clickedButton = buttons[0];
            clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
		
            _SettingsButton = new GradientButton();
            _SettingsButton.SetImage(UIImage.FromFile("Images/settings.png"), UIControlState.Normal);
            //_SettingsButton.Border = 0;
            //_SettingsButton.BackgroundColor = UIColor.Clear;
            //_SettingsButton.Gradient = new GradientHelper(0x00000000.UIColor());
            _SettingsButton.CornerRadius = 0;
            _SettingsButton.TouchUpInside += SettingsButtonClicked;            
            _SettingsButton.Frame = new CGRect(Bounds.Width - 64, (Bounds.Height - 48.0f)/2.0f, 48f, 48f);

            AddSubview (_SettingsButton);

            settingsPopover = new ButtonStringPopover(_SettingsButton);
            var pi = new ButtonStringPopoverItem(){ Text = "Import"};
            settingsPopover.Items.Add(pi);
            pi = new ButtonStringPopoverItem { Text = "Export"};
            settingsPopover.Items.Add(pi);
            settingsPopover.Items.Add(new ButtonStringPopoverItem());
            serverItem = new ButtonStringPopoverItem { Text = "Run Local Service" };
            SetLocalServiceIcon();
            settingsPopover.Items.Add(serverItem);
            pi = new ButtonStringPopoverItem { Text = "Local Service Port" };
            settingsPopover.Items.Add(pi);
            pi = new ButtonStringPopoverItem { Text = "Local Service Passcode" };
            settingsPopover.Items.Add(pi);
            settingsPopover.ItemClicked += (sender, eee) => 
            {
                switch (eee.Index)
                {
                    case 0:

                        Import();                    

                        break;
                    case 1:

                        Export();
                        
                        break;
                    case 2:
                        LocalServiceClicked();
                        break;
                    case 4:
                        LocalServicePortClicked();
                        break;
                    case 5:
                        LocalServicePasscodeClicked();
                        break;
                }
            };


            _AboutButton = new GradientButton();
            _AboutButton.SetImage(UIImage.FromFile("Images/External/info.png"), UIControlState.Normal);
            // _AboutButton.Border = 0;
            //_AboutButton.BackgroundColor = UIColor.Clear;
            //_AboutButton.Gradient = new GradientHelper(0x00000000.UIColor());
            _AboutButton.CornerRadius = 0;
            _AboutButton.TouchUpInside += AboutButtonClicked;            
            _AboutButton.Frame = new CGRect(Bounds.Width - 23, (Bounds.Height - 48.0f)/2.0f, 48f, 48f);

            Add (_AboutButton);
			BackgroundColor = UIColor.Black;

            MobileSettings.Instance.PropertyChanged += MobileSettingsPropertyChanged;
			
		}

        void SetLocalServiceIcon()
        {

            serverItem.Icon = MobileSettings.Instance.RunLocalService ? "check" : null;
        }

        private void MobileSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RunLocalService")
            {
                SetLocalServiceIcon();
            }
        }

        void Import()
        {
            OpenDialog ofd = new OpenDialog(true, new List<string>(){"*.cmx"});
            ofd.FilesOpened += (sn, ee) => 
            {
                try
                {
                    ExportData data = new ExportData();
                    foreach (var x in ee.Files)
                    {
                        try
                        {
                            ExportData newData = XmlLoader<ExportData>.Load(x);
                            data.Append(newData);
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.WriteLine(ex.ToString());
                        }
                    }


                    ieDialog = new ImportExportDialog(data, true);
                    ieDialog.ImportExportComplete += (sss, e) => 
                    {
                        ExportData newData = e.Data;


                        foreach (Monster m in e.Data.Monsters)
                        {
                            m.DBLoaderID = 0;
                            MonsterDB.DB.AddMonster(m);
                            Monster.Monsters.Add(m);
                        }
                        foreach (Spell s in e.Data.Spells)
                        {
                            s.DBLoaderID = 0;
                            Spell.AddCustomSpell(s);
                        }
                        foreach (Feat s in e.Data.Feats)
                        {
                            s.DBLoaderID = 0;
                            Feat.AddCustomFeat(s);
                        }
                        foreach (Condition s in e.Data.Conditions)
                        {
                            Condition.CustomConditions.Add(s);
                        }
                        if (e.Data.Conditions.Count > 0)
                        {
                            Condition.SaveCustomConditions();
                        }
                        MainUI.MainView.ReloadTabs();



                    };
                    MainUI.MainView.AddSubview(ieDialog.View);
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteLine(ex.ToString());
                }
            };
            MainUI.MainView.AddSubview(ofd.View);

        }

        void Export()
        {
            ieDialog = new ImportExportDialog(ExportData.DataFromDBs(), true);
            ieDialog.ImportExportComplete += (sss, e) => 
            {
                OpenDialog ofd = new OpenDialog(false, new List<string>(){"*.cmx"});
                ofd.FilesOpened += (sn, ee) => 
                {
                    try
                    {
                        XmlLoader<ExportData>.Save(e.Data, ee.Files[0]);
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.WriteLine(ex.ToString());
                    }
                };
                MainUI.MainView.AddSubview(ofd.View);
            };
            MainUI.MainView.AddSubview(ieDialog.View);
        }

        void LocalServiceClicked()
        {
            MobileSettings.Instance.RunLocalService = !MobileSettings.Instance.RunLocalService;
        }

        void LocalServicePortClicked()
        {
            var alert = new UIAlertView();
            alert.Title = "Enter Port";
            alert.AddButton("OK");
            alert.AddButton("Cancel");
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            alert.GetTextField(0).Text = MobileSettings.Instance.LocalServicePort.ToString();
            alert.Show();
            alert.Clicked += PortAlertClicked;
        }

        void PortAlertClicked(object sender, UIButtonEventArgs ea)
        {
            UIAlertView alert = (UIAlertView)sender;
            if (ea.ButtonIndex == 0)
            {
                String text = alert.GetTextField(0).Text;
                int port;
                if (int.TryParse(text, out port))
                {
                    if (port > 0 && port < 32768)
                    {
                        MobileSettings.Instance.LocalServicePort = port;
                    }
                }
            }

        }

        void LocalServicePasscodeClicked()
        {
            var alert = new UIAlertView();
            alert.Title = "Enter Port";
            alert.AddButton("OK");
            alert.AddButton("Cancel");
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            alert.GetTextField(0).Text = MobileSettings.Instance.LocalServicePasscode;
            alert.Show();
            alert.Clicked += PasscodeAlertClicked;
        }

        void PasscodeAlertClicked(object sender, UIButtonEventArgs ea)
        {
            UIAlertView alert = (UIAlertView)sender;
            if (ea.ButtonIndex == 0)
            {
                String text = alert.GetTextField(0).Text.Trim();
                MobileSettings.Instance.LocalServicePasscode = text;
            }

        }

        void SettingsButtonClicked (object sender, EventArgs e)
        {

        }

        void AboutButtonClicked (object sender, EventArgs e)
        {
            AboutView view = new AboutView();
            view.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            AppDelegate.RootController.PresentModalViewController(view, true);
        }
		

		void HandleBTouchUpInside (object sender, EventArgs e)
		{

            if (ButtonClicked != null)
            {
                ButtonClicked(this, (int)((UIButton)sender).Tag);
            }
            SetClickedButtonGradients((GradientButton)sender);
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
            float buttonSize = 44f;

            _AboutButton.Frame = new CGRect(Bounds.Width - 54, (Bounds.Height - buttonSize)/2.0f, buttonSize, buttonSize);
            _SettingsButton.Frame = new CGRect(Bounds.Width - 102, (Bounds.Height - buttonSize)/2.0f, buttonSize, buttonSize);


		}

        void SetClickedButtonGradients(GradientButton b)
        {
            if (clickedButton != null)
            {
                clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorMedium, CMUIColors.PrimaryColorDarker);
            }
            clickedButton = b;
            if (clickedButton != null)
            {
                clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
            }

        }

        public void SetClickedButton(int button)
        {
            buttons.RunOnIndex(button, SetClickedButtonGradients);
        }
		
	}
}


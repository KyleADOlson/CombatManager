/*
 *  OpenDialog.cs
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

namespace CombatManagerMono
{
	public class OpenDialogEventArgs
	{
		public string[] Files {get; set;}
	}
	
	public delegate void OpenDialogEventHandler(object sender, OpenDialogEventArgs e);
	
	public partial class OpenDialog : UIViewController
	{
		List<string> files;
		
		static List<String> _OpenExtensions = new List<String>()
		{
			"*.cmpt","*.cmet","*.por","*.rpgrp"	
		};
		
		ViewDataSource viewDataSource;
		ViewDelegate viewDelegate;
		
		int _SelectedIndex = -1;
		
		UIAlertView existsAlert;
		
		public event OpenDialogEventHandler FilesOpened;
		
		private bool _OpenMode;
		
		private string _SaveFile;
		
		public OpenDialog() : this(true)
		{
			
		}
		
		public OpenDialog (bool openMode) : base ("OpenDialog", null)
		{
			_OpenMode = openMode;
			
			files = new List<string>();
				
			foreach (String ext in _OpenExtensions)
			{
				files.AddRange(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ext, SearchOption.TopDirectoryOnly));
		
		
			}

			
			viewDelegate = new ViewDelegate(this);
			viewDataSource = new ViewDataSource(this);
			
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
			
			ButtonView.CornerRadius = 0;
            ButtonView.Border = 0;
            ButtonView.Gradient = new GradientHelper(0x0.UIColor());
            ButtonView.BackgroundColor = UIColor.Clear;

            TitleView.CornerRadius = 0;
            TitleView.Border = 0;
            TitleView.Gradient = new GradientHelper(0x0.UIColor());
            TitleView.BackgroundColor = UIColor.Clear;

            CMStyles.StyleBasicPanel(BackgroundView);

            OKButton.StyleStandardButton();
            CancelButton.StyleStandardButton();

			OKButton.TouchUpInside += HandleOKButtonTouchUpInside;
			CancelButton.TouchUpInside += HandleCancelButtonTouchUpInside;
			BackgroundButton.TouchUpInside += HandleCancelButtonTouchUpInside;
			
			FileNameText.AllEditingEvents += HandleFileNameTextAllEditingEvents;
			
			FileTableView.Delegate = viewDelegate;
			FileTableView.DataSource = viewDataSource;
			
			_SelectedIndex = -1;
			UpdateOK();
			
			if (_OpenMode)
			{
				FileNameText.Enabled = false;	
			}
			else
			{
				TitleLabel.Text = "Save File";
			}
			
		}

		void HandleFileNameTextAllEditingEvents (object sender, EventArgs e)
		{
			UpdateOK();
		}

		void HandleCancelButtonTouchUpInside (object sender, EventArgs e)
		{
			this.View.RemoveFromSuperview();
		}

		void HandleOKButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_OpenMode)
			{
				if (_SelectedIndex != -1)
				{
					
					this.View.RemoveFromSuperview();
					
					if (FilesOpened != null)
					{
						OpenDialogEventArgs ea = new OpenDialogEventArgs();
						ea.Files = new string[] {files[_SelectedIndex]};
						FilesOpened(this, ea);
					}
				}
			}
			else
			{
				
				bool isLegal = true;
				bool exists = false;
				
				
				string name = FileNameText.Text.Trim();
				string shortName = name;
					
				
				if (Regex.Match(name, "[/\\\\]").Success)
				{
					isLegal = false;
				}
				
				if (isLegal)
				{
				
					if (!Regex.Match(name, Regex.Escape(".cmpt") + "$", RegexOptions.IgnoreCase).Success)
					{
						name += ".cmpt";
						shortName = name;
					}
					
					name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), name);
				
					try
					{
						FileInfo info = new FileInfo(name);
						exists = info.Exists;
						
					}
					catch (ArgumentException)
					{
						isLegal = false;
					}
				}
					
				if (isLegal)
				{
					_SaveFile = name;
					if (exists)
					{
						existsAlert = new UIAlertView    
						{        
							Title = shortName + " already exists",
							Message = "Are you sure you want to overwrite this file?"
							
						};        
						existsAlert.AddButton("Cancel");    
						existsAlert.AddButton("Ok");
						existsAlert.Show();
						existsAlert.Clicked += HandleExistsAlertClicked;
					}
					
					
					this.View.RemoveFromSuperview();
					SendSaveEvent();
				}
				else
				{
					SendSaveEvent();
				}
					
			}
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
			OpenDialogEventArgs ea = new OpenDialogEventArgs();
			ea.Files = new string[] {_SaveFile};
			FilesOpened(this, ea);
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
		
		
		public void UpdateOK()
		{
			if (_OpenMode)
			{
				OKButton.Enabled = (_SelectedIndex != -1);	
			}
			else
			{	
				this.OKButton.Enabled = (FileNameText.Text.Trim().Length > 0);
			}
		}
		
		
		private class ViewDataSource : UITableViewDataSource
		{
			OpenDialog state;
			public ViewDataSource(OpenDialog state)	
			{
				this.state = state;
				
				
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return state.files.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("OpenDialogCell");
				
				if (cell == null)
				{
					cell = new DataListViewCell (UITableViewCellStyle.Default, "OpenDialogCell");
				}
			
				cell.Data = state.files[indexPath.Row];
				
				
				cell.TextLabel.Text = new FileInfo(state.files[indexPath.Row]).Name;
				
				return cell;			
			}

            public override void CommitEditingStyle(UITableView tableView,
                                                    UITableViewCellEditingStyle editingStyle,
                                                    NSIndexPath indexPath)
            {
                if (editingStyle == UITableViewCellEditingStyle.Delete)
                {
                    String text = state.files[indexPath.Row];
                    state.files.RemoveAt (indexPath.Row);
                    tableView.DeleteRows(new [] { indexPath }, UITableViewRowAnimation.Fade);
                    File.Delete(text);

                }
            }
			
		}
		
		
		private class ViewDelegate : UITableViewDelegate
		{
			OpenDialog state;
			public ViewDelegate(OpenDialog state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (state != null)
				{
					state._SelectedIndex = indexPath.Row;
					state.UpdateOK();
					
					if (!state._OpenMode)
					{
						state.FileNameText.Text = new FileInfo(state.files[state._SelectedIndex]).Name;	
					}
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
			
			public OpenDialog ListView
			{
				get
				{
					return state;
				}
			}
		}
	}
}


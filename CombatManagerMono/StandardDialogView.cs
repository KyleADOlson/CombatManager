using UIKit;
using CoreGraphics;
using System;
using Foundation;


namespace CombatManagerMono
{
	public class StandardDialogView : UIViewController
	{
		
		private string _HeaderText = "Header";
		private UILabel _HeaderLabel;
		
		public event EventHandler OKClicked;
		public event EventHandler CancelClicked;
		
		public StandardDialogView() : base ()
		{
			Initialize();
		}
		
		public StandardDialogView(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		public StandardDialogView(NSObjectFlag t) : base(t)
		{
			Initialize();
		}
		
		public StandardDialogView(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		
		
		public StandardDialogView (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize();
		}
		
		protected virtual void Initialize()
		{
			
		}
		
		protected void StyleButton(GradientButton b)
		{
            b.StyleStandardButton();
		}
		
		protected void StyleHeader(GradientView headerView, UILabel headerLabel)
		{

			StylePanel1(headerView);
			
			headerLabel.TextColor = UIColor.White;
			headerLabel.BackgroundColor = UIColor.Clear;
			headerLabel.Text = _HeaderText;	
			_HeaderLabel = headerLabel;
		}
		
		protected void StylePanel1(GradientView headerView)
		{

			headerView.BackgroundColor = UIColor.Clear;
			headerView.Border = 0;
			headerView.Gradient = new GradientHelper(CMUIColors.PrimaryColorDark);
			
		}
		
		protected void StylePanel2(GradientView headerView)
		{

			headerView.BackgroundColor = UIColor.Clear;
			headerView.Border = 0;
			headerView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);
			
		}
		
		protected void StyleBackground(GradientView backgroundView)
		{
			
			backgroundView.BackgroundColor = UIColor.Clear;
			backgroundView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
			backgroundView.Border = 2.0f;
			backgroundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);
			
		}
		
		protected void HandleOK()
		{
			View.RemoveFromSuperview();
			if (OKClicked != null)
			{
				OKClicked(this, new EventArgs());
			}
		}
		
		protected void HandleCancel()
		{
			View.RemoveFromSuperview();
			if (CancelClicked != null)
			{
				CancelClicked(this, new EventArgs());
			}
		}
		
		
		
		public string HeaderText
		{
			get
			{
				return _HeaderText;
			}
			set
			{
				_HeaderText = value;
				
				if (_HeaderLabel != null)
				{
					_HeaderLabel.Text = _HeaderText;
				}
			}
		}
	}
}


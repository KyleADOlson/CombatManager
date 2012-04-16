using System;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace CombatManagerMono
{
	[Register("GradientButton")]
	public partial class GradientButton : UIButton
	{
		UIColor _color1 = CMUIColors.PrimaryColorMedium;
		UIColor _color2 = CMUIColors.PrimaryColorDarker;
		float _cornerRadius = 10.0f;
		float _border = 1.0f;
		UIColor _borderColor = CMUIColors.PrimaryColorDark;
		GradientHelper _gradient;
		
		object _Data;
		
		public GradientButton (IntPtr p) : base(p)
		{
			Initialize();
		}
		
		public GradientButton ()
			
		{
			Initialize();
		}
		
		private void Initialize()
		{
			
		}
		
		
		public override void Draw (RectangleF rect)
		{
			SetNeedsDisplay();
			
			base.Draw (rect);
		
			
			
			rect.X += _border/2.0f;
			rect.Width -= _border;
			rect.Y += _border/2.0f;
			rect.Height -= _border;
			
			
			CGContext cr = UIGraphics.GetCurrentContext();
			
			//cr.SetAllowsAntialiasing(true);
			if (_gradient != null)
			{
				cr.DrawRoundRect(_gradient.Gradient, rect, _cornerRadius);
			}
			else
			{
				cr.DrawRoundRect(_color1, _color2, rect, _cornerRadius);
			}
			
			if (_border > 0)
			{
				GraphicUtils.RoundRectPath(cr, rect, _cornerRadius);
				cr.SetStrokeColor(_borderColor.CGColor.Components);
				cr.SetLineWidth(_border);
				cr.StrokePath();	
			}
			
		}
		
		
		public UIColor Color1
		{
			get
			{
				return _color1;
			}
			set
			{
				_color1 = value;
				SetNeedsDisplay();
			}
		}
		
		public UIColor Color2
		{
			get
			{
				return _color2;
			}
			set
			{
				_color2 = value;
				SetNeedsDisplay();
			}
		}
		
		public UIColor BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				SetNeedsDisplay();
			}
		}
		
		public float CornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				_cornerRadius = value;
				SetNeedsDisplay();
			}
		}
		
		public float Border
		{
			get
			{
				return _border;
			}
			set
			{
				_border = value;
				SetNeedsDisplay();
			}
		}
		
		public GradientHelper Gradient
		{
			get
			{
				
				return _gradient;
			}
			set
			{
				_gradient = value;
				SetNeedsDisplay();
			}
		}
		
		public object Data
		{
			get
			{
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}
	}
}


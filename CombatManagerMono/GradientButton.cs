/*
 *  GradientButton.cs
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
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace CombatManagerMono
{
	[Register("GradientButton")]
	public partial class GradientButton : UIButton
	{
		float _cornerRadius = 6.0f;
		float _border = 1.0f;
        UIColor _borderColor = 0xFF000000.UIColor();
		GradientHelper _gradient = new GradientHelper(CMUIColors.PrimaryColorMedium, CMUIColors.PrimaryColorDarker);
        GradientHelper _DisabledGradient = CMUIColors.DisabledGradient;
        bool [] _SkipCorners = new bool[4] ;
        float [] _CornerRadii = null;
		
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

            float [] cornerRadii = new float[4];
            if (_CornerRadii != null)
            {
                Array.Copy(_CornerRadii, cornerRadii, 4);
            }
            else if (_SkipCorners != null)               
            {
                for (int i=0; i<4; i++)
                {
                    cornerRadii[i] = _SkipCorners[i]?0f:CornerRadius;
                }
            }
            else
            {
                for (int i=0; i<4; i++)
                {
                    cornerRadii[i] = CornerRadius;
                }
            }


            GradientHelper useGradient = _gradient;
            if (!Enabled && _DisabledGradient != null)
            {
                useGradient = _DisabledGradient;
            }
			
			cr.DrawRoundRect(useGradient.Gradient, rect, cornerRadii);
			
			
			if (_border > 0)
			{
                if (_SkipCorners == null && _CornerRadii == null)
                {
				
                    GraphicUtils.RoundRectPath(cr, rect, _cornerRadius);
                }
                else if (_SkipCorners !=null)
                {
                    
                    GraphicUtils.RoundRectPath(cr, rect, _cornerRadius, _SkipCorners);
                }
                else if (_CornerRadii != null)
                {
                    
                    GraphicUtils.RoundRectPath(cr, rect, _CornerRadii);
                }

				cr.SetStrokeColor(_borderColor.CGColor.Components);
				cr.SetLineWidth(_border);
				cr.StrokePath();	
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
                _CornerRadii = null;
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

        public GradientHelper DisabledGradient
        {
            get
            {
                
                return _DisabledGradient;
            }
            set
            {
                _DisabledGradient = value;
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

        public bool[] SkipCorners
        {
            get
            {
                return _SkipCorners;
            }
            set
            {
                if (value != null && value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException("value", "value must be length 4 or null");
                }


                _SkipCorners = value;
                if (value != null)
                {
                    _CornerRadii = null;
                }
                SetNeedsDisplay();


            }
        }

        public float[] CornerRadii
        {
            get
            {
                return _CornerRadii;
            }
            set
            {
                if (value != null && value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException("value", "value must be length 4 or null");
                }


                _CornerRadii = value;
                if (value != null)
                {
                    _SkipCorners = null;
                }
                SetNeedsDisplay();


            }
        }
    }
}


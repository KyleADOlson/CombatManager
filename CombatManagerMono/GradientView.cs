/*
 *  GradientView.cs
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
using CoreGraphics;
using UIKit;
using Foundation;

namespace CombatManagerMono
{

	[Register("GradientView")]
	public class GradientView : UIView
	{
		UIColor _color1 = UIExtensions.RGBColor(0x222222);
		UIColor _color2 = UIExtensions.RGBColor(0xAAAAAA);
		float _cornerRadius = 8.0f;
		float _border = 1.0f;
		UIColor _borderColor = UIExtensions.RGBColor(0x0);
		GradientHelper _gradient;
        float [] _CornerRadii = null;
		
		
		public GradientView (IntPtr p) : base(p)
		{

		}
		
		public GradientView ()
		{

		}
		
		
		
		public override void Draw (CGRect rect)
		{
			rect.X += _border/2.0f;
			rect.Width -= _border;
			rect.Y += _border/2.0f;
			rect.Height -= _border;
			
			CGContext cr = UIGraphics.GetCurrentContext ();

            float [] cornerRadii = new float[4];
            if (_CornerRadii != null)
            {
                Array.Copy(_CornerRadii, cornerRadii, 4);
            }
            else
            {
                for (int i=0; i<4; i++)
                {
                    cornerRadii[i] = CornerRadius;
                }
            }
			
			if (_gradient != null)
			{
				cr.DrawRoundRect(_gradient.Gradient, rect, cornerRadii);
			}
			else
			{
				cr.DrawRoundRect(GraphicUtils.CreateNormalGradient(_color1, _color2), rect, cornerRadii);
			}
			
			if (_border > 0)
			{
				GraphicUtils.RoundRectPath(cr, rect, cornerRadii);
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
                SetNeedsDisplay();


            }
        }
	}
				
}


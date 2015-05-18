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
using CoreGraphics;
using UIKit;
using Foundation;

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
        GradientHelper _HighlightedGradient;
        bool [] _SkipCorners = new bool[4] ;
        float [] _CornerRadii = null;
		
		object _Data;

        private UIImage _BonusImage;
        private CGRect _BonusImageRect;

		
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

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
            base.TouchesBegan (touches, evt);
            SetNeedsDisplay();
        }
		
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            SetNeedsDisplay();
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt);
            SetNeedsDisplay();
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled (touches, evt);
            SetNeedsDisplay();
        }

       
		
		public override void Draw (CGRect rect)
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
            bool reversed = false;
            if (!Enabled && _DisabledGradient != null)
            {
                useGradient = _DisabledGradient;
            }
            else if (Highlighted && Tracking)
            {
                if (_HighlightedGradient == null)
                {
                    reversed = true;
                }
                else
                {
                    _gradient = _HighlightedGradient;
                }
            }


			
			cr.DrawRoundRect(useGradient.Gradient, rect, cornerRadii, reversed?(float)Math.PI/2.0f:(float)-Math.PI/2.0f);
			
			
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

            if (BonusImage != null)
            {
                cr.TranslateCTM(0, BonusImage.Size.Height + _BonusImageRect.Y * 2.0f);
                cr.ScaleCTM( 1.0f, -1.0f);

                cr.DrawImage(_BonusImageRect, _BonusImage.CGImage);
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

        public GradientHelper HighlightedGradient
        {
            get
            {
                return _HighlightedGradient;
            }
            set
            {
                _HighlightedGradient = value;
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

        public UIImage BonusImage
        {
            get
            {
                return _BonusImage;
            }
            set
            {
                if (_BonusImage != value)
                {
                    _BonusImage = value;
                    SetNeedsDisplay();
                }
            }
        }

        public CGRect BonusImageRect
        {
            get
            {
                return _BonusImageRect;
            }
            set
            {
                if (_BonusImageRect != value)
                {
                    _BonusImageRect = value;
                    if (_BonusImage != null)
                    {
                        SetNeedsDisplay();
                    }
                }
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


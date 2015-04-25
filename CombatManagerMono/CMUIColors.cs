/*
 *  CMUIColors.cs
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
using UIKit;
namespace CombatManagerMono
{
	public static class CMUIColors
	{
		public static UIColor PrimaryColorLighter
		{
			get
			{
				return UIExtensions.RGBColor(0xBBDBDD);
			}
		}
	
		 public static UIColor PrimaryColorLight
		{
			get
			{
				return UIExtensions.RGBColor(0xAFDBDD);
			}
		}
		 public static UIColor PrimaryColorMedium
		{
			get
			{
				return UIExtensions.RGBColor(0x87B9BB);
			}
		}
		 public static UIColor PrimaryColorDark
		{
			get
			{
				return UIExtensions.RGBColor(0x6F8B8C);
			}
		}  		 
		 public static UIColor PrimaryColorDarker
		{
			get
			{
				return UIExtensions.RGBColor(0x2C767A);
			}
		}   		   		
		 public static UIColor SecondaryColorALighter
		{
			get
			{
				return UIExtensions.RGBColor(0xFFF2D8);
			}
		}
		 public static UIColor SecondaryColorALight
		{
			get
			{
				return UIExtensions.RGBColor(0xFFEDC9);
			}
		}
		 public static UIColor SecondaryColorAMedium
		{
			get
			{
				return UIExtensions.RGBColor(0xFFE8B8);
			}
		}
		 public static UIColor SecondaryColorADark
		{
			get
			{
				return UIExtensions.RGBColor(0xBFB297);
			}
		} 
		 public static UIColor SecondaryColorADarker
		{
			get
			{
				return UIExtensions.RGBColor(0xA6833C);
			}
		}     
		 public static UIColor SecondaryColorBLighter
		{
			get
			{
				return UIExtensions.RGBColor(0xFFD9D8);
			}
		}
		 public static UIColor SecondaryColorBLight
		{
			get
			{
				return UIExtensions.RGBColor(0xFFCBC9);
			}
		}
		 public static UIColor SecondaryColorBMedium
		{
			get
			{
				return UIExtensions.RGBColor(0xFFB9B8);
			}
		}
		 public static UIColor SecondaryColorBDark
		{
			get
			{
				return UIExtensions.RGBColor(0xBF9897);
			}
		}  
		 public static UIColor SecondaryColorBDarker
		{
			get
			{
				return UIExtensions.RGBColor(0xA63E3C);
			}
		}   

        public static GradientHelper DisabledGradient
        {
            get
            {
                return new GradientHelper(0xFF999999.UIColor(), 0xFF555555.UIColor());
            }
        }
	}
}


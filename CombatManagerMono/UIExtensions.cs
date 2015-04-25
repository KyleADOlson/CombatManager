/*
 *  UIExtensions.cs
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

namespace CombatManagerMono
{
	public static class UIExtensions
	{
		public static UIColor RGBColor(uint rgb)
		{
			return ARGBColor(0xFF000000 | rgb);
		}
		
		public static UIColor ARGBColor(uint argb)
		{
		    Byte a =(byte)( (argb >> 24) & (0x000000FF));
		    Byte r = (byte)( (argb >> 16) & (0x000000FF));
		    
		    Byte g = (byte)( (argb >> 8) & (0x000000FF));
		    
		    Byte b = (byte)( (argb) & (0x000000FF));
			
			float af = ((float)a)/255.0f;
			float rf = ((float)r)/255.0f;
			float gf = ((float)g)/255.0f;
			float bf = ((float)b)/255.0f;
			
			return new UIColor(rf, gf, bf, af);
		}

        public static UIColor UIColor(this uint argb)
        {
            return ARGBColor(argb);
        }
        public static UIColor UIColor(this int argb)
        {
            return ((uint)argb).UIColor();
        }
		
    public static void SetWidth(this UIView view, float width)
        {
            CGRect frame = view.Frame;
            frame.Width = width;
            view.Frame = frame;
            
        }
        
        public static void SetHeight(this UIView view, float height)
        {
            CGRect frame = view.Frame;
            frame.Height = height;
            view.Frame = frame;
            
        }   

        
        public static void SetSize(this UIView view, CGSize size)
        {
            view.SetSize((float)size.Width, (float)size.Height);
        }

        public static void SetSize(this UIView view, float width, float height)
        {
            CGRect frame = view.Frame;
            frame.Height = height;
            frame.Width = width;
            view.Frame = frame;
            
        }
        
        public static void SetX(this UIView view, float x)
        {
            CGRect frame = view.Frame;
            frame.X = x;
            view.Frame = frame;
            
        }
        
        public static void SetY(this UIView view, float y)
        {
            CGRect frame = view.Frame;
            frame.Y = y;
            view.Frame = frame;
            
        }
        
        public static void SetLocation(this UIView view, CGPoint loc)
        {
            view.SetLocation((float)loc.X, (float)loc.Y);
        }
        
        public static void SetLocation(this UIView view, float x, float y)
        {
            CGRect frame = view.Frame;
            CGPoint loc = frame.Location;
            loc.X = x;
            loc.Y = y;
            frame.Location = loc;
            view.Frame = frame;
            
        }
        
        public static void SetPosition(this UIView view, float x, float y, float width, float height)
        {
            CGRect frame = view.Frame;
            frame.X = x;
            frame.Y = y;
            frame.Height = height;
            frame.Width = width;
            view.Frame = frame; 
        }

		
		public static void SetText(this UIButton button, string text)
		{
			button.SetTitle((text==null)?"":text, UIControlState.Normal);	
		}
		
		public static void SetSmallIcon(this UIButton button, string name)
		{
			button.SetImage(GetSmallIcon(name), UIControlState.Normal);	
		}
		
		public static bool IsVertical
		{
			get
			{
				return UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait || 
			    UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown;
			}
		}
		
		public static UIImage GetSmallIcon(string name)
		{
			if (name == "notehs")
			{
				return UIImage.FromFile("Images/External/NoteHS.png");
			}
			if (name == "openhs")
			{
				return UIImage.FromFile("Images/External/openHS.png");
			}
			if (name == "savehs")
			{
				return UIImage.FromFile("Images/External/saveHS.png");
			}
			
			return UIImage.FromFile("Images/External/" + name + "-16.png");
		}
		
		public static bool IsEmptyOrNull(this string str)
		{
			return String.IsNullOrEmpty(str);
		}

        public static String NullToEmpty(this string str)
        {
            return (str == null)?"":str;
        }

        public static CGRect OriginRect(this CGSize size)
        {
            return new CGRect(new CGPoint(0, 0), size);
        }

        public static CGPoint Center(this CGRect rect)
        {
            return new CGPoint(rect.Height/2.0f, rect.Width/2.0f);
        }

        public static CGPoint Add(this CGPoint p1, CGPoint p2)
        {
            return new CGPoint(p1.X + p2.X, p1.Y + p2.Y);
        }
		
	}
}


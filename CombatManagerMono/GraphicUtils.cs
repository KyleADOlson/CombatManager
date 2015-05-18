/*
 *  GraphicUtils.cs
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
using CoreGraphics;
using Foundation;
using System.Collections.Generic;


namespace CombatManagerMono
{
	public static class GraphicUtils
	{      
        public static void RoundRectPath(CGContext cr, CGRect rect, float cornerRadius, bool[] skipCorner = null)
        {
            if (skipCorner == null)
            {
                skipCorner = new bool[4];
            }

            if (skipCorner.Length != 4)
            {
                throw new ArgumentOutOfRangeException("skipCorner", "skipCorner count must be 4.");
            }

            float [] corners = new float[4];

            for (int i=0; i<4; i++)
            {
                corners[i] = skipCorner[i]?0:cornerRadius;
            }
            RoundRectPath(cr, rect, corners);

        }



		public static void RoundRectPath(CGContext cr, CGRect rect, float[] cornerRadii)
		{
            if (cornerRadii == null || cornerRadii.Length != 4)
            {

                throw new ArgumentOutOfRangeException("cornerRadii", "cornerRadii count must be 4.");
            }

		    cr.BeginPath();
            float radius = cornerRadii[0];
            cr.AddArc(rect.X + radius, rect.Y+radius, radius, (float)Math.PI, (float)(3.0f*Math.PI/2.0f), false);
            
            radius = cornerRadii[1];
            cr.AddArc(rect.X + rect.Width - radius, rect.Y + radius, radius,  (float)(3.0f*Math.PI/2.0f), 2.0f*(float)Math.PI, false);

            radius = cornerRadii[2];
            cr.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height-radius, radius, (float)(2*Math.PI), 5.0f*((float)Math.PI/2.0f), false);

            radius = cornerRadii[3];
            cr.AddArc(rect.X + radius, rect.Y + rect.Height-radius, radius,(float) (5.0f*Math.PI/2.0f), (float)(3.0f*Math.PI), false);

                
		    cr.ClosePath();
		}
		
		public static CGGradient CreateFlatGradient(UIColor color)
		{
			return CreateNormalGradient(color, color);
		}
            
		
		public static CGGradient CreateNormalGradient(nfloat r1, nfloat g1, nfloat b1, nfloat a1, 
                                   nfloat r2, nfloat g2, nfloat b2, nfloat a2)
		{

			nfloat [] components = new nfloat[] {r1, g1, b1, a1, r2, g2, b2, a2};
			nfloat [] locations = new nfloat[] {0f, 1f};
    
		    CGColorSpace space = CGColorSpace.CreateDeviceRGB();
					
		    CGGradient normalGradient = new CGGradient(space, components, locations);
		    
			space.Dispose();
		    
		    return normalGradient;
			
		}
		
		public static CGGradient CreateNormalGradient(UIColor c1, UIColor c2)
		{
			nfloat[] f1 = c1.CGColor.Components;	
			nfloat[] f2 = c2.CGColor.Components;	
			
			return CreateNormalGradient(f1[0], f1[1], f1[2], f1[3], f2[0], f2[1], f2[2], f2[3]);
		}
		
		public static CGGradient CreateGradient(List<UIColor> colors, List<float> points)
		{
			
			List<nfloat> colorf = new List<nfloat>();
			
			foreach (UIColor c in colors)
			{
				nfloat r, g, b, a;
				c.GetRGBA(out r, out g, out b, out a);
				colorf.Add(r);
				colorf.Add(g);
				colorf.Add(b);
				colorf.Add(a);
				
			}
			nfloat [] components = colorf.ToArray();
			
            nfloat[] locations = new nfloat[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                locations[i] = points[i];
            }
                
    
		    CGColorSpace space = CGColorSpace.CreateDeviceRGB();
					
		    CGGradient normalGradient = new CGGradient(space, components, locations);
		    
			space.Dispose();
		    
		    return normalGradient;
		}
		

        public static void DrawRoundRect(this CGContext cr, CGGradient gradient, CGRect rect, float cornerRadius, float angle = (float)-Math.PI/2.0f)
        {		
            DrawRoundRect(cr, gradient, rect, new float[]{cornerRadius, cornerRadius, cornerRadius, cornerRadius}, angle);
        }
		
    	public static void DrawRoundRect(this CGContext cr, CGGradient gradient, CGRect rect, float[] cornerRadii, float angle = (float)-Math.PI/2.0f)
		{
			
			cr.SaveState();
		      
		    RoundRectPath(cr, rect, cornerRadii);
		    
		    cr.Clip();
		    CGPoint startg = RectIntersect(angle, rect);
		    CGPoint endg = RectIntersect(angle + (float)Math.PI, rect);
		    cr.DrawLinearGradient(gradient, startg, endg,  0);
		    gradient.Dispose();
    
    
    		cr.RestoreState();	
		}
		
		public static CGPoint RectIntersect(float rangle, CGRect rect)
		{	
		    if (rect.Width == 0 || rect.Height == 0)
		    {
		        return new CGPoint(0, 0);
		    }
    
		    float vx = (float)Math.Cos(rangle);
		    float vy = (float)Math.Sin(rangle);
		    
            float px = (float)rect.Width/2;
            float py = (float)rect.Height/2;
		    
		    
		   
		    
            float x1 = (float)rect.X;
            float y1 = (float)rect.Y;
            float x2 = (float)x1 + (float)rect.Width;
            float y2 = (float)y1 + (float)rect.Height;
    
    
		    float [] t = new float[] {0, 0, 0, 0};
		    
		    for (int i=0; i<4; i++)
		    {
		        t[i] = -1;
		    }
		    
		    if (vx != 0)
		    {
		        t[0]=(x1-px)/vx;
		    
		        t[1]=(x2-px)/vx;
		    }
		    
		    if (vy != 0)
		    {
		        
		        t[2]=(y1-py)/vy;
		        t[3]=(y2-py)/vy;
		    }
		    
		    //int minIdx = 0;
		    float minVal = 0;
		    
		    for (int i=0; i<4; i++)
		    {
		        if (t[i] > 0)
		        {
		            if (minVal == 0)
		            {
		                //minIdx = i;
		                minVal = t[i];
		            }
		            else if (t[i] < minVal)
		            {
		                minVal = t[i];
		            }
		        }
			            
		    }
		    
		
		    return new CGPoint(px+minVal*vx, py+minVal*vy);	
		    
		}

	}
}


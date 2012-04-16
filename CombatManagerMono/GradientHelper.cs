using System;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace CombatManagerMono
{
	public class GradientHelper
	{
		bool colorPair;
		UIColor color1;
		UIColor color2;
		
		bool rgba;
		float r1;
		float g1;
		float b1;
		float a1;
		float r2;
		float g2;
		float b2;
		float a2;
		
		bool listColor;
		List<UIColor> colorList;
		List<float> pointList;
		
		
				
		public GradientHelper(UIColor color)
		{
			colorPair = true;
			color1 = color;
			color2 = color;
		}
		
		public GradientHelper(float r1, float g1, float b1, float a1, 
                                   float r2, float g2, float b2, float a2)
		{
		
			rgba = true;
			this.r1 = r1;
			this.g1 = g1;
			this.b1 = b1;
			this.a1 = a1;
			this.r2 = r2;
			this.g2 = g2;
			this.b2 = b2;
			this.a2 = a2;
			
		}
		
		public GradientHelper(UIColor c1, UIColor c2)
		{
			
			colorPair = true;
			color1 = c1;
			color2 = c2;		
		}
		
		public GradientHelper(List<UIColor> colors, List<float> points)
		{
			listColor = true;
			colorList = new List<UIColor>(colors);
			pointList = new List<float>(points);
		}
		
		
		
		public CGGradient Gradient
		{
			get
			{
				if (colorPair)
				{
					return GraphicUtils.CreateNormalGradient(color1, color2);
				}
				else if (rgba)
				{
					return GraphicUtils.CreateNormalGradient(r1, g1, b1, a1, r2, g2, b2, a2);	
				}
				else if (listColor)
				{
					return GraphicUtils.CreateGradient(colorList, pointList); 	
				}
				
				return null;
			}
		}
	}
}


/*
 *  CMUtilities.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace CombatManager
{
 
    public static class CMUIUtilities
    {
        public static T FindVisualParent<T>(this DependencyObject child)
          where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static void ScrollChildToBottom(this DependencyObject e)
        {

            ScrollViewer s = CMUIUtilities.FindVisualChild<ScrollViewer>(e);
            if (s != null)
            {
                s.ScrollToBottom();
            }
        }

        public static ListBoxItem ClickedItem(this ListBox box, MouseEventArgs e)
        {
            return ClickedListBoxItem(box, e);
        }

        public static ListBoxItem ClickedListBoxItem(ListBox box, MouseEventArgs e)
        {
            ListBoxItem target = null;



            for (int i = 0; i < box.Items.Count; i++)
            {
                ListBoxItem item = (ListBoxItem)box.ItemContainerGenerator.ContainerFromIndex(i);

                if (item != null)
                {

                    Point p = e.GetPosition(item);
                    if (p != null)
                    {
                        Rect bounds = VisualTreeHelper.GetDescendantBounds(item);

                        if (bounds.Contains(p))
                        {
                            target = item;
                            break;
                        }
                    }
                }
            }

            return target;
        }

        public static Image GetNamedImageControl(string name)
        {

            Image i = new Image();
            i.Source = StringImageSmallIconConverter.FromName(name);
            i.Width = 16;
            i.Height = 16;

            return i;
        }

        public static void SetNamedIcon(this MenuItem mi, string name)
        {
            mi.Icon = GetNamedImageControl(name); ;
        }

        public static Size Multiply(this Size oldSize, double scale) 
        {
            return new Size(oldSize.Width * scale, oldSize.Height * scale);
        }

        public static Size Divide(this Size oldSize, double scale)
        {
            return new Size(oldSize.Width / scale, oldSize.Height / scale);
        }

        public static Point Subtract(this Size size1, Size size2)
        {
            return new Point(size1.Width - size2.Width, size1.Height - size2.Height);
        }


        public static Point Multiply(this Point oldPoint, double scale)
        {
            return new Point(oldPoint.X * scale, oldPoint.Y * scale);
        }

        public static Point Divide(this Point oldPoint, double scale)
        {
            return new Point(oldPoint.X / scale, oldPoint.Y / scale);
        }

        public static Point Add(this Point oldPoint, double scalar)
        {
            return new Point(oldPoint.X + scalar, oldPoint.Y + scalar);
        }

        public static Point Add(this Point oldPoint, double sX, double sY)
        {
            return new Point(oldPoint.X + sX, oldPoint.Y + sY);
        }

        public static Point Add(this Point oldPoint, Point addPoint)
        {
            return new Point(oldPoint.X + addPoint.X, oldPoint.Y + addPoint.Y);
        }

        public static Size Difference(this Point oldPoint, Point subPoint)
        {

            return new Size(Math.Abs(oldPoint.X - subPoint.X), Math.Abs(oldPoint.Y - subPoint.Y));
        }

        public static Point Subtract(this Point oldPoint, Point subPoint)
        {
            return new Point(oldPoint.X - subPoint.X, oldPoint.Y - subPoint.Y);
        }


        public static Point Lerp(this Point pt1, Point pt2, double pct)
        {
            Point diff = pt2.Subtract(pt1);
            Point change = diff.Multiply(pct.Clamp(0, 1));
            return pt2.Add(change);
        }

        public static Color FindColor(this FrameworkElement element, String name)
        {
            return (Color)element.FindResource(name);
        }

        public static Brush FindSolidBrush(this FrameworkElement element, String name)
        {
            return new SolidColorBrush(element.FindColor(name));
        }

        public static Color ToColor(this UInt32 source)
        {

            byte[] bytes = BitConverter.GetBytes(source);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static UInt32 ToUInt32(this Color color)
        {
            return BitConverter.ToUInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
        }

        public static Point Center(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);

        }

        public static double CircleSize(this Rect rect)
        {
            return Math.Min(rect.Width / 2.0, rect.Height / 2.0);
        }



        public static Rect ScaleCenter(this Rect r, double x, double y)
        {
            Rect rectOut = r;
            double newWidth = r.Width * x;
            double newHeight = r.Height * y;

            double widthInflate = (newWidth - r.Width) / 2.0;
            double heightInflate = (newHeight - r.Height) / 2.0;
            rectOut.Inflate(widthInflate, heightInflate);
            return rectOut;
        }


        public static Geometry RectanglePath(this Rect rect)
        {
            return rect.RectanglePath(0);
        }

        public static Geometry RectanglePath(this Rect rect, double borderWidth)
        {

            double diff = borderWidth / 2.0;

            Rect shrinkRect = rect;
            shrinkRect.Inflate(-diff, -diff);

            return new RectangleGeometry(shrinkRect);

        }

        public static Geometry CirclePath(this Rect rect)
        {
            return rect.CirclePath(0);
        }

        public static Geometry CirclePath(this Rect rect, double borderWidth)
        {
            double circleSize = rect.CircleSize() - (borderWidth / 2.0);

            return new EllipseGeometry(rect.Center(), circleSize, circleSize);
               
        }

        public static Geometry DiamondPath(this Rect rect)
        {
            return rect.DiamondPath(0);
        }






        public static Geometry DiamondPath(this Rect rect, double borderWidth)
        {
            double diff = borderWidth / 2.0;

            Point p1 = new Point(rect.X + rect.Width / 2.0, rect.Y + diff);
            Point p2 = new Point(rect.X + rect.Width - diff, rect.Y + rect.Height / 2.0);
            Point p3 = new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height - diff);
            Point p4 = new Point(rect.X + diff, rect.Y + rect.Height / 2.0);

            return CreatePathGeometry(new Point[] { p1, p2, p3, p4 });

        }
        public static Geometry TargetPath(this Rect rect)
        {
            return rect.TargetPath(0);
        }
        public static Geometry TargetPath(this Rect rect, double borderWidth)
        {

            double diff = borderWidth / 2.0;
            double fullRadius = rect.CircleSize() - diff;
            double bigRadius = fullRadius * .8;
            double smallRadius = bigRadius * .7;

            Rect diffRect = rect;
            diffRect.Inflate(-diff, -diff);

            EllipseGeometry ellipseOut = new EllipseGeometry(rect.Center(), bigRadius, bigRadius);
            EllipseGeometry ellipseIn = new EllipseGeometry(rect.Center(), smallRadius, smallRadius);

            CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Xor, ellipseOut, ellipseIn);

            Rect wRect = diffRect.ScaleCenter(1, .1);
            Rect hRect = diffRect.ScaleCenter(.1, 1);

            cg = new CombinedGeometry(GeometryCombineMode.Union, new RectangleGeometry(wRect), cg);
            cg = new CombinedGeometry(GeometryCombineMode.Union, new RectangleGeometry(hRect), cg);

            Rect cRect = diffRect.ScaleCenter(.3, .3);
            cg = new CombinedGeometry(GeometryCombineMode.Exclude, cg, new RectangleGeometry(cRect));
            return cg;

        }
        public static Geometry StarPath(this Rect rect)
        {
            return rect.StarPath(0);
        }
        public static Geometry StarPath(this Rect rect, double borderWidth)
        {

            double diff = borderWidth / 2.0;
            double radius = (rect.CircleSize() - diff)* .8;
            Point center = rect.Center();
                
            float innerScale = (float)(Math.Cos(2.0 * Math.PI / 5.0) / Math.Cos(Math.PI / 5.0));



            Point[] points = new Point[10];

            for (int i = 0; i < 5; i++)
            {
                double angle = ((double) i) * (Math.PI * 2.0) / 5.0f - Math.PI/2.0 ;
                points[i*2] = new Point(center.X + Math.Cos(angle) * radius, center.Y + Math.Sin(angle) * radius);
                angle += Math.PI * 2.0 / 10.0;
                points[i*2+1] = new Point(center.X + Math.Cos(angle) * radius * innerScale, center.Y + Math.Sin(angle) * radius * innerScale);

            }
            

            return CreatePathGeometry(points);       

        }

        public static PathGeometry CreatePathGeometry(Point[] points)
        {
            if (points.Length > 0)
            {
                Point start = points[0];
                List<LineSegment> segments = new List<LineSegment>();
                for (int i = 1; i < points.Length; i++)
                {
                    segments.Add(new LineSegment(points[i], true));
                }
                PathFigure figure = new PathFigure(start, segments, true);
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                return geometry;
            }
            else
            {
                return null;
            }
        }

        public static DependencyObject FindLogicalNode(this DependencyObject ob, string name)
        {
            return LogicalTreeHelper.FindLogicalNode(ob, name);
        }

        public static BitmapImage LoadBitmapFromImagesDir(String filename)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Images/" + filename));
        }

        public static DependencyObject SetElementVisibility(this DependencyObject dep, String name, Visibility visibility)
        {
            UIElement ui = dep.FindLogicalNode(name) as UIElement;

            if (ui != null)
            {
                ui.Visibility = visibility;
            }

            return dep;
        }

        public static DependencyObject SetElementsVisibility(this DependencyObject dep, String [] names, Visibility visibility)
        {
            foreach (String name in names)
            {
                dep.SetElementVisibility(name, visibility);
            }
            
            return dep;
        }

        public static void ScrollBy(this ScrollViewer viewer, double x, double y)
        {
            double newX = viewer.HorizontalOffset + x;
            double newY = viewer.VerticalOffset + y;

            viewer.ScrollToHorizontalOffset(newX);
            viewer.ScrollToVerticalOffset(newY);

        }

        public static void ScrollTo(this ScrollViewer viewer, double x, double y)
        {
          
            viewer.ScrollToHorizontalOffset(x);
            viewer.ScrollToVerticalOffset(y);

        }

        public static Size ActualSize(this FrameworkElement fe)
        {
            return new Size(fe.ActualWidth, fe.ActualHeight);
        }

        public static Size ScrollableSize(this ScrollViewer sv)
        {
            return new Size(sv.ScrollableWidth, sv.ScrollableHeight);
        }




        public const String PrimaryColorLight = "PrimaryColorLight";
        public const String PrimaryColorMedium = "PrimaryColorMedium";
        public const String PrimaryColorDark = "PrimaryColorDark";
        public const String PrimaryColorDarker = "PrimaryColorDarker";
        public const String SecondaryColorALighter = "SecondaryColorALighter";
        public const String SecondaryColorALight = "SecondaryColorALight";
        public const String SecondaryColorAMedium = "SecondaryColorAMedium";
        public const String SecondaryColorADark = "SecondaryColorADark";
        public const String SecondaryColorADarker = "SecondaryColorADarker";
        public const String SecondaryColorBLighter = "SecondaryColorBLighter";
        public const String SecondaryColorBLight = "SecondaryColorBLight";
        public const String SecondaryColorBMedium = "SecondaryColorBMedium";
        public const String SecondaryColorBDark = "SecondaryColorBDark";
        public const String SecondaryColorBDarker = "SecondaryColorBDarker";
        public const String ThemeTextForeground = "ThemeTextForeground";
        public const String ThemeTextBackground = "ThemeTextBackground";


    }


    public static class CMFileUtilities
    {
        public static string AppDataDir
        {
            get
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = System.IO.Path.Combine(path, "Combat Manager");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public static string AppDataSubDir(String name)
        {
            String path = System.IO.Path.Combine(AppDataDir, name);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

    }

    public abstract class SimpleNotifyClass : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string prop)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }


    public class NotifyValue<T> : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        T _Value;

        public NotifyValue(T val)
        {
            _Value = val;
        }

        public NotifyValue()
        {
        }

        public T Value
        {

            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

    }

    public class DebugTimer
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint timeGetTime();

        uint _Time;
        uint _Last;
        bool _ShowTotals = true;
        string _Name;

        public DebugTimer() : this (null, true)
        {
        }

        public DebugTimer(string name)
            : this(name, true)
        {

        }

        public DebugTimer(string name, bool showTotals) : this(name, showTotals, true)
        {

        }

        public DebugTimer(string name, bool showTotals, bool showStart) 
        {
            _ShowTotals = showTotals;
            _Time = timeGetTime();
            _Last = _Time;
            _Name = name;
            if (showStart)
            {
                System.Diagnostics.Debug.WriteLine("Start Timer " + ((_Name == null) ? "" : _Name));
            }


        }

        private void Mark(string message)
        {

            uint newTime = timeGetTime();
            TimeMessage(message, newTime);
            _Last = newTime;
        }

        public void MarkEvent(string text)
        {
            Mark(text);
        }

        public void MarkEventIf(string message, uint time)
        {

            uint newTime = timeGetTime();

            if (newTime - _Last >= time)
            {

                TimeMessage(message, newTime);
            }
            _Last = newTime;
        }


        public void MarkEventIfTotal(string message, uint time)
        {

            uint newTime = timeGetTime();

            if (newTime - _Time >= time)
            {
                TimeMessage(message, newTime);
            }
            _Last = newTime;
        }

        public void SetLastTime()
        {
            uint newTime = timeGetTime();
            _Last = newTime;
        }


        private void TimeMessage(string message, uint newTime)
        {

            string output = message + ": " + (newTime - _Last);
            if (_ShowTotals)
            {
                output += " Total: " + (newTime - _Time);
            }
            System.Diagnostics.Debug.WriteLine(output);
        }


        public void MarkEvent()
        {
            Mark((_Name==null)?"":_Name);
        }

    }

}

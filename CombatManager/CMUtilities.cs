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

﻿using System;
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
    }


    public static class CMFileUtilities
    {
        public static string AppDataDir
        {
            get
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, "Combat Manager");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
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

/*
 *  SpinControl.xaml.cs
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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for SpinControl.xaml
	/// </summary>
	public partial class SpinControl : UserControl
	{

        int initialVal;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(int?), typeof(SpinControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnValueChanged),
                                              new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// Gets or sets the value assigned to the control.
        /// </summary>
        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceValue(DependencyObject element, object value)
        {
            int? newValue = (int?)value;
            SpinControl control = (SpinControl)element;



            return newValue;
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpinControl control = (SpinControl)obj;

            RoutedPropertyChangedEventArgs<int?> e = new RoutedPropertyChangedEventArgs<int?>(
                (int?)args.OldValue, (int?)args.NewValue, ValueChangedEvent);

            control.OnValueChanged(e);
        }


        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int?>), typeof(SpinControl));

        public event RoutedPropertyChangedEventHandler<int?> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }


        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<int?> args)
        {
            RaiseEvent(args);
        }



		public SpinControl()
		{
			this.InitializeComponent();
		}

        private void HPThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

            Thumb thumb = (Thumb)sender;
            if (Value != null)
            {
                initialVal = Value.Value;
            }
        }

        private void HPThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;

            int newHP = initialVal - (int)(e.VerticalChange / 5.0);

            //int change = newHP - Value.Value;

            //DataContext
        }

        private void HPThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Thumb thumb = (Thumb)sender;

            
            if (Value != null && Value == initialVal)
            {
                if (thumb.Name == "HPUpThumb")
                {

                    Value += 1;

                }
                else
                {
                    Value -= 1;

                }
            }

        }

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            box.SelectAll();
        }

        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {

        }


        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Control box = (Control)sender;

                Popup pop = (Popup)box.FindName("popup");
                pop.IsOpen = true;

                TextBox hpBox = (TextBox)pop.FindName("textBox");

                hpBox.Focus();
                hpBox.SelectAll();
            }
        }

        private void textBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Down ||
                e.Key == Key.Up)
            {



                if (box != null)
                {
                    int val;
                    if (int.TryParse(box.Text, out val))
                    {

                        int change;

                        if (e.Key == Key.Up)
                        {
                            change = val;
                        }
                        else
                        {
                            change = -val;
                        }

                        if (Value != null)
                        {
                            Value += change;
                        }
                    }
                }

                box.SelectAll();
            }
            else if (e.Key == Key.Escape)
            {

                Popup pop = (Popup)box.FindName("popup");
                pop.IsOpen = false;
            }
        }



        private void SubractHPButton_Click(object sender, RoutedEventArgs e)
        {


            Button button = (Button)sender;

            TextBox box = (TextBox)button.FindName("textBox");

            if (box != null)
            {
                int val;
                if (int.TryParse(box.Text, out val))
                {
                    Value += Value;


                }
            }
            
        }

        private void AddHPButton_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;

            TextBox box = (TextBox)button.FindName("textBox");

            if (box != null)
            {
                int val;
                if (int.TryParse(box.Text, out val))
                {
                    Value += val;
                }
            }
            
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //int.TryParse(((TextBox)sender).Text, out lastHPChange);
        }


        private void textBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
	}
}
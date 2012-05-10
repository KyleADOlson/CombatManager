/*
 *  CampaignEventWindow.xaml.cs
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
using System.Windows.Shapes;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for CampaignEventWindow.xaml
	/// </summary>
	public partial class CampaignEventWindow : Window
	{
		private CampaignEvent _Event;
		
		private bool updatingCombos;
		
		public CampaignEventWindow()
		{
			this.InitializeComponent();
			
			Event = new CampaignEvent();

            BuildDateDisplay();
			
		}
		
		public CampaignEvent Event
		{
			get
			{
				return _Event;
			}
			set
			{
				if (_Event != null)
				{
                    _Event.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(EventPropertyChanged);
				}
				
				_Event = value;
                DataContext = _Event;
				

				UpdateDateDisplay();
				
                if (_Event != null)
                {
                    _Event.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(EventPropertyChanged);
                }
				
				
			}
		}

        void EventPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start" || e.PropertyName == "End")
            {
                UpdateDateDisplay();
            }

        }

        void BuildDateDisplay()
        {
            BuildCombos(StartHourCombo, StartMinuteCombo, StartAMPMCombo);

        }
		
		void BuildCombos( ComboBox hourCombo, ComboBox minuteCombo, ComboBox ampmCombo)
        {
            updatingCombos = true;
            for (int i = 0; i < 12; i++)
            {
                int val = (i == 0) ? 12 : i;
                ComboBoxItem item = new ComboBoxItem();
                item.Content = val.ToString();
                item.Tag = i;
                hourCombo.Items.Add(item);

            }
            for (int i = 0; i < 60; i++)
            {

                ComboBoxItem item = new ComboBoxItem();
                item.Content = ":" + i.ToString("00");
                item.Tag = i;
                minuteCombo.Items.Add(item);
            }
            for (int i = 0; i < 2; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = (i==0)?"AM":"PM";
                item.Tag = i;
                ampmCombo.Items.Add(item);
            }
            updatingCombos = false;
			
		}

        void UpdateDateDisplay()
        {
			UpdateTimeCombos(Event.Start, StartDatePicker, StartHourCombo, StartMinuteCombo, StartAMPMCombo);

        }
		
		void UpdateTimeCombos(DateTime time, DatePicker picker, ComboBox hourCombo, ComboBox minuteCombo, ComboBox ampmCombo)
		{
			updatingCombos = true;

            picker.SelectedDate = time;
			
			int hourVal = time.Hour % 12;
			int minuteVal = time.Minute;
			int amPM = time.Hour /12;
			
			hourCombo.SelectedIndex = hourVal;
			minuteCombo.SelectedIndex = minuteVal;
			ampmCombo.SelectedIndex = amPM;
			
			updatingCombos = false;
			
		}

        private void StartCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
			if (!updatingCombos)
			{
				Event.Start = GetComboTime(Event.Start, StartDatePicker, StartHourCombo, StartMinuteCombo, StartAMPMCombo);
			}
        }
		
		
		DateTime GetComboTime(DateTime startTime, DatePicker picker, ComboBox hourCombo, ComboBox minuteCombo, ComboBox ampmCombo)
		{
            DateTime newTime = startTime;
            if (picker.SelectedDate != null)
            {
                startTime = picker.SelectedDate.Value;
            }

            int hour = hourCombo.SelectedIndex + ampmCombo.SelectedIndex * 12;

            newTime = newTime.AddHours(hour - newTime.Hour);

            int minute = minuteCombo.SelectedIndex;
            newTime = newTime.AddMinutes(minute - newTime.Minute);

            return newTime;
            
			
		}

		private void StartDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
            if (!updatingCombos)
            {
                Event.Start = GetComboTime(Event.Start, StartDatePicker, StartHourCombo, StartMinuteCombo, StartAMPMCombo);
            }
		}

		private void EndDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            DialogResult = true;
            Close();
		}
	}
}
/*
 *  AboutWindow.xaml.cs
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            using (Stream textStream = Application.GetResourceStream(new Uri("pack://application:,,,/version")).Stream)
            {

                StreamReader reader = new StreamReader(textStream);

                string text = reader.ReadToEnd();



                RevisionFlowViewer.Document.Blocks.Add(new Paragraph(new Run(text)));
            }


            using (Stream textStream = Application.GetResourceStream(new Uri("pack://application:,,,/license.txt")).Stream)
            {

                StreamReader reader = new StreamReader(textStream);

                string text = reader.ReadToEnd();



                GPLScrollViewer.Document.Blocks.Add(new Paragraph(new Run(text)));
            }


            using (Stream textStream = Application.GetResourceStream(new Uri("pack://application:,,,/Supporters.txt")).Stream)
            {

                StreamReader reader = new StreamReader(textStream);

                string text = reader.ReadToEnd();



                SupporterFlowViewer.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://combatmanager.com");
        }


        private void APIHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://combatmanager.com/localapi.html");
        }

    }
}

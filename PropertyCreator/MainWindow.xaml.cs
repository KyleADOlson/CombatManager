/*
 *  MainWindow.xaml.cs
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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace PropertyCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                FileStream streamIn = new FileStream(filename, FileMode.Open);

                StreamReader reader = new StreamReader(streamIn);

                string outname = filename.Replace(".txt", ".out");

                FileStream streamOut = new FileStream(outname, FileMode.Create);

                StreamWriter writer = new StreamWriter(streamOut);

                ReadWriteVariables(reader, writer);

                writer.Flush();

                streamIn.Close();
                streamOut.Close();


            
            }
               
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            StringReader reader = new StringReader(textBox1.Text);

            StringWriter writer = new StringWriter();


            ReadWriteVariables(reader, writer);



            textBox2.Text = writer.GetStringBuilder().ToString();


        }

        private class VarInfo
        {
            public String Name { get; set; }
            public String VarType { get; set; }
        }

        private void ReadWriteVariables(TextReader reader, TextWriter writer)
        {

            List<VarInfo> variables = new List<VarInfo>();

            String string1 = reader.ReadLine();
            while (string1 != null)
            {
                Regex reg = new Regex("(?<type>[\\p{L}0-9_?<>]+ )?(?<text>[\\p{L}0-9_]+)(;)?(\\s)*(,|$|\\t)");

                foreach (Match m in reg.Matches(string1))
                {
                    string type = "String";
                    if (m.Groups["type"].Success)
                    {
                        type = m.Groups["type"].Value;
                    }

                    var inf = new VarInfo { Name = m.Groups["text"].Value, VarType = type };
                    if (CamelCase)
                    {
                        inf.Name = inf.Name.Substring(0, 1).ToUpper() + inf.Name.Substring(1);
                    }
                    variables.Add(inf);


                }
                string1 = reader.ReadLine();
            }


            foreach (var inf in variables)
            {

                String memberName = GetMemberName(inf.Name);
                writer.WriteLine("        private " + inf.VarType + " " +  memberName  +";");
            }

            writer.WriteLine();

            foreach (var inf in variables)
            {
                String memberName = GetMemberName(inf.Name);
                writer.WriteLine("public " + inf.VarType + " " + inf.Name + "{get{return " + memberName + ";}set{if(" + memberName + "!=value){" + memberName + "=value;");
                if (propertyChangedCheck.IsChecked == true)
                {
                    if (SimpleNotifyCheckBox.IsChecked == true)
                    {
                        writer.WriteLine("Notify(\"" + inf.Name + "\");");
                            
                    }
                    else
                    {


                        writer.WriteLine("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(\"" + inf.Name + "\");");
                    }
                }
            
                writer.WriteLine("} } }");
            }

        }

        bool CamelCase
        {
            get
            {
                return MemberNameComboBox.SelectedIndex == 0;
            }
        }

        string GetMemberName(string name)
        {
            switch (MemberNameComboBox.SelectedIndex)
            {

                case 1:
                    return (name.StartsWith("_")?"":"_") + name;
                case 2:
                    return name + (name.EndsWith("_") ? "" : "_");
                default:
                case 0:
                    return name.Substring(0, 1).ToLower() + name.Substring(1);
            }
                    
        }
    }
}

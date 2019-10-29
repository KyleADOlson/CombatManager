/*
 *  SettingsDialog.xaml.cs
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

using CombatManager.Personalization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {

        bool startDarkScheme;

        public SettingsDialog()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            ConfirmInitiativeCheckbox.IsChecked = UserSettings.Settings.ConfirmInitiativeRoll;
            ConfirmCharacterDeleteCheckbox.IsChecked = UserSettings.Settings.ConfirmCharacterDelete;
            ConfirmApplicationCloseCheckbox.IsChecked = UserSettings.Settings.ConfirmClose;
            ShowAllDamageDice.IsChecked = UserSettings.Settings.ShowAllDamageDice;
            RollAlternativeInitCheckbox.IsChecked = UserSettings.Settings.AlternateInit3d6;
            ShowHiddenInitValueBox.IsChecked = UserSettings.Settings.ShowHiddenInitValue;
            AddMonstersHiddenBox.IsChecked = UserSettings.Settings.AddMonstersHidden;
            StatsOpenByDefaultCheckbox.IsChecked = UserSettings.Settings.StatsOpenByDefault;
            RollAlternateInitDiceBox.Text = UserSettings.Settings.AlternateInitRoll;
            CheckForUpdatesCheckbox.IsChecked = UserSettings.Settings.CheckForUpdates;
            DarkSchemeCheckbox.IsChecked = UserSettings.Settings.DarkScheme;
            startDarkScheme = UserSettings.Settings.DarkScheme;
            LocalWebServiceCheckbox.IsChecked = UserSettings.Settings.RunLocalService;
            PortTextBox.Text = UserSettings.Settings.LocalServicePort.ToString();
            PasscodeTextBox.Text = UserSettings.Settings.LocalServicePasscode;
            AutomaticStabilizationCheckbox.IsChecked = CoreSettings.Instance.AutomaticStabilization;

            RollAlternateInitDiceBox.TextChanged += new TextChangedEventHandler(RollAlternateInitDiceBox_TextChanged);

            int userScheme = UserSettings.Settings.ColorScheme;

            foreach (ColorScheme scheme in ColorSchemeManager.Manager.SortedSchemes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = scheme.Name;
                item.Tag = scheme.ID;
                ColorSchemeCombo.Items.Add(item);
                if (scheme.ID == userScheme)
                {
                    ColorSchemeCombo.SelectedItem = item;
                }
            }



        }

        void RollAlternateInitDiceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DieRoll dr = DieRoll.FromString(RollAlternateInitDiceBox.Text);
            OKButton.IsEnabled = (dr != null);
        }

        private bool EnableOK()
        {
            bool enable = true;
            if (GetLocalPort() == null)
            {
                enable = false;
            }
            OKButton.IsEnabled = enable;
            return enable;
        }
        private ushort? GetLocalPort()
        {
            int port;
            if (int.TryParse(PortTextBox.Text, out port))
            {
                if (port > 0 && port < 65536)
                {
                    return (ushort)port;
                }
            }
            return null;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SaveSettings())
            {
                DialogResult = true;
                Close();
            }

        }

        bool SaveSettings()
        {
            if (EnableOK())
            {
                UserSettings.Settings.ConfirmInitiativeRoll = ConfirmInitiativeCheckbox.IsChecked.Value;
                UserSettings.Settings.ConfirmCharacterDelete = ConfirmCharacterDeleteCheckbox.IsChecked.Value;
                UserSettings.Settings.ConfirmClose = ConfirmApplicationCloseCheckbox.IsChecked.Value;
                UserSettings.Settings.ShowAllDamageDice = ShowAllDamageDice.IsChecked.Value;
                UserSettings.Settings.AlternateInit3d6 = RollAlternativeInitCheckbox.IsChecked.Value;
                UserSettings.Settings.AlternateInitRoll = RollAlternateInitDiceBox.Text;
                UserSettings.Settings.ShowHiddenInitValue = ShowHiddenInitValueBox.IsChecked.Value;
                UserSettings.Settings.AddMonstersHidden = AddMonstersHiddenBox.IsChecked.Value;
                UserSettings.Settings.StatsOpenByDefault = StatsOpenByDefaultCheckbox.IsChecked.Value;
                UserSettings.Settings.CheckForUpdates = CheckForUpdatesCheckbox.IsChecked.Value;
                UserSettings.Settings.ColorScheme = SelectedScheme;
                UserSettings.Settings.DarkScheme = DarkSchemeCheckbox.IsChecked.Value;
                UserSettings.Settings.RunLocalService = LocalWebServiceCheckbox.IsChecked == true;
                UserSettings.Settings.LocalServicePort = GetLocalPort().Value;
                UserSettings.Settings.LocalServicePasscode = PasscodeTextBox.Text;
                CoreSettings.Instance.AutomaticStabilization = AutomaticStabilizationCheckbox.IsChecked == true;
                UserSettings.Settings.SaveOptions();

                CombatState.use3d6 = UserSettings.Settings.AlternateInit3d6;
                CombatState.alternateRoll = UserSettings.Settings.AlternateInitRoll;
                return true;
            }
            return false;

        }

        private void ColorSchemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedColorScheme();
        }

        void ShowSelectedColorScheme()
        {
            if (ColorSchemeCombo.SelectedItem != null)
            {
                ColorManager.SetNewScheme(SelectedScheme, DarkSchemeCheckbox.IsChecked.Value);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            bool needScheme = false;
            if (UserSettings.Settings.DarkScheme != startDarkScheme)
            {
                needScheme = true;
                UserSettings.Settings.DarkScheme = startDarkScheme;
            }

            if (SelectedScheme != UserSettings.Settings.ColorScheme)
            {
                needScheme = true;
            }

            if (needScheme)
            {
                ColorManager.PrepareCurrentScheme();
            }
        }


        private int SelectedScheme
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)ColorSchemeCombo.SelectedItem;
                return (int)item.Tag;

            }


        }

        private void DarkSchemeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            UserSettings.Settings.DarkScheme = true;
            ShowSelectedColorScheme();
        }

        private void DarkSchemeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            UserSettings.Settings.DarkScheme = false;
            ShowSelectedColorScheme();
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOK();
        }

        private static bool IsPortTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            bool allowed = !regex.IsMatch(text);
            return allowed;
        }

        private void PortTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsPortTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }

        private void PortTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsPortTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsPasscodeTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9A-Za-z!@#$%^&*]+");
            bool allowed = !regex.IsMatch(text);
            return allowed;
        }

        private void PascodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void PasscodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsPasscodeTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }

        private void PasscodeTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsPasscodeTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
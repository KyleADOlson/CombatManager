using System;
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
	/// Interaction logic for SettingsDialog.xaml
	/// </summary>
	public partial class SettingsDialog : Window
	{
		
		public SettingsDialog()
		{
			this.InitializeComponent();

            // Insert code required on object creation below this point.
            ConfirmInitiativeCheckbox.IsChecked = UserSettings.Settings.ConfirmInitiativeRoll;
            ConfirmCharacterDeleteCheckbox.IsChecked = UserSettings.Settings.ConfirmCharacterDelete;
            ConfirmApplicationCloseCheckbox.IsChecked = UserSettings.Settings.ConfirmClose;
			ShowAllDamageDice.IsChecked = UserSettings.Settings.ShowAllDamageDice;
			RollAlternativeInitCheckbox.IsChecked = UserSettings.Settings.AlternateInit3d6;
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            UserSettings.Settings.ConfirmInitiativeRoll = ConfirmInitiativeCheckbox.IsChecked.Value;
            UserSettings.Settings.ConfirmCharacterDelete = ConfirmCharacterDeleteCheckbox.IsChecked.Value;
            UserSettings.Settings.ConfirmClose = ConfirmApplicationCloseCheckbox.IsChecked.Value;
			UserSettings.Settings.ShowAllDamageDice = ShowAllDamageDice.IsChecked.Value;
			UserSettings.Settings.AlternateInit3d6 = RollAlternativeInitCheckbox.IsChecked.Value;
            UserSettings.Settings.SaveOptions();

            CombatState.use3d6 = UserSettings.Settings.AlternateInit3d6;

            DialogResult = true;
            Close();
             
		}
			
	}
}
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
	/// Interaction logic for SpellEditorWindow.xaml
	/// </summary>
	public partial class SpellEditorWindow : Window
	{
		private bool _Initialized;
		
		public SpellEditorWindow()
		{
			this.InitializeComponent();

			
			
            foreach (string s in SpellSchoolIndexConverter.Schools)
            {
                SchoolComboBox.Items.Add(s.Capitalize());
            }
			
			_Initialized = true;
			
			UpdateOK();
		}

        private Spell _Spell;

        public Spell Spell
        {
            get { return _Spell; }
            set
            {
                if (_Spell != value)
                {
                    _Spell = value;
					DataContext = _Spell;

                    for (int i = 0; i < SchoolComboBox.Items.Count; i++)
                    {
                        string sch = (string)SchoolComboBox.Items[i];
                        if (sch == StringCapitalizeConverter.Capitalize(_Spell.school))
                        {
                            SchoolComboBox.SelectedIndex = i;
                            break;
                        }
                    }
					
					_Spell.Adjuster.Levels.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Levels_CollectionChanged);


                    CustomBonusBorder.DataContext = _Spell.Bonus;

                    CustomBonusCheckBox.IsChecked = (_Spell.Bonus != null);
                }
            }
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Spell.SpellAdjuster.LevelAdjusterInfo info = (Spell.SpellAdjuster.LevelAdjusterInfo)
                ((FrameworkElement)sender).DataContext;

            Spell.Adjuster.Levels.Remove(info);
        }
        private void UnusedClassesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBox box = ((ListBox)sender);



            var info = (Spell.SpellAdjuster.LevelAdjusterInfo)
                box.SelectedItem;

            Spell.Adjuster.Levels.Add(info);
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            ConditionBonus b = (ConditionBonus)CustomBonusBorder.DataContext;

            _Spell.Bonus = b;

        	DialogResult = true;
            Close();
        }

        private void SpellNameText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        	UpdateOK();
        }

        private void Levels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        	UpdateOK();
        }
		
		private void UpdateOK()
		{
			if (_Initialized)
			{
				OKButton.IsEnabled = SpellNameText.Text.Length > 0 && Spell.Adjuster.Levels.Count > 0;
			}
		}

		private void CustomBonusCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
		{

            CustomBonusBorder.DataContext = new ConditionBonus();
		}

        private void CustomBonusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            CustomBonusBorder.DataContext = null;
        }

		
	}
}
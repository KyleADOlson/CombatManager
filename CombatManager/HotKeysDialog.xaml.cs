using System;
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
using System.Collections.ObjectModel;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for HotKeysDialog.xaml
    /// </summary>
    public partial class HotKeysDialog : Window
    {
		 ObservableCollection<CombatHotKey> _CombatHotKeys;
		
        public HotKeysDialog()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CombatHotKey hk = (CombatHotKey)((FrameworkElement)sender).DataContext;
            _CombatHotKeys.Remove(hk);
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			DialogResult = false;
        	Close();
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			DialogResult = true;
        	Close();
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _CombatHotKeys.Add(new CombatHotKey());
        }       
		
		public List<CombatHotKey> CombatHotKeys 
        {
            get
            {
                return new List<CombatHotKey>(_CombatHotKeys);
            }
            set
            {
                _CombatHotKeys = new ObservableCollection<CombatHotKey>();
                foreach (CombatHotKey hk in _CombatHotKeys)
                {
                    _CombatHotKeys.Add(new CombatHotKey(hk));
                }
                KeyListBox.ItemsSource = _CombatHotKeys;
            }
        }

		private void CommandComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
            ComboBox typeCombo = (ComboBox)sender;
            DependencyObject parent = VisualTreeHelper.GetParent(typeCombo);
            int count = VisualTreeHelper.GetChildrenCount(parent);
            ComboBox subtypeCombo = null;
            for (int i = 0; i < count; i++)
            {
                DependencyObject dob = VisualTreeHelper.GetChild(parent, i);
                if (dob is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)dob;
                    if (fe.Name == "SubtypeComboBox")
                    {
                        subtypeCombo = (ComboBox)fe;
                        break;
                    }
                }
            
            }

            subtypeCombo.Items.Clear();
            switch (typeCombo.SelectedIndex)
            {
                case 0:
                    subtypeCombo.IsEnabled = false;
                    break;
                case 1:
                    subtypeCombo.IsEnabled = false;
                    break;
                case 2:
                    subtypeCombo.IsEnabled = true;
                    subtypeCombo.Items.Add(new ComboBoxItem() { Content = "Fort" });
                    subtypeCombo.Items.Add(new ComboBoxItem() { Content = "Ref" });
                    subtypeCombo.Items.Add(new ComboBoxItem() { Content = "Will" });
                    subtypeCombo.SelectedIndex = 0;
                    break;
                case 3:
                    subtypeCombo.IsEnabled = true;
                    foreach (Monster.SkillInfo si in Monster.SkillsDetails.Values)
                    {
                        subtypeCombo.Items.Add(new ComboBoxItem() { Content = si.Name, Tag = si });
                    }
                    break;
            }
		}
    }
}

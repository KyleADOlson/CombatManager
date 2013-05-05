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
                foreach (CombatHotKey hk in value)
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
            UpdateSubtypeCombo(typeCombo.SelectedIndex, subtypeCombo);

		}

        private void UpdateSubtypeCombo(int selectedIndex, ComboBox subtypeCombo)
        {

            if (subtypeCombo != null)
            {
                subtypeCombo.Items.Clear();
                switch (selectedIndex)
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
                        break;
                    case 3:
                        subtypeCombo.IsEnabled = true;
                        foreach (Monster.SkillInfo si in Monster.SkillsDetails.Values)
                        {
                            subtypeCombo.Items.Add(new ComboBoxItem() { Content = si.Name, Tag = si });
                        }
                        break;
                }

                ComboBox cb = subtypeCombo;
                CombatHotKey hk = (CombatHotKey)cb.DataContext;
                SetIndexForString(cb, hk.Subtype);
            }
        }

        private void SetIndexForString(ComboBox cb, String str)
        {
            int val = -1;
            for (int i = 0; i < cb.Items.Count; i++)
            {
                ComboBoxItem it = (ComboBoxItem)cb.Items[i];
                if ((it.Content as string) == str)
                {
                    val = i;
                    break;
                }
            }

            if (val != -1)
            {

                cb.SelectedIndex = val;
            }
            else
            {
                cb.SelectedIndex = 0;
            }
        }

		private void KeyComboBox_Initialized(object sender, System.EventArgs e)
		{
            ComboBox cb = (ComboBox)sender;
            CombatHotKey hk = (CombatHotKey)cb.DataContext;

            string key = (String)new KeyToStringConverter().Convert(hk.Key, 
                typeof(String), null, System.Globalization.CultureInfo.CurrentCulture);


            SetIndexForString(cb, key);
		}

		private void CommandComboBox_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
		}

		private void CommandComboBox_Initialized(object sender, System.EventArgs e)
		{
		}
       
		private void SubtypeComboBox_Initialized(object sender, System.EventArgs e)
		{
            ComboBox cb = (ComboBox)sender;
			CombatHotKey hk = (CombatHotKey)cb.DataContext;
            int index = hk.IntType;
            UpdateSubtypeCombo(index, cb);

            
		}

		private void SubtypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
            ComboBox cb = ((ComboBox)sender);

			CombatHotKey hk = (CombatHotKey)cb.DataContext;
            if (cb.SelectedValue != null)
            {
                hk.Subtype = (String)((ComboBoxItem)cb.SelectedValue).Content;
            }

		}

		private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            CheckBox cb = ((CheckBox)sender);
			CombatHotKey hk = (CombatHotKey)cb.DataContext;
            Grid parent = (Grid)VisualTreeHelper.GetParent(cb);
            UpdateBackground(parent, hk);
		}

        private void UpdateBackground(Grid grid, CombatHotKey hk)
        {
            if (hk.Modifier == ModifierKeys.None || hk.Modifier == ModifierKeys.Shift)
            {
                grid.Background = new SolidColorBrush(Colors.Pink);
            }
            else
            {
                grid.Background = null;
            }
        }

        private void ItemBackground_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {

            Grid cb = ((Grid)sender);
			CombatHotKey hk = (CombatHotKey)cb.DataContext;
            UpdateBackground(cb, hk);
        }

        private void ItemBackground_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            Grid cb = ((Grid)sender);
			CombatHotKey hk = (CombatHotKey)cb.DataContext;
            UpdateBackground(cb, hk);
        }
    }
}

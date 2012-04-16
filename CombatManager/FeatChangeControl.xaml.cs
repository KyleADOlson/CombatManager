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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for FeatChangeControl.xaml
	/// </summary>
	public partial class FeatChangeControl : UserControl
	{
		Monster _Monster;
        ListCollectionView _FeatsView;
        ListCollectionView _CurrentFeatsView;
        ObservableCollection<CharacterFeat> _CurrentFeats;
		
		public FeatChangeControl()
		{
			this.InitializeComponent();
            //RemoveButton.IsEnabled = false;

            if (Feat.FeatMap != null)
            {

                _FeatsView = new ListCollectionView(new List<Feat>(Feat.FeatMap.Values));
                _FeatsView.Filter = FeatsViewFilter;
                _FeatsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                SelectFeatBox.DataContext = _FeatsView;
            }
        }

        private bool FeatsViewFilter(Object obj)
        {
            Feat feat = (Feat)obj;

            string text = FeatFilterTextBox.Text.Trim();

            if (text.Length == 0)
            {
                return true;
            }

            return feat.Name.ToUpper().Contains(text.ToUpper());
        }

        private void FeatFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_FeatsView != null)
            {
                _FeatsView.Refresh();
            }
        }

        private void SelectFeatBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CMUIUtilities.ClickedListBoxItem((ListBox)sender, e) != null)
            {
                AddSelectedFeats();
            }
        }

        private void AddSelectedFeats()
        {
            foreach (Feat feat in SelectFeatBox.SelectedItems)
            {
                string name = feat.Name;

                if (feat.AltName != null && feat.AltName.Length > 0)
                {
                    name = feat.AltName;
                }

                if (!_Monster.FeatsList.Contains(name))
                {
                    _Monster.AddFeat(name);
                    _CurrentFeats.Add(new CharacterFeat(name));
                }
            }
        }


        public Monster Monster
        {
            get
            {
                return _Monster;
            }
            set
            {
                _Monster = value;
                _CurrentFeats = new ObservableCollection<CharacterFeat>();
                foreach (string feat in _Monster.FeatsList)
                {
                    CharacterFeat cf = new CharacterFeat();
                    _CurrentFeats.Add(new CharacterFeat(feat));
                }
                _CurrentFeatsView = new ListCollectionView(_CurrentFeats);
                _CurrentFeatsView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
                CurrentFeatsBox.DataContext = _CurrentFeatsView;
            }
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;

            CharacterFeat feat = (CharacterFeat)el.DataContext;

            _CurrentFeats.Remove(feat);
            _Monster.RemoveFeat(feat.Text);

        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;

            CharacterFeat feat = (CharacterFeat)el.DataContext;
        	_Monster.RemoveFeat(feat.FeatSource);
            feat.FeatSource = feat.Text;
			_Monster.AddFeat(feat.FeatSource);
			
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			AddSelectedFeats();
        }

        private void CurrentFeatsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentFeatsBox.SelectedItem == null)
            {
                //RemoveButton.IsEnabled = false;
            }
            else
            {
                //RemoveButton.IsEnabled = true;
            }
        }

        private void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CharacterFeat feat = (CharacterFeat)CurrentFeatsBox.SelectedItem;

            if (feat != null)
            {
                _CurrentFeats.Remove(feat);
               _Monster.RemoveFeat(feat.Text);
            }
        }

        private class CharacterFeat : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            private String _Name;
            private String _Choice;
			private String _FeatSource;

            public CharacterFeat()
            {

            }

            public CharacterFeat(string details)
            {
                ParseFeat(details);
            }

            public void ParseFeat(string details)
            {
                Regex reg = new Regex("(?<name>.+?) \\((?<choice>.+?)\\)");


                Match m = reg.Match(details);

                if (m.Success)
                {

                    this.Name = m.Groups["name"].Value;
                    this.Choice = m.Groups["choice"].Value;
                }

                else
                {
                    Name = details;
                }
				
				_FeatSource = details;


            }

            public String Name
            {
                get { return _Name; }
                set
                {
                    if (_Name != value)
                    {
                        _Name = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
                    }
                }
            }
            public String Choice
            {
                get { return _Choice; }
                set
                {
                    if (_Choice != value)
                    {
                        _Choice = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Choice")); }
                    }
                }
            }
			
			public String FeatSource
			{
				
                get { return _FeatSource; }
                set
                {
                    if (_FeatSource != value)
                    {
                        _FeatSource = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FeatSource")); }
                    }
                }
			}

            [XmlIgnore]
            public String Text
            {
                get
                {
                    string text = _Name;

                    if (_Choice != null && _Choice.Length > 0)
                    {
                        text += " (" + _Choice + ")";  
                    }
                    return text;
                }
            }

            public override string ToString()
            {
                return Text;
            }




        }
    
	}
}
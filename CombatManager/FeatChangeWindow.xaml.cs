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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for FeatChangeWindow.xaml
    /// </summary>
    public partial class FeatChangeWindow : Window
    {
        Character _Character;


        public FeatChangeWindow()
        {
            InitializeComponent();

		}
		
		public Character Character
		{
			get
			{
				return _Character;
			}
			set
			{
				if (_Character != value)
				{
					_Character = value;
					ChangeControl.Monster = _Character.Monster;
				}
			}
		}
	}	
}

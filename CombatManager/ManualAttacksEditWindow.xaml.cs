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
	/// Interaction logic for ManualAttacksEditWindow.xaml
	/// </summary>
	public partial class ManualAttacksEditWindow : Window
	{
        private Monster _Monster;

		public ManualAttacksEditWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            string attacks = MeleeTextBox.Text.Trim();
            if (attacks.Length == 0)
            {
                attacks = null;
            }
            _Monster.Melee = attacks;
            attacks = RangedTextBox.Text.Trim();
            if (attacks.Length == 0)
            {
                attacks = null;
            }
            _Monster.Ranged = attacks;
            Close();
		}

        public Monster Monster
        {
            get
            {
                return _Monster;
            }
            set
            {
                if (_Monster != value)
                {
                    _Monster = value;

                    this.DataContext = _Monster;

                    MeleeTextBox.Text = _Monster.Melee;
                    RangedTextBox.Text = _Monster.Ranged;
                }
            }
        }
	}
}
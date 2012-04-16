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
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://combatmanager.com");
        }
    }
}

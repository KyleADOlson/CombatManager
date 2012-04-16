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
    /// Interaction logic for OGLWindow.xaml
    /// </summary>
    public partial class OGLWindow : Window
    {
        public OGLWindow()
        {
            InitializeComponent();

            using (Stream textStream = Application.GetResourceStream(new Uri("pack://application:,,,/ogl.txt")).Stream)
            {

                StreamReader reader = new StreamReader(textStream);

                string text = reader.ReadToEnd();



                OGLViewer.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
            
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void MainRectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            Window w = CMUIUtilities.FindVisualParent<Window>(fe);
            
            w.DragMove();

        }
    }
}

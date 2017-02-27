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

namespace CombatManager.Maps
{
    /// <summary>
    /// Interaction logic for GameMapDisplayWindow.xaml
    /// </summary>
    public partial class GameMapDisplayWindow : Window
    {
        GameMap map;

        bool selectingOrigin;



        public GameMapDisplayWindow()
        {
            InitializeComponent();
        }

        public GameMap Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map != value)
                {
                    if (map != null)
                    {
                        map.PropertyChanged -= MapPropertyChanged;
                    }
                    map = value;
                    map.PropertyChanged += MapPropertyChanged;


                    if (map != null && map.Image != null)
                    {

                        UpdateMapImage();
                        DataContext = map;

                    }
                }
            }

         }

        void  MapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Scale")
            {
                UpdateMapImage();
            }
            else if (e.PropertyName == "CellSize" || e.PropertyName == "CellOrigin")
            {
                UpdateGridBrush();
            }
        }

        void UpdateMapImage()
        {

            MapImageControl.Source = map.Image;

            Size size = new Size(map.Image.Width, map.Image.Height);
            size = size.Multiply(map.Scale);

            MapImageControl.Width = size.Width;
            MapImageControl.Height = size.Height;
            MapGridCanvas.Width = size.Width;
            MapGridCanvas.Height = size.Height;


            UpdateGridBrush();
        }


        void UpdateGridBrush()
        {
            double xSize = map.CellSize.Width/map.Image.Width;
            double ySize = map.CellSize.Height/map.Image.Height;
            double xStart = map.CellOrigin.X / map.Image.Width;
            double yStart = map.CellOrigin.Y / map.Image.Height;

            ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/square.png")));
            brush.TileMode = TileMode.Tile;
            brush.Viewport = new Rect(xStart, yStart, xSize, ySize);

            MapGridCanvas.Background = brush;
        }

        private void SetOriginButton_Click(object sender, RoutedEventArgs e)
        {
            selectingOrigin = !selectingOrigin;
        }

        private void MapGridCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectingOrigin)
            {
                Point p = e.GetPosition((Canvas)sender);
                p = p.Divide(map.Scale);
                map.CellOrigin = p;
                //UpdateGridBrush();
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {


        }
    }
}

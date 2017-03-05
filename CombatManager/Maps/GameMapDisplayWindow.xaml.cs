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
    enum GameMapActionMode
    {
        None,
        SetOrigin,
        SetFog,
        SetMarker
    }

    /// <summary>
    /// Interaction logic for GameMapDisplayWindow.xaml
    /// </summary>
    public partial class GameMapDisplayWindow : Window
    {
        GameMap map;
        GameMapActionMode mode;

        bool settingFog;
        bool draggingMap;
        bool newFogState;

        Point lastPosition;

        GameMap.MarkerStyle markerStyle = GameMap.MarkerStyle.Square;
        Color markerColor = Colors.Red;

        public delegate void MapEventDelegate(GameMap map);

        public event MapEventDelegate ShowPlayerMap;

        public GameMapDisplayWindow() : this(false)
        {
        }

        public GameMapDisplayWindow(bool playerMode)
        {
            InitializeComponent();
            
            UpdateActionButtons();

            this.playerMode = playerMode;
            FogOfWar.PlayerMode = playerMode;

            if (playerMode)
            {
                HideGMControls();
            }
        }

        private void HideGMControls()
        {
            RootGrid.ColumnDefinitions[0].Width = new System.Windows.GridLength(0);
            NameGrid.Visibility = Visibility.Collapsed;
            GridSizeGrid.Visibility = Visibility.Collapsed;
            var v = ScaleGrid.Margin;
            v.Left = 0;
            ScaleGrid.Margin = v;
            Binding b = new Binding("TableScale");
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ScaleBox.SetBinding(TextBox.TextProperty, b);
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
                    map.FogOrMarkerChanged += Map_FogOrMarkerChanged;

                    if (map != null && map.Image != null)
                    {

                        UpdateMapImage();
                        DataContext = map;

                    }
                }
            }

        }

        private void Map_FogOrMarkerChanged(GameMap map)
        {
            UpdateGridBrush();
        }

        bool playerMode;

        public bool PlayerMode
        {
            get
            {
                return playerMode;
            }
        }

        void MapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Scale" && !playerMode)
            {
                UpdateMapImage();
            }
            else if (e.PropertyName == "TableScale" && playerMode)
            {
                UpdateMapImage();
            }
            else if(e.PropertyName == "CellSize" || e.PropertyName == "CellOrigin")
            {
                UpdateGridBrush();
            }
        }

        void UpdateMapImage()
        {

            MapImageControl.Source = map.Image;

            Size size = new Size(map.Image.Width, map.Image.Height);
            size = size.Multiply(UseScale);

            MapImageControl.Width = size.Width;
            MapImageControl.Height = size.Height;
            MapGridCanvas.Width = size.Width;
            MapGridCanvas.Height = size.Height;
            FogOfWar.Width = size.Width;
            FogOfWar.Height = size.Height;


            UpdateGridBrush();
        }


        void UpdateGridBrush()
        {
            double xSize = map.CellSize.Width / map.Image.Width;
            double ySize = map.CellSize.Height / map.Image.Height;
            double xStart = map.CellOrigin.X / map.Image.Width;
            double yStart = map.CellOrigin.Y / map.Image.Height;

            ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/square.png")));
            brush.TileMode = TileMode.Tile;
            brush.Viewport = new Rect(xStart, yStart, xSize, ySize);

            MapGridCanvas.Background = brush;
            FogOfWar.InvalidateVisual();
        }

        private void SetOriginButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(GameMapActionMode.SetOrigin, true);
        }

        void SetMode(GameMapActionMode newMode, bool flip)
        {
            if (newMode == mode)
            {
                if (flip)
                {
                    mode = GameMapActionMode.None;
                }
            }
            else
            {
                mode = newMode;
            }
            UpdateActionButtons();
        }




        private void MapGridCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {

                Point p = e.GetPosition((Canvas)sender);
                lastPosition = p;

                draggingMap = true;
            }
        }



        private void MapGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {

                draggingMap = false;
            }
        }



        private void MapGridCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Canvas)sender);
            lastPosition = p;
            switch (mode)
            {
                case GameMapActionMode.SetOrigin:

                    p = p.Divide(UseScale);
                    map.CellOrigin = p;
                    //UpdateGridBrush();
                    break;
                case GameMapActionMode.SetFog:

                    {
                        GameMap.MapCell cell = PointToCell(p);

                        map[cell.X, cell.Y] = !map[cell.X, cell.Y];
                        map.FireFogOrMarkerChanged();
                        settingFog = true;
                        newFogState = map[cell.X, cell.Y];
                        break;
                    }
                case GameMapActionMode.SetMarker:
                    {
                        GameMap.MapCell cell = PointToCell(p);
                        GameMap.Marker marker = new GameMap.Marker();
                        marker.Style = markerStyle;
                        marker.Color = markerColor;
                        map.SetMarker(cell, marker);

                    }
                    break;

            }
        }

        private void MapGridCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        bool rightClickDown = false;
        Point rightClickPosition;
        GameMap.MapCell rightClickCell;

        private void MapGridCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            rightClickDown = true;
            rightClickPosition = e.GetPosition((Canvas)sender);
        }

        private void MapGridCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rightClickDown)
            {
                rightClickDown = false;

                if (!playerMode)
                {

                    GameMap.MapCell cell = PointToCell(rightClickPosition);

                    rightClickCell = cell;

                    if (CellOnBoard(cell))
                    {
                        bool hasMarkers = map.CellHasMarkers(cell);


                        ContextMenu menu = (ContextMenu)Resources["MapContextMenu"];
                        menu.DataContext = cell;

                        menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        menu.IsOpen = true;

                        MenuItem mi;

                        mi = (MenuItem)menu.FindLogicalNode("ToggleFogItem");
                        mi.DataContext = cell;

                        mi = (MenuItem)menu.FindLogicalNode("DeleteMarkerItem");
                        mi.Visibility = hasMarkers ? Visibility.Visible : Visibility.Collapsed;
                        mi.DataContext = cell;


                        mi = (MenuItem)menu.FindLogicalNode("NameBoxItem");
                        mi.Visibility = hasMarkers ? Visibility.Visible : Visibility.Collapsed;

                        if (hasMarkers)
                        {
                            mi.DataContext = map.GetMarkers(cell)[0];
                        }




                    }
                }
            }
        }

        private void MapGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition((Canvas)sender);
            if (settingFog)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    settingFog = false;
                    map.SaveMap(true);
                }
                else
                {


                    GameMap.MapCell cell = PointToCell(p);

                    if (map[cell.X, cell.Y] != newFogState)
                    {
                        map[cell.X, cell.Y] = newFogState;
                        map.FireFogOrMarkerChanged();
                    }
                }

                lastPosition = p;
            }
            if (draggingMap)
            {
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    draggingMap = false;
                }
                else
                {
                    Vector v = p - lastPosition;

                    double newVerticalOffset = MapScrollViewer.VerticalOffset - v.Y;
                    double newHorizontalOffset = MapScrollViewer.HorizontalOffset - v.X;

                    double verticalDiff = 0;
                    double horizontalDiff = 0;

                    if (newVerticalOffset < 0)
                    {
                        verticalDiff = newVerticalOffset;
                        newVerticalOffset = 0;

                    }
                    else if (newVerticalOffset > MapScrollViewer.ScrollableHeight)
                    {
                        verticalDiff = newVerticalOffset - MapScrollViewer.ScrollableHeight;
                        newVerticalOffset = MapScrollViewer.ScrollableHeight;

                    }

                    if (newHorizontalOffset < 0)
                    {
                        horizontalDiff = newHorizontalOffset;
                        newHorizontalOffset = 0;

                    }
                    else if (newHorizontalOffset > MapScrollViewer.ScrollableWidth)
                    {
                        horizontalDiff = newHorizontalOffset - MapScrollViewer.ScrollableWidth;
                        newHorizontalOffset = MapScrollViewer.ScrollableWidth;
                    }

                    MapScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
                    MapScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);

                    lastPosition.X = lastPosition.X - horizontalDiff;
                    lastPosition.Y = lastPosition.Y - verticalDiff;

                }
            }



        }




        private GameMap.MapCell PointToCell(Point p)
        {
            GameMap.MapCell cell = new GameMap.MapCell();

            Vector v = p - UseGridOrigin;
            cell.X = (int)(v.X / UseCellSize.Width);
            cell.Y = (int)(v.Y / UseCellSize.Height);


            return cell;
        }


        private bool CellOnBoard(GameMap.MapCell cell)
        {
            return !(cell.X < 0 || cell.Y < 0
                || cell.X >= map.CellsWidth || cell.Y >= map.CellsHeight);

        }

        private void SetFogButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(GameMapActionMode.SetFog, true);
        }


        void UpdateActionButtons()
        {
            SetActionButtonState(SetOriginButton, (mode == GameMapActionMode.SetOrigin));
            SetActionButtonState(SetFogButton, (mode == GameMapActionMode.SetFog));
            SetActionButtonState(FogOptionsButton, (mode == GameMapActionMode.SetFog));
            SetActionButtonState(SetMarkerButton, (mode == GameMapActionMode.SetMarker));
            SetActionButtonState(MarkerOptionsButton, (mode == GameMapActionMode.SetMarker));

            UpdateMarkerButtonImage(); 
            

        }

        void UpdateMarkerButtonImage()
        {
            MarkerButtonPath.Fill = new SolidColorBrush(markerColor);
            Rect rect = new Rect(0, 0, 1, 1);
            switch (markerStyle)
            {
                case GameMap.MarkerStyle.Circle:
                    MarkerButtonPath.Data = rect.CirclePath();
                    break;
                case GameMap.MarkerStyle.Square:
                    MarkerButtonPath.Data = rect.RectanglePath();
                    break;
                case GameMap.MarkerStyle.Diamond:
                    MarkerButtonPath.Data = rect.DiamondPath();
                    break;
                case GameMap.MarkerStyle.Target:
                    MarkerButtonPath.Data = rect.TargetPath();
                    break;
                case GameMap.MarkerStyle.Star:
                    MarkerButtonPath.Data = rect.StarPath();
                    break;

            }
        }

        void SetActionButtonState(Button b, bool state)
        {
            b.Background = this.FindSolidBrush(state ? CMUIUtilities.SecondaryColorALight : CMUIUtilities.SecondaryColorADark);

        }



        private void SetMarkerButton_Click(object sender, RoutedEventArgs e)
        {

            SetMode(GameMapActionMode.SetMarker, true);
        }

        private void HideAll_Click(object sender, RoutedEventArgs e)
        {
            map.Fog.SetAll(true);

            map.SaveMap(true);
            map.FireFogOrMarkerChanged();
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            map.Fog.SetAll(false);

            map.SaveMap(true);
            map.FireFogOrMarkerChanged();
        }

        private void FogOptionsButton_Click(object sender, RoutedEventArgs e)
        {

            ContextMenu menu = (ContextMenu)Resources["FogOfWarContextMenu"];

            menu.PlacementTarget = FogOptionsButton;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
            SetMode(GameMapActionMode.SetFog, false);
        }

        private void MarkerOptionsButton_Click(object sender, RoutedEventArgs e)
        {

            ContextMenu menu = (ContextMenu)Resources["MarkerContextMenu"];

            menu.PlacementTarget = MarkerOptionsButton;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
            SetMode(GameMapActionMode.SetMarker, false);
        }

        private void Shape_Click(object sender, RoutedEventArgs e)
        {
            String shapeTag = (String)((FrameworkElement)sender).Tag;
            int id = int.Parse(shapeTag);
            markerStyle = (GameMap.MarkerStyle)id;
            UpdateMarkerButtonImage();
        }


        private void Color_Click(object sender, RoutedEventArgs e)
        {
            markerColor = ((SolidColorBrush)((MenuItem)sender).Background).Color;
            UpdateMarkerButtonImage();


        }

        private void ShowPlayerWindowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPlayerMap?.Invoke(map);
        }

        private void ToggleFogItem_Click(object sender, RoutedEventArgs e)
        {

            GameMap.MapCell cell = (GameMap.MapCell)((FrameworkElement)sender).DataContext;
            if (CellOnBoard(cell))
            {
                map[cell.X, cell.Y] = !map[cell.X, cell.Y];
                map.FireFogOrMarkerChanged();
                map.SaveMap(true);
            }
        }

        private void DeleteMarkerItem_Click(object sender, RoutedEventArgs e)
        {

            GameMap.MapCell cell = (GameMap.MapCell)((FrameworkElement)sender).DataContext;
            if (CellOnBoard(cell))
            {
                map.DeleteAllMarkers(cell);
            }       
        }

        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollControl = sender as ScrollViewer;
            if (!e.Handled && sender != null)
            {
                Point point = e.GetPosition(MapGridCanvas);
                

               
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;


                int delta = e.Delta;
                if (delta != 0)
                {
                    double steps = ((double)e.Delta) / 120.0;
                    double diff = Math.Pow(1.1, steps);
                   
                    double mapWidthStart = MapScrollViewer.ScrollableWidth;
                    double mapHeightStart = MapScrollViewer.ScrollableHeight;



                    double offsetX = MapScrollViewer.HorizontalOffset;
                    double offsetY = MapScrollViewer.VerticalOffset;


                    UseScale = UseScale * diff;



                    double mapWidthEnd = mapWidthStart * diff;
                    double mapHeightEnd = mapHeightStart * diff;

                    double mapWidthDiff = mapWidthEnd - mapWidthStart;
                    double mapHeightDiff = mapHeightEnd - mapHeightStart;


                    MapScrollViewer.ScrollToHorizontalOffset(offsetX + mapWidthDiff/2.0);
                    MapScrollViewer.ScrollToVerticalOffset(offsetY + mapHeightDiff / 2.0);

                   
                }


                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);


                
            }
        }

        private double UseScale
        {
            get
            {
                return playerMode ? map.TableScale : map.Scale;
            }
            set
            {
                if (playerMode)
                {
                    map.TableScale = value;
                }
                else
                {
                    map.Scale = value;
                }
            }
        }

        public Size UseCellSize
        {
            get
            {
                return playerMode ? map.TableCellSize : map.ActualCellSize;
            }
        }

        public Point UseGridOrigin
        {
            get
            {
                return playerMode ? map.TableGridOrigin : map.ActualGridOrigin;
            }
        }

        private void MapGridCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
                
        }
    }
}

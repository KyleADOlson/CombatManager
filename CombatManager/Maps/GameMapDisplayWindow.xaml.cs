
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinInterop = System.Windows.Interop;

namespace CombatManager.Maps
{
    public enum GameMapActionMode
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
        bool settingMarkers;
        bool draggingMap;
        bool newFogState;

        Point lastPosition;

        GameMap.MarkerStyle markerStyle = GameMap.MarkerStyle.Square;
        Color markerColor = Colors.Red;
        bool eraseMode = false;

        int brushSize = 1;

        public delegate void MapEventDelegate(GameMap map);

        public event MapEventDelegate ShowPlayerMap;

        public GameMapDisplayWindow() : this(false)
        {
        }

        public GameMapDisplayWindow(bool playerMode)
        {
            InitializeComponent();
            LoadActionButtonState();
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
            ShowActionButtons(false);
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
            else if(e.PropertyName == "CellSize" || e.PropertyName == "CellOrigin" || e.PropertyName == "ShowGrid")
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
            if (map != null)
            {
                if (map.ShowGrid)
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
                else
                {
                    MapGridCanvas.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                        
                        FogOfWar.InvalidateVisual();
                }
            }
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
            SaveActionButtonState();
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
            if (e.ClickCount == 1)
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
                            if (CellOnBoard(cell))
                            {
                                List<GameMap.MapCell> list = PointToCellArray(p, brushSize);

                                newFogState = !map[cell.X, cell.Y];
                                foreach (var c in list)
                                {
                                    if (CellOnBoard(c))
                                    {
                                        map[c.X, c.Y] = newFogState;
                                    }
                                }
                                map.FireFogOrMarkerChanged();
                                settingFog = true;
                            }
                            break;
                        }
                    case GameMapActionMode.SetMarker:
                        {
                            SetMarkers(p);
                            settingMarkers = true;

                        }
                        break;

                }
            }
            else if (e.ClickCount == 2)
            {
                if (WindowState != WindowState.Maximized)
                {
                    WindowStyle = WindowStyle.None;
                    Topmost = true;
                    WindowState = WindowState.Maximized;
                    

                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    Topmost = false;
                    WindowState = WindowState.Normal;
                }
                Hide();
                Show();
            }
        }

        private void SetMarkers(Point p)
        {
            GameMap.MapCell cell = PointToCell(p);
            if (CellOnBoard(cell))
            {
                List<GameMap.MapCell> list = PointToCellArray(p, brushSize);

                foreach (var c in list)
                {
                    if (CellOnBoard(c))
                    {
                        if (eraseMode)
                        {
                            map.DeleteAllMarkers(c);
                        }
                        else
                        {
                            GameMap.Marker marker = new GameMap.Marker();
                            marker.Style = markerStyle;
                            marker.Color = markerColor;
                            map.SetMarker(c, marker);
                        }
                    }
                }
                map.SaveMap(false);
                map.FireFogOrMarkerChanged();
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

                    List<GameMap.MapCell> list = PointToCellArray(p, brushSize);
                    bool changed = false;
                    foreach (var c in list)
                    {
                        if (CellOnBoard(c))
                        {
                            if (map[c.X, c.Y] != newFogState)
                            {
                                map[c.X, c.Y] = newFogState;
                                changed = true;
                            }
                        }
                    }
                    if (changed)
                    {
                        map.FireFogOrMarkerChanged();
                    }

                }

                lastPosition = p;
            }
            else if (settingMarkers)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    settingMarkers = false;
                    map.SaveMap(true);
                }
                else
                {
                    SetMarkers(p);
                }
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

        private List<GameMap.MapCell> PointToCellArray(Point p, int size)
        {
            List<GameMap.MapCell> list = new List<GameMap.MapCell>();

            if (size == 1)
            {
                
                list.Add(PointToCell(p));
            }
            else if (size.IsOdd())
            {


                GameMap.MapCell c = PointToCell(p);

                int minx = c.X - size / 2;
                int miny = c.Y - size / 2;
                int maxx = c.X + size / 2;
                int maxy = c.Y + size / 2;

                for (int y = miny; y <= maxy; y++)
                {
                    for (int x = minx; x <= maxx; x++)
                    {
                        list.Add(new GameMap.MapCell(x, y));
                    }
                }

            }
            else
            {
                Point start = p;
                start.X = start.X - UseCellSize.Width * ((double)size)/2.0 + UseCellSize.Width / 2.0;
                start.Y = start.Y - UseCellSize.Height * ((double)size)/2.0 + UseCellSize.Height / 2.0;

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        list.Add(PointToCell(new Point(start.X + UseCellSize.Width * (double)x,
                            start.Y + UseCellSize.Width * (double)y)));
                    }
                }
            }


            return list;
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

            BrushSizeComboBox.SelectedIndex = brushSize - 1;
            UpdateMarkerButtonImage();

        }

        private void SaveActionButtonState()
        {
            if (actionButtonStateLoaded)
            {
                XmlLoader<ActionButtonState>.Save(GetActionButtonState(),
                    "GameMapDisplayWindowActionButtonState.xml", true);
            }
        }

        bool actionButtonStateLoaded = false;

        private void LoadActionButtonState()
        {
            try
            {
                ActionButtonState state = XmlLoader<ActionButtonState>.Load(
                    "GameMapDisplayWindowActionButtonState.xml", true);
                if (state != null)
                {
                    mode = state.Mode;
                    brushSize = state.BrushSize;
                    markerColor = state.MarkerColor;
                    markerStyle = state.MarkerStyle;
                    eraseMode = state.EraseMode;
                }
            }
            catch (Exception)
            {

            }
            actionButtonStateLoaded = true;
        }

        private ActionButtonState GetActionButtonState()
        {
            return new ActionButtonState()
            {
                Mode = this.mode,
                MarkerStyle = markerStyle,
                MarkerColor = markerColor,
                BrushSize = brushSize,
                EraseMode = eraseMode,
            };
        }


        public class ActionButtonState
        {
            GameMapActionMode mode;
            GameMap.MarkerStyle markerStyle;
            Color markerColor;
            int brushSize;
            bool eraseMode;

            public GameMapActionMode Mode
            {
                get
                {
                    return mode;
                }

                set
                {
                    mode = value;
                }
            }

            public GameMap.MarkerStyle MarkerStyle
            {
                get
                {
                    return markerStyle;
                }

                set
                {
                    markerStyle = value;
                }
            }

            public Color MarkerColor
            {
                get
                {
                    return markerColor;
                }

                set
                {
                    markerColor = value;
                }
            }

            public int BrushSize
            {
                get
                {
                    return brushSize;
                }

                set
                {
                    brushSize = value;
                }
            }

            public bool EraseMode
            {
                get
                {
                    return eraseMode;
                }

                set
                {
                    eraseMode = value;
                }
            }
        }


        void ShowActionButtons(bool show)
        {
            ShowPlayerWindowButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SetOriginButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SetFogButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            FogOptionsButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SetMarkerButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            MarkerOptionsButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        void UpdateMarkerButtonImage()
        {
            if (!eraseMode)
            {
                Path path = new Path();
                path.Data = GetUnitMarkerStylePath(markerStyle);
                path.Fill = new SolidColorBrush(markerColor);
                path.Height = 16;
                path.Width = 16;
                path.Stretch = Stretch.Fill;
                SetMarkerButton.Content = path;
            }
            else
            {
                Image image = new Image();
                image.Source = CMUIUtilities.LoadBitmapFromImagesDir("eraser-48.png");
                SetMarkerButton.Content = image;
            }
           
        }

        Geometry GetUnitMarkerStylePath(GameMap.MarkerStyle style)
        {

            Rect rect = new Rect(0, 0, 1, 1);
            return GetMarkerStylePath(rect, style);
        }

        Geometry GetMarkerStylePath(Rect rect, GameMap.MarkerStyle style)
        {
            switch (style)
            {
                case GameMap.MarkerStyle.Circle:
                    return rect.CirclePath();
                case GameMap.MarkerStyle.Square:
                    return rect.RectanglePath();
                case GameMap.MarkerStyle.Diamond:
                    return rect.DiamondPath();
                case GameMap.MarkerStyle.Target:
                    return rect.TargetPath();
                case GameMap.MarkerStyle.Star:
                    return rect.StarPath();
            }

            return null;
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

            MenuItem shapeMenu = (MenuItem)menu.FindLogicalNode("ShapeMenuItem");
            foreach (object c in shapeMenu.Items)
            {
                MenuItem mi = c as MenuItem;
                if (mi != null)
                {
                    int i = int.Parse(mi.Tag as String);
                    if (i >= 0)
                    {
                        Path path = new Path();
                        path.Data = GetUnitMarkerStylePath((GameMap.MarkerStyle)i);
                        path.Fill = new SolidColorBrush(markerColor);
                        path.Height = 16;
                        path.Width = 16;
                        path.Stretch = Stretch.Fill;
                        mi.Icon = path;
                    }
                }
            }


            SetMode(GameMapActionMode.SetMarker, false);
        }

        private void Shape_Click(object sender, RoutedEventArgs e)
        {
            String shapeTag = (String)((FrameworkElement)sender).Tag;
            int id = int.Parse(shapeTag);

            if (id == -1)
            {
                eraseMode = true;
            }
            else
            {
                markerStyle = (GameMap.MarkerStyle)id;
                eraseMode = false;
            }
            UpdateMarkerButtonImage();

            SaveActionButtonState();
        }


        private void Color_Click(object sender, RoutedEventArgs e)
        {
            markerColor = ((SolidColorBrush)((MenuItem)sender).Background).Color;
            UpdateMarkerButtonImage();

            SaveActionButtonState();


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

                map.FireFogOrMarkerChanged();
                map.SaveMap(false);
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

        private void BrushSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            brushSize = BrushSizeComboBox.SelectedIndex + 1;
            SaveActionButtonState();
        }

        private void ShowGridCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        DispatcherTimer scaleTimer;


        private void ScaleUpButton_MouseDown(object sender, MouseButtonEventArgs e)
        {

            scaleTimer?.Stop();
            scaleTimer = null;

            scaleTimer = new DispatcherTimer();
            scaleTimer.Tick += ScaleTimer_TickUp;
            scaleTimer.Interval = new TimeSpan(30000);
            scaleTimer.Start();
            double diff = Math.Pow(1.1, 1);
            ChangeScale(diff);

        }

        private void ScaleTimer_TickUp(object sender, EventArgs e)
        {

            double diff = Math.Pow(1.1, 1);
            ChangeScale(diff);
        }

        private void ChangeScale(double diff)
        {
            map.Scale = map.Scale * diff;
        }

        private void ScaleUpButton_MouseUp(object sender, MouseButtonEventArgs e)
        {

            scaleTimer?.Stop();
            scaleTimer = null;
        }

        private void ScaleDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {

            scaleTimer?.Stop();
            scaleTimer = null;

            scaleTimer = new DispatcherTimer();
            scaleTimer.Tick += ScaleTimer_TickDown;
            scaleTimer.Interval = new TimeSpan(30000);
            scaleTimer.Start();
            double diff = Math.Pow(1.1, 1);
            ChangeScale(diff);
        }

        private void ScaleTimer_TickDown(object sender, EventArgs e)
        {
            double diff = Math.Pow(1.1, -1);
            ChangeScale(diff);
        }

        private void ScaleDownButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            scaleTimer?.Stop();
            scaleTimer = null;

        }
    }
}

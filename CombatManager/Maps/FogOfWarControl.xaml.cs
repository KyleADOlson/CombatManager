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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombatManager.Maps
{
    /// <summary>
    /// Interaction logic for FogOfWarControl.xaml
    /// </summary>
    public partial class FogOfWarControl : UserControl
    {
        public FogOfWarControl()
        {
            InitializeComponent();
        }

        bool playerMode;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);


            GameMap map = (GameMap)DataContext;
            if (map != null)
            {
               

                Brush b = new SolidColorBrush(PlayerMode?Color.FromArgb(255, 0, 0, 0):Color.FromArgb(128, 0, 0, 0));


                Rect drawRect = OriginRect();


                Rect rectStart = new Rect();
                bool running = false;
                for (int y = 0; y < map.CellsHeight; y++)
                {
                    Action drawIfRunning = () =>
                    {
                        if (running)
                        {
                            running = false;

                            rectStart.Width = drawRect.X - rectStart.X;

                            drawingContext.DrawRectangle(b, null, rectStart);
                        }
                        
                    };


                    for (int x = 0; x < map.CellsWidth; x++)
                    {

                        if (map[x, y])
                        {
                            if (!running)
                            {
                                rectStart = drawRect;
                                running = true;
                            }
                        }
                        else
                        {
                            drawIfRunning();
                        }


                        drawRect.X += drawRect.Width;
                    }
                    drawIfRunning();
                    drawRect.X = UseGridOrigin.X;
                    drawRect.Y += drawRect.Height;
                }
                foreach (KeyValuePair<int, List<GameMap.Marker>> pair in map.Markers)
                {
                    foreach (GameMap.Marker m in pair.Value)
                    {
                        DrawMarker(drawingContext, pair.Key, m);
                    }

                }

            }
            
        }

        public bool PlayerMode
        {
            get
            {
                return playerMode;
            }
            set
            {
                if (playerMode != value)
                {
                    playerMode = value;
                    InvalidateVisual();
                }

            }
        }

        public void DrawMarker(DrawingContext context, int index, GameMap.Marker marker)
        {
            GameMap map = (GameMap)DataContext;
            if (map != null)
            {
                GameMap.MapCell cell = map.IndexToCell(index);

                Rect rect = CellRect(cell);

                Color brushColor = marker.Color;
                brushColor.A = (byte)(brushColor.A / 2);

                double penWidth = CurrentScale;


                Brush b = new SolidColorBrush(brushColor);
                Pen p = new Pen(new SolidColorBrush(marker.Color), penWidth);


                switch (marker.Style)
                {
                    case GameMap.MarkerStyle.Square:


                        context.DrawGeometry(b, p, rect.RectanglePath(penWidth));
                        break;
                    case GameMap.MarkerStyle.Circle:

                        Rect cicleRect = rect;
                        cicleRect.ScaleCenter(.9, .9);

                        context.DrawGeometry(b, p, rect.CirclePath(penWidth));
                        break;
                    case GameMap.MarkerStyle.Diamond:
                        context.DrawGeometry(b, p, rect.DiamondPath(penWidth));
                        break;
                    case GameMap.MarkerStyle.Target:
                        context.DrawGeometry(b, p, rect.TargetPath(penWidth));
                        break;
                    case GameMap.MarkerStyle.Star:
                        context.DrawGeometry(b, p, rect.StarPath(0));
                        break;
                }



            }
        }

        double CurrentScale
        {
            get
            {
                GameMap map = (GameMap)DataContext;
                if (map != null)
                {
                    return UseScale;
                }
                return 1.0;
            }
        }



        Rect OriginRect()
        {
            Rect originRect = new Rect();
            GameMap map = (GameMap)DataContext;
            if (map != null)
            {
                originRect = new Rect(UseGridOrigin.X, UseGridOrigin.Y, UseCellSize.Width, UseCellSize.Height);
            }

            return originRect;
        }


        Rect CellRect(GameMap.MapCell cell)
        {
            return CellRect(cell.X, cell.Y);
        }


        Rect CellRect(int x, int y)
        {

            Rect drawRect = new Rect();

            drawRect = OriginRect();

            Size cellSize = CellSize();

            drawRect.X += cellSize.Width * (double)x;
            drawRect.Y += cellSize.Height * (double)y;
            

            return drawRect;
        }

        Size CellSize()
        {
            Size size = new Size();

            GameMap map = (GameMap)DataContext;
            if (map != null)
            {

                size = UseCellSize;


            }

            return size;
        }

        private double UseScale
        {
            get
            {
                GameMap map = (GameMap)DataContext;
                return playerMode ? map.TableScale : map.Scale;
            }
            set
            {
                GameMap map = (GameMap)DataContext;
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
                GameMap map = (GameMap)DataContext;
                return playerMode ? map.TableCellSize : map.ActualCellSize;
            }
        }

        public Point UseGridOrigin
        {
            get
            {
                GameMap map = (GameMap)DataContext;
                return playerMode ? map.TableGridOrigin : map.ActualGridOrigin;
            }
        }

    }
}

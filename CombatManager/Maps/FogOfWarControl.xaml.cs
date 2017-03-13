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
        bool drawAnchor;

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

                            Rect finalRect = rectStart;

                            if (playerMode)
                            {
                                double extra = finalRect.Height * .01;
                                finalRect.Y -= extra;
                                finalRect.Height += extra * 2.0;
                            }


                            drawingContext.DrawRectangle(b, null, finalRect);



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

                Point anchorPoint = map.CellOrigin;
                anchorPoint = anchorPoint.Multiply(UseScale);


                if (drawAnchor)
                {
                    LineGeometry ln = new LineGeometry(anchorPoint.Add(-15, -15), anchorPoint.Add(15, 15));

                    LineGeometry ln2 = new LineGeometry(anchorPoint.Add(-15, 15), anchorPoint.Add(15, -15));

                    EllipseGeometry ellipse = new EllipseGeometry(anchorPoint, 12, 12);


                    Pen p = new Pen(new SolidColorBrush(Color.FromRgb(255, 0, 0)), 2.0);

                    drawingContext.DrawGeometry(null, p, ln);
                    drawingContext.DrawGeometry(null, p, ln2);
                    drawingContext.DrawGeometry(null, p, ellipse);

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

        public bool DrawAnchor
        {
            get
            {
                return drawAnchor;
            }
            set
            {
                if (drawAnchor != value)
                {
                    drawAnchor = value;
                    InvalidateVisual();
                }

            }
        }

        public Geometry CreateMarkerPath(int index, GameMap.Marker marker)
        {
            GameMap map = (GameMap)DataContext;
            if (map != null)
            {
                GameMap.MapCell cell = map.IndexToCell(index);

                Rect rect = CellRect(cell);

                double penWidth = CurrentScale;


                switch (marker.Style)
                {
                    case GameMap.MarkerStyle.Square:


                        return rect.RectanglePath(penWidth);
                    case GameMap.MarkerStyle.Circle:

                        Rect cicleRect = rect;
                        cicleRect.ScaleCenter(.9, .9);

                        return rect.CirclePath(penWidth);
                    case GameMap.MarkerStyle.Diamond:
                        return rect.DiamondPath(penWidth);
                    case GameMap.MarkerStyle.Target:
                        return rect.TargetPath(penWidth);
                    case GameMap.MarkerStyle.Star:
                        return rect.StarPath(0);
                }
            }

            return null;
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

                double penWidth = Math.Min(UseCellSize.Width, UseCellSize.Height)/25.0;


                Brush b = new SolidColorBrush(brushColor);
                Pen p = new Pen(new SolidColorBrush(marker.Color), penWidth);

                
                context.DrawGeometry(b, p, CreateMarkerPath(index, marker));
                



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

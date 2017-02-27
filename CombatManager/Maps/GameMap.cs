/*
 *  GameMap.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using CombatManager;

namespace CombatManager.Maps
{
    public class GameMap : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        Point cellOrigin;

        double scale;

        double cellSizeWidth;
        double cellSizeHeight;

        String sourceFile;

        BitmapImage image;

        String name;

        public double Scale
        {
            get { return scale; }
            set
            {
                double setValue = value.Clamp(.01, 20);

                if (scale != setValue)
                {
                    scale = setValue;
                    Notify("Scale");
                }
            }
        }


        public Size CellSize
        {
            get { return new Size(cellSizeWidth, cellSizeHeight); }
            set
            {
                bool width = false;
                bool height = false;
                if (cellSizeWidth != value.Width)
                {
                    cellSizeWidth = value.Width;
                    width = true;
                }
                if (cellSizeHeight != value.Height)
                {
                    cellSizeHeight = value.Height;
                    height = true;
                }
                if (width || height)
                {
                    if (width)
                    {
                        Notify("CellSizeWidth");
                    }
                    if (height)
                    {
                        Notify("CellSizeHeight");
                    }
                    Notify("CellSize");
                }
            }
        }

        public double CellSizeWidth
        {
            get { return cellSizeWidth; }
            set
            {
                if (cellSizeWidth != value)
                {
                    cellSizeWidth = value;
                    Notify("CellSize");
                    Notify("CellSizeWidth");
                }
            }
        }
        public double CellSizeHeight
        {
            get { return cellSizeHeight; }
            set
            {
                if (cellSizeHeight != value)
                {
                    cellSizeHeight = value;
                    Notify("CellSize");
                    Notify("CellSizeHeight");
                }
            }
        }

        public String SourceFile
        {
            get { return sourceFile; }
            set
            {
                if (sourceFile != value)
                {
                    sourceFile = value;
                    Notify("SourceFile");
                }
            }
        }

        public BitmapImage Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    
                    Notify("Image");
                    
                }
            }
        }
        public Point CellOrigin
        {
            get { return cellOrigin; }
            set
            {
                if (cellOrigin != value)
                {
                    cellOrigin = value;
                    Notify("CellOrigin");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    Notify("Name");
                }
            }
        }

        private void Notify(string prop)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }


    }
}

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

namespace CombatManager.Maps
{
    public class GameMap : INotifyPropertyChanged
    {

        public enum CellType
        {
            Square = 0,
            Hex = 1
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private int _Columns;
        private int _Rows;
        private float _CellWidth;
        private float _CellHeight;
        private CellType _Type;
        private ImageSource _Image;
        private Decimal _CellScale;

        public int Columns
        {
            get { return _Columns; }
            set
            {
                if (_Columns != value)
                {
                    _Columns = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Columns")); }
                }
            }
        }
        public int Rows
        {
            get { return _Rows; }
            set
            {
                if (_Rows != value)
                {
                    _Rows = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Height")); }
                }
            }
        }
        public float CellWidth
        {
            get { return _CellWidth; }
            set
            {
                if (_CellWidth != value)
                {
                    _CellWidth = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CellWidth")); }
                }
            }
        }
        public float CellHeight
        {
            get { return _CellHeight; }
            set
            {
                if (_CellHeight != value)
                {
                    _CellHeight = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CellHeight")); }
                }
            }
        }
        public CellType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Type")); }
                }
            }
        }
        public ImageSource Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Image")); }
                }
            }
        }


        public Decimal CellScale
        {
            get { return _CellScale; }
            set
            {
                if (_CellScale != value)
                {
                    _CellScale = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CellScale")); }
                }
            }
        }


    }
}

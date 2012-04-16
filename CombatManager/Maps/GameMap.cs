using System;
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

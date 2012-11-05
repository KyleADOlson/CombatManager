/*
 *  CampaignEvent.cs
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
using System.Xml.Serialization;


namespace CombatManager
{
    public class CampaignEvent : INotifyPropertyChanged, IDBLoadable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int _DBLoaderID;
        private int _CampaignID;
        private DateTime _Start;
        private DateTime _LastStart;
        private DateTime _End;
        private bool _AllDay;
        private string _Title;
        private string _Details;


        public CampaignEvent()
        {

        }

        public CampaignEvent(CampaignEvent old)
        {
            _Start = old._Start;
            _End = old.End;
            _AllDay = old._AllDay;
            _Title = old._Title;
            _Details = old._Details;

        }

        public object Clone()
        {
            return new CampaignEvent(this);
        }

        public DateTime Start
        {
            get { return _Start; }
            set
            {
                if (_Start != value)
                {
                    _LastStart = _Start;
                    _Start = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Start")); }
                }
            }
        }

        [XmlIgnore]
        public DateTime LastStart
        {
            get { return _LastStart; }
        }

        public DateTime End
        {
            get { return _End; }
            set
            {
                if (_End != value)
                {
                    _End = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("End")); }
                }
            }
        }
        public bool AllDay
        {
            get { return _AllDay; }
            set
            {
                if (_AllDay != value)
                {
                    _AllDay = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AllDay")); }
                }
            }
        }
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Title")); }
                }
            }
        }
        public string Details
        {
            get { return _Details; }
            set
            {
                if (_Details != value)
                {
                    _Details = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Details")); }
                }
            }
        }


        public int DBLoaderID
        {
            get
            {
                return _DBLoaderID;
            }
            set
            {
                if (_DBLoaderID != value)
                {
                    _DBLoaderID = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DBLoaderID")); }
                }
            }
        }

        public int CampaignID
        {
            get
            {
                return _CampaignID;
            }
            set
            {
                if (_CampaignID != value)
                {
                    _CampaignID = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CampaignID")); }
                }
            }
        }
    }

}

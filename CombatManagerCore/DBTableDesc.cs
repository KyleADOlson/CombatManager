/*
 *  DBTableDesc.cs
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
using System.Reflection;

namespace CombatManager
{
    public class DBTableDesc
    {
        private String _Name;
        private List<DBFieldDesc> _Fields;
        private bool _Primary;
        private Type _Type;

        private List<DBFieldDesc> _SubtableFields;
        private List<DBFieldDesc> _ValueFields;


        public DBTableDesc()
        {
            _Fields = new List<DBFieldDesc>();
        }

        public DBTableDesc(String name, Type type)
        {
            _Name = name;
            _Fields = new List<DBFieldDesc>();
            _Type = type;
        }

        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }
        public List<DBFieldDesc> Fields
        {
            get { return _Fields; }
            set
            {
                if (_Fields != value)
                {
                    _Fields = value;
                    _ValueFields = null;
                    _SubtableFields = null;
                }
            }
        }
        public bool Primary
        {
            get { return _Primary; }
            set
            {
                if (_Primary != value)
                {
                    _Primary = value;
                }
            }
        }
        public Type Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }
        public IEnumerable<DBFieldDesc> SubtableFields
        {
            get
            {
                if (_SubtableFields == null)
                {
                    SortFields();
                }
                return _SubtableFields;
            }
        }
        public IEnumerable<DBFieldDesc> ValueFields
        {
            get
            {
                if (_ValueFields == null)
                {
                    SortFields();
                }
                return _ValueFields;
            }
        }

        private void SortFields()
        {
            _SubtableFields = new List<DBFieldDesc>();
            _ValueFields = new List<DBFieldDesc>();
            foreach (DBFieldDesc field in _Fields)
            {
                if (field.Subtable != null)
                {
                    _SubtableFields.Add(field);
                }
                else
                {
                    _ValueFields.Add(field);
                }
            }
        }
    }

    public class DBFieldDesc
    {
        public DBFieldDesc()
        {
        }

        public DBFieldDesc(String name, String type, bool nullable, PropertyInfo info)
        {
            _Name = GetUsableName(name);
            _OriginalName = name;
            _Type = type;
            _Nullable = nullable;
            _Info = info;
        }

        private String _Name;
        private String _OriginalName;
        private String _Type;
        private DBTableDesc _Subtable;
        private bool _Nullable;
        private PropertyInfo _Info;

        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }
        public String OriginalName
        {
            get { return _OriginalName; }
        }
        public String Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }
        public bool Nullable
        {
            get { return _Nullable; }
            set
            {
                if (_Nullable != value)
                {
                    _Nullable = value;
                }
            }
        }
        public DBTableDesc Subtable
        {
            get
            {
                return _Subtable;
            }
            set
            {
                _Subtable = value;
            }

        }
        public PropertyInfo Info
        {
            get
            {
                return _Info;
            }
            set
            {
                _Info = value;
            }

        }


        public static string GetUsableName(string name)
        {
            if (string.Compare(name, "Group", true) == 0)
            {
                return "_" + name;
            }
            if (string.Compare(name, "Limit", true) == 0)
            {
                return "_" + name;
            }
            return name;

        }

    }

    public interface IDBLoadable
    {
        int DBLoaderID
        {
            get;
            set;
        }

    }
    

}

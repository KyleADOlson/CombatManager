/*
 *  UndoAction.cs
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
using System.Collections;
using System.Collections.Specialized;

namespace CombatManager
{
    public class UndoAction : IEquatable<UndoAction>
    {
        public enum UndoTypes
        {
            UndoCollection,
            UndoProperty,
            UndoGroupStart,
            UndoGroupEnd,
        }

        public UndoTypes UndoType { get; private set; }
        public ICollection Collection { get; private set; }
        public NotifyCollectionChangedEventArgs Changes { get; private set; }
        public object Object { get; private set; }
        public string Property { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public UndoAction(bool startGroup)
        {
            if (startGroup)
            {
                this.UndoType = UndoTypes.UndoGroupStart;
            }
            else
            {
                this.UndoType = UndoTypes.UndoGroupEnd;
            }
        }

        public UndoAction(ICollection Collection, NotifyCollectionChangedEventArgs Changes)
        {
            this.UndoType = UndoTypes.UndoCollection;
            this.Collection = Collection;
            this.Changes = Changes;
        }

        public UndoAction(object Object, string Property, object OldValue, object NewValue)
        {
            this.UndoType = UndoTypes.UndoProperty;
            this.Object = Object;
            this.Property = Property;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }

        public bool Equals(UndoAction other)
        {
            if (other.UndoType != UndoType)
            {
                return false;
            }

            switch (UndoType)
            {
                default:
                case UndoTypes.UndoGroupEnd:
                case UndoTypes.UndoGroupStart:
                case UndoTypes.UndoCollection:
                    return base.Equals(other);

                case UndoTypes.UndoProperty:
                    return Object == other.Object &&
                        Property == other.Property &&
                        OldValue == other.OldValue &&
                        NewValue == other.NewValue;
            }
        }
    }
}

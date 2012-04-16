using System;
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

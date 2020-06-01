using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CombatManager
{
    public abstract class BaseDBClass : SimpleNotifyClass, IDBLoadable
    {

        protected int _DBLoaderID;
        protected int _DetailsID;

        public BaseDBClass()
        {
            PropertyChanged += SelfPropertyChanged;
        }

        private void SelfPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SelfPropertyChanged(e.PropertyName);
        }

        protected virtual void SelfPropertyChanged(string name)
        {

        }

        [XmlIgnore]
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

                    Notify("DBLoaderID");
                }

            }
        }

        [XmlIgnore]
        public int DetailsID
        {
            get
            {
                return _DetailsID;
            }
        }

        [XmlIgnore]
        public bool IsCustom
        {
            get
            {
                return DBLoaderID != 0;
            }
        }
    }
}

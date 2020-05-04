using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    public static class CMBindingUtils
    {
    }



    public abstract class SimpleNotifyClass : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); 
        }
    }

    public class NotifyValue<T> : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        T _Value;

        public NotifyValue(T val)
        {
            _Value = val;
        }

        public NotifyValue()
        {
        }

        public T Value
        {

            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
                
            }
        }

    }

    public abstract class SimpleNotifyInternalClass<T> : SimpleNotifyClass
    {
        private T parent;
        public SimpleNotifyInternalClass(T parent)
        {
            this.parent = parent;
        }

        protected T Parent
        {
            get
            {
                return parent;
            }
        }
           
    }

}

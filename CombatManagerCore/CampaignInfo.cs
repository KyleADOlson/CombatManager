using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
#if !MONO
using System.Data.Common;
using ScottsUtils;
#else
using Mono.Data.Sqlite;
#endif

namespace CombatManager
{
    public class CampaignInfo : INotifyPropertyChanged
    {
        private int _CampaignID;
        private DateTime _CurrentDate;
        private DateTime _DisplayDate;
        private DateTime _SelectedDate;

        private Dictionary<DateTime, ObservableCollection<CampaignEvent>> _Events;

#if !MONO
    //private SQL_Lite sql;
#else
		
        private static SqliteConnection eventDB;
#endif

        private static void PrepareDB()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CampaignInfo()
        {
            DateTime now = DateTime.Now;
            _CurrentDate = new DateTime(now.Year + 2700, 1, 1, 12, 0, 0);
            _DisplayDate = new DateTime(now.Year + 2700, 1, 1, 12, 0, 0);
            _SelectedDate = new DateTime(now.Year + 2700, 1, 1, 12, 0, 0);
            _Events = new Dictionary<DateTime, ObservableCollection<CampaignEvent>>();
        }


        public void AddEvent(CampaignEvent e)
        {
            ObservableCollection<CampaignEvent> list = new ObservableCollection<CampaignEvent>();
            if (_Events.ContainsKey(e.Start.Date))
            {
                list = _Events[e.Start.Date];
            }
            e.PropertyChanged += new PropertyChangedEventHandler(EventPropertyChanged);
            System.Diagnostics.Debug.Assert(!list.Contains(e));
            list.Add(e);
            _Events[e.Start.Date] = list;

            PropertyChanged(this, new PropertyChangedEventArgs("Events"));
        }

        void EventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start")
            {                
                CampaignEvent ev = (CampaignEvent)sender;

                ObservableCollection<CampaignEvent> list = _Events[ev.LastStart.Date];
                list.Remove(ev);
                if (_Events.ContainsKey(ev.Start.Date))
                {
                    list = _Events[ev.Start.Date];
                }
                else
                {
                    list = new ObservableCollection<CampaignEvent>();
                    _Events[ev.Start.Date] = list;
                }

                list.Add(ev);
                PropertyChanged(this, new PropertyChangedEventArgs("Events"));
            }
        }

        public void RemoveEvent(CampaignEvent e)
        {
            ObservableCollection<CampaignEvent> list = _Events[e.Start.Date];
            list.Remove(e);
            PropertyChanged(this, new PropertyChangedEventArgs("Events"));
        }




        public DateTime CurrentDate 
        {
            get { return _CurrentDate; }
            set
            {
                if (_CurrentDate != value)
                {
                    _CurrentDate = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CurrentDate")); }
                }
            }
        }


        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                if (_SelectedDate != value)
                {
                    _SelectedDate = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SelectedDate")); }
                }
            }
        }


        public DateTime DisplayDate
        {
            get { return _DisplayDate; }
            set
            {
                if (_DisplayDate != value)
                {
                    _DisplayDate = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DisplayDate")); }
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

        public IDictionary<DateTime, ObservableCollection<CampaignEvent>> Events
        {
            get
            {
                return _Events;
            }
        }

        public List<CampaignEvent> EventsForDate(DateTime date)
        {
            DateTime useDate = date.Date;

            List<CampaignEvent> list = new List<CampaignEvent>();

            if (_Events.ContainsKey(date))
            {
                list.AddRange(_Events[date]);
                list.Sort((a, b) => a.Start.CompareTo(b.Start));
            }

            return list;

               
        }

    }
}

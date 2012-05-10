/*
 *  CombatStateService.cs
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CombatManager;
using System.Threading;

namespace CombatViewService
{

    public class CombatStateService : ICombatStateService, IDisposable
    {
        private static CombatState _state;


        private ICombatStateCallback callback = null;
        
        private object QueueCritSec = new object();
        private List<StateQueueItem> Items = new List<StateQueueItem>();
        private Thread QueueThread;
        private AutoResetEvent QueueEvent;
        private bool QueueRunning;

        private enum StateQueueEventType
        {
            CurrentPlayerChanged,
            CombatListChanged,
            CharactersChanged,
        }

        private class StateQueueItem
        {
            public StateQueueEventType EventType { get; set; }
            public Guid CharacterID { get; set; }
            public object Data { get; set; }
        }


        public CombatStateService()
        {
            System.Diagnostics.Debug.WriteLine("Starting Service");
            StartThread();
            _state.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_state_PropertyChanged);
            _state.CombatList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CombatList_CollectionChanged);
            _state.CharacterAdded += new CombatStateCharacterEvent(_state_CharacterAdded);
            _state.CharacterRemoved += new CombatStateCharacterEvent(_state_CharacterRemoved);
            _state.CharacterPropertyChanged += new CombatStateCharacterEvent(_state_CharacterPropertyChanged);
            callback = OperationContext.Current.GetCallbackChannel<ICombatStateCallback>();

        }

        public void Dispose()
        {
            CloseQueue();
        }

        private void StartThread()
        {
            QueueEvent = new AutoResetEvent(false);
            QueueRunning = true;
            QueueThread = new Thread(QueueThreadFunction);
            QueueThread.Start();
        }

        private void PostToQueue(StateQueueItem item)
        {
            lock (QueueCritSec)
            {
                Items.Add(item);
            }
            
            QueueEvent.Set();
        }


        void QueueThreadFunction()
        {
            while (QueueRunning)
            {
                QueueEvent.WaitOne();
                bool continueLookup = true;

                while (continueLookup)
                {
                    StateQueueItem item = null;
                    lock (QueueCritSec)
                    {
                        if (Items.Count == 0)
                        {
                            continueLookup = false;
                        }
                        else
                        {
                            item = Items[0];
                            Items.RemoveAt(0);

                            if (item.EventType == StateQueueEventType.CombatListChanged ||
                                item.EventType == StateQueueEventType.CharactersChanged)
                            {
                                Items.RemoveAll(a => a.EventType == item.EventType);

                            }
                            else if (item.EventType == StateQueueEventType.CurrentPlayerChanged)
                            {
                                StateQueueItem lastItem = Items.FindLast(a => a.EventType == StateQueueEventType.CurrentPlayerChanged);

                                if (lastItem != null)
                                {
                                    item = lastItem;
                                }

                                Items.RemoveAll(a => a.EventType == item.EventType);
                            }
                        }
                        
                    }
                    if (item != null)
                    {
                        switch (item.EventType)
                        {
                            case StateQueueEventType.CharactersChanged:
                                callback.CharactersChanged();
                                break;
                            case StateQueueEventType.CombatListChanged:
                                callback.CombatListChanged();
                                break;
                            case StateQueueEventType.CurrentPlayerChanged:                                
                                callback.CurrentPlayerChanged(item.CharacterID);
                                break;
                        }

                    }

                }
            }
        }

        void CloseQueue()
        {
            QueueRunning = false;
            QueueEvent.Set();
            System.Diagnostics.Debug.WriteLine("Closing Queue");
        }


        void CombatList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PostToQueue(new StateQueueItem() {EventType = StateQueueEventType.CombatListChanged});
        }


        void _state_CharacterPropertyChanged(object sender, CombatStateCharacterEventArgs e)
        {
        }

        void _state_CharacterRemoved(object sender, CombatStateCharacterEventArgs e)
        {

            PostToQueue(new StateQueueItem() { EventType = StateQueueEventType.CharactersChanged });
        }

        void _state_CharacterAdded(object sender, CombatStateCharacterEventArgs e)
        {
            PostToQueue(new StateQueueItem() { EventType = StateQueueEventType.CharactersChanged });
        }

        void _state_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                if (callback != null)
                {
                    PostToQueue(new StateQueueItem() { EventType = StateQueueEventType.CurrentPlayerChanged,
                                    CharacterID = _state.CurrentCharacterID});

                }
            }
        }

        public List<SimpleCombatListItem> GetCombatList()
        {
            return _state.SimpleCombatList;
        }

        public List<Character> GetCharacters()
        {
            List<Character> chars = new List<Character>();

            foreach (Character chOld in _state.Characters)
            {
                Character ch = (Character)chOld.Clone();
                ch.ID = chOld.ID;

                ch.Monster.DescHTML = "";
                ch.Monster.Description = "";
                ch.Monster.Description_Visual = "";
                ch.Monster.BeforeCombat = "";
                ch.Monster.DuringCombat = "";
                chars.Add(ch);
            }

            return chars;
        }

        public Character GetCurrentCharacter()
        {
            return _state.CurrentCharacter;
        }
        public Guid GetCurrentCharacterID()
        {
            return _state.CurrentCharacterID;
        }

        public int? GetRound()
        {
            return _state.Round;
        }


        public static CombatState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;                
            }
        }

    }
}

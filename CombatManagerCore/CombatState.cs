/*
 *  CombatState.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Timers;

namespace CombatManager
{

    [DataContract]
    public class SimpleCombatListItem
    {
        [DataMember]
        public Guid ID {get; set;}

        [DataMember]
        public List<Guid> Followers { get; set; }
    }

    public class CombatStateCharacterEventArgs
    {
        public Character Character { get; set; }
        public string Property { get; set; }
    }

    public delegate void CombatStateNotificationEvent(object sender, CombatStateNotification notification);


    public delegate void CombatStateCharacterEvent(object sender, CombatStateCharacterEventArgs e);


    [DataContract]
    public class CombatState : INotifyPropertyChanged
    {
        public event CombatStateCharacterEvent CharacterAdded;
        public event CombatStateCharacterEvent CharacterRemoved;
        public event CombatStateCharacterEvent CharacterPropertyChanged;
        public event EventHandler CharacterSortCompleted;
        public event CombatStateCharacterEvent TurnChanged;

        public event CombatStateNotificationEvent CombatStateNotificationSent;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<RollEventArgs> RollRequested;

        private int? _Round;
        private string _CR;
        private long? _XP;

        private ObservableCollection<Character> _Characters;
        private ObservableCollection<Character> _CombatList;
        private ObservableCollection<Character> _UnfilteredCombatList;
        private Character _CurrentCharacter;
        private List<Guid> _CombatIDList;
        private bool _CombatListNeedsUpdate;
        private bool _CombatIDListNeedsUpdate;

        private static Random rand = new Random();

        public static bool use3d6;
        public static string alternateRoll;

        private RulesSystem _RulesSystem;

        private DateTime _CurrentTurnStartTime;
        private bool _ClockPaused;
        private TimeSpan _PausedTime;


        private bool sortingList;


        public CombatState()
        {
            _Characters = new ObservableCollection<Character>();
            _Characters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Characters_CollectionChanged);
            _CombatList = new ObservableCollection<Character>();
            _UnfilteredCombatList = new ObservableCollection<Character>();
            _CombatIDList = new List<Guid>();
            _RulesSystem = RulesSystem.PF1;
        }


        public CombatState(CombatState s)
        {
            _Round = s._Round;
            _CR = s._CR;
            _XP = s._XP;
            _CombatIDList = new List<Guid>();
            _RulesSystem = s._RulesSystem;


            _CombatList = new ObservableCollection<Character>();

            _UnfilteredCombatList = new ObservableCollection<Character>();
            _Characters = new ObservableCollection<Character>();

            if (s._Characters != null)
            {
                foreach (Character c in s._Characters)
                {
                    Character newChar = (Character)c.Clone();
                    AddCharacter(newChar);
                    if (s._CurrentCharacter == c)
                    {
                        _CurrentCharacter = newChar;
                    }
                    newChar.ID = c.ID;

                }
                _Characters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Characters_CollectionChanged);

            }
            foreach (Character ch in s._CombatList)
            {
                _CombatIDList.Add(ch.ID);
            }

            _CombatIDListNeedsUpdate = false;
            _CombatListNeedsUpdate = true;
            _CurrentTurnStartTime = s._CurrentTurnStartTime;
            _ClockPaused = s._ClockPaused;

        }

        public void Copy(CombatState s)
        {
            Round = s.Round;
            CR = s.CR;
            XP = s.XP;
            RulesSystem = s.RulesSystem;

            Characters.Clear();
            foreach (Character c in s.Characters)
            {
                Characters.Add(c);
            }
            CombatList.Clear();
            foreach (Character c in s.CombatList)
            {
                CombatList.Add(c);
            }
            CurrentCharacter = s.CurrentCharacter;
            CurrentTurnStartTime = s.CurrentTurnStartTime;
            ClockPaused = s.ClockPaused;

        }

        void HandleTurnChanged()
        {
            CurrentTurnStartTime = DateTime.UtcNow;

            TurnChanged?.Invoke(this, new CombatStateCharacterEventArgs() { Character = this.CurrentCharacter });
        }

        void PauseTimer()
        {
            _ClockPaused = true;
            _PausedTime = DateTime.UtcNow - CurrentTurnStartTime;

        }


        void _Characters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Character ch in e.NewItems)
                {
                    ch.PropertyChanged += Character_PropertyChanged;

                    CharacterAdded?.Invoke(this, new CombatStateCharacterEventArgs() { Character = ch });
                }
            }
            if (e.OldItems != null)
            {
                foreach (Character ch in e.OldItems)
                {
                    ch.PropertyChanged -= Character_PropertyChanged;

                    CharacterRemoved?.Invoke(this, new CombatStateCharacterEventArgs() { Character = ch });

                }
            }
        }



        void Character_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CharacterPropertyChanged?.Invoke(this, new CombatStateCharacterEventArgs() { Character = (Character)sender, Property = e.PropertyName });

        }

        [DataMember]
        public int? Round
        {
            get { return _Round; }
            set
            {
                if (_Round != value)
                {
                    _Round = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Round"));
                }
            }
        }

        [DataMember]
        public string CR
        {
            get { return _CR; }
            set
            {
                if (_CR != value)
                {
                    _CR = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CR"));
                }
            }

        }

        [DataMember]
        public long? XP
        {
            get { return _XP; }
            set
            {
                if (_XP != value)
                {
                    _XP = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("XP"));
                }
            }
        }

        [DataMember]
        public ObservableCollection<Character> Characters
        {
            get { return _Characters; }
            set
            {
                if (_Characters != value)
                {
                    _Characters = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Characters")); }
            }
        }

        [XmlIgnore]
        public ObservableCollection<Character> CombatList
        {
            get
            {

                if (_CombatList == null)
                {
                    _CombatList = new ObservableCollection<Character>();
                    _CombatListNeedsUpdate = true;
                }
                if (_UnfilteredCombatList == null)
                {
                    _UnfilteredCombatList = new ObservableCollection<Character>();
                }
                if (_CombatListNeedsUpdate)
                {
                    _CombatList.Clear();

                    foreach (Guid g in _CombatIDList)
                    {
                        Character c = Characters.FirstOrDefault(a => a.ID == g);
                        _CombatList.Add(c);
                    }
                    _CombatListNeedsUpdate = false;
                }
                else
                {
                    _CombatIDListNeedsUpdate = true;
                }
                return _CombatList;
            }
            set
            {
                if (_CombatList != value)
                {
                    _CombatList = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CombatList"));
                }
            }
        }

        [XmlIgnore]
        public List<SimpleCombatListItem> SimpleCombatList
        {
            get
            {
                var v = new List<SimpleCombatListItem>(
                    from c in CombatList select new SimpleCombatListItem()
                    { ID = c.ID,
                        Followers = (c.InitiativeFollowers == null) ? null :
                            new List<Guid>(
                                from x in c.InitiativeFollowers select x.ID) });

                return v;
            }
        }


        [DataMember]
        public List<Guid> CombatIDList
        {
            get
            {
                if (_CombatIDListNeedsUpdate)
                {
                    _CombatIDList.Clear();

                    foreach (Character ch in Characters)
                    {
                        _CombatIDList.Add(ch.ID);
                    }
                    _CombatIDListNeedsUpdate = false;
                }
                else
                {
                    _CombatListNeedsUpdate = true;
                }
                return _CombatIDList;
            }
            set
            {
                _CombatIDList = value;
            }
        }

        public void ResetIDFlag()
        {
            _CombatListNeedsUpdate = false;
        }

        [DataMember]
        public Guid CurrentCharacterID
        {
            get
            {
                if (CurrentCharacter == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return _CurrentCharacter.ID;
                }

            }
            set
            {
                CurrentCharacter = Characters.FirstOrDefault(a => a.ID == value);
            }
        }

        [XmlIgnore]
        public Character CurrentCharacter
        {
            get { return _CurrentCharacter; }
            set
            {
                if (_CurrentCharacter != value)
                {
                    if (_CurrentCharacter != null)
                    {
                        _CurrentCharacter.IsActive = false;
                    }

                    _CurrentCharacter = value;

                    if (_CurrentCharacter != null)
                    {
                        _CurrentCharacter.IsActive = true;
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentCharacter"));
                }
            }
        }

        [XmlIgnore]
        public InitiativeCount CurrentInitiativeCount
        {
            get
            {
                InitiativeCount count = null;

                if (CurrentCharacter != null)
                {

                    count = CurrentCharacter.InitiativeCount;
                }
                else
                {
                    count = new InitiativeCount(-1000, 0, 0);
                }

                return count;
            }
        }

        [XmlIgnore]
        public DateTime CurrentTurnStartTime
        {
            get =>  _CurrentTurnStartTime;
            set
            {
                if (_CurrentTurnStartTime != value)
                {
                    _CurrentTurnStartTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTurnStartTime"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTurnStartTimeTicks"));
                }
            }

        }

        public long ? CurrentTurnStartTimeTicks
        {
            get
            {
                return _CurrentTurnStartTime.Ticks;
            }
            set
            {
                bool changed = false;
                if (value.Value != _CurrentTurnStartTime.Ticks)
                {
                    _CurrentTurnStartTime = new DateTime(value.Value);
                    changed = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTurnStartTime"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTurnStartTimeTicks"));
                }
                if (changed)
                {

                }

            }

        }

        public bool ClockPaused
        {
            get => _ClockPaused;
            set
            {
                if (_ClockPaused != value)
                {
                    _ClockPaused = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClockPaused"));
                }
            }

        }




        [XmlIgnore]
        public RulesSystem RulesSystem
        {
            get { return _RulesSystem; }
            set
            {
                if (_RulesSystem != value)
                {
                    _RulesSystem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulesSystem"));
                }
            }
        }


        [DataMember]
        public int RulesSystemInt
        {
            get
            {
                return (int)RulesSystem;
            }
            set
            {
                RulesSystem = (RulesSystem)value;
            }
        }

        public void UpdateAllConditions()
        {
            foreach (Character ch in _UnfilteredCombatList)
            {
                UpdateConditions(ch);
            }
        }

        public void UpdateConditions(Character ch)
        {
            if (ch == null)
            {
                return;
            }

            List<ActiveCondition> remove = new List<ActiveCondition>();

            int? round = Round;

            if (round == 0 || round == null)
            {
                round = 0;
                ResetAllAppliedRounds();
            }

            if (ch.Stats.ActiveConditions != null)
            {
                foreach (ActiveCondition condition in ch.Stats.ActiveConditions)
                {
                    System.Diagnostics.Debug.Assert(condition != null);

                    if (condition != null)
                    {
                        bool passedConditionInitiative = false;

                        if (condition.InitiativeCount == null)
                        {
                            condition.InitiativeCount = CurrentInitiativeCount;
                        }

                        if (CurrentInitiativeCount != null)
                        {
                            passedConditionInitiative = CurrentInitiativeCount <= condition.InitiativeCount;

                        }

                        if (condition.Turns != null)
                        {
                            if (condition.EndTurn == null)
                            {
                                condition.EndTurn = condition.Turns + round + (passedConditionInitiative ? 0 : -1);
                            }
                            else
                            {
                                condition.Turns = condition.EndTurn - round + (passedConditionInitiative ? 0 : 1);
                            }

                            if (condition.EndTurn < round || (condition.EndTurn == round && passedConditionInitiative))
                            {
                                remove.Add(condition);
                            }
                        }
                    }
                }
            }

            if (remove.Count > 0)
            {

                foreach (ActiveCondition condition in remove)
                {
                    ch.Stats.RemoveCondition(condition);
                }

                if (ch.Stats.ActiveConditions.Count == 0)
                {
                    ch.IsConditionsOpen = false;
                }
            }

        }

        public void TriggerPlayerConditions(InitiativeCount last)
        {
            if (_Round != null && CurrentInitiativeCount != null)
            {
                if (CoreSettings.Instance.AutomaticStabilization)
                {

                    foreach (Character ch in from x in _Characters where x.IsDying select x)
                    {
                        ActiveCondition dycon = ch.FindCondition("Dying");
                        if (dycon != null && ch.InitiativeCount != null
                            && last > ch.InitiativeCount && CurrentInitiativeCount <= ch.InitiativeCount
                            && (dycon.LastAppliedRound == null || dycon.LastAppliedRound.Value < _Round.Value))

                        {
                            dycon.LastAppliedRound = Round;
                            ApplyDying(ch);
                        }

                    }
                }
            }
        }

        public void AddConditionTurns(Character ch, ActiveCondition ac, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (ac.Turns != null)
                                {
                    if (ac.EndTurn == null)
                    {
                        UpdateConditions(ch);
                    }

                    ac.EndTurn++;

                }
                else
                {
                    ac.Turns = 1;
                    ac.InitiativeCount = CurrentInitiativeCount;
                }


                UpdateConditions(ch);
            }
        }

        public void RemoveConditionTurns(Character ch, ActiveCondition ac, int count)
        {

            if (ac.Turns != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!ch.Stats.ActiveConditions.Contains(ac))
                    {
                        break;
                    }

                    if (ac.EndTurn == null)
                    {
                        UpdateConditions(ch);
                    }

                    ac.EndTurn--;

                    UpdateConditions(ch);
                }
            }
        }

        void ApplyDying(Character ch)
        {
            int target = 10 - ch.HP;
            RollResult res = RollSave(ch, Monster.SaveType.Fort, target);
            bool passed = res.Total >= target;
            if (passed)
            {
                ch.Stabilize();
                SendStabilizedMessaged(ch, target, res.Total);
            }
            else
            {
                ch.AdjustHP(-1);
                if (ch.IsDead)
                {
                    SendDyingDiedMessage(ch, target, res.Total);
                }
                else
                {
                    SendNotStabilizedMessaged(ch, target, res.Total);
                }
            }
        }

        void SendNotStabilizedMessaged(Character ch, int target, int res)
        {
            string title = ch.Name + " Not Stabilized";
            string body = ch.Name + " rolled a " + res + " on a DC " + target + " Fortitude save and failed to stabilize, losing 1 HP.";
            SendCombatStateNotification(CombatStateNotification.EventType.NotStabilized, title, body, ch);
        }

        void SendStabilizedMessaged(Character ch, int target, int res)
        {
            string title = ch.Name + " Stabilized";
            string body = ch.Name + " rolled a " + res + " on a DC " + target + " Fortitude save to stabilize.";
            SendCombatStateNotification(CombatStateNotification.EventType.Stabilized, title, body, ch);
        }

        void SendDyingDiedMessage(Character ch, int target, int res)
        {
            string title = ch.Name + " Died";
            string body = ch.Name + " rolled a " + res + " on a DC " + target + " Fortitude save, failed to save, lost 1 HP, and died.";
            SendCombatStateNotification(CombatStateNotification.EventType.DyingDied, title, body, ch);

        }

        void SendCombatStateNotification(CombatStateNotification.EventType type,
            string title, string body, object data = null)
        {
            CombatStateNotification not = new CombatStateNotification()
            {
                Type = type,
                Title = title,
                Body = body,
                Data = data
            };
            CombatStateNotificationSent?.Invoke(this, not);
        }

        public void RollInitiative()
        {
            RollInitiative(true);
        }

        public void RollInitiative(bool resetRound)
        {
            foreach (Character character in Characters)
            {
                character.CurrentInitiative =
                    InitDieRoll() + character.Stats.Init;
                character.InitiativeRolled = character.CurrentInitiative;
                character.InitiativeTiebreaker = rand.Next();
                character.HasInitiativeChanged = false;
                character.InitiativeCount = new InitiativeCount
                    (character.CurrentInitiative, character.Stats.Init, character.InitiativeTiebreaker);

            }

            if (resetRound)
            {
                Round = 1;
            }

            ResetAllAppliedRounds();

            MoveCurrentCharacterToIndex(0);
            HandleTurnChanged();
        }

        private void ResetAllAppliedRounds()
        {
            foreach (Character ch in _Characters)
            {
                if (ch.Monster != null)
                {
                    foreach (var ac in ch.Monster.ActiveConditions)
                    {
                        if (ac != null)
                        {
                            ac.LastAppliedRound = null;
                        }
                    }
                }
            }
        }

        public void RollIndividualInitiative(Character character)
        {
            if (character != null)
            {
                character.CurrentInitiative =
                    InitDieRoll() + character.Stats.Init;
                character.InitiativeRolled = character.CurrentInitiative;
                character.InitiativeTiebreaker = rand.Next();
                character.HasInitiativeChanged = false;
                character.InitiativeCount = new InitiativeCount
                    (character.CurrentInitiative, character.Stats.Init, character.InitiativeTiebreaker);
            }

        }




        private int InitDieRoll()
        {
            if (use3d6)
            {
                return DieRoll.FromString(alternateRoll).Roll().Total;
            }
            else
            {
                return rand.Next(1, 21);
            }
        }

        public void MoveNext()
        {
            UpdateAllConditions();

            int next = CombatList.IndexOf(CurrentCharacter) + 1;

            bool roundIncreased = false;

            if (next == CombatList.Count)
            {
                next = 0;

                if (Round == null)
                {
                    ResetAllAppliedRounds();
                    Round = 0;
                }
                Round++;
                roundIncreased = true;
            }
            else  if (Round == null)
            {
                ResetAllAppliedRounds();
                Round = 1;
            }

            InitiativeCount lastCount;

            if (CurrentCharacter == null)
            {
                lastCount = InitiativeCount.MaxValue;
            }
            else
            {
                lastCount = CurrentInitiativeCount;
            }



            MoveCurrentCharacterToIndex(next);

            Character character = CurrentCharacter;

            UpdateAllConditions();

            if (roundIncreased)
            {
                TriggerPlayerConditions(InitiativeCount.MaxValue);
            }
            else
            {
                TriggerPlayerConditions(lastCount);
            }
			
			if (character != null)
			{
	            if (character.IsReadying)
	            {
	                character.IsReadying = false;
	            }
	            if (character.IsDelaying)
	            {
	                character.IsDelaying = false;
	            }
			}
           
            HandleTurnChanged();
        }

        public void MovePrevious()
        {
            UpdateAllConditions();


            int next = CombatList.IndexOf(CurrentCharacter) - 1;


            if (next < 0)
            {
                next = CombatList.Count - 1;


                if (Round == null)
                {
                    ResetAllAppliedRounds();
                    Round = 1;
                }
                Round--;
            }

            MoveCurrentCharacterToIndex(next);

            UpdateAllConditions();
            HandleTurnChanged();
        }

        private void MoveCurrentCharacterToIndex(int next)
        {
            if (CombatList.Count > next)
            {
                CurrentCharacter = CombatList[next];
            }
            else
            {
                CurrentCharacter = null;
            }
        }

        public void SortInitiative()
        {
            SetInitiativeTiebreakers();

            SortCombatList();
       }


        private void SetInitiativeTiebreakers()
        {
            foreach (Character character in Characters)
            {

                character.InitiativeTiebreaker = rand.Next();
                character.HasInitiativeChanged = false;

                CurrentCharacter = CombatList.FirstOrDefault();
            }


        }

        public void SortCombatList()
        {
            SortCombatList(true, true);
        }

        public void SortCombatList(bool moveToFirst, bool assignNewInitiative, bool rawCombatList = false)
        {
			sortingList = true;
             
            if (!rawCombatList)
            {
                CombatList.Clear();
            }
            _UnfilteredCombatList.Clear();

            List<Character> init = new List<Character>();

            init.AddRange(Characters);



            if (assignNewInitiative)
            {
                foreach (Character character in Characters)
                {
                    character.InitiativeCount = new InitiativeCount
                        (character.CurrentInitiative, character.Stats.Init, character.InitiativeTiebreaker);

                }
            }
            if (rawCombatList)
            {
                init.Clear();
                init.AddRange(CombatList);
            }
            else
            {
                init.Sort((a, b) => b.InitiativeCount.CompareTo(a.InitiativeCount));
            }




            foreach (Character character in init)
            {
                if (character != null)
                {
                    _UnfilteredCombatList.Add(character);
                }
            }
            FilterList();
			
			
			sortingList = false;
			CharacterSortCompleted?.Invoke(this, new EventArgs());

            if (moveToFirst)
            {
                CurrentCharacter = CombatList.FirstOrDefault();
            }
            HandleTurnChanged();


        }
		
		public bool SortingList
		{
			get
			{
				return sortingList;
			}
		}

        private List<Character> CorrectCombatList
        {
            get
            {

                return new List<Character>(from c in _UnfilteredCombatList
                                           where c != null && !c.IsIdle && c.InitiativeLeader == null
                                           select c);
            }
        }

        public void FilterList()
        {
            List<Character> remove = new List<Character>();



            List<Character> newList = CorrectCombatList;

            foreach (Character ch in CombatList)
            {
                if (!_UnfilteredCombatList.Contains(ch) || ch.IsIdle || ch.InitiativeLeader != null)
                {
                    remove.Add(ch);
                }
            }


            foreach (Character ch in remove)
            {
                _CombatList.Remove(ch);
                
            }


            int i = 0;
            foreach (Character c in newList)
            {
                if (_CombatList.Count <= i)
                {
                    _CombatList.Add(c);
                }
                else
                {

                    if (_CombatList[i] != c)
                    {
                        int index = _CombatList.IndexOf(c);
                        if (index > i)
                        {
                            _CombatList.RemoveAt(index);
                        }
                        _CombatList.Insert(i, c);
                    }
                }
                i++;
            }
                   
			CalculateEncounterXP();
		


        }

        public void MoveCharacterAfter(Character charMove, Character charAfter)
        {
            int nIndexMove = CombatList.IndexOf(charMove);
            int nIndexAfter = CombatList.IndexOf(charAfter);

            if (nIndexAfter + 1 != nIndexMove && nIndexMove != -1)
            {
                if (nIndexMove < nIndexAfter)
                {
                    for (int i = 0; i < nIndexAfter - nIndexMove; i++)
                    {
                        MoveDownCharacter(charMove);
                    }
                }
                else if (nIndexAfter < nIndexMove)
                {
                    for (int i = 0; i < nIndexMove - nIndexAfter - 1; i++)
                    {
                        MoveUpCharacter(charMove);
                    }
                }
            }
        }
		

        public void MoveCharacterBefore(Character charMove, Character charBefore)
        {
            int nIndexMove = CombatList.IndexOf(charMove);
            int nIndexTarget = CombatList.IndexOf(charBefore);

            if (nIndexTarget -1 != nIndexMove && nIndexMove != -1)
            {
                if (nIndexMove < nIndexTarget)
                {
                    for (int i = 0; i < nIndexTarget - nIndexMove - 1; i++)
                    {
                        MoveDownCharacter(charMove);
                    }
                }
                else if (nIndexTarget < nIndexMove)
                {
                    for (int i = 0; i < nIndexMove - nIndexTarget; i++)
                    {
                        MoveUpCharacter(charMove);
                    }
                }
            }
        }		

        public void MoveUpCharacter(Character character)
        {
            
            System.Diagnostics.Debug.WriteLine("MoveUp");
            if (character != null)
            {

                UpdateAllConditions();

                if (character == CurrentCharacter)
                {
                    MoveNext();
                }

                int index = _UnfilteredCombatList.IndexOf(character);
                int nextIndex = index - 1;

                if (nextIndex >= 0)
                {
                    _UnfilteredCombatList.Move(index, nextIndex);
                    character.HasInitiativeChanged = true;


                    //update initiative count
                    character.InitiativeCount = PrevCount(_UnfilteredCombatList[index].InitiativeCount);

                    while (nextIndex > 0 &&
                        _UnfilteredCombatList[nextIndex].InitiativeCount == _UnfilteredCombatList[nextIndex - 1].InitiativeCount)
                    {
                        _UnfilteredCombatList[nextIndex - 1].InitiativeCount = NextCount(_UnfilteredCombatList[nextIndex - 1].InitiativeCount);

                        ShiftActiveConditionCount(_UnfilteredCombatList[nextIndex].InitiativeCount, _UnfilteredCombatList[nextIndex - 1].InitiativeCount);

                        nextIndex--;
                    }
                }

                CorrectCharacterLocation(character);

                UpdateAllConditions();
                HandleTurnChanged();

            }
            
        }

        public void CharacterActNow(Character character)
        {
            if (character != null)
            {
                if (character != CurrentCharacter && CurrentCharacter != null)
                {
                    if (character.IsIdle)
                    {
                        character.IsIdle = false;
                    }


                    character.IsDelaying = false;
                    character.IsReadying = false;

                    MoveCharacterBefore(character, CurrentCharacter);

                    MovePrevious();

                }
                HandleTurnChanged();
            }
        }

        private void CorrectCharacterLocation(Character character)
        {


            CombatList.Remove(character);

            List<Character> newList = CorrectCombatList;
            int clIndex = newList.IndexOf(character);

            if (clIndex != -1 && clIndex < CombatList.Count)
            {
                CombatList.Insert(clIndex, character);
            }
            else if (clIndex != -1)
            {
                CombatList.Add(character);
            }

        }

        public void MoveDownCharacter(Character character)
        {
            System.Diagnostics.Debug.WriteLine("MoveDown");
            if (character != null)
            {

                UpdateAllConditions();

                if (character == CurrentCharacter)
                {
                    MoveNext();

                }

                int index = _UnfilteredCombatList.IndexOf(character);
                int nextIndex = index + 1;




                if (nextIndex < _UnfilteredCombatList.Count)
                {


                    _UnfilteredCombatList.Move(index, nextIndex);
                    character.HasInitiativeChanged = true;

                    //update initiative count
                    character.InitiativeCount = NextCount(_UnfilteredCombatList[index].InitiativeCount);

                    while (nextIndex < _UnfilteredCombatList.Count - 1 &&
                        _UnfilteredCombatList[nextIndex].InitiativeCount == _UnfilteredCombatList[nextIndex + 1].InitiativeCount)
                    {
                        _UnfilteredCombatList[nextIndex + 1].InitiativeCount = NextCount(_UnfilteredCombatList[nextIndex + 1].InitiativeCount);

                        ShiftActiveConditionCount(_UnfilteredCombatList[nextIndex].InitiativeCount, _UnfilteredCombatList[nextIndex + 1].InitiativeCount);

                        nextIndex++;


                    }


                    CorrectCharacterLocation(character);


                }



                UpdateAllConditions();
                HandleTurnChanged();

            }
            
            System.Diagnostics.Debug.WriteLine("MoveDown - Complete");
            
        }

        //shift afflictions from one count to another.
        private void ShiftActiveConditionCount(InitiativeCount oldCount, InitiativeCount newCount)
        {
            foreach (Character cha in CombatList)
            {
                if (cha.Stats.ActiveConditions != null)
                {
                    foreach (ActiveCondition con in cha.Stats.ActiveConditions)
                    {
                        if (con.InitiativeCount != null)
                        {
                            if (con.InitiativeCount == oldCount)
                            {
                                con.InitiativeCount = newCount;
                            }
                        }
                    }
                }
            }
        }

        public InitiativeCount NextCount(InitiativeCount startCount)
        {
            InitiativeCount count = (InitiativeCount)startCount.Clone();

            if (count.Tiebreaker > 0)
            {
                count.Tiebreaker--;
            }
            else
            {
                count.Dex--;
                count.Tiebreaker = int.MaxValue;
            }

            return count;
        }


        private InitiativeCount PrevCount(InitiativeCount lastCount)
        {
            InitiativeCount count = (InitiativeCount)lastCount.Clone();

            if (count.Tiebreaker < int.MaxValue)
            {
                count.Tiebreaker++;
            }
            else
            {
                count.Dex++;
                count.Tiebreaker = 0;
            }

            return count;
        }



        private InitiativeCount GetAfterLastInitiative()
        {
            InitiativeCount count = null;
            if (CombatList.Count > 0)
            {
                Character lastChar = (Character)CombatList[CombatList.Count - 1];
                InitiativeCount lastCount = lastChar.InitiativeCount;
                count = NextCount(lastCount);

            }
            else
            {
                count = new InitiativeCount(0, 0, 0);
            }

            return count;
        }

        public void AddCharacter(Character character)
        {
            
            sortingList = true;
            character.InitiativeCount = GetAfterLastInitiative();
            Characters.Add(character);
            _UnfilteredCombatList.Add(character);
            FilterList();
            sortingList = false;
            CharacterSortCompleted?.Invoke(this, new EventArgs());
        }

        public void RemoveCharacter(Character character)
        {
            RegroupFollowers(character);
            UnlinkLeader(character);

            if (_CurrentCharacter == character)
            {
                MoveNext();
            }

            sortingList = true;
            Characters.Remove(character);
            _UnfilteredCombatList.Remove(character);
            FilterList();
            sortingList = false;
            
            CharacterSortCompleted?.Invoke(this, new EventArgs());
        }


        public void RegroupFollowers(Character ch)
        {
            List<Character> followers = new List<Character>(ch.InitiativeFollowers);

            UnlinkFollowers(ch);

            if (followers.Count > 1)
            {
                Character leader = followers[0];

                for (int i = 1; i < followers.Count; i++)
                {
                    LinkInitiative(followers[i], leader);
                }
            }
            
        }


        public void LinkInitiative(Character ch, Character leader)
        {
            if (ch.InitiativeLeader != leader)
            {
                if (ch.InitiativeLeader != null)
                {
                    UnlinkLeader(ch);
                }
                ch.InitiativeLeader = leader;
                Character afterChar = leader;
                if (leader.InitiativeFollowers.Count > 0)
                {
                    afterChar = leader.InitiativeFollowers[leader.InitiativeFollowers.Count - 1];
                }


                int followerIndex = Characters.IndexOf(ch);
                int leaderIndex =Characters.IndexOf(leader);
                int followerCount = leader.InitiativeFollowers.Count;
                if (followerCount > 0)
                {
                    leaderIndex += followerCount;
                }

                leader.InitiativeFollowers.Add(ch);
                if (CurrentCharacter == ch)
                {
                    MoveNext();
                }



                if (followerIndex != leaderIndex + 1)
                {
                    Characters.Remove(ch);

                    leaderIndex = Characters.IndexOf(leader) + followerCount;
                    Characters.Insert(leaderIndex + 1, ch);
                }

                MoveCharacterAfter(ch, afterChar);


                FilterList();
            }
            
        }

        public void UnlinkLeader(Character ch)
        {
            if (ch.InitiativeLeader != null)
            {
                ch.InitiativeLeader.InitiativeFollowers.Remove(ch);

                Character charAfter = ch.InitiativeLeader;

                if (charAfter.InitiativeFollowers.Count > 0)
                {
                    charAfter = charAfter.InitiativeFollowers[charAfter.InitiativeFollowers.Count - 1];
                }
                MoveCharacterAfter(ch, charAfter);

                ch.InitiativeLeader = null;

                int followerIndex = Characters.IndexOf(ch);
                int leaderIndex = Characters.IndexOf(charAfter);
                if (followerIndex != leaderIndex + 1)
                {
                    Characters.Move(followerIndex, leaderIndex);
                }


                FilterList();

                
            }
        }


        public void UnlinkFollowers(Character ch)
        {
            List<Character> followers = new List<Character>(ch.InitiativeFollowers);

            foreach (Character check in followers)
            {
                System.Diagnostics.Debug.Assert(check.InitiativeLeader != null);
                UnlinkLeader(check);
            }
            
        }

        public void FixInitiativeLinksList(List<Character> list)
        {
            foreach (Character character in list)
            {
                if (character.InitiativeLeaderID != null)
                {
                    Character leader = GetCharacterByID(character.InitiativeLeaderID.Value);

                    if (leader != null)
                    {
                        LinkInitiative(character, leader);
                    }

                }
            }
            FilterList();

        }

        public void FixInitiativeLinks()
        {
            foreach (Character character in Characters)
            {
                if (character.InitiativeLeaderID != null)
                {
                    Character leader = GetCharacterByID(character.InitiativeLeaderID.Value);

                    if (leader != null)
                    {
                        LinkInitiative(character, leader);
                    }

                }
            }
            FilterList();
        }

        public bool TryGetCharacterById(Guid ID, out Character ch)
        {
            ch = GetCharacterByID(ID);
            return ch != null;
                
        }

        public Character GetCharacterByID(Guid ID)
        {
            foreach (Character ch in Characters)
            {
                if (ch.ID == ID)
                {
                    return ch;
                }

            }
            return null;
        }

        public Character CloneCharacter(Character ch)
        {

            Character newCharacter = (Character)ch.Clone();
            if (newCharacter.Monster != null)
            {
                newCharacter.Name = GetUnusedName(newCharacter.Monster.Name);
            }
            AddCharacter(newCharacter);
            return newCharacter;

        }

        public Character AddMonster(Monster m, bool rollHP)
        {
            return AddMonster(m, rollHP?Character.HPMode.Roll:Character.HPMode.Default);
        }

        public Character AddMonster(Monster m, Character.HPMode mode)
        {
     
            return AddMonster(m, mode, true);
        }

        public Character AddMonster(Monster m, bool rollHP, bool monster, bool hidden = false)
        {
            return AddMonster(m, rollHP ? Character.HPMode.Roll : Character.HPMode.Default, monster, hidden);
        }

        public Character AddMonster(Monster m, Character.HPMode mode, bool monster, bool hidden = false)
        {
            Character character = new Character(m, mode);
            character.IsMonster = monster;
            character.IsHidden = hidden;
            character.Name = GetUnusedName(character.Name);
            AddCharacter(character);

            return character;
        }

        public void AddBlank(bool monster, bool hidden = false )
        {

            Character character = new Character();
            character.IsBlank = true;
            character.IsMonster = monster;
            character.IsHidden = hidden;
            character.Name = GetUnusedName(monster?"Monster":"Player");
            AddCharacter(character);
        }
		
		private void CalculateEncounterXP()
		{
            long xp = 0;

            foreach (Character c in from x in Characters where x.IsMonster select x)
            {
                if (c.Monster != null && c.Monster.XP != null)
                {
                    long? monsterXP = c.Monster.XPValue;
                    if (monsterXP != null)
                    {
                        xp += monsterXP.Value;
                    }
                }
            }

            XP = xp;

            if (xp == 0)
            {
                CR = null;
            }

            CR = Monster.EstimateCR(xp);
			
		}


        public string GetUnusedName(string name)
        {
            string checkname = name + " 1";

            for (int i = 2; i < int.MaxValue; i++)
            {
                bool nameFound = false;
                foreach (Character character in Characters)
                {
                    if (character.Name == checkname)
                    {
                        nameFound = true;
                        break;
                    }
                }
                if (nameFound)
                {

                    checkname = name + " " + i;
                    continue;
                }

                break;
            }

            return checkname;

        }

        public void MoveDroppedCharacter(Character chMove, Character chTarget, bool toMonster)
        {
            if (chMove != chTarget)
            {
                if (chMove.IsMonster != toMonster)
                {
                    RegroupFollowers(chMove);
                    UnlinkLeader(chMove);
                    chMove.IsMonster = toMonster;

                    Characters.Remove(chMove);

                    if (chTarget != null)
                    {
                        int index = Characters.IndexOf(chTarget);
                        Characters.Insert(index, chMove);
                    }
                    else
                    {
                        Characters.Add(chMove);
                    }
                }
                else
                {
                    Characters.Remove(chMove);

                    if (chMove.InitiativeFollowers != null && chMove.InitiativeFollowers.Count > 0)
                    {
                        foreach (Character ch in chMove.InitiativeFollowers)
                        {
                            Characters.Remove(ch);
                        }
                    }

                    if (chTarget != null)
                    {
                        int index = Characters.IndexOf(chTarget);
                        Characters.Insert(index, chMove);

                        if (chMove.InitiativeFollowers != null && chMove.InitiativeFollowers.Count > 0)
                        {
                            foreach (Character ch in chMove.InitiativeFollowers)
                            {
                                index++;
                                Characters.Insert(index, ch);
                            }
                        }
                    }
                    else
                    {
                        Characters.Add(chMove);

                        if (chMove.InitiativeFollowers != null && chMove.InitiativeFollowers.Count > 0)
                        {
                            foreach (Character ch in chMove.InitiativeFollowers)
                            {
                                Characters.Add(ch);
                            }
                        }

                    }
                }

            }

        }
		
		public void SavePartyFile(string filename, bool saveMonsters)
		{
			List<Character> list = new List<Character>(from c in _Characters where c.IsMonster == saveMonsters select c);
			
			XmlListLoader<Character>.Save(list, filename);
			
			
		}
		
		public void LoadPartyFiles(string[] files, bool isMonster)
        {
            // Open document
            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                if (String.Compare(fi.Extension, ".por", true) == 0 || String.Compare(fi.Extension, ".rpgrp", true) == 0)
                {
                    ImportFromFile(filename, isMonster);
                }
                else
                {


                    XmlSerializer serializer = new XmlSerializer(typeof(List<Character>));

                    // A FileStream is needed to read the XML document.
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);


                    List<Character> list = (List<Character>)serializer.Deserialize(fs);

                    foreach (Character character in list)
                    {
                        //fix duplicate ID issues
                        if (GetCharacterByID(character.ID) != null)
                        {
                            Guid original = character.ID;
                            character.ID = Guid.NewGuid();

                            foreach (Character other in list)
                            {
                                if (other.InitiativeLeaderID == original)
                                {
                                    other.InitiativeLeaderID = character.ID;
                                }
                            }
                        }

                    }

                    //add characters
                    foreach (Character character in list)
                    {
                        character.IsMonster = isMonster;

                        AddCharacter(character);
                    }

                    //add followers
                    FixInitiativeLinksList(list);

                    fs.Close();
                }
            }
        }
		        
		private void ImportFromFile(string filename, bool isMonster)
        {
			List<Monster> monsters = Monster.FromFile(filename);

            if (monsters != null)
            {
                Character ch = null;

                foreach (Monster m in monsters)
                {
                    ch = new Character(m, false);

                    ch.IsMonster = isMonster;

                    AddCharacter(ch);
                }

                if (monsters.Count == 1)
                {
                    ch.OriginalFilename = filename;
                }
            }
            
			
        }

        public class RollEventArgs : EventArgs
        {
            public RequestedRoll Roll {get; set;}
        }


        private void SendRollEvent(RequestedRoll roll)
        {
            RollRequested?.Invoke(this, new RollEventArgs() {Roll = roll});
        }
      

        public enum RollType
        {
            Attack,
            AttackSet,
            Save,
            Skill
        }

        public class RequestedRoll
        {
            public RequestedRoll()
            {
            }

            public RollType Type { get; set;}
            public Character Character {get; set;}
            public object Param1 { get; set;}
            public object Param2 { get; set;}
            public DieRoll Roll { get; set; }
            public object Result {get; set;}
            public int? Target { get; set; }
        }

        public RequestedRoll Roll(RollType type, Character character, object param1, object param2 = null, int ? target = null)
        {
            RequestedRoll req = new RequestedRoll() {
                Type = type,
                Character = character,
                Param1 = param1,
                Param2 = param2,
                Target = target,
            };

            switch (type)
            {
                case RollType.Save:
                    {
                    
                        int? mod = character.Monster.GetSave((Monster.SaveType)param1);
                        if (mod == null)
                        {
                            DieRoll roll = new DieRoll(1, 20, 0);
                            RollResult res = roll.Roll();
                            res.Rolls[0].Result = 1;
                            req.Roll = roll;
                            req.Result = res;
                        }
                        else
                        {
                            DieRoll roll = new DieRoll(1, 20, (int)mod);
                            req.Result = roll.Roll();
                            req.Roll = roll;
                        }
                    }
                    break;
                case RollType.Skill:
                    {
                        string skill = (string)param1;
                        string subtype = (string)param2;
                        int mod = character.Monster.GetSkillMod(skill, subtype);

                        DieRoll roll = new DieRoll(1, 20, mod);
                        req.Result = roll.Roll();
                    }
                    break;
                case RollType.Attack:
                    {
                        Attack atk = (Attack)param1;
                        AttackRollResult res = new AttackRollResult(atk);
                        res.Character = character;
                        req.Result = res;

                        res.Character = character;
                    }
                    break;
                case RollType.AttackSet:
                    {
                        AttackSet atkSet = (AttackSet)param1;
                        AttackSetResult res = new AttackSetResult();
                        res.Character = character;

                        foreach (Attack at in atkSet.WeaponAttacks)
                        {
                            AttackRollResult ares = new AttackRollResult(at);
                            res.Results.Add (ares);
                        }
                        foreach (Attack at in atkSet.NaturalAttacks)
                        {
                            AttackRollResult ares = new AttackRollResult(at);
                            res.Results.Add (ares);
                        }
                        req.Result = res;
                    }
                    break;
            }
            SendRollEvent(req);
            return req;


        }

        public RollResult RollSave(Character character, Monster.SaveType save, int ? target = null)
        {
            return (RollResult)Roll(RollType.Save, character, save, target: target).Result;
        }

        public class SingleAttackRoll
        {
            public SingleAttackRoll()
            {
                BonusDamage = new List<BonusDamage>();
            }
            public RollResult Result {get; set;}
            public RollResult Damage {get; set;}
            public RollResult CritResult {get; set;}
            public RollResult CritDamage {get; set;}
            public List<BonusDamage> BonusDamage {get; set;}
        }

        public class BonusDamage
        {
            public String DamageType { get; set; }
            public RollResult Damage { get; set; }
        }

        public class AttackSetResult
        {

            Character _Character;

            List<AttackRollResult> _Results = new List<AttackRollResult>();

            public Character Character
            {
                get
                {
                    return _Character;
                }
                set
                {
                    _Character = value;
                }
            }

            public List<AttackRollResult> Results
            {
                get
                {
                    return _Results;
                }
            }

        }

        public class AttackRollResult
        {
            Attack _Attack;

            String _Name;

            Character _Character;

            public String Name
            {
                get
                {
                    return _Name;
                }
            }

            public Character Character
            {
                get
                {
                    return _Character;
                }
                set
                {
                    _Character = value;
                }
            }


            public Attack Attack
            {
                get
                {
                    return _Attack;
                }
            }


            public List<SingleAttackRoll> Rolls {get; set;}



            public AttackRollResult(Attack atk)
            {
                _Attack = atk;

                _Name = atk.Name;

                Rolls = new List<SingleAttackRoll>();

                if (atk.Weapon != null)
                {
                    _Name = atk.Weapon.Name;
                }

                int totalAttacks = atk.Count * atk.Bonus.Count;

                for (int atkcount = 0; atkcount < atk.Count; atkcount++)
                {
                    foreach (int mod in atk.Bonus)
                    {
                        SingleAttackRoll sr = new SingleAttackRoll();

                        DieRoll roll = new DieRoll(1, 20, mod);

                        sr.Result = roll.Roll();
                        sr.Damage = atk.Damage.Roll();

                        if (atk.Plus != null)
                        {
                            Regex plusRegex = new Regex("(?<die>[0-9]+d[0-9]+((\\+|-)[0-9]+)?) (?<type>[a-zA-Z]+)");
                            Match dm = plusRegex.Match(atk.Plus);
                            if (dm.Success)
                            {
                                DieRoll bonusRoll = DieRoll.FromString(dm.Groups["die"].Value);
                                BonusDamage bd = new BonusDamage();
                                bd.Damage = bonusRoll.Roll();
                                bd.DamageType = dm.Groups["type"].Value;
                                sr.BonusDamage.Add(bd);
                            }
                        }



                        if (sr.Result.Rolls[0].Result >= atk.CritRange)
                        {
                            sr.CritResult = roll.Roll();

                            sr.CritDamage = new RollResult();

                            for (int i = 1; i < atk.CritMultiplier; i++)
                            {
                                RollResult crit = atk.Damage.Roll();

                                sr.CritDamage = crit + sr.CritDamage;
                            }
                        }


                        Rolls.Add (sr);
                    }
                }

            }
        }

        public void StopAnyTurnClockManager()
        {
            if (_TurnClockManager != null && _TurnClockManager.Running)
            {
                _TurnClockManager.Stop();
            }
        }

        TurnClockManager _TurnClockManager = null;
        [XmlIgnore]
        public TurnClockManager ClockManager
        {
            get
            {
                if (_TurnClockManager == null)
                {
                    _TurnClockManager = new TurnClockManager(this);
                }
                return _TurnClockManager;
            }
        }


        public class TurnClockManager
        {
            private CombatState state;
            private double eventMS = 50.0;
            private bool running;

            Timer clockTimer;

            internal TurnClockManager(CombatState state)
            {
                this.state = state;
                clockTimer = new Timer(eventMS);
                clockTimer.Elapsed += ClockTimer_Elapsed;

            }

            public void Start()
            {
                clockTimer.Start();
                running = true;
            }

            public event EventHandler ClockTimerElapsed;


            private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
            {
                ClockTimerElapsed?.Invoke(this, new EventArgs());
            }

            public void Stop()
            {
                clockTimer.Stop();
                running = false;

            }

            public double EventMS
            {
                get
                {
                    return eventMS;
                }
                set
                {
                    if (eventMS != value)
                    {
                        eventMS = value;
                        clockTimer.Interval = value;
                    }
                }
            }

            public bool Running
            {
                get => running;
            }

        }

    }
}

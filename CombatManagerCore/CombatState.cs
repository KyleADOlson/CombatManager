using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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


    public delegate void CombatStateCharacterEvent(object sender, CombatStateCharacterEventArgs e);


    [DataContract]
    public class CombatState: INotifyPropertyChanged
    {
        public event CombatStateCharacterEvent CharacterAdded;
        public event CombatStateCharacterEvent CharacterRemoved;
        public event CombatStateCharacterEvent CharacterPropertyChanged;
		public event EventHandler CharacterSortCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        private int? _Round;		
		private string _CR;
        private int? _XP;

        private ObservableCollection<Character> _Characters;
        private ObservableCollection<Character> _CombatList;
        private ObservableCollection<Character> _UnfilteredCombatList;
        private Character _CurrentCharacter;
        private List<Guid> _CombatIDList;
        private bool _CombatListNeedsUpdate;
        private bool _CombatIDListNeedsUpdate;

        private static Random rand = new Random();

        public static bool use3d6;
		
		private bool sortingList;


        public CombatState()
        {
            _Characters = new ObservableCollection<Character>();
            _Characters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Characters_CollectionChanged);
            _CombatList = new ObservableCollection<Character>();
            _UnfilteredCombatList = new ObservableCollection<Character>();
            _CombatIDList = new List<Guid>();
        }


        public CombatState(CombatState s)
        {
            _Round = s._Round;
            _CR = s._CR;
            _XP = s._XP;
            if (_Characters != null)
            {
                _Characters = new ObservableCollection<Character>(s._Characters);
                _Characters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Characters_CollectionChanged);

            }
            if (_CombatList != null)
            {
                _CombatList = new ObservableCollection<Character>(s._CombatList);
            }
            if (_UnfilteredCombatList != null)
            {
                _UnfilteredCombatList = new ObservableCollection<Character>(s._UnfilteredCombatList);
            }
            _CurrentCharacter = (Character)s._CurrentCharacter.Clone();
            _CurrentCharacter.ID = s._CurrentCharacter.ID;
            if (_CombatIDList != null)
            {
                _CombatIDList = new List<Guid>(s._CombatIDList);
            }

            _CombatIDListNeedsUpdate = s._CombatIDListNeedsUpdate;
            _CombatListNeedsUpdate = s._CombatListNeedsUpdate;

        }

        public void Copy(CombatState s)
        {
            Round = s.Round;
	        CR = s.CR;
            XP = s.XP;

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

        }


        void _Characters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Character ch in e.NewItems)
                {
                    ch.PropertyChanged += Character_PropertyChanged;

                    if (CharacterAdded != null)
                    {
                        CharacterAdded(this, new CombatStateCharacterEventArgs() { Character = ch });
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Character ch in e.OldItems)
                {
                    ch.PropertyChanged -= Character_PropertyChanged;
                    if (CharacterRemoved != null)
                    {
                        CharacterRemoved(this, new CombatStateCharacterEventArgs() { Character = ch });
                    }
                }
            }
        }



        void Character_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CharacterPropertyChanged != null)
            {
                CharacterPropertyChanged(this, new CombatStateCharacterEventArgs() { Character = (Character)sender, Property = e.PropertyName});
            }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Round")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CR")); }
                }
            }
			
		}

        [DataMember]
        public int? XP
        {
            get { return _XP; }
            set
            {
                if (_XP != value)
                {
                    _XP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("XP")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Characters")); }
                }
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
                        _CombatList.Add(Characters.FirstOrDefault(a => a.ID == g));
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CombatList")); }
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
                        {ID = c.ID, 
                            Followers =  (c.InitiativeFollowers == null)?null:
                            new List<Guid>(
                                from x in c.InitiativeFollowers select x.ID)});

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

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CurrentCharacter")); }
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

        public void UpdateAllConditions()
        {
            foreach (Character ch in CombatList)
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
            }


            if (ch.Stats.ActiveConditions != null)
            {
                foreach (ActiveCondition condition in ch.Stats.ActiveConditions)
                {
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

        public void RollInitiative()
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



            Round = 1;


            if (CombatList.Count > 0)
            {
                CurrentCharacter = CombatList[0];
            }
            else
            {
                CurrentCharacter = null;
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
                return rand.Next(1, 7) + rand.Next(1, 7) + rand.Next(1, 7);
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

            if (next == CombatList.Count)
            {
                next = 0;

                if (Round == null)
                {
                    Round = 0;
                }
                Round++;
            }
            else
            {
                if (Round == null)
                {
                    Round = 1;
                }
            }

            if (CombatList.Count > 0)
            {
                CurrentCharacter = CombatList[next];
            }
            else
            {
                CurrentCharacter = null;
            }

            Character character = CurrentCharacter;

            UpdateAllConditions();
			
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
                   Round = 1;
                }
                Round--;
            }

            if (CombatList.Count > 0)
            {
                CurrentCharacter = CombatList[next];
            }

            UpdateAllConditions();
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

        public void SortCombatList(bool moveToFirst, bool assignNewInitiative)
        {
			sortingList = true;
            CombatList.Clear();
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

            init.Sort((a, b) => b.InitiativeCount.CompareTo(a.InitiativeCount));

            foreach (Character character in init)
            {
                _UnfilteredCombatList.Add(character);
            }
			
            FilterList();
			
			
			sortingList = false;
			if (CharacterSortCompleted != null)
			{
				CharacterSortCompleted(this, new EventArgs());
			}

            if (moveToFirst)
            {
                CurrentCharacter = CombatList.FirstOrDefault();
            }


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
                                           where !c.IsIdle && c.InitiativeLeader == null
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

            }
            
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

            character.InitiativeCount = GetAfterLastInitiative();
            Characters.Add(character);
            _UnfilteredCombatList.Add(character);
            FilterList();
        }

        public void RemoveCharacter(Character character)
        {
            RegroupFollowers(character);
            UnlinkLeader(character);

            if (_CurrentCharacter == character)
            {
                MoveNext();
            }

            Characters.Remove(character);
            _UnfilteredCombatList.Remove(character);
            FilterList();
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

        public void CloneCharacter(Character ch)
        {

            Character newCharacter = (Character)ch.Clone();
            if (newCharacter.Monster != null)
            {
                newCharacter.Name = GetUnusedName(newCharacter.Monster.Name);
            }
            AddCharacter(newCharacter);

        }

        public void AddMonster(Monster m, bool rollHP)
        {
            AddMonster(m, rollHP, true);
        }
        public void AddMonster(Monster m, bool rollHP, bool monster)
        {
            Character character = new Character(m, rollHP);
            character.IsMonster = monster;
            character.Name = GetUnusedName(character.Name);
            AddCharacter(character);
        }

        public void AddBlank(bool monster)
        {

            Character character = new Character();
            character.IsBlank = true;
            character.IsMonster = monster;
            character.Name = GetUnusedName(monster?"Monster":"Player");
            AddCharacter(character);
        }
		
		private void CalculateEncounterXP()
		{
            int xp = 0;

            foreach (Character c in from x in Characters where x.IsMonster select x)
            {
                if (c.Monster != null && c.Monster.XP != null)
                {
                    int? monsterXP = c.Monster.XPValue;
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
                    FileStream fs = new FileStream(filename, FileMode.Open);


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
                foreach (Monster m in monsters)
                {

                    Character ch = new Character(m, false);

                    ch.IsMonster = isMonster;

                    AddCharacter(ch);
                }
            }
            
			
        }

    }
}

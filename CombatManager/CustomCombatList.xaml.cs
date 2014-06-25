/*
 *  CustomCombatList.xaml.cs
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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for CustomCombatList.xaml
	/// </summary>
	public partial class CustomCombatList : UserControl
    {
        private CombatState _CombatState;
		private ListCollectionView _CurrentPlayerView;
		
		private bool _ShowPlayers;
        private bool _ShowMonsters;
        private bool _HidePlayerNames;
        private bool _HideMonsterNames;
        private bool _ShowConditions;
        private double _ConditionSize;

		
		public CustomCombatList()
		{
			_ShowPlayers = true;
			_ShowMonsters = true;
			this.InitializeComponent();
		}

        public CombatState CombatState
        {
            get { return _CombatState; }
            set
            {
                if (_CombatState != value)
                {
                    _CombatState = value;
					if (_CombatState != null)
					{
						CreateCurrentPlayerView();
					}
						
                }
            }
        }
		
		private void CreateCurrentPlayerView()
		{
		   _CurrentPlayerView = new ListCollectionView(_CombatState.CombatList);
           foreach (Character ch in _CombatState.CombatList)
           {
               ch.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);
           }

            _CombatState.CombatList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CombatList_CollectionChanged);
            _CurrentPlayerView.Filter += CharacterFilter;
            _CombatState.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_CombatState_PropertyChanged);

			CombatListBox.DataContext = _CurrentPlayerView;

            this.Unloaded += new RoutedEventHandler(CustomCombatList_Unloaded);
		}

        void CustomCombatList_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (Character ch in _CombatState.CombatList)
            {
                ch.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);
            }
        }

        


        void  Character_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "IsHidden" || e.PropertyName == "IsIdle")
            {
                _CurrentPlayerView.Refresh();
            }
        } 

        
        
        void CombatList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Character ch in e.NewItems)
                {
                    ch.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);

                }
            }

            if (e.OldItems != null)
            {
                foreach (Character ch in e.OldItems)
                {
                    ch.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);
                }
            }
        }

        void _CombatState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                object obj = CombatState.CurrentCharacter ;
				if (CombatListBox.Items.Contains(obj))
				{
                    CombatListBox.ScrollIntoView(obj);

				}
            }
        }

        void _CurrentPlayerView_CurrentChanged(object sender, EventArgs e)
        {
        }
		
		public bool CharacterFilter(object item)
		{
			Character c = (Character)item;
			return c.InitiativeLeader == null &&
               (c.IsMonster ? _ShowMonsters : ShowPlayers) && !c.IsHidden && !c.IsIdle;
		}
		
		
		public bool ShowPlayers
		{
			get {return _ShowPlayers;}
			set
			{
				if (_ShowPlayers != value)
				{
					_ShowPlayers = value;
                    RefreshView();
				}
			}
		}
		public bool ShowMonsters
		{
			get {return _ShowMonsters;}
			set
			{
				if (_ShowMonsters != value)
				{
					_ShowMonsters = value;
                    RefreshView();
				}
			}
		}
        public bool ShowConditions
        {
            get { return _ShowConditions; }
            set
            {
                if (_ShowConditions != value)
                {
                    _ShowConditions = value;
                    RefreshView();
                }
            }
        }
        public double ConditionSize
        {
            get { return _ConditionSize; }
            set
            {
                if (_ConditionSize != value)
                {
                    _ConditionSize = value;
                    RefreshView();
                }
            }
        }

        public bool HidePlayerNames
        {
            get { return _HidePlayerNames; }
            set
            {
                if (_HidePlayerNames != value)
                {
                    _HidePlayerNames = value;
                    RefreshView();
                }
            }
        }
        public bool HideMonsterNames
        {
            get { return _HideMonsterNames; }
            set
            {
                if (_HideMonsterNames != value)
                {
                    _HideMonsterNames = value;
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            if (_CurrentPlayerView != null)
            {
                _CurrentPlayerView.Refresh();
            }
        }

        private void CombatListItemGrid_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            UpdateFollowerGrid((Grid)sender);

        }

        private void CombatListItemGrid_Initialized(object sender, EventArgs e)
        {

            UpdateFollowerGrid((Grid)sender);
        }

        private void UpdateFollowerGrid(Grid grid)
        {

            if (grid.DataContext is Character)
            {

                Character ch = (Character)grid.DataContext;

                if (ch != null)
                {
                    ListBox box = (ListBox)LogicalTreeHelper.FindLogicalNode(grid, "FollowerListBox");

                    ListCollectionView view = new ListCollectionView(ch.InitiativeFollowers);
                    view.Filter += new Predicate<object>(delegate(object ob) { return !((Character)ob).IsHidden; });

                    box.DataContext = view;

                    box = (ListBox)LogicalTreeHelper.FindLogicalNode(grid, "ConditionsListBox");
                    box.Visibility = UserSettings.Settings.InitiativeShowConditions ? Visibility.Visible : Visibility.Collapsed;
                    box.LayoutTransform = new ScaleTransform(_ConditionSize, _ConditionSize);
                }
            }
        }
		
	}



    class CustomCombatListCharacterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Character c = null;
            CustomCombatList list = null;

            foreach (object ob in values)
            {
                if (ob != null)
                {
                    if (ob.GetType() == typeof(Character))
                    {
                        c = (Character)ob;
                    }
                    else if (ob.GetType() == typeof(CustomCombatList))
                    {
                        list = (CustomCombatList)ob;
                    }
                }
            }

            if (c != null && list != null)
            {
                if (c.IsMonster ? !list.ShowMonsters : !list.ShowPlayers)
                {
                    return "";
                }

                if (c.IsMonster ? list.HideMonsterNames : list.HidePlayerNames)
                {
                    return "??????";
                }
                else
                {
                    return c.Name;
                }
            }

            return null;
        }


        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[targetTypes.Length];
        }

    }
}
/*
 *  UserSettings.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.IO;
using CombatManager.WinUtil;

namespace CombatManager
{

    
    public class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static UserSettings _Settings;

        public enum SettingsSaveSection
        {
            All,
            WindowState,
            Sources,
            Initiative,
            Filters
        }

        public enum MonsterSetFilter
        {
            Monsters = 0,
            NPCs = 1,
            Custom = 2,
            All = 3
        }

    
        private bool _RollHP;
        private bool _UseCore;
        private bool _UseAPG;
        private bool _UseChronicles;
        private bool _UseModules;
        private bool _UseUltimateMagic;
        private bool _UseUltimateCombat;
        private bool _UseOther; 
        private bool _ConfirmInitiativeRoll;
        private bool _ConfirmCharacterDelete;
        private bool _ConfirmClose;
        private bool _ShowAllDamageDice;
        private bool _AlternateInit3d6;
        private string _AlternateInitRoll;
        private bool _ShowHiddenInitValue;
        private bool _AddMonstersHidden;
        private bool _StatsOpenByDefault;
        private bool _CheckForUpdates;

        private bool _PlayerMiniMode;
        private bool _MonsterMiniMode;
        private int _MainWindowWidth;
        private int _MainWindowHeight;
        private int _MainWindowLeft;
        private int _MainWindowTop;
        private int _SelectedTab;

        private MonsterSetFilter _MonsterDBFilter;
        private MonsterSetFilter _MonsterTabFilter;


        private bool _RunCombatViewService;

  


        private bool _InitiativeShowPlayers;
        private bool _InitiativeShowMonsters;        
		private bool  _InitiativeHideMonsterNames;
        private bool  _InitiativeHidePlayerNames;
        private bool _InitiativeShowConditions;

        private int _InitiativeConditionsSize;


        private bool _InitiativeAlwaysOnTop;
        private double _InitiativeScale;
        private bool _InitiativeFlip;



        private bool optionsLoaded;

        public UserSettings()
        {
            _RollHP = false;
            _UseAPG = true;
            _UseCore = true;
            _UseChronicles = true;
            _UseModules = true;
            _UseUltimateMagic = true;
            _UseUltimateCombat = true;
            _UseOther = true;
            _AlternateInitRoll = "3d6";
            _PlayerMiniMode = false;
            _MonsterMiniMode = false;
            _RunCombatViewService = false;
            _CheckForUpdates = true;
            _MainWindowWidth = -1;
            _MainWindowHeight = -1;
            _MainWindowLeft = int.MinValue;
            _MainWindowTop = int.MinValue;
            _MonsterDBFilter = MonsterSetFilter.Monsters;
            _MonsterTabFilter = MonsterSetFilter.Monsters;
            _SelectedTab = 0;
            _InitiativeConditionsSize = 2;
            LoadOptions();
        }

        public bool RollHP
        {
            get { return _RollHP; }
            set
            {
                if (_RollHP != value)
                {
                    _RollHP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RollHP")); }
                }
            }
        }

        public bool UseCore
        {
            get { return _UseCore; }
            set
            {
                if (_UseCore != value)
                {
                    _UseCore = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseCore")); }
                }
            }
        }
        public bool UseAPG
        {
            get { return _UseAPG; }
            set
            {
                if (_UseAPG != value)
                {
                    _UseAPG = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseAPG")); }
                }
            }
        }
        public bool UseChronicles
        {
            get { return _UseChronicles; }
            set
            {
                if (_UseChronicles != value)
                {
                    _UseChronicles = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseChronicles")); }
                }
            }
        }
        public bool UseModules
        {
            get { return _UseModules; }
            set
            {
                if (_UseModules != value)
                {
                    _UseModules = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseModules")); }
                }
            }
        }
        public bool UseUltimateMagic
        {
            get { return _UseUltimateMagic; }
            set
            {
                if (_UseUltimateMagic != value)
                {
                    _UseUltimateMagic = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseUltimateMagic")); }
                }
            }
        }
        public bool UseUltimateCombat
        {
            get { return _UseUltimateCombat; }
            set
            {
                if (_UseUltimateCombat != value)
                {
                    _UseUltimateCombat = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseUltimateCombat")); }
                }
            }
        }
        public bool UseOther
        {
            get { return _UseOther; }
            set
            {
                if (_UseOther != value)
                {
                    _UseOther = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("UseOther")); }
                }
            }
        }
        public bool ConfirmInitiativeRoll
        {
            get { return _ConfirmInitiativeRoll; }
            set
            {
                if (_ConfirmInitiativeRoll != value)
                {
                    _ConfirmInitiativeRoll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ConfirmInitiativeRoll")); }
                }
            }
        }
        public bool ConfirmCharacterDelete
        {
            get { return _ConfirmCharacterDelete; }
            set
            {
                if (_ConfirmCharacterDelete != value)
                {
                    _ConfirmCharacterDelete = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ConfirmCharacterDelete")); }
                }
            }
        }
        public bool ConfirmClose
        {
            get { return _ConfirmClose; }
            set
            {
                if (_ConfirmClose != value)
                {
                    _ConfirmClose = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ConfirmClose")); }
                }
            }
        }
        public bool ShowAllDamageDice
        {
            get { return _ShowAllDamageDice; }
            set
            {
                if (_ShowAllDamageDice != value)
                {
                    _ShowAllDamageDice = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ShowAllDamageDice")); }
                }
            }
        }
        public bool AlternateInit3d6
        {
            get { return _AlternateInit3d6; }
            set
            {
                if (_AlternateInit3d6 != value)
                {
                    _AlternateInit3d6 = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AlternateInit3d6")); }
                }
            }
        }

        public DieRoll AlternateInitDieRoll
        {
            get
            {
                return DieRoll.FromString(AlternateInitRoll);
            }
        }

        public String AlternateInitRoll
        {
            get { return _AlternateInitRoll; }
            set
            {
                if (_AlternateInitRoll != value)
                {
                    _AlternateInitRoll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AlternateInitRoll")); }
                }
            }
        }
        public bool PlayerMiniMode
        {
            get { return _PlayerMiniMode; }
            set
            {
                if (_PlayerMiniMode != value)
                {
                    _PlayerMiniMode = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PlayerMiniMode")); }
                }
            }
        }
        public bool MonsterMiniMode
        {
            get { return _MonsterMiniMode; }
            set
            {
                if (_MonsterMiniMode != value)
                {
                    _MonsterMiniMode = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MonsterMiniMode")); }
                }
            }
        }


        public int MainWindowWidth
        {
            get { return _MainWindowWidth; }
            set
            {
                if (_MainWindowWidth != value)
                {
                    _MainWindowWidth = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MainWindowWidth")); }
                }
            }
        }
        public int MainWindowHeight
        {
            get { return _MainWindowHeight; }
            set
            {
                if (_MainWindowHeight != value)
                {
                    _MainWindowHeight = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MainWindowHeight")); }
                }
            }
        }

        public int MainWindowLeft
        {
            get { return _MainWindowLeft; }
            set
            {
                if (_MainWindowLeft != value)
                {
                    _MainWindowLeft = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MainWindowLeft")); }
                }
            }
        }
        public int MainWindowTop
        {
            get { return _MainWindowTop; }
            set
            {
                if (_MainWindowTop != value)
                {
                    _MainWindowTop = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MainWindowTop")); }
                }
            }
        }
        public int SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                if (_SelectedTab != value)
                {
                    _SelectedTab = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SelectedTab")); }
                }
            }
        }


        public bool InitiativeShowPlayers
        {
            get { return _InitiativeShowPlayers; }
            set
            {
                if (_InitiativeShowPlayers != value)
                {
                    _InitiativeShowPlayers = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeShowPlayers")); }
                }
            }
        }
        public bool InitiativeShowMonsters
        {
            get { return _InitiativeShowMonsters; }
            set
            {
                if (_InitiativeShowMonsters != value)
                {
                    _InitiativeShowMonsters = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeShowMonsters")); }
                }
            }
        }
        public bool InitiativeHideMonsterNames
        {
            get { return _InitiativeHideMonsterNames; }
            set
            {
                if (_InitiativeHideMonsterNames != value)
                {
                    _InitiativeHideMonsterNames = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeHideMonsterNames")); }
                }
            }
        }
        public bool InitiativeHidePlayerNames
        {
            get { return _InitiativeHidePlayerNames; }
            set
            {
                if (_InitiativeHidePlayerNames != value)
                {
                    _InitiativeHidePlayerNames = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeHidePlayerNames")); }
                }
            }
        }
        public bool InitiativeShowConditions
        {
            get { return _InitiativeShowConditions; }
            set
            {
                if (_InitiativeShowConditions != value)
                {
                    _InitiativeShowConditions = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeShowConditions")); }
                }
            }
        }
        public bool CheckForUpdates
        {
            get { return _CheckForUpdates; }
            set
            {
                if (_CheckForUpdates != value)
                {
                    _CheckForUpdates = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CheckForUpdates")); }
                }
            }
        }

        public int InitiativeConditionsSize
        {
            get { return _InitiativeConditionsSize; }
            set
            {
                if (_InitiativeConditionsSize != value)
                {
                    _InitiativeConditionsSize = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeConditionsSize")); }
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeConditionsSizePercent")); }
                    
                }
            }
        }
        public double InitiativeConditionsSizePercent
        {
            get { return .5 + .25 * (double)_InitiativeConditionsSize ; }
        }

        public bool InitiativeAlwaysOnTop
        {
            get { return _InitiativeAlwaysOnTop; }
            set
            {
                if (_InitiativeAlwaysOnTop != value)
                {
                    _InitiativeAlwaysOnTop = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeAlwaysOnTop")); }
                }
            }
        }
        public double InitiativeScale
        {
            get { return _InitiativeScale; }
            set
            {
                if (_InitiativeScale != value)
                {
                    _InitiativeScale = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeScale")); }
                }
            }
        }
        public bool InitiativeFlip
        {
            get { return _InitiativeFlip; }
            set
            {
                if (_InitiativeFlip != value)
                {
                    _InitiativeFlip = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeFlip")); }
                }
            }
        }

        public MonsterSetFilter MonsterDBFilter
        {
            get { return _MonsterDBFilter; }
            set
            {
                if (_MonsterDBFilter != value)
                {
                    _MonsterDBFilter = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MonsterDBFilter")); }
                }
            }
        }
        public MonsterSetFilter MonsterTabFilter
        {
            get { return _MonsterTabFilter; }
            set
            {
                if (_MonsterTabFilter != value)
                {
                    _MonsterTabFilter = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MonsterTabFilter")); }
                }
            }
        }


        public bool RunCombatViewService
        {
            get { return _RunCombatViewService; }
            set
            {
                if (_RunCombatViewService != value)
                {
                    _RunCombatViewService = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RunCombatViewService")); }
                }
            }
        }

        public bool ShowHiddenInitValue
        {
            get { return _ShowHiddenInitValue; }
            set
            {
                if (_ShowHiddenInitValue != value)
                {
                    _ShowHiddenInitValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ShowHiddenInitValue")); }
                }
            }
        }

        public bool AddMonstersHidden
        {
            get { return _AddMonstersHidden; }
            set
            {
                if (_AddMonstersHidden != value)
                {
                    _AddMonstersHidden = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AddMonstersHidden")); }
                }
            }
        }
        
        public bool StatsOpenByDefault
        {
            get { return _StatsOpenByDefault; }
            set
            {
                if (_StatsOpenByDefault != value)
                {
                    _StatsOpenByDefault = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("StatsOpenByDefault")); }
                }
            }
        }


        void LoadOptions()
        {
            try
            {

                RegistryKey key = RegistryHelper.LoadCU();
                
                RollHP = key.LoadBool("RollHP", false);
                UseCore = key.LoadBool("UseCore", true);
                UseAPG = key.LoadBool("UseAPG", true);
                UseChronicles = key.LoadBool("UseChronicles", true);
                UseModules = key.LoadBool("UseModules", true); 
                UseUltimateMagic = key.LoadBool("UseUltimateMagic", true);
                UseUltimateCombat = key.LoadBool("UseUltimateCombat", true);
                UseOther = key.LoadBool("UseOther", true);
                ConfirmCharacterDelete = key.LoadBool("ConfirmCharacterDelete", false);
                ConfirmInitiativeRoll = key.LoadBool("ConfirmInitiativeRoll", false);
                ConfirmClose = key.LoadBool("ConfirmClose", false);
                ShowAllDamageDice = key.LoadBool("ShowAllDamageDice", false);
                PlayerMiniMode = key.LoadBool("PlayerMiniMode", false);
                MonsterMiniMode = key.LoadBool("MonsterMiniMode", false);
                MainWindowWidth = key.LoadInt("MainWindowWidth", -1);
                MainWindowHeight = key.LoadInt("MainWindowHeight", -1);
                MainWindowLeft = key.LoadInt("MainWindowLeft", int.MinValue);
                MainWindowTop = key.LoadInt("MainWindowTop", int.MinValue);
                SelectedTab = key.LoadInt("SelectedTab", 0);
                AlternateInitRoll = key.LoadString("AlternateInitRoll", "3d6");
                AlternateInit3d6 = key.LoadBool("AlternateInit3d6", false);
                InitiativeShowPlayers = key.LoadBool("InitiativeShowPlayers", true);
                InitiativeShowMonsters = key.LoadBool("InitiativeShowMonsters", true);
                InitiativeHideMonsterNames = key.LoadBool("InitiativeHideMonsterNames", false);
                InitiativeHidePlayerNames = key.LoadBool("InitiativeHidePlayerNames", false);
                InitiativeShowConditions = key.LoadBool("InitiativeShowConditions", false);
                InitiativeConditionsSize = key.LoadInt("InitiativeConditionsSize", 2);
                InitiativeAlwaysOnTop  = key.LoadBool("InitiativeAlwaysOnTop", false);
                InitiativeScale = key.LoadDouble("InitiativeScale", 1.0);
                InitiativeFlip  = key.LoadBool("InitiativeFlip", false);
                RunCombatViewService = key.LoadBool("RunCombatViewService", false);
                ShowHiddenInitValue = key.LoadBool("ShowHiddenInitValue", false);
                AddMonstersHidden = key.LoadBool("AddMonstersHidden", false);
                StatsOpenByDefault = key.LoadBool("StatsOpenByDefault", false);
                CheckForUpdates = key.LoadBool("CheckForUpdates", true); 
                MonsterDBFilter = (MonsterSetFilter)key.LoadInt("MonsterDBFilter", (int)MonsterSetFilter.Monsters);
                MonsterTabFilter = (MonsterSetFilter)key.LoadInt("MonsterTabFilter", (int)MonsterSetFilter.Monsters);

                optionsLoaded = true;
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        
        public void SaveOptions()
        {
            SaveOptions(SettingsSaveSection.All);
        }
        public void SaveOptions(SettingsSaveSection section)
        {

            if (optionsLoaded)
            {
                try
                {

                    RegistryKey key = RegistryHelper.LoadCU();


                    if (section == SettingsSaveSection.All)
                    {
                        key.SaveBool("RollHP", RollHP);
                        key.SaveBool("ConfirmCharacterDelete", ConfirmCharacterDelete);
                        key.SaveBool("ConfirmInitiativeRoll", ConfirmInitiativeRoll);
                        key.SaveBool("ConfirmClose", ConfirmClose);
                        key.SaveBool("ShowAllDamageDice", ShowAllDamageDice);
                        key.SaveBool("AlternateInit3d6", AlternateInit3d6);
                        key.SaveString("AlternateInitRoll", AlternateInitRoll);
                        key.SaveBool("RunCombatViewService", RunCombatViewService);
                        key.SaveBool("ShowHiddenInitValue", ShowHiddenInitValue);
                        key.SaveBool("AddMonstersHidden", AddMonstersHidden);
                        key.SaveBool("StatsOpenByDefault", StatsOpenByDefault);
                        key.SaveBool("CheckForUpdates", CheckForUpdates);
                        
                    }

                    if (section == SettingsSaveSection.WindowState || section == SettingsSaveSection.All)
                    {
                        key.SaveBool("PlayerMiniMode", PlayerMiniMode);
                        key.SaveBool("MonsterMiniMode", MonsterMiniMode);
                        key.SaveInt("MainWindowWidth", MainWindowWidth);
                        key.SaveInt("MainWindowHeight", MainWindowHeight);
                        key.SaveInt("MainWindowLeft", MainWindowLeft);
                        key.SaveInt("MainWindowTop", MainWindowTop);
                        key.SaveInt("SelectedTab", SelectedTab);
                    }

                    if (section == SettingsSaveSection.Sources || section == SettingsSaveSection.All)
                    {

                        key.SaveBool("UseCore", UseCore);
                        key.SaveBool("UseAPG", UseAPG);
                        key.SaveBool("UseChronicles", UseChronicles);
                        key.SaveBool("UseModules", UseModules);
                        key.SaveBool("UseUltimateMagic", UseUltimateMagic);
                        key.SaveBool("UseUltimateCombat", UseUltimateCombat);
                        key.SaveBool("UseOther", UseOther);
                    }

                    if (section == SettingsSaveSection.All || section == SettingsSaveSection.Initiative)
                    {
                        key.SaveBool("InitiativeShowPlayers", InitiativeShowPlayers);
                        key.SaveBool("InitiativeShowMonsters", InitiativeShowMonsters);
                        key.SaveBool("InitiativeHideMonsterNames", InitiativeHideMonsterNames);
                        key.SaveBool("InitiativeHidePlayerNames", InitiativeHidePlayerNames);
                        key.SaveBool("InitiativeShowConditions", InitiativeShowConditions);
                        key.SaveInt("InitiativeConditionsSize", InitiativeConditionsSize);
                        key.SaveBool("InitiativeAlwaysOnTop", InitiativeAlwaysOnTop);
                        key.SaveDouble("InitiativeScale", InitiativeScale);
                        key.SaveBool("InitiativeFlip", InitiativeFlip);
                    }
                    if (section == SettingsSaveSection.All || section == SettingsSaveSection.Filters)
                    {
                        key.SaveInt("MonsterDBFilter", (int)MonsterDBFilter);
                        key.SaveInt("MonsterTabFilter", (int)MonsterTabFilter);
                    }


                }

                catch (System.Security.SecurityException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }




        public static bool Loaded
        {
            get
            {
                return (_Settings != null && _Settings.optionsLoaded);
            }
        }
 


        public static UserSettings Settings
        {
            get
            {
                if (_Settings == null)
                {
                    _Settings = new UserSettings();
                }
                return _Settings;
            }
        }

    }



 
}

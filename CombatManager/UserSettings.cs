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
using CombatManager.LocalService;

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
            Filters,
            System,
            LocalService,
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
        private Character.HPMode _DefaultHPMode;

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

        private int _ColorScheme;
        private bool _DarkScheme;

        private bool _RunLocalService;
        private bool _RunWebService;
        private ushort _LocalServicePort;
        private string _LocalServicePasscode;

        private int _RulesSystem;

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
            _DefaultHPMode = Character.HPMode.Default;
            _MainWindowWidth = -1;
            _MainWindowHeight = -1;
            _MainWindowLeft = int.MinValue;
            _MainWindowTop = int.MinValue;
            _MonsterDBFilter = MonsterSetFilter.Monsters;
            _MonsterTabFilter = MonsterSetFilter.Monsters;
            _SelectedTab = 0;
            _InitiativeConditionsSize = 2;
            _ColorScheme = 0;
            _DarkScheme = false;
            _RunLocalService = false;
            _RunWebService = true;
            _RulesSystem = 0;
            _LocalServicePort = LocalCombatManagerService.DefaultPort;
            _LocalServicePasscode = "";
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

        public Character.HPMode DefaultHPMode
        {
            get { return _DefaultHPMode; }
            set
            {
                if (_DefaultHPMode != value)
                {
                    _DefaultHPMode = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DefaultHPMode")); }
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

        public int ColorScheme
        {
            get { return _ColorScheme; }
            set
            {
                if (_ColorScheme != value)
                {
                    _ColorScheme = value;
                    if (optionsLoaded)
                    {
                        ColorManager.PrepareCurrentScheme();
                    }

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ColorScheme")); }

                }
            }
        }

        public bool DarkScheme
        {
            get { return _DarkScheme; }
            set
            {
                if (_DarkScheme != value)
                {
                    _DarkScheme = value;
                    if (optionsLoaded)
                    {
                        ColorManager.PrepareCurrentScheme();
                    }

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DarkScheme")); }

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

        public bool RunLocalService
        {
            get { return _RunLocalService; }
            set
            {
                if (_RunLocalService != value)
                {
                    _RunLocalService = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RunLocalService")); }
                }
            }
        }

        public bool RunWebService
        {
            get { return _RunWebService; }
            set
            {
                if (_RunWebService != value)
                {
                    _RunWebService = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RunWebService")); }
                }
            }
        }

        public ushort LocalServicePort
        {
            get { return _LocalServicePort; }
            set
            {
                if (_LocalServicePort != value)
                {
                    _LocalServicePort = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("LocalServicePort")); }
                }
            }
        }

        public String LocalServicePasscode
        {
            get { return _LocalServicePasscode; }
            set
            {
                if (_LocalServicePasscode != value)
                {
                    _LocalServicePasscode = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("LocalServicePasscode")); }
                }
            }
        }

        public RulesSystem RulesSystem
        {
            get { return (RulesSystem)_RulesSystem; }
            set
            {
                if (_RulesSystem != (int)value)
                {
                    _RulesSystem = (int)value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulesSystem"));
                }
            }
        }



        void LoadOptions()
        {
            try
            {
                RollHP = CoreSettings.LoadBoolValue("RollHP", false);
                UseCore = CoreSettings.LoadBoolValue("UseCore", true);
                UseAPG = CoreSettings.LoadBoolValue("UseAPG", true);
                UseChronicles = CoreSettings.LoadBoolValue("UseChronicles", true);
                UseModules = CoreSettings.LoadBoolValue("UseModules", true); 
                UseUltimateMagic = CoreSettings.LoadBoolValue("UseUltimateMagic", true);
                UseUltimateCombat = CoreSettings.LoadBoolValue("UseUltimateCombat", true);
                UseOther = CoreSettings.LoadBoolValue("UseOther", true);
                ConfirmCharacterDelete = CoreSettings.LoadBoolValue("ConfirmCharacterDelete", false);
                ConfirmInitiativeRoll = CoreSettings.LoadBoolValue("ConfirmInitiativeRoll", false);
                ConfirmClose = CoreSettings.LoadBoolValue("ConfirmClose", false);
                ShowAllDamageDice = CoreSettings.LoadBoolValue("ShowAllDamageDice", false);
                PlayerMiniMode = CoreSettings.LoadBoolValue("PlayerMiniMode", false);
                MonsterMiniMode = CoreSettings.LoadBoolValue("MonsterMiniMode", false);
                MainWindowWidth = CoreSettings.LoadIntValue("MainWindowWidth", -1);
                MainWindowHeight = CoreSettings.LoadIntValue("MainWindowHeight", -1);
                MainWindowLeft = CoreSettings.LoadIntValue("MainWindowLeft", int.MinValue);
                MainWindowTop = CoreSettings.LoadIntValue("MainWindowTop", int.MinValue);
                SelectedTab = CoreSettings.LoadIntValue("SelectedTab", 0);
                AlternateInitRoll = CoreSettings.LoadStringValue("AlternateInitRoll", "3d6");
                AlternateInit3d6 = CoreSettings.LoadBoolValue("AlternateInit3d6", false);
                InitiativeShowPlayers = CoreSettings.LoadBoolValue("InitiativeShowPlayers", true);
                InitiativeShowMonsters = CoreSettings.LoadBoolValue("InitiativeShowMonsters", true);
                InitiativeHideMonsterNames = CoreSettings.LoadBoolValue("InitiativeHideMonsterNames", false);
                InitiativeHidePlayerNames = CoreSettings.LoadBoolValue("InitiativeHidePlayerNames", false);
                InitiativeShowConditions = CoreSettings.LoadBoolValue("InitiativeShowConditions", false);
                InitiativeConditionsSize = CoreSettings.LoadIntValue("InitiativeConditionsSize", 2);
                InitiativeAlwaysOnTop  = CoreSettings.LoadBoolValue("InitiativeAlwaysOnTop", false);
                InitiativeScale = CoreSettings.LoadDoubleValue("InitiativeScale", 1.0);
                InitiativeFlip  = CoreSettings.LoadBoolValue("InitiativeFlip", false);
                RunCombatViewService = CoreSettings.LoadBoolValue("RunCombatViewService", false);
                ShowHiddenInitValue = CoreSettings.LoadBoolValue("ShowHiddenInitValue", false);
                AddMonstersHidden = CoreSettings.LoadBoolValue("AddMonstersHidden", false);
                StatsOpenByDefault = CoreSettings.LoadBoolValue("StatsOpenByDefault", false);
                CheckForUpdates = CoreSettings.LoadBoolValue("CheckForUpdates", true);
                DefaultHPMode = (Character.HPMode)CoreSettings.LoadIntValue("DefaultHPMode", 0);
                MonsterDBFilter = (MonsterSetFilter)CoreSettings.LoadIntValue("MonsterDBFilter", (int)MonsterSetFilter.Monsters);
                MonsterTabFilter = (MonsterSetFilter)CoreSettings.LoadIntValue("MonsterTabFilter", (int)MonsterSetFilter.Monsters);
                ColorScheme = CoreSettings.LoadIntValue("ColorScheme", 0);
                DarkScheme = CoreSettings.LoadBoolValue("DarkScheme", false);
                RulesSystem = (RulesSystem)CoreSettings.LoadIntValue("RulesSystem", 0);
                RunLocalService = CoreSettings.LoadBoolValue("RunLocalService", false);
                RunWebService = CoreSettings.LoadBoolValue("RunWebService", true);
                LocalServicePort = (ushort)CoreSettings.LoadIntValue("LocalServicePort", LocalCombatManagerService.DefaultPort);
                LocalServicePasscode = CoreSettings.LoadStringValue("LocalServicePasscode", "");

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

                    RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\CombatManager", RegistryKeyPermissionCheck.Default);


                    if (section == SettingsSaveSection.All)
                    {
                        CoreSettings.SaveBoolValue("RollHP", RollHP);
                        CoreSettings.SaveBoolValue("ConfirmCharacterDelete", ConfirmCharacterDelete);
                        CoreSettings.SaveBoolValue("ConfirmInitiativeRoll", ConfirmInitiativeRoll);
                        CoreSettings.SaveBoolValue("ConfirmClose", ConfirmClose);
                        CoreSettings.SaveBoolValue("ShowAllDamageDice", ShowAllDamageDice);
                        CoreSettings.SaveBoolValue("AlternateInit3d6", AlternateInit3d6);
                        CoreSettings.SaveStringValue("AlternateInitRoll", AlternateInitRoll);
                        CoreSettings.SaveBoolValue("RunCombatViewService", RunCombatViewService);
                        CoreSettings.SaveBoolValue("ShowHiddenInitValue", ShowHiddenInitValue);
                        CoreSettings.SaveBoolValue("AddMonstersHidden", AddMonstersHidden);
                        CoreSettings.SaveBoolValue("StatsOpenByDefault", StatsOpenByDefault);
                        CoreSettings.SaveBoolValue("CheckForUpdates", CheckForUpdates);
                        CoreSettings.SaveIntValue("ColorScheme", ColorScheme);
                        CoreSettings.SaveIntValue("DefaultHPMode", (int)DefaultHPMode);


                    }

                    if (section == SettingsSaveSection.System || section == SettingsSaveSection.All)
                    {
                        CoreSettings.SaveIntValue("RulesSystem", (int)RulesSystem);
                    }

                    if (section == SettingsSaveSection.WindowState || section == SettingsSaveSection.All)
                    {
                        CoreSettings.SaveBoolValue("PlayerMiniMode", PlayerMiniMode);
                        CoreSettings.SaveBoolValue("MonsterMiniMode", MonsterMiniMode);
                        CoreSettings.SaveIntValue("MainWindowWidth", MainWindowWidth);
                        CoreSettings.SaveIntValue("MainWindowHeight", MainWindowHeight);
                        CoreSettings.SaveIntValue("MainWindowLeft", MainWindowLeft);
                        CoreSettings.SaveIntValue("MainWindowTop", MainWindowTop);
                        CoreSettings.SaveIntValue("SelectedTab", SelectedTab);
                    }

                    if (section == SettingsSaveSection.Sources || section == SettingsSaveSection.All)
                    {

                        CoreSettings.SaveBoolValue("UseCore", UseCore);
                        CoreSettings.SaveBoolValue("UseAPG", UseAPG);
                        CoreSettings.SaveBoolValue("UseChronicles", UseChronicles);
                        CoreSettings.SaveBoolValue("UseModules", UseModules);
                        CoreSettings.SaveBoolValue("UseUltimateMagic", UseUltimateMagic);
                        CoreSettings.SaveBoolValue("UseUltimateCombat", UseUltimateCombat);
                        CoreSettings.SaveBoolValue("UseOther", UseOther);
                    }

                    if (section == SettingsSaveSection.All || section == SettingsSaveSection.Initiative)
                    {
                        CoreSettings.SaveBoolValue("InitiativeShowPlayers", InitiativeShowPlayers);
                        CoreSettings.SaveBoolValue("InitiativeShowMonsters", InitiativeShowMonsters);
                        CoreSettings.SaveBoolValue("InitiativeHideMonsterNames", InitiativeHideMonsterNames);
                        CoreSettings.SaveBoolValue("InitiativeHidePlayerNames", InitiativeHidePlayerNames);
                        CoreSettings.SaveBoolValue("InitiativeShowConditions", InitiativeShowConditions);
                        CoreSettings.SaveIntValue("InitiativeConditionsSize", InitiativeConditionsSize);
                        CoreSettings.SaveBoolValue("InitiativeAlwaysOnTop", InitiativeAlwaysOnTop);
                        CoreSettings.SaveDoubleValue("InitiativeScale", InitiativeScale);
                        CoreSettings.SaveBoolValue("InitiativeFlip", InitiativeFlip);
                    }
                    if (section == SettingsSaveSection.All || section == SettingsSaveSection.LocalService)
                    {
                        CoreSettings.SaveBoolValue("RunLocalService", RunLocalService);
                        CoreSettings.SaveBoolValue("RunWebService", RunWebService);
                        CoreSettings.SaveIntValue("LocalServicePort", LocalServicePort);
                        CoreSettings.SaveStringValue("LocalServicePasscode", LocalServicePasscode);

                    }
                    if (section == SettingsSaveSection.All || section == SettingsSaveSection.Filters)
                    {
                        CoreSettings.SaveIntValue( "MonsterDBFilter", (int)MonsterDBFilter);
                        CoreSettings.SaveIntValue( "MonsterTabFilter", (int)MonsterTabFilter);
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

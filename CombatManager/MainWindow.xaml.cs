/*
 *  MainWindow.xaml.cs
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

using System;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Interop;
using System.ServiceModel;
using System.Threading;
using System.Net;
using System.Xml.Linq;
using System.Reflection;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window, IInitiativeController
    {
        bool mainWindowLoaded;
        bool checkBoxSettingsLoaded;

        UndoHelper undo = new UndoHelper();


        ListCollectionView combatView;
        ICollectionView monsterView;
        ICollectionView playerView;
        
        //ListCollectionView currentPlayerView;
        
        ICollectionView spellsView;
        ICollectionView featsView;
        ICollectionView dbView;
        ICollectionView rulesView;
        ICollectionView magicItemsView;

        bool noRulesViewRefresh;

        Monster currentViewMonster;
        Character currentViewCharacter;
        Character lastCurrentCharacter;

        Rule lastViewRule;

        ICollectionView monsterTabView;

        //for hp drag
        int initialHP;

        int lastHPChange;

        //combat round        
        CombatState combatState;

        bool combatLayoutLoaded;
        bool restoreDefaultLayout;

        //campaign info
        CampaignInfo campaignInfo;
		ListCollectionView campaignDayView;

        //for dragging top and left
        double dragStartLeft;
        double dragStartWidth;
        double dragStartTop;
        double dragStartHeight;

        const string savePartyFileFilter = "Group Files (*.cmpt)|*.cmpt";
        const string loadPartyFileFilter = "Group Files (*.cmpt;*.cmet)|*.cmpt;*.cmet";
        const string herolabFileFilter = "Hero Lab Files (*.por)|*.por";
        const string pcGenExportFileFilter = "PCGen Export Files (*.rpgrp)|*.rpgrp";
        const string saveAllFilesFilter = "All Files(*.cmpt;*.por;*.rpgrp)|*.cmpt;*.por;*.rpgrp";
        const string loadAllFilesFilter = "All Files(*.cmpt;*.cmet;*.por;*.rpgrp)|*.cmpt;*.cmet;*.por;*.rpgrp";

        const string saveCombatStateFileFilter = "Combat State Files (*.cmcs)|*.cmcs";
        const string loadCombatStateFileFilter = "Combat State Files (*.cmcs)|*.cmcs";


        const string exportFileFilter = "Export Files (*.cmx)|*.cmx";

        Monster currentDBMonster;

        ListViewDragDropManager<Character> combatListDragManager;
        ListViewDragDropManager<Character> playerListDragManager;
        ListViewDragDropManager<Character> monsterListDragManager;

        private CombatListWindow combatListWindow;

        List<CheckBox> treasureCheckboxesList;

        List<string> _RecentDieRolls;

        List<CombatHotKey> _CombatHotKeys;

        List<InputBinding> _HotKeys = new List<InputBinding>();


        public static RoutedUICommand UndoCommand = new RoutedUICommand();
        public static RoutedUICommand RedoCommand = new RoutedUICommand();

        public static RoutedUICommand NextCommand = new RoutedUICommand();
        public static RoutedUICommand PrevCommand = new RoutedUICommand();

        private static Random rand = new Random();

        private ServiceHost _CombatViewService;

        private PipeServer _PipeServer;

        static string startupTimeText = "";



        public MainWindow()
        {



#if !DEBUG
            App.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
#endif
            SplashScreen splash = new SplashScreen();
            splash.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            splash.Show();

            DateTime t = DateTime.Now;
            DateTime start = t;
            DateTime last = DateTime.Now;
            InitializeComponent();



            RestoreWindowState();



            this.SourceInitialized += new EventHandler(win_SourceInitialized);
            MarkTime("Preload", ref t, ref last);

            LoadBestiary();
            MarkTime("Bestiary",ref t, ref last);

            LoadSpells();
            MarkTime("Spells", ref t, ref last);

            LoadFeats();
            MarkTime("Feats", ref t, ref last);

            LoadRules();
            MarkTime("Rules", ref t, ref last);

            LoadMagicItems();
            MarkTime("Magic Items", ref t, ref last);

            LoadRecentDieRolls();
            MarkTime("Die Rolls", ref t, ref last);

            LoadCampaignInfo();


            mainWindowLoaded = true;
            UpdateMonsterFlowDocument();

            combatState = XmlLoader<CombatState>.Load("CombatState.xml", true);

            if (combatState == null)
            {

                combatState = new CombatState();

            }

            CombatState.use3d6 = UserSettings.Settings.AlternateInit3d6;
            CombatState.alternateRoll = UserSettings.Settings.AlternateInitRoll;
            if (UserSettings.Settings.RunCombatViewService)
            {
                StartService();
            }

            CurrentRoundText.DataContext = combatState;
            EncounterXPDisplay.DataContext = combatState;

            combatState.Characters.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedForUndo);
            combatState.CombatList.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedForUndo);
            combatState.PropertyChanged += new PropertyChangedEventHandler(combatState_PropertyChanged);

            dbView = new ListCollectionView(Monster.Monsters);
            dbView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));

            dbView.Filter += new Predicate<object>(DBViewFilter);
            dbView.CurrentChanging += new CurrentChangingEventHandler(dbView_CurrentChanging);
            dbView.CurrentChanged += new EventHandler(dbView_CurrentChanged);
            dbView.MoveCurrentTo(null);



            monsterDBBox.DataContext = dbView;
            AddMonsterFromListButton.DataContext = dbView;

            monsterFilterBox.TextChanged += new TextChangedEventHandler(monsterFilterBox_TextChanged);



            monsterView = new ListCollectionView(combatState.Characters);
            monsterView.Filter += delegate(object item)
                            {
                                if (item == null)
                                {
                                    return false;
                                }
                                return ((Character)item).IsMonster;
                            };
            monsterView.CurrentChanged += new EventHandler(monsterView_CurrentChanged);
            monsterView.CollectionChanged += new NotifyCollectionChangedEventHandler(monsterView_CollectionChanged);
            monsterListBox.DataContext = monsterView;
            monsterListBox.SelectionChanged += new SelectionChangedEventHandler(monsterListBox_SelectionChanged);


            playerView = new ListCollectionView(combatState.Characters);
            playerView.CurrentChanged += new EventHandler(playerView_CurrentChanged);
            playerView.Filter = delegate(object item)
                            {
                                if (item == null)
                                {
                                    return false;
                                }
                                return !((Character)item).IsMonster;
                            };
            playerListBox.DataContext = playerView;
            playerListBox.SelectionChanged += new SelectionChangedEventHandler(monsterListBox_SelectionChanged);

            combatView = new ListCollectionView(combatState.CombatList);
            combatView.Filter += delegate(object item)
            {
                if (item == null)
                {
                    return false;
                }
                else
                {
                    Character cha = (Character)item;
                    return cha.InitiativeLeader == null && !cha.IsIdle;
                }
            };

            /*currentPlayerView = new ListCollectionView(combatState.CombatList);
            currentPlayerView.Filter += delegate(object item)
            {
                if (item == null)
                {
                    return false;
                }
                else
                {
                    Character cha = (Character)item;
                    return cha.InitiativeLeader == null && !cha.IsIdle;
                }
            };*/


            combatListBox.DataContext = combatView;
            CurrentPlayerText.DataContext = combatState;
            CurrentPlayerConditions.DataContext = combatState;

            combatState.CombatList.CollectionChanged += new NotifyCollectionChangedEventHandler(CombatList_CollectionChanged);
            
            //currentPlayerView.CurrentChanging += new CurrentChangingEventHandler(currentPlayerView_CurrentChanging);
            //currentPlayerView.CurrentChanged += new EventHandler(currentPlayerView_CurrentChanged);

            MoveUpButton.DataContext = combatView;
            MoveDownButton.DataContext = combatView;
            PrevButton.DataContext = combatState.CombatList;
            NextButton.DataContext = combatState.CombatList;

            SpellsTab.DataContext = spellsView;
            MonstersTab.DataContext = monsterTabView;

            combatListDragManager = new ListViewDragDropManager<Character>(combatListBox);
            combatListBox.DragEnter += new DragEventHandler(combatListBox_DragEnter);
            combatListBox.DragOver += new DragEventHandler(combatListBox_DragOver);
            combatListBox.Drop += new DragEventHandler(combatListBox_Drop);
            combatListDragManager.ProcessDrop += new EventHandler<ProcessDropEventArgs<Character>>(combatListDragManager_ProcessDrop);
            combatListDragManager.ManagerDragOver += new EventHandler(combatListDragManager_ManagerDragOver);

            playerListDragManager = new ListViewDragDropManager<Character>(playerListBox);
            playerListBox.DragEnter += new DragEventHandler(characterListBox_DragEnter);
            playerListBox.DragOver += new DragEventHandler(characterListBox_DragOver);
            playerListBox.Drop += new DragEventHandler(characterListBox_Drop);
            playerListDragManager.ProcessDrop += new EventHandler<ProcessDropEventArgs<Character>>(playerListDragManager_ProcessDrop);
            playerListDragManager.ManagerDragOver += new EventHandler(characterListDragManager_ManagerDragOver);



            monsterListDragManager = new ListViewDragDropManager<Character>(monsterListBox);
            monsterListBox.DragEnter += new DragEventHandler(characterListBox_DragEnter);
            monsterListBox.DragOver += new DragEventHandler(characterListBox_DragOver);
            monsterListBox.Drop += new DragEventHandler(characterListBox_Drop);
            monsterListDragManager.ProcessDrop += new EventHandler<ProcessDropEventArgs<Character>>(monsterListDragManager_ProcessDrop);
            monsterListDragManager.ManagerDragOver += new EventHandler(characterListDragManager_ManagerDragOver);
            



            this.Closing += new CancelEventHandler(MainWindow_Closing);

            UndoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            RedoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));
            RedoCommand.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));

            NextCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            PrevCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            
            LoadSettings();

            treasureCheckboxesList = new List<CheckBox>(new CheckBox[] {
                        	GenerateMagicalArmorCheck,
                GenerateMagicalWeaponCheck,
                GeneratePotionCheck,
                GenerateRingCheck,
                GenerateRodCheck,
                GenerateScrollCheck,
                GenerateStaffCheck,
                GenerateWandCheck,
                GenerateWondrousItemCheck});


            UpdateItemsGenerateButtons();

            Character ch = combatState.CurrentCharacter;
            combatState.SortCombatList(false, false);
            combatState.FixInitiativeLinksList(new List<Character>(combatState.Characters));
            if (ch != null)
            {
                //combatState.CurrentCharacter = ch;
            }
            UpdateCurrentMonsterFlowDocument();

            CombatViewDockingManager.Loaded += new RoutedEventHandler(CombatViewDockingManager_Loaded);
            /*CombatViewDockingManager.ActiveContentChanged += new EventHandler(CombatViewDockingManager_ActiveContentChanged);
            CombatViewDockingManager.ActiveDocumentChanged += new EventHandler(CombatViewDockingManager_ActiveDocumentChanged);
            CombatViewDockingManager.LostFocus += new RoutedEventHandler(CombatViewDockingManager_LostFocus);
            */
            CombatViewDockingManager.ManagerUnloading += new EventHandler(CombatViewDockingManager_ManagerUnloading);
            splash.Close();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);

            CalendarTab.DataContext = campaignInfo;
			UpdateCampaignDayView();
            


            MarkTime("Setup UI", ref t, ref last);

            DateTime t2 = DateTime.Now;
            TimeSpan startupTime = t2 - start;

            System.Diagnostics.Debug.WriteLine(startupTimeText);

            System.Diagnostics.Debug.WriteLine("Startup Time: " + startupTime.TotalSeconds);

            LoadHotkeys();

            PerformUpdateCheck();
            
        }


        void PipeServer_FileRecieved(object sender, PipeServer.PipeServerEventArgs e)
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    string file = e.File;

                    HandleFile(file);
                }));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

        }

        void HandleFile(string filename)
        {
            FileInfo file = new FileInfo(filename);

            if (file.Exists)
            {
                if (file.Extension == ".cmpt" || file.Extension == ".por" || file.Extension == ".rpgrp")
                {

                    PlayersOrMonstersDialog dlg = new PlayersOrMonstersDialog();
                    dlg.Filename = filename;
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        combatState.LoadPartyFiles(new string[] { filename }, dlg.Monsters);
                    }
                }
                else if (file.Extension == ".cmx")
                {
                    ImportDateFromFile(filename);
                }
                else if (file.Extension == ".cmcs")
                {
                    LoadCombatStateFromFile(filename);
                }
            }
        }

        void MarkTime(string info, ref DateTime t, ref DateTime last)
        {

            last = t;
            t = DateTime.Now;
            startupTimeText += info + ": " + (t - last).TotalSeconds + "\r\n";
        }

        void CombatViewDockingManager_ManagerUnloading(object sender, EventArgs e)
        {
            SaveCombatViewLayout();
        }

        void CombatViewDockingManager_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCombatViewLayout();
        }

        void CombatViewDockingManager_ActiveDocumentChanged(object sender, EventArgs e)
        {
            SaveCombatViewLayout();
        }

        void CombatViewDockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            SaveCombatViewLayout();
        }
        

        void CombatViewDockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (!combatLayoutLoaded)
            {
                //AvalonDock.ThemeFactory.ChangeTheme(new Uri("/AvalonDock.Themes;component/themes/dev2010.xaml", UriKind.RelativeOrAbsolute));

                AvalonDock.ThemeFactory.ChangeColors((Color)FindResource("PrimaryColorDarker"));
                SaveDefaultLayout();
                LoadCombatViewLayout();
                combatLayoutLoaded = true;
            }
            else if (!restoreDefaultLayout)
            {
                SaveCombatViewLayout();
            }
            if (restoreDefaultLayout)
            {
                RestoreDefaultLayout();
                SaveCombatViewLayout();
            }

        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1)
            {
                string file = args[1];

                HandleFile(file);
            }

            _PipeServer = new PipeServer(new WindowInteropHelper(this).Handle);
            _PipeServer.FileRecieved += new EventHandler<PipeServer.PipeServerEventArgs>(PipeServer_FileRecieved);
            _PipeServer.RunServer();
        }


        void SaveDefaultLayout()
        {
            string defaultLayoutFile = System.IO.Path.Combine(CMFileUtilities.AppDataDir, "DefaultLayout.xml");
            CombatViewDockingManager.SaveLayout(defaultLayoutFile);
        }
        void RestoreDefaultLayout()
        {
            if (CombatViewDockingManager.IsLoaded)
            {

                string defaultLayoutFile = System.IO.Path.Combine(CMFileUtilities.AppDataDir, "DefaultLayout.xml");
                if (File.Exists(defaultLayoutFile))
                {
                    try
                    {
                        CombatViewDockingManager.RestoreLayout(defaultLayoutFile);
                    }

                    catch (System.Xml.XmlException)
                    {

                    }
                }
                restoreDefaultLayout = false;
            }
            else
            {
                restoreDefaultLayout = true;
            }
        }

        void LoadCombatViewLayout()
        {
            if (CombatViewDockingManager.IsLoaded)
            {
                string layoutFile = System.IO.Path.Combine(CMFileUtilities.AppDataDir, "CombatLayout.xml");

                if (File.Exists(layoutFile))
                {
                    try
                    {
                        CombatViewDockingManager.RestoreLayout(layoutFile);
                    }

                    catch (System.Xml.XmlException)
                    {

                    }
                }
            }
        }

        void SaveCombatViewLayout()
        {
            if (CombatViewDockingManager.IsLoaded)
            {
                string layoutFile = System.IO.Path.Combine(CMFileUtilities.AppDataDir, "CombatLayout.xml");

                try
                {
                    CombatViewDockingManager.SaveLayout(layoutFile);
                }

                catch (InvalidOperationException)
                {

                }
            }
        }


        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {

                e.Handled = false;
                Exception ex = e.Exception;

                
                string file = System.IO.Path.Combine(CMFileUtilities.AppDataDir, "error" + DateTime.Now.Ticks + ".txt");



                using (FileStream f = new FileStream(file, FileMode.OpenOrCreate))
                {

                    using (StreamWriter writer = new StreamWriter(f))
                    {
                        writer.WriteLine("Combat Manager error");
                        writer.WriteLine(DateTime.Now.ToLongTimeString());
                        Exception writeEx = ex;

                        while (writeEx != null)
                        {
                            writer.WriteLine(writeEx.ToString());
                            writeEx = writeEx.InnerException;
                        }


                    }
                }


                
            }
            catch (Exception)
            {

            }

        }

        void RestoreWindowState()
        {
            PartyMiniModeButton.IsChecked = UserSettings.Settings.PlayerMiniMode;
            SetCharacterListBoxTemplate(playerListBox, UserSettings.Settings.PlayerMiniMode);
            MonsterMiniModeButton.IsChecked = UserSettings.Settings.MonsterMiniMode;
            SetCharacterListBoxTemplate(monsterListBox, UserSettings.Settings.MonsterMiniMode);
            if (UserSettings.Settings.SelectedTab >= 0 && UserSettings.Settings.SelectedTab < MainTabControl.Items.Count)
            {
                MainTabControl.SelectedIndex = UserSettings.Settings.SelectedTab;
            }

            if (UserSettings.Settings.MainWindowWidth >= MinWidth && UserSettings.Settings.MainWindowHeight >= MinHeight)
            {
                int newWidth = UserSettings.Settings.MainWindowWidth;
                int newHeight = UserSettings.Settings.MainWindowHeight;

                newWidth = Math.Min(newWidth, (int)System.Windows.SystemParameters.VirtualScreenWidth);
                newHeight = Math.Min(newHeight, (int)System.Windows.SystemParameters.VirtualScreenHeight);

                Width = newWidth;
                Height = newHeight;

                if (UserSettings.Settings.MainWindowLeft != int.MinValue && UserSettings.Settings.MainWindowTop != int.MinValue)
                {
                    EnumMonitorData data = new EnumMonitorData();
                    data.Rect = new RECT(
                        UserSettings.Settings.MainWindowLeft,
                        UserSettings.Settings.MainWindowTop,
                        (int)(UserSettings.Settings.MainWindowLeft + Width),
                        (int)(UserSettings.Settings.MainWindowTop + Height));
                    data.FoundMatch = false;
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(data));
                    Marshal.StructureToPtr(data, ptr, false);

                    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, EnumMonitors, ptr);

                    data = (EnumMonitorData)Marshal.PtrToStructure(ptr, typeof(EnumMonitorData));

                    if (data.FoundMatch)
                    {
                        WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                        Left = UserSettings.Settings.MainWindowLeft;
                        Top = UserSettings.Settings.MainWindowTop;
                    }
                    else
                    {
                        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    }
                }

                MonsterTabNPCComboBox.SelectedIndex = (int)UserSettings.Settings.MonsterTabFilter;
                CombatTabNPCFilterBox.SelectedIndex = (int)UserSettings.Settings.MonsterDBFilter;
            }
        }

        struct EnumMonitorData
        {
            public RECT Rect;
            public bool FoundMatch;
        }

        bool EnumMonitors(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            EnumMonitorData data = (EnumMonitorData)Marshal.PtrToStructure(dwData, typeof(EnumMonitorData));

            MONITORINFO mi = new MONITORINFO();
            GetMonitorInfo(hMonitor, mi);

            RECT temp;
            if (IntersectRect(out temp, ref mi.rcWork, ref data.Rect))
            {
                data.FoundMatch = true;
            }

            Marshal.StructureToPtr(data, dwData, true);

            return !data.FoundMatch;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (!UserSettings.Settings.ConfirmClose || ShowConfirmationBox("Do you want to exit Combat Manager?", "Exit Combat Manager"))
            {
                e.Cancel = false;
            }

            if (!e.Cancel)
            {
                SaveCombatState();

                if (CombatViewDockingManager.IsLoaded)
                {
                    SaveCombatViewLayout();
                }

                _PipeServer.EndServer();
            }
            

        }

        private void SaveAsCombatState()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = saveCombatStateFileFilter;

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    XmlLoader<CombatState>.Save(combatState, dlg.FileName);
                }
                catch (IOException)
                {
                    MessageBox.Show("Failed to Save file " + dlg.FileName);
                }
            }
        }

        private void LoadCombatState()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = loadCombatStateFileFilter;

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    LoadCombatStateFromFile(dlg.FileName);
                }
            }
        }

        void LoadCombatStateFromFile(string file)
        {
            try
            {

                CombatState cs = XmlLoader<CombatState>.Load(file);

                combatState.Copy(cs);

                combatView.Refresh();
                //currentPlayerView.Refresh();
                monsterView.Refresh();
                playerView.Refresh();


                combatState.FixInitiativeLinks();
                combatState.SortCombatList(false, false);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failure saving combat state: " + ex.ToString());
            }
        }

        void SaveCombatState()
        {
            try
            {
                DateTime x = DateTime.Now;

                XmlLoader<CombatState>.Save(combatState, "CombatState.xml", true);

                DateTime y = DateTime.Now;

                TimeSpan span = y - x;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failure saving combat state: " + ex.ToString());
            }
        }

        void CollectionChangedForUndo(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                undo.AddToUndo(new UndoAction((ICollection)sender, e));
                if (e.NewItems != null)
                {
                    foreach (var cur in e.NewItems)
                    {
                        if (cur is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged prop = (INotifyPropertyChanged)cur;
                            System.Reflection.PropertyInfo undoProperty = null;
                            Dictionary<string, object> state = new Dictionary<string, object>();
                            foreach (var curProp in cur.GetType().GetProperties())
                            {
                                if (curProp.Name == "UndoInfo")
                                {
                                    undoProperty = curProp;
                                }
                                else
                                {
                                    if (typeof(INotifyCollectionChanged).IsAssignableFrom(curProp.PropertyType))
                                    {
                                        INotifyCollectionChanged temp = (INotifyCollectionChanged)curProp.GetValue(cur, null);
                                        temp.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedForUndo);
                                    }

                                    state.Add(curProp.Name, curProp.GetValue(cur, null));
                                }
                            }

                            undoProperty.SetValue(cur, state, null);

                            prop.PropertyChanged += new PropertyChangedEventHandler(PropertyChangedForUndo);
                        }
                    }
                }
            }
            catch
            {
                // Nothing to do
            }
        }

        void PropertyChangedForUndo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var prop = sender.GetType().GetProperty(e.PropertyName);
                if (prop != null)
                {
                    var undoInfo = sender.GetType().GetProperty("UndoInfo");
                    if (undoInfo != null)
                    {
                        Dictionary<string, object> state = (Dictionary<string, object>)undoInfo.GetValue(sender, null);

                        var targetProp = sender.GetType().GetProperty(e.PropertyName);
                        if (targetProp != null)
                        {
                            object newValue = targetProp.GetValue(sender, null);

                            if (state.ContainsKey(e.PropertyName))
                            {
                                undo.AddToUndo(new UndoAction(sender, e.PropertyName, state[e.PropertyName], newValue));

                                state[e.PropertyName] = newValue;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Nothing to do
            }
        }

        void UndoLastAction()
        {
            List<UndoAction> actions = undo.UndoAction();
            using (var owner = undo.TakeOwner())
            {
                foreach (var act in actions)
                {
                    switch (act.UndoType)
                    {
                        case UndoAction.UndoTypes.UndoCollection:
                            if (act.Changes.NewItems != null)
                            {
                                var method = act.Collection.GetType().GetMethod("RemoveAt");

                                for (int i = 0; i < act.Changes.NewItems.Count; i++)
                                {
                                    if (act.Collection.Count > act.Changes.NewStartingIndex)
                                    {
                                        method.Invoke(act.Collection, new object[] { act.Changes.NewStartingIndex });
                                    }
                                }
                            }

                            if (act.Changes.OldItems != null)
                            {
                                var method = act.Collection.GetType().GetMethod("Insert");

                                foreach (var cur in act.Changes.OldItems)
                                {
                                    method.Invoke(act.Collection, new object[] { act.Changes.OldStartingIndex, cur });
                                }
                            }
                            break;

                        case UndoAction.UndoTypes.UndoProperty:
                            if (act.Object.GetType().GetProperty(act.Property).GetSetMethod() != null)
                            {
                                act.Object.GetType().GetProperty(act.Property).SetValue(act.Object, act.OldValue, null);
                            }
                            break;
                    }
                }

                monsterView.Refresh();
                playerView.Refresh();
                combatView.Refresh();
                //currentPlayerView.Refresh();
            }
        }

        void RedoNextAction()
        {
            List<UndoAction> actions = undo.RedoAction();
            using (var owner = undo.TakeOwner())
            {
                foreach (var act in actions)
                {
                    switch (act.UndoType)
                    {
                        case UndoAction.UndoTypes.UndoCollection:
                            if (act.Changes.OldItems != null)
                            {
                                var method = act.Collection.GetType().GetMethod("RemoveAt");

                                for (int i = 0; i < act.Changes.OldItems.Count; i++)
                                {
                                    if (act.Collection.Count > act.Changes.OldStartingIndex)
                                    {
                                        method.Invoke(act.Collection, new object[] { act.Changes.OldStartingIndex });
                                    }
                                }
                            }

                            if (act.Changes.NewItems != null)
                            {
                                var method = act.Collection.GetType().GetMethod("Insert");

                                foreach (var cur in act.Changes.NewItems)
                                {
                                    method.Invoke(act.Collection, new object[] { act.Changes.NewStartingIndex, cur });
                                }
                            }
                            break;

                        case UndoAction.UndoTypes.UndoProperty:
                            if (act.Object.GetType().GetProperty(act.Property).GetSetMethod() != null)
                            {
                                act.Object.GetType().GetProperty(act.Property).SetValue(act.Object, act.NewValue, null);
                            }
                            break;
                    }
                }

                monsterView.Refresh();
                playerView.Refresh();
                combatView.Refresh();
                //currentPlayerView.Refresh();
            }
        }

        void LoadSettings()
        {
            RollHPCheck.IsChecked = UserSettings.Settings.RollHP;
            UseCoreContentCheck.IsChecked = UserSettings.Settings.UseCore;
            UseAPGContentCheck.IsChecked = UserSettings.Settings.UseAPG;
            UseChroniclesContentCheck.IsChecked = UserSettings.Settings.UseChronicles;
            UseModulesContentCheck.IsChecked = UserSettings.Settings.UseModules;
            UseUltimateMagicContentCheck.IsChecked = UserSettings.Settings.UseUltimateMagic;
            UseUltimateCombatContentCheck.IsChecked = UserSettings.Settings.UseUltimateCombat;
            UseOtherContentCheck.IsChecked = UserSettings.Settings.UseOther;
            checkBoxSettingsLoaded = true;

        }

        void SaveSettings()
        {
            if (checkBoxSettingsLoaded)
            {
                UserSettings.Settings.RollHP = RollHPCheck.IsChecked.Value;
                UserSettings.Settings.UseCore = UseCoreContentCheck.IsChecked.Value;
                UserSettings.Settings.UseAPG = UseAPGContentCheck.IsChecked.Value;
                UserSettings.Settings.UseChronicles = UseChroniclesContentCheck.IsChecked.Value;
                UserSettings.Settings.UseModules = UseModulesContentCheck.IsChecked.Value;
                UserSettings.Settings.UseUltimateMagic = UseUltimateMagicContentCheck.IsChecked.Value;
                UserSettings.Settings.UseOther = UseOtherContentCheck.IsChecked.Value;

                UserSettings.Settings.SaveOptions();
            }

        }

        void combatListDragManager_ProcessDrop(object sender, ProcessDropEventArgs<Character> e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (e.OldIndex != e.NewIndex)
                {
                    if (e.NewIndex > e.OldIndex)
                    {
                        int moveCount = e.NewIndex - e.OldIndex;

                        for (int i = 0; i < moveCount; i++)
                        {
                            MoveDownCharacter(e.DataItem);
                        }

                    }
                    else if (e.NewIndex < e.OldIndex)
                    {

                        int moveCount = e.OldIndex - e.NewIndex;

                        for (int i = 0; i < moveCount; i++)
                        {
                            MoveUpCharacter(e.DataItem);
                        }
                    }
                }
            }
        }



        void combatListDragManager_ManagerDragOver(object sender, EventArgs e)
        {

        }


        void combatListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Effects == DragDropEffects.None)
                return;

            Object ob = e.Data.GetData(typeof(Object)) as Object;
            if (sender == combatListBox)
            {
                if (!combatListDragManager.IsDragInProgress)
                {

                }

            }

        }

        void combatListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }


        void combatListBox_DragOver(object sender, DragEventArgs e)
        {

        }


        void characterListDragManager_ManagerDragOver(object sender, EventArgs e)
        {

        }

        void playerListDragManager_ProcessDrop(object sender, ProcessDropEventArgs<Character> e)
        {
            ProcessCharacterListDrop(e, playerListBox);
        }


        void monsterListDragManager_ProcessDrop(object sender, ProcessDropEventArgs<Character> e)
        {
            ProcessCharacterListDrop(e, monsterListBox);
        }

        void characterListBox_DragOver(object sender, DragEventArgs e)
        {

        }

        void characterListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }
        void characterListBox_Drop(object sender, DragEventArgs e)
        {
            HandleListFileDrop(e.Data, sender == monsterListBox);
        }

        void HandleListFileDrop(IDataObject data, bool isMonsters)
        {

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFilePaths = data.GetData(DataFormats.FileDrop, true) as string[];

                LoadPartyFiles(droppedFilePaths, isMonsters);
            }
        }

        void ProcessCharacterListDrop(ProcessDropEventArgs<Character> e, ListBox targetList)
        {
            bool toMonster = (targetList == monsterListBox);

            using (var undoGroup = undo.CreateUndoGroup())
            {
                int insert = e.NewIndex;

                //get moving item;
                Character chMove = e.DataItem;

                if (chMove.IsMonster == toMonster)
                {
                    if (chMove.InitiativeLeader != null)
                    {
                        chMove = chMove.InitiativeLeader;
                    }
                }

                Character chTarget = null;


                if (insert < targetList.Items.Count)
                {
                    chTarget = (Character)targetList.Items[insert];

                    if (chTarget.InitiativeLeader != null)
                    {
                        chTarget = chTarget.InitiativeLeader;
                    }
                }

                combatState.MoveDroppedCharacter(chMove, chTarget, toMonster);
            }

        }

       


        void monsterView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateEncounterXP();
        }

        void dbView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            e.Cancel = false;
        }

        void monsterView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SetCurrentViewMonster((Character)monsterView.CurrentItem);

                UpdateSmallMonsterFlowDocument();
            }
        }

        void playerView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SetCurrentViewMonster((Character)playerView.CurrentItem);
            }
        }

        void monsterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                if (e.AddedItems.Count > 0)
                {
                    Character ch = null;
                    if (e.AddedItems[0].GetType() == typeof(Character))
                    {
                        ch = (Character)e.AddedItems[0];
                    }
                    else if (e.AddedItems[0].GetType() == typeof(ComboBoxItem))
                    {
                        ch = (Character)((ComboBoxItem)e.AddedItems[0]).DataContext;
                    }
                    if (ch != null)
                    {
                        SetCurrentViewMonster(ch);
                    }
                }
            }
        }

        void SetCurrentViewMonster(Character ch)
        {
            Monster m = null;

            if (ch != null)
            {
                m = ch.Monster;
            }

            if (m != currentViewMonster)
            {


                if (currentViewMonster != null)
                {

                    currentViewMonster.PropertyChanged -= new PropertyChangedEventHandler(monster_PropertyChanged);
                    currentViewMonster.ActiveConditions.CollectionChanged -= new NotifyCollectionChangedEventHandler(ActiveConditions_CollectionChanged);
                }

                currentViewMonster = m;
                currentViewCharacter = ch;

                if (currentViewMonster != null)
                {

                    currentViewMonster.PropertyChanged += new PropertyChangedEventHandler(monster_PropertyChanged);
                    currentViewMonster.ActiveConditions.CollectionChanged += new NotifyCollectionChangedEventHandler(ActiveConditions_CollectionChanged);

                }
                UpdateSmallMonsterFlowDocument();
            }
        }

        void ActiveConditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSmallMonsterFlowDocument();
        }

        void monster_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                if (IsCharacterRefreshProperty(e.PropertyName))
                {
                    UpdateSmallMonsterFlowDocument();
                }
            }
        }

        bool IsCharacterRefreshProperty(string propertyName)
        {
            return propertyName == "Strength" ||
                    propertyName == "Dexterity" ||
                    propertyName == "Constitution" ||
                    propertyName == "Intelligence" ||
                    propertyName == "Wisdom" ||
                    propertyName == "Charisma" ||
                    propertyName == "FullAC" ||
                    propertyName == "TouchAC" ||
                    propertyName == "FlatFootedAC" ||
                    propertyName == "AC_Mods" ||
                    propertyName == "Fort" ||
                    propertyName == "Ref" ||
                    propertyName == "Will" ||
                    propertyName == "Name" ||
                    propertyName == "HP";
        }

        void dbView_CurrentChanged(object sender, EventArgs e)
        {

        }

        void LoadBestiary()
        {
            SortedDictionary<String, String> types = new SortedDictionary<String, String>();
            SortedDictionary<double, String> crs = new SortedDictionary<double, string>();

            Regex regslash = new Regex("/");


            foreach (Monster monster in Monster.Monsters)
            {
                if (monster.Type != null && monster.Type.Length > 0)
                {

                    string type = monster.Type.Trim();

                    type = type[0].ToString().ToUpper() + type.Substring(1);
                    if (!types.ContainsKey(type))
                    {
                        types.Add(type, type);
                    }

                }

                if (monster.CR != null && monster.CR.Length > 0)
                {
                    if (!crs.ContainsValue(monster.CR.Trim()))
                    {

                        Match match = regslash.Match(monster.CR);
                        if (match.Success)
                        {
                            string text = monster.CR.Substring(match.Index + match.Length);

                            double val;
                            if (double.TryParse(text, out val))
                            {
                                crs.Add(1.0 / val, monster.CR.Trim());
                            }

                        }
                        else
                        {
                            double val;
                            if (double.TryParse(monster.CR, out val))
                            {

                                crs.Add(val, monster.CR.Trim());
                            }
                        }
                    }
                }



            }


            foreach (String type in types.Keys)
            {
                ComboBoxItem item = new ComboBoxItem();



                item.Content = type;
                MonsterTypeFilterComboBox.Items.Add(item);
            }

            AddCRsToCombo(MonsterCRComboBox, crs);
            AddCRsToCombo(BetweenCRLowCombo, crs);
            BetweenCRLowCombo.SelectedIndex = 0;
            AddCRsToCombo(BetweenCRHighCombo, crs);
            BetweenCRHighCombo.SelectedIndex = BetweenCRHighCombo.Items.Count - 1;

            AddClassesToCombo(MonsterClassCombo);

            AddEnvironmentsToCombo(MonsterEnvironmentCombo);


            monsterTabView = new ListCollectionView(Monster.Monsters);
            monsterTabView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
            monsterTabView.CollectionChanged += new NotifyCollectionChangedEventHandler(monsterTabView_CollectionChanged);
            monsterTabView.CurrentChanged += new EventHandler(monsterTabView_CurrentChanged);
            monsterTabView.Filter += new Predicate<object>(MonsterViewFilter);
            monsterTabView.MoveCurrentToFirst();

            Condition.LoadConditions();


        }

        private void AddCRsToCombo(ComboBox box, SortedDictionary<double, String> crs)
        {
            foreach (String cr in crs.Values)
            {

                if (cr != "0")
                {
                    ComboBoxItem item = new ComboBoxItem();

                    item.Content = cr;
                    box.Items.Add(item);
                }
            }

        }

        private void AddClassesToCombo(ComboBox box)
        {
            
            box.Items.Add("All Classes");
            List<string> names = new List<string>();
            foreach (CharacterClassEnum e in Enum.GetValues(typeof(CharacterClassEnum)))
            {
                if (e != CharacterClassEnum.None)
                {
                    names.Add(CharacterClass.GetName(e));
                }

            }
            names.Sort();
            foreach (String s in names)
            {
                box.Items.Add(s);
            }
            box.SelectedIndex = 0;
        }

        private void AddEnvironmentsToCombo(ComboBox box)
        {
            box.Items.Add("All Environments");

            HashSet<String> set = new HashSet<string>();

            foreach (Monster m in Monster.Monsters)
            {
                if (m.Environment != null && m.Environment.Trim().Length > 0 && m.Environment != "?")
                {
                    set.Add(m.Environment.ToLower().Capitalize().Trim());
                }
            }

            List<String> list = new List<String>(set);
            list.Sort();

            foreach (String s in list)
            {
                box.Items.Add(s);
            }

            box.SelectedIndex = 0;
        }

        void monsterTabView_CurrentChanged(object sender, EventArgs e)
        {
                UpdateMonsterFlowDocument();
        }

        void monsterTabView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        void UpdateSmallMonsterFlowDocument()
        {

            SmallMonsterFlowDocument.Document.Blocks.Clear();
            if (currentViewMonster != null)
            {
                MonsterBlockCreator creator = new MonsterBlockCreator(SmallMonsterFlowDocument.Document, BlockLinkClicked);
                SmallMonsterFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(currentViewCharacter, false));

            }
        }

        void UpdateCurrentMonsterFlowDocument()
        {

            CurrentMonsterFlowDocument.Document.Blocks.Clear();
            if (combatState != null && combatState.CurrentCharacter != null)
            {
                MonsterBlockCreator creator = new MonsterBlockCreator(CurrentMonsterFlowDocument.Document, BlockLinkClicked);
                CurrentMonsterFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(combatState.CurrentCharacter, false));

            }
        }

        void UpdateMonsterFlowDocument()
        {
            try
            {
                if (monsterTabView != null && mainWindowLoaded)
                {
                    currentDBMonster = ((Monster)monsterTabView.CurrentItem);

                    MonsterFlowDocument.Document.Blocks.Clear();
                    if (currentDBMonster != null)
                    {
                        currentDBMonster.ParseSpellLikeAbilities();
                        currentDBMonster.ParseSpellsPrepared();
                        currentDBMonster.ParseSpellsKnown();
                        currentDBMonster = (Monster)currentDBMonster.Clone();

                        if (this.MonsterAdvancerExpander.IsExpanded)
                        {
                            AdvanceMonsterFromExpander(currentDBMonster);
                        }

                        MonsterBlockCreator creator = new MonsterBlockCreator(MonsterFlowDocument.Document, BlockLinkClicked);
                        MonsterFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(currentDBMonster, true));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        void BlockLinkClicked(object sender, DocumentLinkEventArgs e)
        {
            if (e.Type ==  "Feat")
            {
                ShowFeat(e.Name);

           
            }
            else if (e.Type == "School")
            {
                RulesTabFilterBox.Text = e.Name;

                RuleTypeFilterComboBox.SelectedIndex = 0;



                Rule rule = Rule.Rules.FirstOrDefault(a => String.Compare(a.Name, e.Name, true) == 0);

                if (rule != null)
                {
                    rulesView.MoveCurrentTo(rule);
                }

                MainTabControl.SelectedItem = RulesTab;
            }
            else if (e.Type == "Spell")
            {
                ShowSpell(e.Name);
            }
            else if (e.Type == "MagicItem")
            {
                ShowMagicItem(e.Name);
            }


        }

        void ShowSpell(string name)
        {
            spellFilterBox.Text = name;

            ResetSpellFilters();

            MainTabControl.SelectedItem = SpellsTab;

        }



        void ShowMagicItem(string name)
        {
            MagicItemsTabFilterBox.Text = name;

            ResetMagicItemFilters();

            MainTabControl.SelectedItem = TreasureTab;
            TreasureTabControl.SelectedItem = MagicItemsTab;

        }

        void ShowFeat(string feat)
        {
            Regex regValue = new Regex("(?<name>.+?) \\(.+?\\)");

            Match m = regValue.Match(feat);

            if (m.Success)
            {
                feat = m.Groups["name"].Value;
            }

            Regex regGreater = new Regex("(?<type>(Greater|Improved)) (?<feat>.+)", RegexOptions.IgnoreCase);
            m = regGreater.Match(feat);
            if (m.Success)
            {
                string switchName = m.Groups["feat"].Value + ", " + m.Groups["type"].Value;

                if (Feat.FeatMap.ContainsKey(switchName))
                {
                    feat = switchName;
                }
            }



            featFilterBox.Text = feat;
            FeatTypeFilterComboBox.SelectedIndex = 0;

            if (Feat.FeatMap.ContainsKey(feat))
            {
                featsView.MoveCurrentTo(Feat.FeatMap[feat]);
            }

            MainTabControl.SelectedItem = FeatsTab;
        }

        void ResetSpellFilters()
        {
            ClassFilterComboBox.SelectedIndex = 0;
            SpellLevelFilterComboBox.SelectedIndex = 0;
            SpellSchoolFilterComboBox.SelectedIndex = 0;
            SpellSubschoolFilterComboBox.SelectedIndex = 0;
            SpellDescriptorFilterComboBox.SelectedIndex = 0;
            CustomSpellFilterComboBox.SelectedIndex = 0;
        }

        void AdvanceMonsterFromExpander(Monster monster)
        {
            if (RacialHDCheck.IsChecked == true)
            {
                int dice = (RacialHDNumberCombo.SelectedIndex + 1) *
                    (RacialHDTypeCombo.SelectedIndex == 0 ? 1 : -1);

                Stat stat = (Stat)RacialHDStatCombo.SelectedIndex;

                bool size = RacialHDSizeChangeCombo.SelectedItem == RacialHDSizeChange50Item;

                int res = monster.AddRacialHD(dice, stat, size);

                if (res != 0)
                {
                    monster.Name = monster.Name + " " + CMStringUtilities.PlusFormatNumber(res) + " HD";
                }
            }
            if (OtherTemplateTabControl.SelectedItem == HalfDragonTab)
            {
                if (monster.MakeHalfDragon((string)((ComboBoxItem)HalfDragonColorCombo.SelectedValue).Content))
                {
                    monster.Name = "Half-Dragon " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == HalfFiendTab)
            {
                HashSet<Stat> bonusStats = new HashSet<Stat>();

                if (HalfFiendStrengthCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Strength);
                }
                if (HalfFiendDexterityCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Dexterity);
                }
                if (HalfFiendConstitutionCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Constitution);
                }
                if (HalfFiendIntelligenceCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Intelligence);
                }
                if (HalfFiendWisdomCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Wisdom);
                }
                if (HalfFiendCharismaCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Charisma);
                }

                if (monster.MakeHalfFiend(bonusStats))
                {
                    monster.Name = "Half-Fiend " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == HalfCelestialTab)
            {
                HashSet<Stat> bonusStats = new HashSet<Stat>();

                if (HalfCelestialStrengthCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Strength);
                }
                if (HalfCelestialDexterityCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Dexterity);
                }
                if (HalfCelestialConstitutionCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Constitution);
                }
                if (HalfCelestialIntelligenceCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Intelligence);
                }
                if (HalfCelestialWisdomCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Wisdom);
                }
                if (HalfCelestialCharismaCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Charisma);
                }

                if (monster.MakeHalfCelestial(bonusStats))
                {
                    monster.Name = "Half-Celestial " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == OgrekinTab)
            {
                if (monster.MakeOgrekin(OgrekinBeneficialCombo.SelectedIndex + 1, OgrekinDisadvantageousCommbo.SelectedIndex + 1))
                {
                    monster.Name = "Ogrekin " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == SkeletonTab)
            {
                bool bloody = (BloodySkeletonCheckBox.IsChecked == true);
                bool burning = (BurningSkeletonCheckBox.IsChecked == true);
                bool champion = (SkeletalChampionCheckBox.IsChecked == true);

                if (monster.MakeSkeleton(bloody, burning, champion))
                {

                    if (champion)
                    {
                        monster.Name = "Skeletal Champion " + monster.Name;
                    }
                    else
                    {
                        monster.Name += " Skeleton";
                    }


                    if (burning)
                    {
                        monster.Name = "Burning " + monster.Name;
                    }

                    if (bloody)
                    {
                        monster.Name = "Bloody " + monster.Name;
                    }

                }
            }
            if (OtherTemplateTabControl.SelectedItem == VampireTab)
            {
                if (monster.MakeVampire())
                {
                    monster.Name = "Vampire " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == ZombieTab)
            {
                Monster.ZombieType zt = (Monster.ZombieType)ZombieTypeComboBox.SelectedIndex;

                if (monster.MakeZombie(zt))
                {

                    monster.Name = "Zombie " + monster.Name;

                    if (zt == Monster.ZombieType.Fast)
                    {
                        monster.Name = "Fast " + monster.Name;
                    }
                    else if (zt == Monster.ZombieType.Plague)
                    {
                        monster.Name = "Plague " + monster.Name;
                    }
                    else if (zt == Monster.ZombieType.Juju)
                    {
                        monster.Name = "Juju " + monster.Name;
                    }
                    else if (zt == Monster.ZombieType.Void)
                    {
                        monster.Name = "Void " + monster.Name;
                    }

                }
            }

            if (AdvancedMonsterCheck.IsChecked == true)
            {

                int count = 1 + this.AdvancedMonsterMultiplierCombo.SelectedIndex;

                int added = 0;

                for (int i = 0; i < count; i++)
                {
                    if (monster.MakeAdvanced())
                    {
                        added++;
                    }
                }

                if (added > 0)
                {
                    monster.Name += " (Adv " + added.PlusFormat() + ")";
                }
            }
            if (SizeAdvancedMonsterCheck.IsChecked == true)
            {
                int count = 1 + SizeAdvancedMultiplierCombo.SelectedIndex;

                if (SizeAdvancedMonsterCombo.SelectedIndex == 0)
                {
                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeGiant())
                        {
                            added++;
                        }

                    }
                    if (added == 1)
                    {
                        monster.Name = "Giant " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Giant x" + added + " " + monster.Name;
                    }
                }
                else
                {
                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeYoung())
                        {
                            added++;
                        }
                    }
                    if (added == 1)
                    {
                        monster.Name = "Young " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Young x" + added + " " + monster.Name;
                    }
                }
            }
            if (this.SummonAdvancedMonsterCheck.IsChecked == true)
            {
                if (SummonAdvancedMonsterCombo.SelectedIndex == 0)
                {
                    if (monster.MakeCelestial())
                    {
                        monster.Name = "Celestial " + monster.Name;
                    }
                }
                else if (SummonAdvancedMonsterCombo.SelectedIndex == 1)
                {
                    if (monster.MakeEntropic())
                    {
                        monster.Name = "Entopic " + monster.Name;
                    }
                }
                else if (SummonAdvancedMonsterCombo.SelectedIndex == 2)
                {
                    if (monster.MakeFiendish())
                    {
                        monster.Name = "Fiendish " + monster.Name;
                    }
                }
                else if (SummonAdvancedMonsterCombo.SelectedIndex == 3)
                {
                    if (monster.MakeResolute())
                    {
                        monster.Name = "Resolute " + monster.Name;
                    }
                }
            }
            if (this.AugmentSummoningCheck.IsChecked == true)
            {
                monster.AugmentSummoning();
                monster.Name = "Augmented " + monster.Name;
            }
        }



        void LoadSpells()
        {

            spellsView = new ListCollectionView(Spell.Spells);

            foreach (string school in Spell.Schools)
            {
                SpellSchoolFilterComboBox.Items.Add(StringCapitalizeConverter.Capitalize(school));
            }

            foreach (string subschool in Spell.Subschools)
            {
                SpellSubschoolFilterComboBox.Items.Add(subschool);
            }

            foreach (string descriptor in Spell.Descriptors)
            {
                SpellDescriptorFilterComboBox.Items.Add(descriptor);
            }


            spellsView.SortDescriptions.Add(
                new SortDescription("nameforsort", ListSortDirection.Ascending));
            spellsView.CollectionChanged += new NotifyCollectionChangedEventHandler(spellsView_CollectionChanged);
            spellsView.CurrentChanged += new EventHandler(spellsView_CurrentChanged);
            spellsView.Filter += new Predicate<object>(SpellsViewFilter);
            spellsView.MoveCurrentToFirst();

        }

        void spellsView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateSpellFlowDocument();
            }
        }

        void spellsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        void UpdateSpellFlowDocument()
        {
            Spell spell = (Spell)spellsView.CurrentItem;
            SpellFlowDocument.Document.Blocks.Clear();
            if (spell != null)
            {
                SpellBlockCreator creator = new SpellBlockCreator(SpellFlowDocument.Document, BlockLinkClicked);
                SpellFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(spell));
            }


        }

        void LoadFeats()
        {

            Feat.LoadFeats();







            foreach (string type in Feat.FeatTypes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = type;
                FeatTypeFilterComboBox.Items.Add(item);

            }



            featsView = new ListCollectionView(Feat.Feats);

            featsView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
            featsView.CollectionChanged += new NotifyCollectionChangedEventHandler(featsView_CollectionChanged);
            featsView.CurrentChanged += new EventHandler(featsView_CurrentChanged);
            featsView.Filter += new Predicate<object>(FeatViewFilter);


            featsView.MoveCurrentToFirst();


            FeatsTab.DataContext = featsView;

        }

        void LoadRules()
        {
            rulesView = new ListCollectionView(Rule.Rules);

            rulesView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
            rulesView.CollectionChanged += new NotifyCollectionChangedEventHandler(rulesView_CollectionChanged);
            rulesView.CurrentChanged += new EventHandler(rulesView_CurrentChanged);
            rulesView.Filter += new Predicate<object>(RuleViewFilter);

            foreach (string type in Rule.Types)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = type;
                RuleTypeFilterComboBox.Items.Add(item);

            }


            rulesView.MoveCurrentToFirst();


            RulesTab.DataContext = rulesView;
            UpdateRuleSubtypeComboBoxItems();

        }

        void LoadMagicItems()
        {



            magicItemsView = new ListCollectionView(new List<MagicItem>(MagicItem.Items.Values));

            magicItemsView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
            magicItemsView.CollectionChanged += new NotifyCollectionChangedEventHandler(magicItemsView_CollectionChanged);
            magicItemsView.CurrentChanged += new EventHandler(magicItemsView_CurrentChanged);
            magicItemsView.Filter += new Predicate<object>(MagicItemViewFilter);

            foreach (string type in MagicItem.Groups)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = type;
                MagicItemGroupFilterComboBox.Items.Add(item);

            }

            foreach (int cl in MagicItem.CLs)
            {
                ComboBoxItem item = new ComboBoxItem();
                if (cl == -1)
                {
                    item.Content = "Varies";
                }
                else
                {
                    item.Content = "CL " + BlockCreator.PastTenseNumber(cl);
                }
                item.DataContext = cl;
                MagicItemCLFilterComboBox.Items.Add(item);
            }


            magicItemsView.MoveCurrentToFirst();


            TreasureTab.DataContext = magicItemsView;

        }

        void rulesView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        void rulesView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateRuleFlowDocument();
            }
        }

        void magicItemsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        void magicItemsView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateMagicItemsFlowDocument();
            }
        }

        void featsView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateFeatFlowDocument();
            }
        }

        void featsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        void UpdateFeatFlowDocument()
        {

            Feat feat = (Feat)featsView.CurrentItem;
            FeatFlowDocument.Document.Blocks.Clear();
            if (feat != null)
            {
                FeatBlockCreator creator = new FeatBlockCreator(FeatFlowDocument.Document);
                FeatFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(feat));
            }


        }

        void UpdateRuleFlowDocument()
        {

            Rule rule = (Rule)rulesView.CurrentItem;

            if (rule != lastViewRule)
            {
                lastViewRule = rule;
                RuleFlowDocument.Document.Blocks.Clear();
                if (rule != null)
                {
                    RuleBlockCreator creator = new RuleBlockCreator(RuleFlowDocument.Document);
                    RuleFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(rule));
                }
            }


        }

        void UpdateMagicItemsFlowDocument()
        {

            MagicItem item = (MagicItem)magicItemsView.CurrentItem;
            MagicItemFlowDocument.Document.Blocks.Clear();
            if (item != null)
            {
                MagicItemBlockCreator creator = new MagicItemBlockCreator(MagicItemFlowDocument.Document);
                MagicItemFlowDocument.Document.Blocks.AddRange(creator.CreateBlocks(item));
            }


        }

        /*void currentPlayerView_CurrentChanged(object sender, EventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character current = (Character)currentPlayerView.CurrentItem;
                if (lastCurrent != null && lastCurrent != current)
                {
                    lastCurrent.IsActive = false;
                }

                lastCurrent = current;
                if (current != null)
                {
                    current.IsActive = true;
                }
                combatState.CurrentCharacter = current;
            }
        }*/

        /*void currentPlayerView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (monsterView.CurrentItem != null)
                {
                    Monster monster = (Monster)((Character)monsterView.CurrentItem).Monster;

                    if (monster != null)
                    {

                        monster.PropertyChanged -= new PropertyChangedEventHandler(monster_PropertyChanged);
                    }
                }
            }
        }*/

        void CombatList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }


        void monsterFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshMonsterDBView();

        }

        void RefreshMonsterDBView()
        {
            if (dbView != null)
            {

                dbView.Refresh();

                if (dbView.CurrentItem == null)
                {
                    dbView.MoveCurrentToFirst();
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                ListBox box = (ListBox)sender;


                ListBoxItem target = CMUIUtilities.ClickedListBoxItem(box, e);

                if (target != null)
                {

                    AddCurrentDBViewMonster();
                }
            }
        }


        private void AddCurrentDBViewMonster()
        {

            Monster monster = (Monster)dbView.CurrentItem;

            if (monster != null)
            {
                combatState.AddMonster(monster, RollHPCheck.IsChecked == true, true, UserSettings.Settings.AddMonstersHidden);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var group = undo.CreateUndoGroup())
            {
                combatState.AddBlank(false);
            }

            SaveCombatState();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var group = undo.CreateUndoGroup())
            {
                combatState.AddBlank(true, UserSettings.Settings.AddMonstersHidden);
            }

            SaveCombatState();
        }

        private void AddCharacter(Character character)
        {
            combatState.AddCharacter(character);
        }




        private void RemoveCharacter(Character character)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.RemoveCharacter(character);
            }
        }

        private void RemoveAllCharacters(ICollectionView view)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> removeList = new List<Character>();

                foreach (Character character in view)
                {
                    removeList.Add(character);
                }
                foreach (Character character in removeList)
                {
                    RemoveCharacter(character);
                }
            }
        }

        public void RollInitiative()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.RollInitiative();
                

            }
        }

        public void RollInitiativeWithoutReset()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.RollInitiative(false);


            }
        }

        public void MoveUpCharacter(Character ch)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.MoveUpCharacter(ch);


            }
        }

        public void MoveDownCharacter(Character ch)
        {

            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.MoveDownCharacter(ch);


            }
        }

        public void MoveCharacterAfter(Character a, Character b)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.MoveCharacterAfter(a, b);


            }
        }


        private int InitDieRoll()
        {
            if (UserSettings.Settings.AlternateInit3d6)
            {
                return UserSettings.Settings.AlternateInitDieRoll.Roll().Total;
            }
            else
            {
                return rand.Next(1, 21);
            }
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MoveNextPlayer();

                combatListBox.ScrollIntoView(combatState.CurrentCharacter);
            }

            SaveCombatState();
        }

        public void MoveNextPlayer()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.MoveNext();

            }
        }

 

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MovePreviousPlayer();


                combatListBox.ScrollIntoView(combatState.CurrentCharacter);
            }

            SaveCombatState();
        }

        public void MovePreviousPlayer()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.MovePrevious();

            

            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSettings.Settings.ConfirmCharacterDelete || ShowConfirmationBox("Do you want to delete this character?", "Delete Character"))
            {
                using (var undoGroup = undo.CreateUndoGroup())
                {

                    Button button = (Button)sender;
                    Character character = (Character)button.DataContext;
                    RemoveCharacter(character);
                }
            }
        }

        private bool ShowConfirmationBox(string message, string caption)
        {
            return MessageBox.Show(this, message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;

        }

        private bool DBViewFilter(object ob)
        {

            try
            {
                Monster monster = (Monster)ob;


                bool res = (monster != null && DBViewTextFilter(monster) && DBViewNPCFilter(monster) && SourceFilter(monster.Source));

                return res;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                throw;
            }

        }

        private bool DBViewTextFilter(Monster monster)
        {

            if (this.monsterFilterBox.Text == null)
            {
                return true;
            }

            if (this.monsterFilterBox.Text.Trim().Length == 0)
            {
                return true;
            }

            return monster.Name.ToUpper().
                Contains(monsterFilterBox.Text.Trim().ToUpper());
        }

        private bool DBViewNPCFilter(Monster monster)
        {
            if (CombatTabNPCFilterBox.SelectedItem == CombatNPCFilterAllItem)
            {
                return true;
            }

             bool custom = monster.DBLoaderID != 0;

             if (CombatTabNPCFilterBox.SelectedItem == CombatNPCFilterCustomItem)
             {
                 return custom;
             }
             else
             {

                 bool useNPC = (CombatTabNPCFilterBox.SelectedItem == CombatNPCFilterNPCItem);

                 return (useNPC == monster.NPC) && !custom;
             }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                monsterFilterBox.Text = "";
            }
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character character = (Character)combatView.CurrentItem;

                MoveUpCharacter(character);

                combatView.MoveCurrentTo(character);
            }

        }



        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character character = (Character)combatView.CurrentItem;

                MoveDownCharacter(character);


                combatView.MoveCurrentTo(character);
            }
        }





        private void endCombatButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.CombatList.Clear();
            }
        }

        private void Initiative_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSettings.Settings.ConfirmInitiativeRoll || ShowConfirmationBox("Do you want to roll initiative?", "Roll Initiative"))
            {
                using (var undoGroup = undo.CreateUndoGroup())
                {

                    RollInitiative();


                    combatState.SortCombatList();
                }

                SaveCombatState();
            }
        }

 

        private void SaveParty()
        {

            SaveCombatState();
            SaveGroup(playerView, savePartyFileFilter);

        }

        private void LoadParty()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                LoadGroup(playerView, loadAllFilesFilter + "|" + loadPartyFileFilter + "|" + herolabFileFilter + "|" + pcGenExportFileFilter);

                
            }

            SaveCombatState();
        }

        private void SaveEncounter()
        {

            SaveCombatState();
            SaveGroup(monsterView, savePartyFileFilter);

        }

        private void LoadEncounter()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                LoadGroup(monsterView, loadAllFilesFilter + "|" + loadPartyFileFilter + "|" + herolabFileFilter + "|" + pcGenExportFileFilter);
            }

            SaveCombatState();
        }

        private void SaveGroup(ICollectionView view, string filter)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = filter;

            if (dlg.ShowDialog() == true)
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(List<Character>));
                try
                {
                    TextWriter writer = new StreamWriter(dlg.FileName);

                    List<Character> list = new List<Character>();

                    foreach (Character character in view)
                    {
                        list.Add(character);
                    }

                    serializer.Serialize(writer, list);
                    writer.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("Failed to Save file " + dlg.FileName);
                }
            }
        }

        private void LoadGroup(ICollectionView view, string filter)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = filter;
                dlg.Multiselect = true;

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    LoadPartyFiles(dlg.FileNames, view==monsterView);
                }

            }
        }

        private void LoadPartyFiles(string[] files, bool isMonster)
        {
            // Open document
            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                if (String.Compare(fi.Extension, ".por", true) == 0 || String.Compare(fi.Extension, ".rpgrp", true) == 0)
                {
                    ImportFromFile(filename, isMonster ? monsterView : playerView);
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
                        if (combatState.GetCharacterByID(character.ID) != null)
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
                    combatState.FixInitiativeLinksList(list);

                    fs.Close();
                }
            }
        }




        private void SavePartyButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SaveParty();
            }
        }

        private void LoadPartyButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                LoadParty();
            }
        }

        private void SaveEncounterButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SaveEncounter();
            }
        }

        private void LoadEncounterButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                LoadEncounter();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Close();
            }
        }

        private void MainRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                DragMove();
            }
        }

        private void MainRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MainRectangle_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;

                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        private bool SpellsViewFilter(object ob)
        {
            bool result = false;


            Spell spell = (Spell)ob;

            if (this.spellFilterBox.Text == null)
            {
                result = true;
            }

            if (this.spellFilterBox.Text.Trim().Length == 0)
            {
                result = true;
            }

            if (result != true)
            {

                result = spell.name.ToUpper().
                    Contains(spellFilterBox.Text.Trim().ToUpper());
            }

            return result && SpellsClassViewFilter(ob) && SourceFilter(((Spell)ob).source)
                && SpellSchoolFilter(ob) && SpellCustomFilter((Spell)ob) && SpellSubschoolFilter(ob) && SpellDescriptorFilter(ob);
        }

        private bool SpellsClassViewFilter(object ob)
        {

            Spell spell = (Spell)ob;

            int? spellLevel = null;

            if (SpellLevelFilterComboBox.SelectedIndex != 0)
            {
                spellLevel = SpellLevelFilterComboBox.SelectedIndex - 1;
            }


            if (ClassFilterComboBox.SelectedItem == AllItem)
            {
                if (spellLevel == null)
                {
                    return true;
                }
                else
                {
                    string[] classSpellLevels = new string[]
                    {
                        spell.alchemist,
                        spell.antipaladin,
                        spell.bard,
                        spell.cleric,
                        spell.druid,
                        spell.inquisitor,
                        spell.oracle,
                        spell.paladin,
                        spell.ranger,
                        spell.sor,
                        spell.summoner ,
                        spell.witch
                    };

                    foreach (string s in classSpellLevels)
                    {
                        if (CheckSpellLevel(s, spellLevel))
                        {
                            return true;
                        }
                    }
                    return false;
                }

            }
            else if (ClassFilterComboBox.SelectedItem == AlchemistItem)
            {
                return CheckSpellLevel(spell.alchemist, spellLevel);

            }
            else if (ClassFilterComboBox.SelectedItem == AntipaladinItem)
            {
                return CheckSpellLevel(spell.antipaladin, spellLevel) || CheckSpellLevel(spell.paladin, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == BardItem)
            {

                return CheckSpellLevel(spell.bard, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == ClericItem)
            {

                return CheckSpellLevel(spell.cleric, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == DruidItem)
            {
                return CheckSpellLevel(spell.druid, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == InquisitorItem)
            {
                return CheckSpellLevel(spell.inquisitor, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == MagusItem)
            {
                return CheckSpellLevel(spell.magus, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == OracleItem)
            {
                return CheckSpellLevel(spell.oracle, spellLevel) || CheckSpellLevel(spell.cleric, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == PaladinItem)
            {
                return CheckSpellLevel(spell.paladin, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == RangerItem)
            {
                return CheckSpellLevel(spell.ranger, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == SorcererItem)
            {

                return CheckSpellLevel(spell.sor, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == SummonerItem)
            {
                return CheckSpellLevel(spell.summoner, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == WitchItem)
            {
                return CheckSpellLevel(spell.witch, spellLevel);
            }
            else if (ClassFilterComboBox.SelectedItem == WizardItem)
            {
                return CheckSpellLevel(spell.wiz, spellLevel);
            }

            return true;
        }

        private bool SpellSchoolFilter(object ob)
        {
            Spell sp = (Spell)ob;

            if (SpellSchoolFilterComboBox.SelectedIndex == 0)
            {
                return true;
            }

            else
            {
                string text = (string)SpellSchoolFilterComboBox.SelectedItem;

                return String.Compare(text, sp.school, true) == 0;
            }
        }


        private bool SpellSubschoolFilter(object ob)
        {
            Spell sp = (Spell)ob;

            if (SpellSubschoolFilterComboBox.SelectedIndex == 0)
            {
                return true;
            }

            else
            {
                string text = (string)SpellSubschoolFilterComboBox.SelectedItem;

                if (!sp.subschool.NotNullString())
                {
                    return false;
                }
                else
                {
                    return sp.subschool.Contains(text);
                }
            }
        }
        private bool SpellDescriptorFilter(object ob)
        {
            Spell sp = (Spell)ob;

            if (SpellDescriptorFilterComboBox.SelectedIndex == 0)
            {
                return true;
            }

            else
            {
                string text = (string)SpellDescriptorFilterComboBox.SelectedItem;

                if (!sp.descriptor.NotNullString())
                {
                    return false;
                }
                else
                {
                    return sp.descriptor.Contains(text);
                }
            }
        }

        private bool CheckSpellLevel(string spellLevel, int? targetLevel)
        {
            if (spellLevel == null || spellLevel.Length == 0)
            {
                return false;
            }
            else if (targetLevel == null)
            {
                return true;
            }
            else
            {
                int val;
                if (int.TryParse(spellLevel, out val))
                {
                    if (val == targetLevel)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool SpellCustomFilter(Spell sp)
        {
            return CustomSpellFilterComboBox.SelectedIndex == 0 ? true : sp.IsCustom;
        }

        private bool SourceFilter(string source)
        {
            SourceType st = SourceInfo.GetSourceType(source);

            switch (st)
            {
                case SourceType.Core:
                    return UseCoreContentCheck.IsChecked == true;

                case SourceType.APG:
                    return UseAPGContentCheck.IsChecked == true;
                case SourceType.AdventuresAndModules:
                    return UseModulesContentCheck.IsChecked == true;
                case SourceType.ChroniclesAndCompanions:
                    return UseChroniclesContentCheck.IsChecked == true;
                case SourceType.Other:
                    return UseOtherContentCheck.IsChecked == true;
                case SourceType.UltimateMagic:
                    return UseUltimateMagicContentCheck.IsChecked == true;
                case SourceType.UltimateCombat:
                    return UseUltimateCombatContentCheck.IsChecked == true;
            }


            return false;


        }

        private void spellFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSpellsView();
        }

        private void ClearSpellText_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                spellFilterBox.Clear();
            }
        }

        private void ClassFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshSpellsView();
            }
        }

        private void ClearFeatText_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                featFilterBox.Clear();
            }
        }

        private void featFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshFeatsView();
            }
        }

        private bool FeatViewFilter(object ob)
        {

            Feat feat = (Feat)ob;


            return FeatTextFilter(feat) && FeatTypeFilter(feat) && SourceFilter(feat.Source) && FeatCustomFilter(feat) ;
        }

        private bool FeatTextFilter(Feat feat)
        {
			string filterText = this.featFilterBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(filterText))
            {
                return true;
            }

			return feat.Name.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0 || (feat.AltName != null && feat.AltName.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        private bool FeatTypeFilter(Feat feat)
        {
            if (FeatTypeFilterComboBox.SelectedItem == AllFeatsItem || FeatTypeFilterComboBox.SelectedItem == null)
            {
                return true;
            }
            else
            {

                ComboBoxItem item = (ComboBoxItem)FeatTypeFilterComboBox.SelectedItem;


                return feat.Types.Contains((string)item.Content);
            }

        }

        private bool FeatCustomFilter(Feat feat)
        {
            if (FeatCustomFilterComboBox.SelectedItem == AllCustomFeatsItem)
            {
                return true;
            }
            else
            {

                return feat.IsCustom;
            }

        }

        private bool RuleViewFilter(object ob)
        {

            Rule rule = (Rule)ob;


            return RuleTextFilter(rule) && RuleTypeFilter(rule) && RuleSubtypeFilter(rule) && SourceFilter(rule.Source);
        }

        private bool RuleTextFilter(Rule rule)
        {
            bool result = false;

            if (this.RulesTabFilterBox.Text == null)
            {
                result = true;
            }

            if (this.RulesTabFilterBox.Text.Trim().Length == 0)
            {
                result = true;
            }

            if (result != true)
            {

                result = rule.Name.ToUpper().
                    Contains(RulesTabFilterBox.Text.Trim().ToUpper());
            }

            return result;
        }

        private bool RuleTypeFilter(Rule rule)
        {
            if (RuleTypeFilterComboBox.SelectedItem == AllRulesItem || RuleTypeFilterComboBox.SelectedItem == null)
            {
                return true;
            }
            else
            {

                ComboBoxItem item = (ComboBoxItem)RuleTypeFilterComboBox.SelectedItem;


                return String.Compare(rule.Type, (string)item.Content, true) == 0;
            }

        }

        private bool RuleSubtypeFilter(Rule rule)
        {
            if (RuleSubtypeFilterComboBox.SelectedIndex == 0 || RuleSubtypeFilterComboBox.SelectedItem == null || rule.Subtype == null)
            {
                return true;
            }
            else
            {

                string text = (string)RuleSubtypeFilterComboBox.SelectedItem;


                return String.Compare(rule.Subtype, text, true) == 0;
            }
        }

        private bool MagicItemViewFilter(object ob)
        {

            MagicItem item = (MagicItem)ob;


            return MagicItemTextFilter(item) && MagicItemTypeFilter(item) && MagicItemCLFilter(item) && SourceFilter(item.Source);
        }

        private bool MagicItemTextFilter(MagicItem item)
        {
            bool result = false;

            if (this.MagicItemsTabFilterBox.Text == null)
            {
                result = true;
            }

            if (this.MagicItemsTabFilterBox.Text.Trim().Length == 0)
            {
                result = true;
            }

            if (result != true)
            {

                result = item.Name.ToUpper().
                    Contains(MagicItemsTabFilterBox.Text.Trim().ToUpper());
            }

            return result;
        }

        private bool MagicItemTypeFilter(MagicItem item)
        {
            if (MagicItemGroupFilterComboBox.SelectedItem == AllMagicItemGroupsItem || MagicItemGroupFilterComboBox.SelectedItem == null)
            {
                return true;
            }
            else
            {

                ComboBoxItem boxItem = (ComboBoxItem)MagicItemGroupFilterComboBox.SelectedItem;


                return String.Compare(item.Group, (string)boxItem.Content, true) == 0;
            }

        }

        private bool MagicItemCLFilter(MagicItem item)
        {
            if (MagicItemCLFilterComboBox.SelectedItem == AllMagicItemCLItem || MagicItemCLFilterComboBox.SelectedItem == null)
            {
                return true;
            }

            ComboBoxItem cbi = (ComboBoxItem)MagicItemCLFilterComboBox.SelectedItem;

            return ((int)cbi.DataContext) == item.CL;
        }

        private void ClearMonsterText_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MonsterTabFilterBox.Clear();
            }
        }

        private void MonsterCRFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveMonsterTabFilter();

            Visibility crVis = (MonsterCRComboBox.SelectedItem == BetweenCRsItem) ? Visibility.Visible : Visibility.Hidden;

            if (BetweenCRHighCombo != null)
            {
                BetweenCRHighCombo.Visibility = crVis;
            }
            if (BetweenCRLowCombo != null)
            {
                BetweenCRLowCombo.Visibility = crVis;
            }


            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshMonsterTabView();
            }
        }
		
		
        private void ClassCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveMonsterTabFilter();
        	
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshMonsterTabView();
            }
        }


        private void MonsterEnvironmentCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveMonsterTabFilter();

            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshMonsterTabView();
            }
        }

        private void MonsterTabFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshMonsterTabView();
            }
        }

        private bool MonsterViewFilter(object ob)
        {

            Monster monster = (Monster)ob;


            return MonsterTextFilter(monster) && MonsterCRFilter(monster) && MonsterTypeFilter(monster)
                && MonsterNPCFilter(monster) && SourceFilter(monster.Source) && MonsterClassFilter(monster)
                && MonsterEnvironmentFilter(monster);
        }

        private bool MonsterTextFilter(Monster monster)
        {
            bool result = false;

            if (this.MonsterTabFilterBox.Text == null)
            {
                result = true;
            }

            if (this.MonsterTabFilterBox.Text.Trim().Length == 0)
            {
                result = true;
            }

            if (result != true)
            {

                result = monster.Name.ToUpper().
                    Contains(MonsterTabFilterBox.Text.Trim().ToUpper());
            }
            return result;
        }

        private bool MonsterCRFilter(Monster monster)
        {
            if (MonsterCRComboBox.SelectedItem == AllMonsterCRsItem ||
                MonsterCRComboBox.SelectedItem == null)
            {
                return true;
            }
            else if (MonsterCRComboBox.SelectedItem == BetweenCRsItem)
            {
                if (monster.CR != null && monster.CR.Length > 0)
                {

                    ComboBoxItem item1 = (ComboBoxItem)BetweenCRLowCombo.SelectedItem;
                    ComboBoxItem item2 = (ComboBoxItem)BetweenCRHighCombo.SelectedItem;

                    long val1 = Monster.GetCRValue((string)item1.Content);
                    long val2 = Monster.GetCRValue((string)item2.Content);

                    long monsterVal = Monster.GetCRValue(monster.CR);

                    if ((monsterVal >= val1 && monsterVal <= val2)
                        || (monsterVal >= val2 && monsterVal <= val1))
                    {
                        return true;
                    }

                }
                return false;


            }
            else
            {
                ComboBoxItem item = (ComboBoxItem)MonsterCRComboBox.SelectedItem;


                return (String.Compare(monster.CR.Trim(),
                   (string)item.Content, true) == 0);
            }
        }

        private bool MonsterTypeFilter(Monster monster)
        {
            if (MonsterTypeFilterComboBox.SelectedItem == AllMonsterTypesItem ||
                MonsterTypeFilterComboBox.SelectedItem == null)
            {
                return true;
            }
            else
            {
                ComboBoxItem item = (ComboBoxItem)MonsterTypeFilterComboBox.SelectedItem;


                return (String.Compare(monster.Type.Trim(),
                   (string)item.Content, true) == 0);
            }

        }

        private bool MonsterNPCFilter(Monster monster)
        {
            if (MonsterTabNPCComboBox.SelectedItem == NPCFilterAllComboItem)
            {
                return true;
            }
            else
            {
                bool custom = monster.DBLoaderID != 0;

                if (MonsterTabNPCComboBox.SelectedItem == NPCFilterCustomComboItem)
                {
                    return custom;
                }
                else
                {

                    bool useNPC = MonsterTabNPCComboBox.SelectedItem == NPCFilterNPCComboItem;

                    return (monster.NPC == useNPC) && !custom;
                }
            }
        }

        private bool MonsterClassFilter(Monster monster)
        {
            if (MonsterClassCombo.SelectedIndex == 0)
            {
                return true;
            }

            if (monster.Class == null)
            {
                return false;
            }

            return (monster.Class.ToUpper().Contains(((string)MonsterClassCombo.SelectedValue).ToUpper()));

        }
        private bool MonsterEnvironmentFilter(Monster monster)
        {
            if (MonsterEnvironmentCombo.SelectedIndex == 0)
            {
                return true;
            }

            if (monster.Environment == null)
            {
                return false;
            }

            string environment = ((string)MonsterEnvironmentCombo.SelectedValue).ToUpper();
            if (environment.Length > 3 && environment.Substring(0, 4) == "ANY ")
            {
                environment = environment.Replace("ANY ", "");
            }
            
            return (monster.Environment.ToUpper().Contains(environment) && !monster.Environment.ToUpper().Contains("NON-" + environment));

        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TitleBarButton_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;

                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        private void AddToCombatButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Monster monster = currentDBMonster;

                combatState.AddMonster(monster,  RollHPCheck.IsChecked == true, true, UserSettings.Settings.AddMonstersHidden);
            }
        }

        private void MonsterTypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshMonsterTabView();
            }
        }

        private void NPCCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshMonsterTabView();
        }

        private void FeatTypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (featsView != null)
                {
                    featsView.Refresh();
                }
            }
        }

        private UndoHelper.UndoGroupHelper hpThumbUndo = null;

        private void HPThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            hpThumbUndo = undo.CreateUndoGroup();

            Thumb thumb = (Thumb)sender;
            Character character = (Character)thumb.DataContext;
            initialHP = character.HP;
        }

        private void HPThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            Character character = (Character)thumb.DataContext;

            int newHP = initialHP - (int)(e.VerticalChange / 5.0);

            int change = newHP - character.HP;

            AdjustCharacterHP(character, change);
        }

        private void HPThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            Character character = (Character)thumb.DataContext;
            if (character.HP == initialHP)
            {
                if (thumb.Name == "HPUpThumb")
                {

                    AdjustCharacterHP(character, 1);

                }
                else
                {
                    AdjustCharacterHP(character, -1);

                }
            }

            if (hpThumbUndo != null)
            {
                hpThumbUndo.Dispose();
                hpThumbUndo = null;
            }
        }

        private void BottomThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double yadjust = this.Height + e.VerticalChange;
            if (yadjust >= 0)
            {
                this.Height = yadjust;

            }

        }

        private void RightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double xadjust = this.Width + e.HorizontalChange;


            if (xadjust >= 0)
            {
                this.Width = xadjust;

            }

        }

        private void LeftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            double xadjust = this.Left + e.HorizontalChange;


            if (xadjust >= 0)
            {
                this.Left = xadjust;


                this.Width = Math.Max(dragStartLeft - this.Left + dragStartWidth, MinWidth);

            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {

            dragStartWidth = Width;
            dragStartLeft = Left;
        }

        private void TopThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double yadjust = this.Top + e.VerticalChange;


            if (yadjust >= 0)
            {
                this.Top = yadjust;


                this.Height = Math.Max(dragStartTop - this.Top + dragStartHeight, MinHeight);

            }
        }

        private void TopThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            dragStartHeight = Height;
            dragStartTop = Top;
        }



        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			
            //select items
            Character ch = (Character)(sender as TextBox).DataContext;

            ListBox lb = ch.IsMonster ? monsterListBox : playerListBox;

            if (ctrl)
            {
                if (lb.SelectedItems.Contains(ch))
                {
                    lb.SelectedItems.Remove(ch);
                }
                else
                {
                    lb.SelectedItems.Add(ch);
                }
            }
            else
            {
                lb.SelectedItem = ch;
            }

        }

        private void OGL_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                OGLWindow oglWindow = new OGLWindow();
                oglWindow.Owner = this;
                oglWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                oglWindow.ShowDialog();
            }
        }

        private void About_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                AboutWindow aboutWindow = new AboutWindow();
                aboutWindow.Owner = this;
                aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                aboutWindow.ShowDialog();
            }
        }

        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }
        [DebuggerStepThrough]
        private System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                mmi.ptMinTrackSize.x = (int)MinWidth;
                mmi.ptMinTrackSize.y = (int)MinHeight;
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }

        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32")]
        public static extern bool IntersectRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

        [DllImport("user32")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private void MonsterAdvancerCheck_Checked(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateMonsterFlowDocument();
            }
        }

        private void AdvancedMonsterMultiplierCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateMonsterFlowDocument();
            }
        }

        private void TabControl_Initialized(object sender, System.EventArgs e)
        {
            OtherTemplateTabControl.SelectedItem = null;
        }

        private void RollHPCheck_Checked(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SaveSettings();
            }
        }

        private void RollHPCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SaveSettings();
            }
        }

        private void OtherTemplateTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                UpdateMonsterFlowDocument();
            }
        }

        private void AddMonsterFromListButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                AddCurrentDBViewMonster();
            }
            SaveCombatState();
        }

        private void SourcesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
            }
        }

        private void UseContentCheck_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                RefreshSpellsView();
                RefreshFeatsView();
                RefreshMonsterTabView();
                RefreshMonsterDBView();
                RefreshRulesView();
                RefreshMagicItemsView();

                SaveSettings();
            }
        }

        private void RefreshSpellsView()
        {

            RefreshView(spellsView);
        }

        private void RefreshFeatsView()
        {

            RefreshView(featsView);
        }

        private void RefreshMonsterTabView()
        {

            RefreshView(monsterTabView);
        }

        private void RefreshRulesView()
        {
            if (!noRulesViewRefresh)
            {
                RefreshView(rulesView);
            }


        }

        private void UpdateRuleSubtypeComboBoxItems()
        {
            noRulesViewRefresh = true;

            if (RuleTypeFilterComboBox != null && RuleSubtypeFilterComboBox != null)
            {

                while (RuleSubtypeFilterComboBox.Items.Count > 1)
                {
                    RuleSubtypeFilterComboBox.Items.RemoveAt(1);
                }

                RuleSubtypeFilterComboBox.SelectedIndex = 0;

                ICollection<string> subtypes = null;

                if (this.RuleTypeFilterComboBox.SelectedIndex != 0)
                {

                    ComboBoxItem item = (ComboBoxItem)RuleTypeFilterComboBox.SelectedItem;
                    string type = (string)item.Content;

                    if (Rule.Subtypes.ContainsKey(type))
                    {
                        subtypes = Rule.Subtypes[type].Values;
                    }
                }

                if (subtypes == null)
                {
                    RuleSubtypeFilterComboBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    RuleSubtypeFilterComboBox.Visibility = Visibility.Visible;

                    foreach (string str in subtypes)
                    {
                        RuleSubtypeFilterComboBox.Items.Add(str);
                    }
                }
            }


            noRulesViewRefresh = false;
            RefreshRulesView();

        }

        private void RefreshMagicItemsView()
        {

            RefreshView(magicItemsView);
        }


        private void RefreshView(ICollectionView view)
        {

            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (view != null)
                {
                    view.Refresh();

                    if (view.CurrentItem == null)
                    {
                        view.MoveCurrentToFirst();
                    }
                }
            }
        }

        private void SubractHPButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Button button = (Button)sender;
                Character character = (Character)button.DataContext;

                TextBox box = (TextBox)button.FindName("textBox");

                if (box != null)
                {
                    int val;
                    if (int.TryParse(box.Text, out val))
                    {

                        int change = -val;
                        Popup pop = (Popup)box.FindName("popup");
                        if (pop.PlacementTarget == box.FindName("HPTextBox"))
                        {
                            character.AdjustHP(change, 0, 0);
                        }
                        else if (pop.PlacementTarget == box.FindName("TempTextBox"))
                        {
                            character.AdjustHP(0, 0, change);
                        }
                        else if (pop.PlacementTarget == box.FindName("NLTextBox"))
                        {
                            character.AdjustHP(0, change, 0);
                        }

                    }
                }
            }
        }

        private void AddHPButton_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Button button = (Button)sender;
                Character character = (Character)button.DataContext;

                TextBox box = (TextBox)button.FindName("textBox");

                if (box != null)
                {
                    int val;
                    if (int.TryParse(box.Text, out val))
                    {
                        int change = val;
                        Popup pop = (Popup)box.FindName("popup");
                        if (pop.PlacementTarget == box.FindName("HPTextBox"))
                        {
                            character.AdjustHP(change, 0, 0);
                        }
                        else if (pop.PlacementTarget == box.FindName("TempTextBox"))
                        {
                            character.AdjustHP(0, 0, change);
                        }
                        else if (pop.PlacementTarget == box.FindName("NLTextBox"))
                        {
                            character.AdjustHP(0, change, 0);
                        }
                    }
                }
            }
        }

        private void AdjustCharacterHP(Character ch, int val)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                ch.AdjustHP(val);

            }
        }

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            box.SelectAll();
        }

        private void TextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void EditAttackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character character = (Character)((Button)sender).DataContext;
                EditAttacks(character);
            }
        }

        private void EditAttacks(Character character)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                AttacksWindow window = new AttacksWindow();
                window.Monster = character.Monster;
                window.Owner = this;
                window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                window.ShowDialog();
            }
        }

        private void EditFeats(Character character)
        {
            FeatChangeWindow window = new FeatChangeWindow();
            window.Character = character;
            window.Owner = this;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void SortInitiaitveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!UserSettings.Settings.ConfirmInitiativeRoll || ShowConfirmationBox("Do you want to sort the initiative list?", "Sort Initiative List"))
            {
                SortInitiative();
            }
        }

        public void SortInitiative()
        {
            combatState.SortInitiative();

            SaveCombatState();

        }

        public void ResetInitiative()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                foreach (Character character in combatState.Characters)
                {
                    character.CurrentInitiative = 0;
                }
            }
        }


        private void monsterFilterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.IsRepeat)
            {
                using (var undoGroup = undo.CreateUndoGroup())
                {
                    AddCurrentDBViewMonster();
                }
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Control control = (Control)sender;

            FrameworkElement ob = (FrameworkElement)control.TemplatedParent;

            if (ob != null)
            {
                Character character = (Character)ob.DataContext;

                if (character.IsMonster)
                {
                    monsterView.MoveCurrentTo(character);
                }
                else
                {
                    playerView.MoveCurrentTo(character);
                }

            }
        }

        private void ResetRoundButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.Round = null;
            }
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Control box = (Control)sender;

                Popup pop = (Popup)box.FindName("popup");
                pop.PlacementTarget = box;
                pop.IsOpen = true;

                TextBox hpBox = (TextBox)pop.FindName("textBox");

                hpBox.Focus();
                hpBox.SelectAll();
            }
        }
		
		
        private void HPButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            Control box = (Control)sender;

            Popup pop = (Popup)box.FindName("popup");
			
            TextBox hpBox = (TextBox)pop.FindName("HPTextBox");
            pop.PlacementTarget = hpBox;
            pop.IsOpen = true;

            TextBox tb = (TextBox)pop.FindName("textBox");

            tb.Focus();
            tb.SelectAll();
        }

        private void textBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                TextBox box = (TextBox)sender;

                if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Down ||
                    e.Key == Key.Up)
                {


                    Character character = (Character)((FrameworkElement)box.Parent).DataContext;

                    if (box != null)
                    {
                        int val;
                        if (int.TryParse(box.Text, out val))
                        {

                            int change;

                            if (e.Key == Key.Up)
                            {
                                change = val;
                            }
                            else
                            {
                                change = -val;
                            }


                            Popup pop = (Popup)box.FindName("popup");
                            if (pop.PlacementTarget == box.FindName("HPTextBox"))
                            {
                                character.AdjustHP(change, 0, 0);
                            }
                            else if (pop.PlacementTarget == box.FindName("TempTextBox"))
                            {
                                character.AdjustHP(0, 0, change);
                            }
                            else if (pop.PlacementTarget == box.FindName("NLTextBox"))
                            {
                                character.AdjustHP(0, change, 0);
                            }

                        }
                    }

                    box.SelectAll();
                }
                else if (e.Key == Key.Escape)
                {

                    Popup pop = (Popup)box.FindName("popup");
                    pop.IsOpen = false;
                }
            }
        }

        private List<Character> GetViewSelectedCharactersFromChar(Character source)
        {
            ListBox box;
            if (source.IsMonster)
            {
                box = monsterListBox;
            }
            else
            {
                box = playerListBox;
            }

            List<Character> list = new List<Character>();


            foreach (Character ch in box.SelectedItems)
            {
                list.Add(ch);
            }


            if (list.Count == 0)
            {
                list.Add(source);
            }

            return list;
        }

        private List<Character> GetViewSelectedCharacters(object sender)
        {
            Control control = (Control)sender;

            Character source = ((Character)control.DataContext);

            return GetViewSelectedCharactersFromChar(source);


        }

        private void MenuItem_Clone(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> list = GetViewSelectedCharacters(sender);

                foreach (Character ch in list)
                {
                    combatState.CloneCharacter(ch);
                }
            }
        }

        private void MenuItem_EditAttacks(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character character = (Character)((FrameworkElement)sender).DataContext;
                EditAttacks(character);
            }
        }

        private void MenuItem_EditFeats(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character character = (Character)((FrameworkElement)sender).DataContext;
                EditFeats(character);
            }
        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> list = GetViewSelectedCharacters(sender);

                foreach (Character ch in list)
                {
                    RemoveCharacter(ch);
                }
            }
        }

        private void ConditionsBox_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                FrameworkElement button = (FrameworkElement)sender;


                Character character = (Character)((FrameworkElement)sender).DataContext;
                if (character.IsMonster)
                {
                    monsterListBox.SelectedItem = character;
                }
                else
                {
                    playerListBox.SelectedItem = character;
                }


                Grid grid = (Grid)button.FindName("CharacterGrid");

                grid.ContextMenu.PlacementTarget = (UIElement)sender;
                grid.ContextMenu.IsOpen = true;

            }
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;

            ToolTipService.SetShowDuration(grid, 360000);
        }

        private void DeleteConditionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                FrameworkElement el = (FrameworkElement)sender;
                ActiveCondition co = (ActiveCondition)el.DataContext;

                el = (FrameworkElement)el.TemplatedParent;
                ListBoxItem item = (ListBoxItem)el.TemplatedParent;
                el = (FrameworkElement)VisualTreeHelper.GetParent(item);
                Character ch = (Character)el.DataContext;

                ch.Stats.RemoveCondition(co);


                if (ch.Stats.ActiveConditions.Count == 0)
                {
                    ch.IsConditionsOpen = false;

                }


                if (ch.Monster != null && ch.Monster == currentViewMonster)
                {
                    UpdateSmallMonsterFlowDocument();
                }

            }
        }

        private void FlowDocument_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                FlowDocumentScrollViewer viewer = (FlowDocumentScrollViewer)sender;

                if (viewer.DataContext is ActiveCondition)
                {

                    Condition condition = (Condition)((ActiveCondition)viewer.DataContext).Condition;
                    if (condition != null)
                    {
                        if (condition.Spell != null)
                        {

                            viewer.Document.Blocks.Clear();
                            SpellBlockCreator b = new SpellBlockCreator(viewer.Document, BlockLinkClicked);
                            viewer.Document.Blocks.AddRange(b.CreateBlocks(condition.Spell, true, false));
                        }
                    }
                }
            }
        }

        private void AfflictionFlowDocument_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                FlowDocumentScrollViewer viewer = (FlowDocumentScrollViewer)sender;
                viewer.Document.Blocks.Clear();
                if (viewer.DataContext is ActiveCondition)
                {


                    Condition condition = (Condition)((ActiveCondition)viewer.DataContext).Condition;
                    if (condition != null)
                    {
                        if (condition.Affliction != null)
                        {
                            Paragraph p = new Paragraph(new Run(condition.Affliction.Text));
                            p.FontFamily = new FontFamily("Segoe UI");
                            p.FontSize = 12.0;
                            viewer.Document.Blocks.Add(p);
                        }
                    }
                }

            }
        }

        private void MiniTemplateGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            ToolTipService.SetShowDuration(grid, 360000);
        }

        private void MenuItem_AddCondition(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                ConditionSelector conditionSelector = new ConditionSelector();
                conditionSelector.InitiativeCount = combatState.CurrentInitiativeCount;
                conditionSelector.Owner = this;
                conditionSelector.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                conditionSelector.Characters = GetViewSelectedCharacters(sender);
                conditionSelector.ShowDialog();
                //just added may not stay
                UpdateMonsterFlowDocument();
                //
                UpdateSmallMonsterFlowDocument();
            }
        }

        private class AfflictionItemData
        {
            private Condition _Condition;
            private ObservableCollection<Character> _Targets;
            private Character _Character;

            public Condition Condition
            {
                get
                {
                    return _Condition;
                }
                set
                {
                    _Condition = value;
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
            public ObservableCollection<Character> Targets
            {
                get
                {
                    return _Targets;
                }
                set
                {
                    _Targets = value;
                }
            }
        }

        private void OnAfflictionItemLoaded(object sender, RoutedEventArgs e)
        {


        }

        private void AfflictionMenuItem_Loaded(object sender, RoutedEventArgs e)
        {

            MenuItem mi = (MenuItem)sender;

            SetAfflictionItemData(mi);
        }

        private void AfflictionMenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            MenuItem mi = (MenuItem)sender;

            SetAfflictionItemData(mi);
        }

        private void MenuItem_Initialized(object sender, EventArgs e)
        {
        }

        private void MenuItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                using (var undoGroup = undo.CreateUndoGroup())
                {
                    MenuItem mi = (MenuItem)sender;

                    SetAfflictionItemData(mi);
                }
            }
        }

        private void InitiativeMenuItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                using (var undoGroup = undo.CreateUndoGroup())
                {
                    MenuItem mi = (MenuItem)sender;


                    SetInitiativeItemData(mi);
                }
            }
        }

        private void InitiativeMenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MenuItem mi = (MenuItem)sender;

                SetInitiativeItemData(mi);
            }
        }

        private class InitiativeItemData
        {
            private Character _Leader;
            private Character _Character;
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
            public Character Leader
            {
                get
                {
                    return _Leader;
                }
                set
                {
                    _Leader = value;
                }
            }
        }

        private void SetInitiativeItemData(MenuItem mi)
        {

            FrameworkElement el = (FrameworkElement)mi.Parent;

            if (el.DataContext != null && el.DataContext.GetType() == typeof(Character))
            {

                List<InitiativeItemData> availableCharacters = new List<InitiativeItemData>();

                Character ch = (Character)el.DataContext;

                List<Character> list = GetViewSelectedCharactersFromChar(ch);

                if (ch.InitiativeFollowers.Count == 0)
                {
                    foreach (Character chList in combatState.Characters)
                    {
                        if (!list.Contains(chList) && chList.IsMonster == ch.IsMonster && chList.InitiativeLeader == null)
                        {
                            InitiativeItemData data = new InitiativeItemData();
                            data.Character = ch;
                            data.Leader = chList;

                            availableCharacters.Add(data);
                        }
                    }
                }

                mi.ItemsSource = availableCharacters;
                mi.IsEnabled = availableCharacters.Count > 0;


            }

        }

        private void SetAfflictionItemData(MenuItem mi)
        {


            List<AfflictionItemData> list = new List<AfflictionItemData>();


            FrameworkElement el = (FrameworkElement)mi.Parent;

            if (el.DataContext != null && el.DataContext.GetType() == typeof(Character))
            {

                Character ch = (Character)el.DataContext;

                foreach (Condition c in ch.Stats.UsableConditions)
                {

                    AfflictionItemData data = new AfflictionItemData();
                    data.Character = ch;
                    data.Condition = c;
                    data.Targets = combatState.CombatList;

                    list.Add(data);
                }
            }

            mi.ItemsSource = list;
            mi.IsEnabled = list.Count > 0;
        }

        private void AfflictionPlayerItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                MenuItem mi = (MenuItem)sender;

                Character target = (Character)mi.DataContext;

                object ob = VisualTreeHelper.GetParent(mi);
                FrameworkElement parent = (FrameworkElement)ob;
                AfflictionItemData data = (AfflictionItemData)parent.DataContext;

                ActiveCondition ac = new ActiveCondition();
                ac.Condition = data.Condition;
                ac.InitiativeCount = combatState.CurrentInitiativeCount;

                target.Stats.AddCondition(ac);
                combatState.UpdateConditions(target);
            }
        }

        private void InitiativeLinkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                MenuItem mi = (MenuItem)sender;

                InitiativeItemData data = (InitiativeItemData)mi.DataContext;

                List<Character> list = GetViewSelectedCharactersFromChar(data.Character);

                foreach (Character ch in list)
                {
                    combatState.LinkInitiative(ch, data.Leader);
                }
            }
        }



        private void ConditionMenuItem_Delete(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MenuItem mi = (MenuItem)sender;
                ActiveCondition ac = (ActiveCondition)mi.DataContext;

                Character ch = GetConditionMenuItemCharacter(mi);

                if (ch != null)
                {
                    ch.Stats.RemoveCondition(ac);



                    if (ch.Stats.ActiveConditions.Count == 0)
                    {
                        ch.IsConditionsOpen = false;

                    }
                }


            }
        }


        private void ConditionMenuItem_DeleteFromAll(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MenuItem mi = (MenuItem)sender;
                ActiveCondition ac = (ActiveCondition)mi.DataContext;

                foreach (Character ch in combatState.Characters)
                {
                    ch.RemoveConditionByName(ac.Condition.Name);
                }

            }
        }

        private Character GetConditionMenuItemCharacter(MenuItem mi)
        {
            FrameworkElement el = (FrameworkElement)mi.Parent;
            Popup p = (Popup)el.Parent;
            el = (FrameworkElement)p.PlacementTarget;
            Character ch = null;

            ListBox box = FindAncestor<ListBox>(el);

            if (box != null)
            {
                ch = (Character)box.DataContext;
            }

            return ch;
        }

        private static T FindAncestor<T>(FrameworkElement element)
        {
            FrameworkElement el = (FrameworkElement)VisualTreeHelper.GetParent(element);


            while (el != null && el.GetType() != typeof(T))
            {
                el = (FrameworkElement)VisualTreeHelper.GetParent(el);
            }

            return (T)(object)el;
        }

        private void ConditionMenuItem_AddTurn(object sender, RoutedEventArgs e)
        {

            ActiveCondition ac = (ActiveCondition)((FrameworkElement)sender).DataContext;

            MenuItem mi = (MenuItem)sender;

            Character ch = GetConditionMenuItemCharacter(mi);


            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.AddConditionTurns(ch, ac, 1);
            }
        }

        private void ConditionMenuItem_Add5Turns(object sender, RoutedEventArgs e)
        {

            ActiveCondition ac = (ActiveCondition)((FrameworkElement)sender).DataContext;

            MenuItem mi = (MenuItem)sender;

            Character ch = GetConditionMenuItemCharacter(mi);


            using (var undoGroup = undo.CreateUndoGroup())
            {
                combatState.AddConditionTurns(ch, ac, 5);
            }
        }


        private void ConditionMenuItem_RemoveTurn(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {


                ActiveCondition ac = (ActiveCondition)((FrameworkElement)sender).DataContext;
                MenuItem mi = (MenuItem)sender;

                Character ch = GetConditionMenuItemCharacter(mi);

                combatState.RemoveConditionTurns(ch, ac, 1);

            }
        }



        private void ConditionMenuItem_Remove5Turns(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {


                ActiveCondition ac = (ActiveCondition)((FrameworkElement)sender).DataContext;
                MenuItem mi = (MenuItem)sender;

                Character ch = GetConditionMenuItemCharacter(mi);

                combatState.RemoveConditionTurns(ch, ac, 5);

            }
        }

   

        private void ImportFromFile(string filename, ICollectionView view)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                try
                {
                    List<Monster> monsters = Monster.FromFile(filename);

                    if (monsters != null)
                    {
                        foreach (Monster m in monsters)
                        {

                            Character ch = new Character(m, false);

                            ch.IsMonster = (view != playerView);

                            AddCharacter(ch);
                        }
                    }
                }
                catch (MonsterParseException ex)
                {
                    MessageBox.Show("Combat Manager encountered an error importing " + filename + "\r\n\r\n" + ex.Message, "Combat Manager Import", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CalculateEncounterXP()
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                long xp = 0;

                foreach (Character c in monsterView)
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

                combatState.XP = xp;

                if (xp == 0)
                {
                    combatState.CR = null;
                }

                combatState.CR = Monster.EstimateCR(xp);

            }
        }

        private void ResetAdvancerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                OtherTemplateTabControl.SelectedIndex = 0;
                AdvancedMonsterCheck.IsChecked = false;
                AdvancedMonsterMultiplierCombo.SelectedIndex = 0;

                SizeAdvancedMonsterCheck.IsChecked = false;
                SizeAdvancedMonsterCombo.SelectedIndex = 0;
                SizeAdvancedMultiplierCombo.SelectedIndex = 0;

                SummonAdvancedMonsterCheck.IsChecked = false;
                RacialHDCheck.IsChecked = false;
                RacialHDNumberCombo.SelectedIndex = 0;
                RacialHDSizeChangeCombo.SelectedIndex = 0;
                RacialHDStatCombo.SelectedIndex = 0;
                RacialHDTypeCombo.SelectedIndex = 0;

                BloodySkeletonCheckBox.IsChecked = false;
                BurningSkeletonCheckBox.IsChecked = false;
                SkeletalChampionCheckBox.IsChecked = false;

                HalfCelestialStrengthCheck.IsChecked = true;
                HalfCelestialDexterityCheck.IsChecked = true;
                HalfCelestialConstitutionCheck.IsChecked = true;
                HalfCelestialIntelligenceCheck.IsChecked = false;
                HalfCelestialWisdomCheck.IsChecked = false;
                HalfCelestialCharismaCheck.IsChecked = false;

                HalfDragonColorCombo.SelectedIndex = 0;

                HalfFiendStrengthCheck.IsChecked = true;
                HalfFiendDexterityCheck.IsChecked = true;
                HalfFiendConstitutionCheck.IsChecked = true;
                HalfFiendIntelligenceCheck.IsChecked = false;
                HalfFiendWisdomCheck.IsChecked = false;
                HalfFiendCharismaCheck.IsChecked = false;


                ZombieTypeComboBox.SelectedIndex = 0;


            }
        }

        private void MenuItem_MoveToParty(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> list = GetViewSelectedCharacters(sender);


                MoveCharacterList(list, false);

            }
        }

        private void MenuItem_MoveToMonsters(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> list = GetViewSelectedCharacters(sender);

                MoveCharacterList(list, true);

            }
        }

        private void MoveCharacter(Character ch, bool monster)
        {

            combatState.RegroupFollowers(ch);
            combatState.UnlinkLeader(ch);
            ch.IsMonster = monster;
            playerView.Refresh();
            monsterView.Refresh();
        }

        private void MoveCharacterList(IList<Character> chars, bool monster)
        {
            foreach (var ch in chars)
            {
                combatState.RegroupFollowers(ch);
                combatState.UnlinkLeader(ch);
                ch.IsMonster = monster;
            }
            playerView.Refresh();
            monsterView.Refresh();
        }

        private void MenuItem_Unlink(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> list = GetViewSelectedCharacters(sender);

                foreach (Character ch in list)
                {
                    combatState.UnlinkLeader(ch);
                }
            }
        }

        private void playerListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBox box = (ListBox)sender;

            Character ch = (Character)box.SelectedValue;
            if (ch != null)
            {
                SetCurrentViewMonster(ch);
            }
        }

        private void monsterListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBox box = (ListBox)sender;

            Character ch = (Character)box.SelectedValue;
            if (ch != null)
            {
                SetCurrentViewMonster(ch);
            }
        }

        private void ClearDelayReadyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character ch = (Character)((FrameworkElement)sender).DataContext;
                ch.IsReadying = false;
                ch.IsDelaying = false;
            }
        }

        private void ReadyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character ch = (Character)((FrameworkElement)sender).DataContext;
                ch.IsReadying = true;
            }
        }

        private void ActNowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character ch = (Character)((FrameworkElement)sender).DataContext;
                combatState.CharacterActNow(ch);
            }
        }

        private void DelayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                Character ch = (Character)((FrameworkElement)sender).DataContext;
                ch.IsDelaying = true;
            }
        }

        private void RulesTabFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshRulesView();
        }

        private void RuleTypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            RefreshRulesView();


            UpdateRuleSubtypeComboBoxItems();


        }

        private void RuleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClearRuleFilterText_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                RulesTabFilterBox.Clear();
                RefreshRulesView();
            }
        }

        private void MagicItemsTabFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshMagicItemsView();
        }

        private void MagicItemTypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMagicItemsView();
        }

        private void MagicItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClearMagicItemFilterText_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                MagicItemsTabFilterBox.Clear();
                RefreshMagicItemsView();
            }
        }

        private void MagicItemGroupFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            RefreshMagicItemsView();
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't create an undo group for this
            UndoLastAction();
        }

        private void SettingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                SettingsDialog dlg = new SettingsDialog();
                dlg.Owner = this;
                dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

                if (dlg.ShowDialog() == true)
                {

                }
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(((TextBox)sender).Text, out lastHPChange);
        }

        private void textBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if (((bool)e.NewValue) == true)
                {
                    TextBox box = (TextBox)sender;
                    box.Text = lastHPChange.ToString();
                }
            }
        }

        private void ContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                if ((bool)e.NewValue)
                {
                    ContextMenu menu = (ContextMenu)sender;

                    List<Character> list = GetViewSelectedCharacters(menu);

                    MenuItem item = (MenuItem)LogicalTreeHelper.FindLogicalNode(menu, "UnlinkMenuItem");

                    bool needUnlink = false;

                    foreach (Character ch in list)
                    {
                        if (ch.InitiativeLeader != null)
                        {
                            needUnlink = true;
                            break;
                        }
                    }


                    item.IsEnabled = needUnlink;
                }
            }
        }

        private void GenerateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                TreasureGenerator gen = new TreasureGenerator();
                gen.Level = TreasureLevelComboBox.SelectedIndex + 1;
                gen.Coin = CoinAmountComboBox.SelectedIndex;
                gen.Items = ItemsAmountComboBox.SelectedIndex;
                gen.Goods = GoodsAmountComboBox.SelectedIndex;

                

                Treasure t = gen.Generate();


                if (t != null)
                {
                    TreasureFlowDocument.Document.Blocks.Clear();

                    TreasureBlockCreator c = new TreasureBlockCreator(TreasureFlowDocument.Document, BlockLinkClicked);

                    TreasureFlowDocument.Document.Blocks.AddRange(c.CreateBlocks(t));

                }
            }
        }

        private void ConditionContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                ContextMenu menu = (ContextMenu)sender;

                if (menu.DataContext is ActiveCondition)
                {

                    ActiveCondition c = (ActiveCondition)menu.DataContext;

                    MenuItem item = (MenuItem)LogicalTreeHelper.FindLogicalNode(menu, "StabalizeMenuItem");


                    if (item != null)
                    {
                        if (String.Compare(c.Condition.Name, "dying", true) == 0)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                }



            }
        }

        private void ConditionMenuItem_Stabalize(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                MenuItem mi = (MenuItem)sender;
                ActiveCondition ac = (ActiveCondition)mi.DataContext;

                Character ch = GetConditionMenuItemCharacter(mi);

                if (ch != null)
                {
                    ch.RemoveConditionByName("dying");
                    ch.AddConditionByName("stable");
                }
            }
        }

        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UndoLastAction();
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RedoNextAction();
        }

        private void CombatTabNPCFilterBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshMonsterDBView();
            SaveMonsterDBFilter();
        }

        private void RuleSubtypeFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshRulesView();
        }

        private void SaveMonsterDBFilter()
        {
            if (CombatTabNPCFilterBox != null && UserSettings.Loaded)
            {
                UserSettings.Settings.MonsterDBFilter = (UserSettings.MonsterSetFilter)CombatTabNPCFilterBox.SelectedIndex;
                UserSettings.Settings.SaveOptions(UserSettings.SettingsSaveSection.Filters);
            }
        }

        private void SaveMonsterTabFilter()
        {
            if (MonsterTabNPCComboBox != null && UserSettings.Loaded)
            {
                UserSettings.Settings.MonsterTabFilter = (UserSettings.MonsterSetFilter)MonsterTabNPCComboBox.SelectedIndex;
                UserSettings.Settings.SaveOptions(UserSettings.SettingsSaveSection.Filters);
            }
        }


        private void DamageHealMenuItem_Click(object sender, RoutedEventArgs e)
        {
            HPChangeDialog dlg = new HPChangeDialog();
            dlg.Owner = this;
            dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;


            Control control = (Control)sender;

            Character source = ((Character)control.DataContext);

            if (source.IsMonster)
            {
                dlg.ListBox = monsterListBox;
            }
            else
            {
                dlg.ListBox = playerListBox;
            }

            dlg.Show();

        }

        private void GenerateItemsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            TreasureGenerator.RandomItemType types = GetSelectedRandomItemTypes();

            if (CanGenerateRandomItems)
            {
                int count = ItemGenerateCountComboBox.SelectedIndex + 1;

                ItemLevel level = ItemLevel.Minor;

                if (ItemGenerateLevelComboBox.SelectedIndex == 0)
                {
                    level = ItemLevel.Minor;
                }
                else if (ItemGenerateLevelComboBox.SelectedIndex == 1)
                {
                    level = ItemLevel.Medium;
                }
                else if (ItemGenerateLevelComboBox.SelectedIndex == 2)
                {
                    level = ItemLevel.Major;
                }

                Treasure t = new Treasure();
                TreasureGenerator g = new TreasureGenerator();


                for (int i = 0; i < count; i++)
                {
                    t.Items.Add(g.GenerateRandomItem(level, types));
                }

                TreasureFlowDocument.Document.Blocks.Clear();

                TreasureBlockCreator c = new TreasureBlockCreator(TreasureFlowDocument.Document, BlockLinkClicked);

                TreasureFlowDocument.Document.Blocks.AddRange(c.CreateBlocks(t));

            }

        }

        private bool CanGenerateRandomItems
        {
            get
            {
                if (mainWindowLoaded)
                {

                    TreasureGenerator.RandomItemType types = GetSelectedRandomItemTypes();

                    bool generate = false;

                    if (ItemGenerateLevelComboBox.SelectedIndex == 0)
                    {
                        if ((types &
                            ~TreasureGenerator.RandomItemType.Rod & ~TreasureGenerator.RandomItemType.Staff)
                            != TreasureGenerator.RandomItemType.None)
                        {
                            generate = true;
                        }
                    }
                    else
                    {
                        if (types != TreasureGenerator.RandomItemType.None)
                        {
                            generate = true;
                        }
                    }

                    return generate;
                }
                return true;
            }
        }

        private TreasureGenerator.RandomItemType GetSelectedRandomItemTypes()
        {
            TreasureGenerator.RandomItemType types = TreasureGenerator.RandomItemType.None;
            if (mainWindowLoaded)
            {


                if (GenerateMagicalArmorCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.MagicalArmor;
                }
                if (GenerateMagicalWeaponCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.MagicalWeapon;
                }
                if (GeneratePotionCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Potion;
                }
                if (GenerateRingCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Ring;
                }
                if (GenerateRodCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Rod;
                }
                if (GenerateScrollCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Scroll;
                }
                if (GenerateStaffCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Staff;
                }
                if (GenerateWandCheck.IsChecked == true)
                {
                    types |= TreasureGenerator.RandomItemType.Wand;
                }
                if (GenerateWondrousItemCheck.IsChecked == true)
                {
                    if (ItemGenerateLevelComboBox.SelectedIndex == 0)
                    {
                        types |= TreasureGenerator.RandomItemType.MinorWondrous;
                    }
                    else if (ItemGenerateLevelComboBox.SelectedIndex == 1)
                    {
                        types |= TreasureGenerator.RandomItemType.MediumWondrous;
                    }
                    else if (ItemGenerateLevelComboBox.SelectedIndex == 2)
                    {
                        types |= TreasureGenerator.RandomItemType.MajorWondrous;
                    }
                }
            }

            return types;
        }

        private void ItemGenerateLevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainWindowLoaded)
            {
                UpdateItemsGenerateButtons();

            }
        }

        private void GenerateItemCheck_Checked(object sender, RoutedEventArgs e)
        {

            if (mainWindowLoaded)
            {
                UpdateItemsGenerateButtons();
            }
        }

        void UpdateItemsGenerateButtons()
        {
            if (mainWindowLoaded)
            {

                GenerateItemsButton.IsEnabled = CanGenerateRandomItems;

                bool rodStaff = ItemGenerateLevelComboBox.SelectedIndex != 0;

                GenerateRodCheck.IsEnabled = rodStaff;
                GenerateStaffCheck.IsEnabled = rodStaff;
            }
        }

        private void FavoritesMenuItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {

                MenuItem item = (MenuItem)sender;

                item.Items.Clear();

                AddFavoriteConditionItems(Condition.FavoriteConditions, item);
                AddFavoriteConditionItems(Condition.RecentConditions, item);

                MenuItem EditFavorites = new MenuItem();
                EditFavorites.Header = "Other...";
                EditFavorites.Click += new RoutedEventHandler(MenuItem_AddCondition);
                item.Items.Add(EditFavorites);

            }
        }

        void AddFavoriteConditionItems(List<FavoriteCondition> list, MenuItem item)
        {
            foreach (FavoriteCondition fc in list)
            {
                Condition c = Condition.FromFavorite(fc);

                if (c != null)
                {
                    MenuItem mi = new MenuItem();

                    mi.Header = c.Name;
                    Image i = new Image();
                    BitmapImage bi = StringImageSmallIconConverter.FromName(c.Image);
                    i.Source = bi;
                    i.Width = 16;
                    i.Height = 16;

                    mi.Icon = i;
                    mi.Click += new RoutedEventHandler(FavoritesMenuItem_Click);
                    mi.DataContext = c;
                    item.Items.Add(mi);
                }
            }

            if (list.Count > 0)
            {
                item.Items.Add(new Separator());
            }
        }


        void FavoritesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Condition c = (Condition)mi.DataContext;

            using (var undoGroup = undo.CreateUndoGroup())
            {
                List<Character> chars = GetViewSelectedCharacters(mi.Parent);

                foreach (Character ch in chars)
                {
                    ActiveCondition a = new ActiveCondition();
                    a.Condition = c;
                    ch.Stats.AddCondition(a);
                }

            }
        }

        private void ClearPlayersButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                if (!UserSettings.Settings.ConfirmCharacterDelete || ShowConfirmationBox("Do you want to clear the players list?", "Clear Players"))
                {
                    RemoveAllCharacters(playerView);
                }
            }
        }

        private void ClearMonstersButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                if (!UserSettings.Settings.ConfirmCharacterDelete || ShowConfirmationBox("Do you want to clear the monsters list?", "Clear Monsters"))
                {
                    RemoveAllCharacters(monsterView);
                }
            }
        }

        private void NextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoveNextPlayer();
        }

        private void PrevCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovePreviousPlayer();
        }

        private void TitleBarMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)Resources["MainMenu"];

            menu.PlacementTarget = (Button)sender;
            menu.Placement = PlacementMode.Bottom;
            menu.IsOpen = true;

        }

        private void LoadCombatStateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadCombatState();
        }

        private void SaveCombatStateMenuItem_Click(object sender, RoutedEventArgs e)
        {

            SaveAsCombatState();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void MonsterPrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoThePrint(MonsterFlowDocument.Document);
        }

        private void DoThePrint(System.Windows.Documents.FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
            // and allowing the user to select a printer.

            // get information about the dimensions of the seleted printer+media.
            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(72);  // copy.PagePadding;
                copy.PagePadding = new Thickness(
                                 Math.Max(ia.OriginWidth, t.Left),
                                   Math.Max(ia.OriginHeight, t.Top),
                                   Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                                   Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

                copy.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                docWriter.Write(paginator);
            }

        }

        private void FeatsPrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FeatFlowDocument.Print();
        }

        private void RulesPrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RuleFlowDocument.Print();
        }

        private void SpellsPrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SpellFlowDocument.Print();
        }

        private void MagicItemPrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MagicItemFlowDocument.Print();
        }

        private void TreasurePrintButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TreasureFlowDocument.Print();
        }

        private void InitiativeWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenCombatListWindow();
        }

        private void OpenCombatListWindow()
        {
            if (combatListWindow != null)
            {
                combatListWindow.Show();
                combatListWindow.WindowState = WindowState.Normal;
                combatListWindow.Focus();

            }
            else
            {
                combatListWindow = new CombatListWindow();
                combatListWindow.CombatState = combatState;
                combatListWindow.Closed += delegate { combatListWindow = null; };
                combatListWindow.Show();
            }

        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            CloseToolWindows();
            StopService();
        }

        private void CloseToolWindows()
        {
            if (combatListWindow != null)
            {
                combatListWindow.Close();
            }
        }

        private void ResetInitiaitveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!UserSettings.Settings.ConfirmInitiativeRoll || ShowConfirmationBox("Do you want to reset the initiative list?", "Reset Initiative List"))
            {
                ResetInitiative();
            }
        }


        void combatState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                if (lastCurrentCharacter != null)
                {
                    lastCurrentCharacter.PropertyChanged -= new PropertyChangedEventHandler(CombatStateCurrentCharacter_PropertyChanged);

                }
                lastCurrentCharacter = combatState.CurrentCharacter;
                if (lastCurrentCharacter != null)
                {
                    lastCurrentCharacter.PropertyChanged += new PropertyChangedEventHandler(CombatStateCurrentCharacter_PropertyChanged);
                }




                UpdateCurrentMonsterFlowDocument();
            }
        }

        void CombatStateCurrentCharacter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsCharacterRefreshProperty(e.PropertyName))
            {
                UpdateCurrentMonsterFlowDocument();
            }
        }

        private void GridSplitter_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void ToggleRow(RowDefinition row, double size)
        {
            if (row.ActualHeight <= row.MinHeight)
            {
                row.Height = new GridLength(size, GridUnitType.Star);
            }
            else
            {
                row.Height = new GridLength(row.MinHeight);
            }
        }

        private void TreasureSelectAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (CheckBox c in treasureCheckboxesList)
            {
                c.IsChecked = true;
            }
        }

        private void TreasureUnselectAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (CheckBox c in treasureCheckboxesList)
            {
                c.IsChecked = false;
            }
        }

        private void RollInitiativeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {

                MenuItem mi = (MenuItem)sender;

                FrameworkElement el = (FrameworkElement)sender;

                Character ch = (Character)el.DataContext;

                List<Character> list = GetViewSelectedCharactersFromChar(ch);

                foreach (Character c in list)
                {
                    combatState.RollIndividualInitiative(c);
                }

                combatState.SortCombatList(false, false);
            }
        }

        private void MenuItem_EditMonster(object sender, RoutedEventArgs e)
        {
            MonsterEditorWindow w = new MonsterEditorWindow();

            MenuItem mi = (MenuItem)sender;

            FrameworkElement el = (FrameworkElement)sender;

            Character ch = (Character)el.DataContext;

            w.Owner = this;
            w.Monster = ch.Monster;
            if (w.ShowDialog() == true)
            {
                CalculateEncounterXP();
            }
        }


        private void SetCharacterListBoxTemplate(ListBox box, bool small)
        {
            box.ItemTemplate = (DataTemplate)FindResource(small ? "CharacterDataTemplateSmall" : "CharacterDataTemplate");

        }

        private void PartyMiniModeButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (playerListBox != null && mainWindowLoaded)
            {
                SetCharacterListBoxTemplate(playerListBox, true);
                UserSettings.Settings.PlayerMiniMode = true;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void PartyMiniModeButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (playerListBox != null && mainWindowLoaded)
            {
                SetCharacterListBoxTemplate(playerListBox, false);
                UserSettings.Settings.PlayerMiniMode = false;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void MonsterMiniModeButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (monsterListBox != null && mainWindowLoaded)
            {
                SetCharacterListBoxTemplate(monsterListBox, true);
                UserSettings.Settings.MonsterMiniMode = true;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void MonsterMiniModeButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (monsterListBox != null && mainWindowLoaded)
            {
                SetCharacterListBoxTemplate(monsterListBox, false);
                UserSettings.Settings.MonsterMiniMode = false;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void NotesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            ContextMenu cm = (ContextMenu)mi.Parent;
            FrameworkElement el = (FrameworkElement)cm.PlacementTarget;
            Popup popup = (Popup)el.FindName("NotesPopup");
            popup.IsOpen = true;
        }

        private ListBoxItem GetListBoxItemForCharater(Character ch)
        {
            return null;
        }

        private ListBox GetListBoxForCharacter(Character ch)
        {
            if (ch.IsMonster)
            {
                return monsterListBox;
            }
            else
            {
                return playerListBox;
            }
        }



        private void MonsterColumnToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
             UserSettings.Settings.SaveOptions();
        }

        private void CombatManagerWindow_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                UserSettings.Settings.MainWindowWidth = (int)Width;
                UserSettings.Settings.MainWindowHeight = (int)Height;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void CombatManagerWindow_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                UserSettings.Settings.MainWindowLeft = (int)Left;
                UserSettings.Settings.MainWindowTop = (int)Top;
                UserSettings.Settings.SaveOptions();
            }
        }

        private void MenuItem_AddToCustomMonsters(object sender, RoutedEventArgs e)
        {

            MenuItem mi = (MenuItem)sender;

            FrameworkElement el = (FrameworkElement)sender;

            Character ch = (Character)el.DataContext;

            AddNewCustomMonster(ch.Monster);
        }

        private Monster AddNewCustomMonster(Monster oldmonster)
        {
            Monster m = (Monster)oldmonster.Clone();
            m.DBLoaderID = 0;
            MonsterDB.DB.AddMonster(m);
            Monster.Monsters.Add(m);
            return m;
            
        }

        private void CustomizeMonsterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (currentDBMonster != null)
            {
                Monster m = (Monster)(currentDBMonster).Clone();

                MonsterEditorWindow w = new MonsterEditorWindow();

                w.Owner = this;
                w.Monster = m;
                if (w.ShowDialog() == true)
                {
                    m.DBLoaderID = 0;
                    m = AddNewCustomMonster(m);
                    MakeMonsterVisible(m);
                }
            }
		
        }

        private void EditCustomMonsterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Monster m = ((Monster)monsterTabView.CurrentItem);


            if (m != null)
            {
                MonsterEditorWindow w = new MonsterEditorWindow();

                w.Owner = this;
                w.Monster = m;
                if (w.ShowDialog() == true)
                {
                    MonsterDB.DB.UpdateMonster(m);
                    UpdateMonsterFlowDocument();
                }
            }
        }

        private void DeleteCustomMonsterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Monster m = ((Monster)monsterTabView.CurrentItem);

            if (m != null)
            {

                MonsterDB.DB.DeleteMonster(m);
                Monster.Monsters.Remove(m);
            }
        }

        private void MakeMonsterVisible(Monster m)
        {

            object item = MonsterTabNPCComboBox.SelectedItem;
            if (item != NPCFilterAllComboItem)
            {
                bool showAll = false;
                if (m.IsCustom && item != NPCFilterCustomComboItem)
                {
                    showAll = true;
                }
                else if (!m.IsCustom)
                {
                    if ((m.NPC && item != NPCFilterNPCComboItem) ||
                        (!m.NPC && item != NPCFilterMonstersComboItem))
                    {
                        showAll = true;
                    }
                }


                if (showAll)
                {
                    MonsterTabNPCComboBox.SelectedItem = NPCFilterAllComboItem;
                }
            }

            monsterTabView.Refresh();
            monsterTabView.MoveCurrentTo(m);
        }

        private void MakeSpellVisible(Spell s)
        {
            MainTabControl.SelectedItem = SpellsTab;


            if (!spellsView.Contains(s))
            {
                ResetSpellFilters();
                spellFilterBox.Text = "";
            }

            spellsView.Refresh();
            spellsView.MoveCurrentTo(s);
        }

        private void MainTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mainWindowLoaded)
            {
                UserSettings.Settings.SelectedTab = MainTabControl.SelectedIndex;
                UserSettings.Settings.SaveOptions(UserSettings.SettingsSaveSection.WindowState);

                UpdateLayout();
                if (MainTabControl.SelectedItem == FeatsTab)
                {
                    featFilterBox.Focus();   
                }
                else if (MainTabControl.SelectedItem == RulesTab)
                {
                    RulesTabFilterBox.Focus();
                }
                else if (MainTabControl.SelectedItem == SpellsTab)
                {
                    spellFilterBox.Focus();
                }
                else if (MainTabControl.SelectedItem == MonstersTab)
                {
                    MonsterTabFilterBox.Focus();
                }
                else if (MainTabControl.SelectedItem == TreasureTab)
                {
                    if (TreasureTabControl.SelectedItem == MagicItemsTab)
                    {
                        MagicItemsTabFilterBox.Focus();
                    }
                }
            }
        }

        private void ObjectToolTipFlowDocument_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FlowDocumentScrollViewer viewer = (FlowDocumentScrollViewer)sender;

            viewer.Document.Blocks.Clear();

            if (viewer.DataContext is Spell)
            {
                Spell spell = (Spell)viewer.DataContext;
                if (spell != null)
                {
                    SpellBlockCreator creator = new SpellBlockCreator(viewer.Document, null);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(spell, false, false));
                }

            }
            else if (viewer.DataContext is Feat)
            {
                Feat feat = (Feat)viewer.DataContext;
                if (feat != null)
                {
                    FeatBlockCreator creator = new FeatBlockCreator(viewer.Document);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(feat, false));
                }
            }

            else if (viewer.DataContext is Rule)
            {

                Rule rule = (Rule)viewer.DataContext;
                if (rule != null)
                {
                    RuleBlockCreator creator = new RuleBlockCreator(viewer.Document);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(rule, false));
                }
            }

            else if (viewer.DataContext is MagicItem)
            {

                MagicItem magicItem = (MagicItem)viewer.DataContext;
                if (magicItem != null)
                {
                    MagicItemBlockCreator creator = new MagicItemBlockCreator(viewer.Document);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(magicItem, false));
                }
            }

            else if (viewer.DataContext is Monster)
            {

                Monster m = viewer.DataContext as Monster;

                if (m != null)
                {
                    MonsterBlockCreator creator = new MonsterBlockCreator(viewer.Document, null);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(m, true));
                }
            }

            else if (viewer.DataContext is Character)
            {

                Character m = viewer.DataContext as Character;

                if (m != null)
                {
                    MonsterBlockCreator creator = new MonsterBlockCreator(viewer.Document, null);
                    viewer.Document.Blocks.AddRange(creator.CreateBlocks(m, true));
                }
            }
        }

        #region Die roll section

            private void RollDiceButton_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                RollCurrentTextDie();
            }

            private void DieRollText_KeyUp(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    RollCurrentTextDie();
                }
            }

            private void RollCurrentTextDie()
            {
                DieRoll roll = DieRoll.FromString(DieRollText.Text);

                if (roll != null)
                {
                    RollDie(roll, null, true);
                }

                AddRecentDieRoll(roll);
            }

            private Button MakeDieRollButton()
            {

                Button b = new Button();
                b.ToolTip = "Reroll";
                Image im = CMUIUtilities.GetNamedImageControl("dice");
                im.Width = 13;
                im.Height = 13;
                b.Content = im;
                b.Padding = new Thickness(0);

                return b;
            }

            private class DieRollerRollInfo
            {
                public DieRoll Roll { get; set; }
                public string Header { get; set; }
                public bool Full { get; set; }
            }

            private Button AddDieRollButton(Paragraph p)
            {

                p.Margin = new Thickness(0);
                Button b = MakeDieRollButton();
                InlineUIContainer c = new InlineUIContainer();
                c.Child = b;
                p.Inlines.Add(c);
                p.Inlines.Add(" ");

                return b;
            }

            private void RollDie(DieRoll roll, string header, bool full)
            {
                RollResult res = roll.Roll();


                Paragraph p = new Paragraph();

                Button b = AddDieRollButton(p);
                b.Tag = new DieRollerRollInfo() { Roll = roll, Header = header, Full = full };
                b.Click += new RoutedEventHandler(DieReroll_Click);

                if (header != null)
                {
                    p.Inlines.Add(header);
                }


                if (full)
                {

                    p.Inlines.Add(CreateRollElement(res.Total.ToString()));
                    string text = "=";

                    bool first = true;
                    foreach (DieResult die in res.Rolls)
                    {
                        if (!first)
                        {
                            text += "+";
                        }
                        first = false;
                        if (die.Die == 20 && (die.Result == 20 || die.Result == 1))
                        {
                            p.Inlines.Add(new Run(text));
                            p.Inlines.Add(new Bold(new Run(die.Result.ToString())));
                            text = "";
                        }
                        else
                        {
                            text += die.Result;
                        }
                        text += "(d" + die.Die + ")";
                    }
                    if (res.Mod != 0)
                    {
                        text += CMStringUtilities.PlusFormatNumber(res.Mod);
                    }
                    p.Inlines.Add(new Run(text));
                }
                else
                {
                    if (res.Rolls.Count == 1 && res.Rolls[0].Die == 20 && (res.Rolls[0].Result == 20 || res.Rolls[0].Result == 1))
                    {
                        int result = res.Rolls[0].Result;
                        p.Inlines.Add(CreateRollElement(res.Total.ToString() + " (" + result + ")", Colors.White, (result == 1) ? Colors.Red : Colors.Green));

                    }
                    else
                    {
                        p.Inlines.Add(CreateRollElement(res.Total.ToString()));
                    }
                }

                bool firstIl = true;
                foreach (Inline il in p.Inlines)
                {
                    if (firstIl)
                    {
                        firstIl = false;
                    }
                    else
                    {
                        il.BaselineAlignment = BaselineAlignment.Center;
                    }
                }

                DieRollDocument.Blocks.Add(p);


                DieRollViewer.ScrollChildToBottom();
            }

            Inline CreateRollElement(string text)
            {
                return CreateRollElement(text, Colors.White, Colors.Black);
            }

            Inline CreateRollElement(string text, Color foreground, Color background)
            {
                return CreateRollElement(text, foreground, background, null);
            }

            Inline CreateRollElement(string text, Color foreground, Color background, string tooltip)
            {
                return CreateRollElement(text, new SolidColorBrush(foreground), new SolidColorBrush(background), tooltip);
            }

            Inline CreateRollElement(string text, Brush foreground, Brush background, string tooltip)
            {
                Border b = new Border();
                b.Background = background;

                Inline rt = new Bold(new Run(text));
                rt.Foreground = foreground;
                b.CornerRadius = new CornerRadius(8);
                TextBlock tb = new TextBlock(rt);
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                b.Child = tb;
                Thickness pad = b.Padding;
                b.MinWidth = 15;
                pad.Left += 4;
                pad.Right += 4;
                b.Padding = pad;
                b.ToolTip = tooltip;


                InlineUIContainer co = new InlineUIContainer(b);
                co.BaselineAlignment = BaselineAlignment.Center;

                return co;

            }

            void DieReroll_Click(object sender, RoutedEventArgs e)
            {
                Button b = (Button)sender;
                DieRollerRollInfo r = (DieRollerRollInfo)b.Tag;

                RollDie(r.Roll, r.Header, r.Full);
            }

            private void DieButtonPressed(object sender, RoutedEventArgs e)
            {

                int die = int.Parse((string)((Button)sender).Tag);

                DieRoll roll = DieRoll.FromString(DieRollText.Text);
                if (roll != null)
                {
                    roll.AddDie(die);
                }
                else
                {
                    int mod = 0;

                    int.TryParse(DieRollText.Text, out mod);

                    roll = new DieRoll(1, die, mod);
                }


                DieRollText.Text = roll.Text;
            }

            private void ClearDieText_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                DieRollText.Text = "";
            }

            private void ClearDieRollDocumentButton_Click(object sender, RoutedEventArgs e)
            {
                DieRollDocument.Blocks.Clear();
            }

            private void InstantDieButtonPressed(object sender, System.Windows.RoutedEventArgs e)
            {

                int die = int.Parse((string)((Button)sender).Tag);

                DieRoll roll = new DieRoll(1, 1, die, 0);

                RollDie(roll, null, true);
            }

            #region Save Roll Menu Support Code
                private void FortitudeMenuItem_Click(object sender, RoutedEventArgs e)
                {
                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                    RollSave(list, Monster.SaveType.Fort);
                }

                private void ReflexMenuItem_Click(object sender, RoutedEventArgs e)
                {

                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                    RollSave(list, Monster.SaveType.Ref);

                }

                private void WillMenuItem_Click(object sender, RoutedEventArgs e)
                {
                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                    RollSave(list, Monster.SaveType.Will);

                }

                private void RollSave(List<Character> list, Monster.SaveType type)
                {
                    if (list.Count > 0)
                    {
                        Paragraph p = new Paragraph();
                        p.Margin = new Thickness(0);

                        p.Inlines.Add(new Underline(new Run(SaveName(type) + " save" + (list.Count > 1 ? "s" : ""))));
                        DieRollDocument.Blocks.Add(p);
                    }



                    foreach (Character ch in list)
                    {
                        RollSave(ch, type);
                    }


                }

                private void RollSave(Character ch, Monster.SaveType type)
                {
                    int? mod = ch.Monster.GetSave(type);
                    if (mod != null)
                    {
                        DieRoll roll;
                        if (UserSettings.Settings.AlternateInit3d6)
                        {

                            roll = UserSettings.Settings.AlternateInitDieRoll;
                        }
                        else
                        {
                            roll = new DieRoll(1, 20, (int)mod);
                        }
                        RollDie(roll, ch.Name + " (" + CMStringUtilities.PlusFormatNumber(mod) + "): ", false);
                    }


                }

                private string SaveName(Monster.SaveType type)
                {
                    switch (type)
                    {
                        case Monster.SaveType.Fort:
                            return "Fort";
                        case Monster.SaveType.Ref:
                            return "Ref";
                        default:
                            return "Will";
                    }
                } 
            #endregion

            #region Combat Maneuver Roll Menu Support Code

                private void RollCombatManeuvers(List<Character> list, string manoeuvreType)
                {
                    if (list.Count > 0)
                    {
                        Paragraph p = new Paragraph { Margin = new Thickness(0) };

                        p.Inlines.Add(new Underline(new Run(manoeuvreType + (list.Count > 1 ? "s" : ""))));
                        DieRollDocument.Blocks.Add(p);
                    }
                    foreach (Character ch in list)
                    {
                        RollCombatManeuver(ch, manoeuvreType);
                    }
                }
                private void RollCombatManeuver(Character ch, string maneuverType)
                {
                    int? mod = ch.Monster.GetManoeuver(maneuverType);
                    if (mod == null) return;
                    DieRoll roll = UserSettings.Settings.AlternateInit3d6 ? UserSettings.Settings.AlternateInitDieRoll : new DieRoll(1, 20, (int)mod);
                    RollDie(roll, ch.Name + " (" + CMStringUtilities.PlusFormatNumber(mod) + "): ", false);
                }
                private void RollManeuverMenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
                {
                    SetupManeuverMenuItem((MenuItem)sender);

                }
                private void RollCombatManeuverMenuItem_Loaded(object sender, RoutedEventArgs e)
                {
                    SetupManeuverMenuItem((MenuItem)sender);
                }
                private void SetupManeuverMenuItem(MenuItem menuItem)
                {
                    menuItem.Items.Clear();

                    foreach (var cm in Monster.CombatManeuvers.Select(Maneuver => new MenuItem { Header = Maneuver, Tag = Maneuver }))
                    {

                        cm.Click += ManeuverMenuItemClick;
                        menuItem.Items.Add(cm);
                    }
                }
                void ManeuverMenuItemClick(object sender, RoutedEventArgs e)
                {

                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                    var mt = (string)mi.Tag;
                    RollCombatManeuvers(list, mt);
                }
        
            #endregion

            #region Ability Check Roll Menu Support Code
                    private void RollAbilityCheckMenuItem_Loaded(object sender, RoutedEventArgs e)
                        {
                            SetupAbilityCheckMenuItem((MenuItem)sender);
                        }
                    private void SetupAbilityCheckMenuItem(MenuItem menuItem)
                        {
                                menuItem.Items.Clear();
                                var Stats = Enum.GetNames(typeof(Stat));
                            foreach (var Statistic in Stats.Select(Statistics => new MenuItem {Header = Statistics, Tag = Statistics}))
                                {
                                    Statistic.Click += AbilityCheckMenuItemClick;
                                    menuItem.Items.Add(Statistic);
                                }

                        }
                    void AbilityCheckMenuItemClick(object sender, RoutedEventArgs e)
                        {
                            MenuItem mi = (MenuItem)sender;

                            List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                            var mt = (string)mi.Tag;
                            RollAbilityCheck(list, mt);
                        }
                    private void RollAbilityCheck(List<Character> list, string Ability)
                    {
                        if (list.Count > 0)
                        {
                            Paragraph p = new Paragraph { Margin = new Thickness(0) };

                            p.Inlines.Add(new Underline(new Run(Ability + (list.Count > 1 ? "s" : ""))));
                            DieRollDocument.Blocks.Add(p);
                        }
                        foreach (Character ch in list)
                            {
                                RollAbilityCheck(ch, Ability);
                            }
                    }
                    private void RollAbilityCheck(Character ch, string Ability)
                    {   
                        int? mod = Monster.AbilityBonus(ch.Monster.GetStat((Stat)Enum.Parse(typeof(Stat),Ability)));
                        DieRoll roll = UserSettings.Settings.AlternateInit3d6 ? UserSettings.Settings.AlternateInitDieRoll : new DieRoll(1, 20, (int)mod);
                        RollDie(roll, ch.Name + " (" + CMStringUtilities.PlusFormatNumber(mod) + "): ", false);
                    }
                #endregion

            #region Skill Roll Menu Support Code
                private void RollSkillMenuItem_Loaded(object sender, RoutedEventArgs e)
                {
                    SetupSkillsMenuItem((MenuItem)sender);
                }

                private void RollSkillMenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
                {
                    SetupSkillsMenuItem((MenuItem)sender);

                }

                void SetupSkillsMenuItem(MenuItem item)
                {
                    item.Items.Clear();

                    foreach (Monster.SkillInfo info in Monster.SkillsDetails.Values)
                    {
                        MenuItem mi = new MenuItem();
                        mi.Header = info.Name;
                        mi.Tag = info;

                        if (info.Subtypes != null && info.Subtypes.Count > 0)
                        {
                            foreach (string subtype in info.Subtypes)
                            {
                                MenuItem si = new MenuItem();
                                si.Header = subtype;
                                SkillValue s = new SkillValue(info.Name);
                                s.Subtype = subtype;
                                si.Tag = s;
                                si.Click += new RoutedEventHandler(SkillSubtypeMenuItemClick);

                                mi.Items.Add(si);

                            }
                        }
                        else
                        {
                            mi.Click += new RoutedEventHandler(SkillMenuItemClick);
                        }

                        item.Items.Add(mi);
                    }
                }

                void SkillMenuItemClick(object sender, RoutedEventArgs e)
                {

                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);
                    RollSkillCheck(list, ((Monster.SkillInfo)mi.Tag).Name, null);
                }

                void SkillSubtypeMenuItemClick(object sender, RoutedEventArgs e)
                {

                    MenuItem mi = (MenuItem)sender;

                    List<Character> list = GetViewSelectedCharactersFromChar((Character)mi.DataContext);


                    SkillValue v = (SkillValue)mi.Tag;

                    RollSkillCheck(list, v.Name, v.Subtype);
                }

                private void RollSkillCheck(List<Character> list, string skill, string subtype)
                {
                    if (list.Count > 0)
                    {
                        Paragraph p = new Paragraph();
                        p.Margin = new Thickness(0);

                        string name = skill;

                        if (subtype != null)
                        {
                            name += " " + subtype;
                        }

                        p.Inlines.Add(new Underline(new Run(name + " check" + (list.Count > 1 ? "s" : ""))));
                        DieRollDocument.Blocks.Add(p);
                    }

                    foreach (Character ch in list)
                    {
                        RollSkillCheck(ch, skill, subtype);
                    }
                }

                private void RollSkillCheck(Character ch, string skill, string subtype)
                {
                    Monster.SkillInfo info = Monster.SkillsDetails[skill];
                    SkillValue val = new SkillValue(skill, subtype);

                    if (!info.TrainedOnly || ch.Monster.SkillValueDictionary.ContainsKey(val.FullName))
                    {

                        int mod = ch.Monster.GetSkillMod(skill, subtype);


                        DieRoll roll;
                        if (UserSettings.Settings.AlternateInit3d6)
                        {

                            roll = UserSettings.Settings.AlternateInitDieRoll;
                        }
                        else
                        {
                            roll = new DieRoll(1, 20, mod);
                        }

                        RollDie(roll, ch.Name + " (" + CMStringUtilities.PlusFormatNumber(mod) + "): ", false);
                    }
                    else
                    {
                        Paragraph p = new Paragraph();
                        p.Margin = new Thickness(0);


                        p.Inlines.Add(new Italic(new Run(ch.Name + " Untrained")));
                        DieRollDocument.Blocks.Add(p);
                    }

                    DieRollViewer.ScrollChildToBottom();
                }
        
            #endregion

            #region Attack Roll Menu Support Code

                    private void RollAttacksMenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
                    {
                        UpdateRollAttacksMenuItem((MenuItem)sender);
                    }
                    private void RollAttacksMenuItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
                    {
                        UpdateRollAttacksMenuItem((MenuItem)sender);
                    }
                    private class AttackSetRollInfo
                    {

                        public List<Character> Characters { get; set; }
                        public AttackSet Attacks;
                    }
                    private class AttackRollInfo
                    {
                        public List<Character> Characters { get; set; }
                        public Attack Attack;
                    }
                    private void UpdateRollAttacksMenuItem(MenuItem sender)
                    {
                        MenuItem attacksItem = (MenuItem)sender;
                        attacksItem.Items.Clear();

                        if (attacksItem.DataContext is Character)
                        {
                            Character ch = (Character)attacksItem.DataContext;

                            List<Character> all = GetViewSelectedCharacters(sender);
                            Dictionary<string, List<Character>> meleesets = new Dictionary<string, List<Character>>();
                            Dictionary<string, List<Character>> rangedsets = new Dictionary<string, List<Character>>();

                            foreach (Character c in all)
                            {
                                List<AttackSet> melee = c.Monster.MeleeAttacks;
                                foreach (AttackSet set in melee)
                                {
                                    string s = set.ToString();
                                    if (!meleesets.ContainsKey(s))
                                    {
                                        meleesets[s] = new List<Character>();
                                    }

                                    meleesets[s].Add(c);
                                }

                                List<Attack> ranged = c.Monster.RangedAttacks;
                                foreach (Attack set in ranged)
                                {
                                    string s = set.ToString();
                                    if (!rangedsets.ContainsKey(s))
                                    {
                                        rangedsets[s] = new List<Character>();
                                    }

                                    rangedsets[s].Add(c);
                                }



                            }

                            if (ch != null)
                            {
                                List<AttackSet> melee = ch.Monster.MeleeAttacks;

                                List<Attack> ranged = ch.Monster.RangedAttacks;

                                foreach (AttackSet set in melee)
                                {
                                    MenuItem mi = new MenuItem();
                                    string s = set.ToString();
                                    mi.Header = set.ToString();

                                    if (meleesets[s] != null && meleesets[s].Count > 1)
                                    {

                                        mi.SetNamedIcon("clone");
                                    }
                                    else
                                    {
                                        mi.SetNamedIcon("sword");

                                    }
                                    AttackSetRollInfo ri = new AttackSetRollInfo();
                                    ri.Characters = meleesets[s];
                                    ri.Characters.Sort((a, b) => String.Compare(a.Name, b.Name, true));
                                    ri.Attacks = set;
                                    mi.Tag = ri;

                                    mi.Click += new RoutedEventHandler(RollMeleeAttackItem_Click);
                                    attacksItem.Items.Add(mi);
                                }

                                foreach (Attack attack in ranged)
                                {
                                    MenuItem mi = new MenuItem();
                                    string s = attack.ToString();
                                    mi.Header = attack.ToString();
                                    if (rangedsets[s].Count > 1)
                                    {

                                        mi.SetNamedIcon("clone");
                                    }
                                    else
                                    {

                                        mi.SetNamedIcon("bow");

                                    }

                                    AttackRollInfo ri = new AttackRollInfo();
                                    ri.Characters = rangedsets[s];
                                    ri.Characters.Sort((a, b) => String.Compare(a.Name, b.Name, true));
                                    ri.Attack = attack;
                                    mi.Tag = ri;
                                    mi.Click += new RoutedEventHandler(RollRangedAttackItem_Click);
                                    attacksItem.Items.Add(mi);

                                }
                            }

                        }
                    }
                    public Button CreateAttackCharacterHeader(Character ch)
                    {

                        Paragraph p = new Paragraph();
                        Button b = AddDieRollButton(p);

                        p.Background = new SolidColorBrush((Color)FindResource("SecondaryColorBDarker"));

                        p.Margin = new Thickness(0, 2, 0, 0);
                        b.Margin = new Thickness(3, 3, 1, 0);

                        Run r = new Run(ch.Name);
                        r.FontWeight = FontWeights.Bold;

                        r.Foreground = new SolidColorBrush(Colors.White);
                        r.BaselineAlignment = BaselineAlignment.Center;
                        p.Inlines.Add(r);
                        DieRollDocument.Blocks.Add(p);

                        return b;
                    }
                    void RollMeleeAttackItem_Click(object sender, RoutedEventArgs e)
                    {
                        FrameworkElement mi = (FrameworkElement)sender;
                        Character ch = (Character)mi.DataContext;
                        AttackSetRollInfo ri = (AttackSetRollInfo)mi.Tag;
                        AttackSet set = ri.Attacks;

                        List<Attack> attacks = new List<Attack>();

                        attacks.AddRange(set.WeaponAttacks);
                        attacks.AddRange(set.NaturalAttacks);

                        foreach (Character c in ri.Characters)
                        {
                            Button b = CreateAttackCharacterHeader(c);
                            AttackSetRollInfo cri = new AttackSetRollInfo();
                            cri.Characters = new List<Character>() { c };
                            cri.Attacks = ri.Attacks;
                            b.Tag = cri;

                            b.Click += RollMeleeAttackItem_Click;

                            foreach (Attack atk in attacks)
                            {
                                RollAttack(c, atk);
                            }
                        }
                    }
                    void RollRangedAttackItem_Click(object sender, RoutedEventArgs e)
                    {
                        FrameworkElement mi = (FrameworkElement)sender;
                        Character ch = (Character)mi.DataContext;
                        var ri = (AttackRollInfo)mi.Tag;



                        Attack atk = (Attack)ri.Attack;

                        foreach (Character c in ri.Characters)
                        {

                            Button b = CreateAttackCharacterHeader(c);
                            AttackRollInfo cri = new AttackRollInfo();
                            cri.Characters = new List<Character>() { c };
                            cri.Attack = ri.Attack;
                            b.Tag = cri;
                            b.Click += RollRangedAttackItem_Click;

                            RollAttack(c, atk);
                        }
                    }
                    void RollAttack(Character ch, Attack atk)
                    {
                        Paragraph p = new Paragraph();
                        p.Margin = new Thickness(0);

                        string attackname = atk.Name;

                        if (atk.Weapon != null)
                        {
                            attackname = atk.Weapon.Name;
                        }


                        p.Inlines.Add(new Underline(new Run(StringCapitalizeConverter.Capitalize(attackname))));

                        int totalattacks = atk.Count * atk.Bonus.Count;


                        for (int atkcount = 0; atkcount < atk.Count; atkcount++)
                        {
                            foreach (int mod in atk.Bonus)
                            {
                                if (totalattacks > 0)
                                {
                                    if (atk.Count > 0)
                                    {
                                        p.Inlines.Add(new LineBreak());
                                    }
                                }

                                DieRoll roll;
                                if (UserSettings.Settings.AlternateInit3d6)
                                {

                                    roll = UserSettings.Settings.AlternateInitDieRoll;
                                }
                                else
                                {
                                    roll = new DieRoll(1, 20, mod);
                                }

                                RollResult res = roll.Roll();

                                RollResult dmg = atk.Damage.Roll();

                                RollResult bonusDmg = null;
                                String bonusType = null;
                                DieRoll bonusRoll = null;
                                if (atk.Plus != null)
                                {
                                    Regex plusRegex = new Regex("(?<die>[0-9]+d[0-9]+((\\+|-)[0-9]+)?) (?<type>[a-zA-Z]+)");
                                    Match dm = plusRegex.Match(atk.Plus);
                                    if (dm.Success)
                                    {
                                        bonusRoll = DieRoll.FromString(dm.Groups["die"].Value);
                                        bonusDmg = bonusRoll.Roll();
                                        bonusType = dm.Groups["type"].Value;
                                    }
                                }

                                p.Inlines.Add(new Run(CMStringUtilities.PlusFormatNumber(mod) + " hit "));

                                int actualDie = 0;
                                foreach (DieResult val in res.Rolls)
                                {
                                    actualDie += val.Result;
                                }
                                if (actualDie >= atk.CritRange || actualDie == 1)
                                {
                                    string text = res.Total.ToString() + " (" + actualDie + ")";
                                    Inline co = CreateRollElement(text, Colors.White,
                                        (actualDie == 1) ? Colors.Red : Colors.Green);
                                    p.Inlines.Add(co);
                                    p.Inlines.Add(" ");

                                }
                                else
                                {
                                    Inline co = CreateRollElement(res.Total.ToString());
                                    p.Inlines.Add(co);
                                    p.Inlines.Add(" ");
                                    p.Inlines.Add(new Run("(" + actualDie + ") "));
                                }


                                if (actualDie != 1)
                                {
                                    p.Inlines.Add(new Run("dmg "));

                                    string dmgtext = "";

                                    p.Inlines.Add(" ");
                                    foreach (DieResult dmgroll in dmg.Rolls)
                                    {
                                        if (dmgtext.Length > 0)
                                        {
                                            dmgtext += "+";
                                        }
                                        dmgtext += dmgroll.Result;
                                    }
                                    if (dmg.Mod != 0)
                                    {
                                        dmgtext += "+";
                                        dmgtext += dmg.Mod;

                                    }

                                    p.Inlines.Add(CreateRollElement(dmg.Total.ToString(), Colors.White, Colors.Blue, dmgtext));




                                    if (UserSettings.Settings.ShowAllDamageDice)
                                    {
                                        p.Inlines.Add(new Run("(" + dmgtext + ")"));
                                    }

                                    if (bonusRoll != null)
                                    {
                                        p.Inlines.Add(new Run("+"));
                                        p.Inlines.Add(CreateRollElement(bonusDmg.Total.ToString(), Colors.White, Colors.DarkMagenta));
                                        p.Inlines.Add(" " + bonusType);
                                    }



                                    if (res.Rolls[0].Result >= atk.CritRange)
                                    {
                                        RollResult critRes = roll.Roll();
                                        int actualCrit = critRes.Rolls[0].Result;

                                        int critTotal = dmg.Total;

                                        for (int i = 1; i < atk.CritMultiplier; i++)
                                        {
                                            RollResult crit = atk.Damage.Roll();
                                            critTotal += crit.Total;

                                            foreach (DieResult dmgroll in crit.Rolls)
                                            {
                                                dmgtext += "+";
                                                dmgtext += dmgroll.Result;
                                            }

                                            if (crit.Mod != 0)
                                            {
                                                dmgtext += "+";
                                                dmgtext += crit.Mod;

                                            }

                                        }

                                        string text = " Crit: ";
                                        text += critRes.Total;
                                        text += " (" + actualCrit + ")";
                                        if (actualCrit != 1)
                                        {
                                            text += " dmg " + critTotal;

                                            if (UserSettings.Settings.ShowAllDamageDice)
                                            {
                                                text += " (" + dmgtext + ")";
                                            }


                                        }


                                        p.Inlines.Add(new Italic(new Run(text)));
                                    }

                                }

                            }
                        }


                        DieRollDocument.Blocks.Add(p);
                        DieRollViewer.ScrollChildToBottom();
                    }

            #endregion

            private void LoadRecentDieRolls()
            {
                _RecentDieRolls = XmlListLoader<string>.Load("DieRolls.xml", true);

                if (_RecentDieRolls == null)
                {
                    _RecentDieRolls = new List<string>();
                }

                UpdateDieRollCombo();
            }

            private void SaveRecentDieRolls()
            {
                XmlListLoader<string>.Save(_RecentDieRolls, "DieRolls.xml", true);
            }

            private void AddRecentDieRoll(DieRoll roll)
            {
                if (roll != null)
                {
                    string text = roll.Text;

                    _RecentDieRolls.RemoveAll(a => a == text);
                    _RecentDieRolls.Insert(0, text);

                    while (_RecentDieRolls.Count > 20)
                    {
                        _RecentDieRolls.RemoveAt(20);
                    }

                    SaveRecentDieRolls();
                    UpdateDieRollCombo();
                }

            }

            private void UpdateDieRollCombo()
            {
                string current = DieRollText.Text;

                DieRollText.Items.Clear();

                foreach (string text in _RecentDieRolls)
                {
                    DieRollText.Items.Add(text);
                }

                if (DieRollText.Text != current)
                {
                    DieRollText.Text = current;
                }
            }
        
        #endregion



        private void ResetMagicItemsFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMagicItemFilters();
        }

        private void ResetMagicItemFilters()
        {

            MagicItemGroupFilterComboBox.SelectedIndex = 0;
            MagicItemCLFilterComboBox.SelectedIndex = 0;
        }

        private void ResetFeatsFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FeatTypeFilterComboBox.SelectedIndex = 0;
        }

        private void ResetMonstersFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MonsterTabNPCComboBox.SelectedIndex = 0;
            MonsterTypeFilterComboBox.SelectedIndex = 0;
            MonsterCRComboBox.SelectedIndex = 0;
            BetweenCRLowCombo.SelectedIndex = 0;
            BetweenCRHighCombo.SelectedIndex = BetweenCRHighCombo.Items.Count - 1 ;

        }

        private void ResetRulesFilterButton_Click(object sender, RoutedEventArgs e)
        {
            RuleTypeFilterComboBox.SelectedIndex = 0;
            RuleSubtypeFilterComboBox.SelectedIndex = 0;
        }

        private void ResetSpelsFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSpellFilters();
        }



        private void LoadCampaignInfo()
        {
            campaignInfo = XmlLoader<CampaignInfo>.Load("CurrentCampaignInfo.xml", true);

            if (campaignInfo == null)
            {
                campaignInfo = new CampaignInfo();
            }
			
			campaignInfo.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(campaignInfo_PropertyChanged);
        }

        private void SaveCampaignInfo()
        {
            XmlLoader<CampaignInfo>.Save(campaignInfo, "CurrentCampaignInfo.xml", true);
        }



        private void MenuItem_IdleMonster(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character root = (Character)((FrameworkElement)sender).DataContext;

                List<Character> list = GetViewSelectedCharactersFromChar(root);

                bool newState = !root.IsIdle;

                foreach (Character ch in list)
                {
                    ch.IsIdle = newState;
                }


                combatView.Refresh();
                combatState.FilterList();
            }
        }

        private void MenuItem_HideMonster(object sender, RoutedEventArgs e)
        {
            using (var undoGroup = undo.CreateUndoGroup())
            {
                Character root = (Character)((FrameworkElement)sender).DataContext;

                List<Character> list = GetViewSelectedCharactersFromChar(root);

                bool newState = !root.IsHidden;

                foreach (Character ch in list)
                {
                    ch.IsHidden = newState;
                }

                combatView.Refresh();
                combatState.FilterList();
            }

        }

        private void CustomizeSpellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Spell s = (Spell)spellsView.CurrentItem;

            if (s != null)
            {

                SpellEditorWindow w = new SpellEditorWindow();
                w.Spell = (Spell)s.Clone();
                w.Spell.DBLoaderID = 0;
                w.Owner = this;

                if (w.ShowDialog() == true)
                {
					if (s.description != w.Spell.description)
					{
						w.Spell.description_formated = null;	
					}

                    Spell.AddCustomSpell(w.Spell);
                    MakeSpellVisible(w.Spell);
                }
            }
            
        }

        private void EditCustomSpellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Spell s = (Spell)spellsView.CurrentItem;

            if (s != null)
            {

                SpellEditorWindow w = new SpellEditorWindow();
                w.Spell = (Spell)s.Clone();
                w.Owner = this;

                if (w.ShowDialog() == true)
                {
					if (s.description != w.Spell.description)
					{
						w.Spell.description_formated = null;	
					}
					
                    s.CopyFrom(w.Spell);
                    Spell.UpdateCustomSpell(s);
                    UpdateSpellFlowDocument();
                }
            }
        }

        private void DeleteCustomSpellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {


            Spell s = (Spell)spellsView.CurrentItem;

            if (s != null)
            {

                Spell.RemoveCustomSpell(s);
            }
        }

        private void NewCustomSpellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SpellEditorWindow w = new SpellEditorWindow();
            w.Spell = new Spell();
            w.Spell.school = "abjuration";
            w.Spell.saving_throw = "none";
            w.Spell.spell_resistence = "no";
            w.Spell.casting_time = "1 round";
            w.Owner = this;

            if (w.ShowDialog() == true)
            {
            	Spell.AddCustomSpell(w.Spell);
                MakeSpellVisible(w.Spell);
            }
        }

        private void NewCustomMonsterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Monster m = new Monster();

            MonsterEditorWindow w = new MonsterEditorWindow();

            w.Owner = this;
            w.Monster = Monster.BlankMonster();
            if (w.ShowDialog() == true)
            {
                m.DBLoaderID = 0;
                m = AddNewCustomMonster(w.Monster);
                MakeMonsterVisible(m);
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            Grid g = new Grid();
            ((Panel)DiceRollerGrid.Parent).Children.Remove(DiceRollerGrid);
            g.Children.Add(DiceRollerGrid);
            w.Content = g;
            w.Owner = this;
            w.Show();

        }

        private void ImportDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = exportFileFilter;
            if (fd.ShowDialog() == true)
            {
                ImportDateFromFile(fd.FileName);

            }
        }

        private void ImportDateFromFile(string filename)
        {
            ExportData x = XmlLoader<ExportData>.Load(filename);
            ExportDialog dlg = new ExportDialog(x);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowDialog();
        }

        private void ExportDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog d = new ExportDialog();
            d.Owner = this;
            d.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            d.ShowDialog();
        }

        private void CustomizeFeatButton_Click(object sender, RoutedEventArgs e)
        {
            Feat f = (Feat)featsView.CurrentItem;

            if (f != null)
            {
                FeatEditorWindow win = new FeatEditorWindow();
                win.Feat = new Feat(f);
                win.Owner = this;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (win.ShowDialog() == true)
                {
                    win.Feat.DBLoaderID = 0;
                    Feat.AddCustomFeat(win.Feat);
                    featsView.Refresh();
                }
            }
        }

        private void DeleteCustomFeatButton_Click(object sender, RoutedEventArgs e)
        {
            Feat f = (Feat)featsView.CurrentItem;

            if (f != null)
            {
                Feat.RemoveCustomFeat(f);
            }

        }

        private void EditCustomFeatButton_Click(object sender, RoutedEventArgs e)
        {
            Feat f = (Feat)featsView.CurrentItem;

            if (f != null)
            {

                FeatEditorWindow w = new FeatEditorWindow();
                w.Feat = (Feat)f.Clone();
                w.Owner = this;

                if (w.ShowDialog() == true)
                {

                    f.CopyFrom(w.Feat);
                    Feat.UpdateCustomFeat(f);
                    UpdateFeatFlowDocument();
                    featsView.Refresh();
                }
            }
        }

        private void NewCustomFeatButton_Click(object sender, RoutedEventArgs e)
        {
            Feat f = new Feat();
            f.Type = "General";
            

            FeatEditorWindow win = new FeatEditorWindow();
            win.Feat = new Feat(f);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (win.ShowDialog() == true)
            {
                Feat.AddCustomFeat(win.Feat);
                featsView.Refresh();
            }
            
        }

        private void RestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RestoreDefaultLayout();
        }

        private void StartService()
        {
            /*try
            {
                _CombatViewService = new ServiceHost(typeof(CombatViewService.CombatStateService));
                _CombatViewService.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
                _CombatViewService.Open();

                CombatViewService.CombatStateService.State = this.combatState;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _CombatViewService = null;
            }*/
        }

        private void StopService()
        {
            if (_CombatViewService != null)
            {
                try
                {
                    _CombatViewService.Close();
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
                _CombatViewService = null;
            }
        }

        private void CombatStateServiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_CombatViewService == null)
            {
                StartService();
                UserSettings.Settings.RunCombatViewService = true;
            }
            else
            {
                StopService();
                UserSettings.Settings.RunCombatViewService = false;
            }
        }

        private void AmountComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            while (box.Items.Count < 101)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "x" + (box.Items.Count);
                box.Items.Add(item);
            }
        }

        private void ItemGenerateCountComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            while (box.Items.Count < 100)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = (box.Items.Count +1).ToString();
                box.Items.Add(item);
            }
        }

        #region Campaign Section

        private void Add100YearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(100);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Remove100YearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(-100);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }


        private void Add10YearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(10);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Remove10YearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(-10);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }


        private void AddYearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void RemoveYearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddYears(-1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void AddMonthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMonths(1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void RemoveMonthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMonths(-1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void AddDayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddDays(1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void RemoveDayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddDays(-1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void AddHourButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddHours(1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void RemoveHourButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddHours(-1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Add10MinuteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMinutes(15);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Remove10MinuteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMinutes(-15);
                SaveCampaignInfo();

            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Add1MinuteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMinutes(1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Remove1MinuteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddMinutes(-1);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Add6SecButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddSeconds(6);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Remove6SecButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                campaignInfo.CurrentDate = campaignInfo.CurrentDate.AddSeconds(-6);
                SaveCampaignInfo();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void UpdateCampaignDayView()
        {
            campaignDayView = new ListCollectionView(campaignInfo.EventsForDate(campaignInfo.SelectedDate));
            CampaignEventListBox.DataContext = campaignDayView;
        }

        private void GoToCurrentDate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            campaignInfo.SelectedDate = campaignInfo.CurrentDate;
            campaignInfo.DisplayDate = campaignInfo.CurrentDate;
        }

        private void campaignInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDate")
            {
                UpdateCampaignDayView();
            }
        }

        private void AddCampaignEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CampaignEventWindow w = new CampaignEventWindow();
            CampaignEvent ce = new CampaignEvent();
            ce.Start = campaignInfo.SelectedDate;
            ce.End = ce.Start.AddHours(1);
            w.Event = ce;
            w.Owner = this;
            if (w.ShowDialog() == true)
            {
                campaignInfo.AddEvent(w.Event);
                SaveCampaignInfo();
            }
        }
        
        #endregion

		private void NameTextBox_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

		private void NameTextBox_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://combatmanager.com");
        }

        private void PerformUpdateCheck()
        {
            if (UserSettings.Settings.CheckForUpdates)
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        HttpWebRequest rq1 = (HttpWebRequest)WebRequest.Create("http://CombatManager.com/version.xml");
                        HttpWebResponse rsp1 = (HttpWebResponse)rq1.GetResponse();

                        XDocument doc = XDocument.Load(rsp1.GetResponseStream());

                        String val = doc.Root.Value;

                        rsp1.Close();

                        Version remotev = Version.Parse(val);
                        Version localv = Assembly.GetExecutingAssembly().GetName().Version;

                        if (remotev.CompareTo(localv) > 0)
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    UpdateButton.Visibility = System.Windows.Visibility.Visible;
                                }));
                        }



                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error loading version: \r\n" + ex.ToString());
                    }
                });
                t.Start();
            }
            
        }
        #region Hotkey Section

        private void HotKeysButton_Click(object sender, RoutedEventArgs e)
        {
            var hkd = new HotKeysDialog();
            hkd.Owner = this;
            if (_CombatHotKeys == null)
            {
                _CombatHotKeys = new List<CombatHotKey>();
            }
            hkd.CombatHotKeys = _CombatHotKeys;
            if (hkd.ShowDialog() == true)
            {
                _CombatHotKeys = hkd.CombatHotKeys;
                SaveHotkeys();
            }

        }

        private void LoadHotkeys()
        {
            _CombatHotKeys = XmlListLoader<CombatHotKey>.Load("CombatHotKeys.xml", true);
            UpdateHotKeys();
        }

        private void SaveHotkeys()
        {
            if (_CombatHotKeys != null)
            {
                XmlListLoader<CombatHotKey>.Save(_CombatHotKeys, "CombatHotKeys.xml", true);
            }
            UpdateHotKeys();
        }

        private void UpdateHotKeys()
        {
            if (_CombatHotKeys != null)
            {
                foreach (InputBinding key in _HotKeys)
                {
                    InputBindings.Remove(key);
                }
                _HotKeys.Clear();
                foreach (CombatHotKey hk in _CombatHotKeys)
                {
                    CombatHotKey chk = hk;
                    try
                    {
                        InputBinding ib = new KeyBinding(new RelayCommand((object x) =>
                        {
                            TakeHotkeyAction(chk);
                        }), hk.Key, hk.Modifier);
                        _HotKeys.Add(ib);
                        InputBindings.Add(ib);
                    }
                    catch (NotSupportedException)
                    {

                    }
                }

            }

        }

        private void TakeHotkeyAction(CombatHotKey chk)
        {
            //get targets
            Character ch = currentViewCharacter;

            if (ch != null)
            {
                List<Character> list = GetViewSelectedCharactersFromChar(ch);

                foreach (Character c in list)
                {
                    //run command
                    switch (chk.Type)
                    {
                        case CombatHotKeyType.MeleeAttack:
                            List<AttackSet> latk = c.Monster.MeleeAttacks;
                            if (latk.Count > 0)
                            {
                                AttackSet set = latk[0];

                                List<Attack> attacks = new List<Attack>();

                                attacks.AddRange(set.WeaponAttacks);
                                attacks.AddRange(set.NaturalAttacks);


                                foreach (Attack atk in attacks)
                                {
                                    RollAttack(c, atk);
                                }
                            }

                            break;
                        case CombatHotKeyType.RangedAttack:
                            List<Attack> ran = c.Monster.RangedAttacks;
                            if (ran != null && ran.Count > 0)
                            {

                                foreach (Attack atk in ran)
                                {
                                    RollAttack(c, atk);
                                }
                            }

                            break;
                        case CombatHotKeyType.Save:
                            if (chk.Subtype == "Fort")
                            {
                                RollSave(c, Monster.SaveType.Fort);
                            }
                            else if (chk.Subtype == "Ref")
                            {
                                RollSave(c, Monster.SaveType.Ref);
                            }
                            else if (chk.Subtype == "Will")
                            {
                                RollSave(c, Monster.SaveType.Will);
                            }
                            break;
                        case CombatHotKeyType.Skill:
                            RollSkillCheck(c, chk.Subtype, null);
                            break;
                    }
                }
            }


        }

        #endregion
        private void RollInitWithoutResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RollInitiativeWithoutReset();


            combatState.SortCombatList();
        }
        private void RollInitWithoutSortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RollInitiative();


        }
        private void RollInitWithoutResetSortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RollInitiativeWithoutReset();


        }

        #region Tracked Resource Section

        private void DescreaseResourceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveResource r = (ActiveResource)((FrameworkElement)sender).DataContext;
            r.Current = Math.Max(0, r.Current - 1);
        }

        private void IncreaseResourceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            ActiveResource r = (ActiveResource)((FrameworkElement)sender).DataContext;
            r.Current++;
        }

        private void DeleteResourceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameworkElement el = ((FrameworkElement)sender);

            ListBox box = el.FindVisualParent<ListBox>();
            Character c = (Character)box.DataContext;
            c.Resources.Remove((ActiveResource)el.DataContext);
        }

        private void AddResourceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Character c = (Character)((FrameworkElement)sender).DataContext;
            c.Resources.Add(new ActiveResource() { Name = "Resource", Current = 0 });
        }
        
        #endregion

        private void BookmarkFeatButton_Click(object sender, RoutedEventArgs e)
        {

            Feat f = (Feat)featsView.CurrentItem;
            if (f != null)
            {
                BookmarkList.List.AddFeat(f);
            }
        }

        private void StatsCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSettings.Settings.StatsOpenByDefault)
            {
                ((ToggleButton)sender).IsChecked = true;
            }
        }

        private void OgrekinRandomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int beneficial = rand.Next(6);
            int disadventageous = rand.Next(6);
            OgrekinBeneficialCombo.SelectedIndex = beneficial;
            OgrekinDisadvantageousCommbo.SelectedIndex = disadventageous;
        }

        private void ColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Character root = (Character)((FrameworkElement)sender).DataContext;

            List<Character> list = GetViewSelectedCharactersFromChar(root);

            bool newState = !root.IsHidden;

            uint? color = null;

            if (mi.Name == "RedColorMenuItem")
            {
                color =  0xffff0000;
            }

            foreach (Character ch in list)
            {
                ch.Color = color;
            }

        }




    }

    public class RelayCommand : ICommand 
    { 


        readonly Action<object> _execute; readonly Predicate<object> _canExecute; 

        public RelayCommand(Action<object> execute) : this(execute, null) { } 
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        { 
            if (execute == null) throw new ArgumentNullException("execute"); 
            _execute = execute; _canExecute = canExecute; 
        } 
        [DebuggerStepThrough] 
        public bool CanExecute(object parameter) 
        { 
            return _canExecute == null ? true : _canExecute(parameter); 
        } 
        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            } 
        } 
        public void Execute(object parameter) 
        { 
            _execute(parameter); 
        } 
    }

}

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

﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ServiceModel;
using CombatManager;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;


namespace CombatStateViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, CombatStateService.ICombatStateServiceCallback
    {
        CombatStateService.CombatStateServiceClient serviceClient;

        List<Character> _Characters;
        List<CombatStateService.SimpleCombatListItem> _GuidCombatList;
        List<Character> _CombatList;
        Guid _CurrentGuid;
        Character _CurrentCharacter;

        object flagSec = new object();
        bool needsCharacters = false;
        bool needsCombatList = false;

        public MainWindow()
        {
            CombatManager.DBSettings.UseDB = false;

            InitializeComponent();

        }

        bool OpenServiceClient(string server)
        {
            try
            {

                NetTcpBinding b = new NetTcpBinding();
                b.Security.Mode = SecurityMode.None;
               

                string address = "net.tcp://" + server + ":8001/CombatStateService";
                if (Regex.Match(server, ".+:[0-9]+").Success)
                {
                    address = "net.tcp://" + server + "/CombatStateService";
                }

                EndpointAddress a = new EndpointAddress(address);


                serviceClient = new CombatStateService.CombatStateServiceClient(new InstanceContext(this), b, a);
                serviceClient.Open();

                return serviceClient.State == CommunicationState.Opened;
            }
            catch (System.UriFormatException)
            {
                MessageBox.Show("Invalid server name");
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                MessageBox.Show("Unable to locate requested server");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return false;
        }

        protected override void OnClosed(EventArgs e)
        {
            CloseSeviceClient();
        }

        void CloseSeviceClient()
        {

            if (serviceClient != null && serviceClient.State == CommunicationState.Opened)
            {
                serviceClient.Close();
            }
            serviceClient = null;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	MessageBox.Show(CombatListBox.Items.Count + "");
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (serviceClient == null || serviceClient.State != CommunicationState.Opened)
            {
                string server = ServerNameBox.Text.Trim();

                if (server.Length > 0)
                {

                    if (OpenServiceClient(server))
                    {

                        _Characters = new List<Character>(serviceClient.GetCharacters());

                        CombatStateService.SimpleCombatListItem[] items = serviceClient.GetCombatList();
                        _GuidCombatList = new List<CombatStateService.SimpleCombatListItem>(items);

                        _CombatList = new List<Character>();

                        foreach (Guid g in from r in _GuidCombatList select r.ID)
                        {
                            _CombatList.Add(_Characters.FirstOrDefault(a => a.ID == g));
                        }

                        _CurrentGuid = serviceClient.GetCurrentCharacterID();
                        _CurrentCharacter = _Characters.FirstOrDefault(a => a.ID == _CurrentGuid);

                        ActiveCharacterBorder.DataContext = _CurrentCharacter;

                        CombatListBox.DataContext = _CombatList;
                        ConnectButton.Content = "Disconnect";
                    }
                }

            }
            else
            {
                CloseSeviceClient();
                ConnectButton.Content = "Connect";
            }
        }

        delegate void GuidDelegate(Guid id);
        delegate void EventDelegate();

        public void CurrentPlayerChanged(Guid id)
        {
            Dispatcher.BeginInvoke(new GuidDelegate(CurrentPlayerChangedInvoke), DispatcherPriority.Normal, new object[] { id });
            

        }
        public void CurrentPlayerChangedInvoke(Guid id)
        {

            ActiveCharacterBorder.DataContext = _Characters.FirstOrDefault(a => a.ID == id);
        }

        public void CombatListChanged()
        {
            lock (flagSec)
            {
                needsCombatList = true;
            }
            Dispatcher.BeginInvoke(new EventDelegate(CombatListChangedInvoke), DispatcherPriority.Normal);
            

        }

        public void CombatListChangedInvoke()
        {
            bool load = false;

            lock (flagSec)
            {
                if (needsCombatList)
                {
                    load = true;
                    needsCombatList = false;
                }
            }
            if (load)
            {
                _CombatList = new List<Character>();


                CombatStateService.SimpleCombatListItem[] items = serviceClient.GetCombatList();
                _GuidCombatList = new List<CombatStateService.SimpleCombatListItem>(items);

                foreach (Guid g in from r in _GuidCombatList select r.ID)
                {
                    _CombatList.Add(_Characters.FirstOrDefault(a => a.ID == g));
                }

                CombatListBox.DataContext = _CombatList;
            }
        }

        public void CharactersChanged()
        {
            lock (flagSec)
            {
                needsCharacters = true;
            }
            Dispatcher.BeginInvoke(new EventDelegate(CharactersChangedInvoke), DispatcherPriority.Normal);
            
        }

        public void CharactersChangedInvoke()
        {
            bool load = false;

            lock (flagSec)
            {
                if (needsCharacters)
                {
                    load = true;
                    needsCharacters = false;
                }
            }
            if (load)
            {
                _Characters = new List<Character>(serviceClient.GetCharacters());
            }

        }
    }
}

/*
 *  CloudConnection.cs
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
using System.Security;

namespace CombatManager.Cloud
{
    public class CloudConnection
    {
        private bool _IsConnected;
        private MonsterLibrary _MonsterLibrary;


        public event ConnectionEventHandler Connected;
        public event ConnectionEventHandler ConnectionFailed;
        public event ConnectionEventHandler ConnectionLost;

        public delegate void ConnectionEventHandler(object o);
        
        
        public void Connect(string user, SecureString pass)
        {
            _IsConnected = false;

            if (_IsConnected)
            {
                if (Connected != null)
                {
                    Connected(this);
                }
            }
            else
            {
                if (ConnectionFailed != null)
                {
                    ConnectionFailed(this);
                }
            }
        }

        public void Disconnect()
        {
            if (ConnectionLost != null)
            {
                ConnectionLost(this);
            }
        }

        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
        }

        public MonsterLibrary MonsterLibrary
        {
            get
            {
                if (MonsterLibrary == null)
                {
                    _MonsterLibrary = new MonsterLibrary(this);
                }

                return _MonsterLibrary;
            }
        }

    }
}

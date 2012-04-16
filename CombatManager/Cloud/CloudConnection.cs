using System;
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

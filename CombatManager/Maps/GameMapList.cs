using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CombatManager.Maps
{
    public class GameMapList
    {
        ObservableCollection<GameMap> maps = new ObservableCollection<GameMap>();

        public ObservableCollection<GameMap> Maps
        {
            get
            {
                return maps;
            }
        }
       
    }
}

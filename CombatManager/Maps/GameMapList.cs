using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager.Maps
{
    public class GameMapList
    {
        const String MapsDir = "Maps";

        ObservableCollection<MapStub> maps = new ObservableCollection<MapStub>();
        
        public delegate void MapChangedDelegate(GameMapList.MapStub map);

        public event MapChangedDelegate MapChanged;

        int id;

        public GameMapList()
        {
            maps.CollectionChanged += Maps_CollectionChanged;
        }

        private void Maps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GameMapList.MapStub map in e.NewItems)
                {
                    map.PropertyChanged += Map_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (GameMapList.MapStub map in e.OldItems)
                {
                    map.PropertyChanged -= Map_PropertyChanged;
                }
            }
        }

        private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (MapChanged != null)
            {
                MapChanged((GameMapList.MapStub)sender);
            }
        }
       
        [XmlIgnore]
        public static String MapFileDir
        {
            get
            {
               return CMFileUtilities.AppDataSubDir(MapsDir);
            }
        }

        public static string GetMapFileName(int mapiId, String name)
        {
            FileInfo file = new FileInfo(name);
            String filename = "MapFile" + mapiId + file.Extension;
            return Path.Combine(MapFileDir, filename);
        }
        

        public GameMap CreateMap(String name)
        {
            FileInfo file = new FileInfo(name);

            int newId = Id++;

            String filename = GetMapFileName(newId, name);

            file.CopyTo(filename);

            GameMap map = new GameMap(newId, filename, file.Name.Substring(0, file.Name.Length - file.Extension.Length));

            MapStub stub = new MapStub(map);

            Maps.Add(stub);
            map.CanSave = true;
            map.SaveMap(true);

            return map;
        }

        public GameMap LoadStub(MapStub stub)
        {
            GameMap map = GameMap.LoadMap(stub.Id);
            stub.Map = map;
            if (stub.SourceFile == null)
            {
                stub.SourceFile = map.SourceFile;
            }

            map.CanSave = true;

            return map;
        }

        public void RemoveMap(MapStub stub)
        {
            DeleteMapFile(stub);
            Maps.Remove(stub);
            GameMap.Delete(stub.Id);
        }

        public void DeleteMapFile(MapStub stub)
        {
            if (stub.CachedMap)
            {
                try
                {
                    File.Delete(stub.SourceFile);
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {

                    }
                }
            }
        }


        public void UpdateMap(String name, MapStub stub)
        {
            GameMap map = stub.Map;
            DeleteMapFile(stub);

            FileInfo file = new FileInfo(name);
            
            String filename = GetMapFileName(stub.Id, name);

            file.CopyTo(filename);

            map.ForceUpdateSourceFile(filename);

        }


        public class MapStub : SimpleNotifyClass
        {
            String name;
            String sourceFile;
            int id;
            GameMap map;
            bool cachedMap;

            public MapStub() { }
            public MapStub(GameMap map)
            {
                name = map.Name;
                id = map.Id;
                this.map = map;
                this.sourceFile = map.SourceFile;
                this.cachedMap = map.CachedMap;

            }
            public string Name
            {
                get
                {
                    return name;
                }

                set
                {
                    name = value;
                    Notify("Name");
                }
            }

            public int Id
            {
                get
                {
                    return id;
                }

                set
                {
                    id = value;
                    Notify("Id");
                }
            }

            [XmlIgnore]
            public GameMap Map
            {
                get
                {
                    return map;
                }

                set
                {
                    if (map != value)
                    {
                        map = value;
                        map.PropertyChanged += Map_PropertyChanged;
                        Notify("Map");
                    }
                }
            }

            public bool CachedMap
            {
                get
                {
                    return cachedMap;
                }
                set
                {
                    cachedMap = value;
                }
            }

            public string SourceFile
            {
                get
                {
                    return sourceFile;
                }

                set
                {
                    sourceFile = value;
                }
            }

            private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Name")
                {
                    name = map.Name;
                    Notify("Name");
                }
                else if (e.PropertyName == "SourceFile")
                {
                    sourceFile = map.SourceFile;
                    Notify("SourceFile");
                }
                else if (e.PropertyName == "CachedMap")
                {
                    cachedMap = map.CachedMap;
                    Notify("CachedMap");
                }
            }
        }

        public ObservableCollection<MapStub> Maps
        {
            get
            {
                return maps;
            }
            set
            {
                maps = value;
            }
        }
             


        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}

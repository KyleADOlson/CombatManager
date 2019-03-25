using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager.Maps
{
    public class GameMapList : SimpleNotifyClass
    {
        const String MapsDir = "Maps";

        ObservableCollection<MapStub> maps = new ObservableCollection<MapStub>();


        public delegate void MapChangedDelegate(GameMapList.MapStub map);

        public event MapChangedDelegate MapChanged;


        ObservableCollection<int> currentFolderPath;

        int id;
        int folderId;
        int version;

        MapFolder rootFolder;

        public static GameMapList CreateEmptyList()
        {
            GameMapList list = new GameMapList();
            list.version = 1;

            list.RootFolder = new MapFolder();
            list.currentFolderPath = new ObservableCollection<int>() { 0 };
            return list;


        }


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
            return CreateMap(name, CurrentFolder);
        }

        public GameMap CreateMap(String name, MapFolder folder)
        {
            FileInfo file = new FileInfo(name);

            int newId = Id++;

            String filename = GetMapFileName(newId, name);

            file.CopyTo(filename);

            GameMap map = new GameMap(newId, filename, file.Name.Substring(0, file.Name.Length - file.Extension.Length), new List<int>(folder.FolderPath));

            MapStub stub = new MapStub(map);

            CurrentFolder.Maps.Add(stub);
            map.CanSave = true;
            map.SaveMap(true);

            return map;
        }

        public MapFolder CreateFolder(String name)
        {
            return CreateFolder(name, CurrentFolder);
        }

        public MapFolder CreateFolder(String name, MapFolder parent)
        {
            folderId++;
            MapFolder folder = new MapFolder(name, folderId, parent.FolderPath);
            parent.Folders.Add(folder);
            return folder;
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
                if (id != value)
                {
                    id = value;

                    Notify("Id");
                }
            }
        }

        public int FolderId
        {
            get
            {
                return folderId;
            }

            set
            {
                if (folderId != value)
                {
                    folderId = value;

                    Notify("FolderId");
                }
            }
        }

        public int Version
        {
            get
            {
                return version;
            }

            set
            {
                if (version != value)
                {
                    version = value;

                    Notify("Version");
                }
            }
        }


        public MapFolder RootFolder
        {
            get
            {
                return rootFolder;
            }
            set
            {
                if (rootFolder != value)
                {
                    rootFolder = value;
                }
            }
        }

        public bool UpdateVersions()
        {
            bool updated = false;
            if (version == 0)
            {
                ConvertToVersion1();
                updated = true;
            }
            return updated;
        }


        public void ConvertToVersion1()
        {
            if (version == 0)
            {
                currentFolderPath = new ObservableCollection<int>() { 0 };
                MapFolder folder = CurrentFolder;
                foreach (MapStub stub in maps)
                {
                    folder.Maps.Add(stub);
                }
                maps.Clear();
               
                version = 1;
            }

        }

        public ObservableCollection<int> CurrentFolderPath
        {
            get
            {
                return currentFolderPath;
            }
            set
            {
                if (currentFolderPath != value)
                {
                    bool startEmpty = currentFolderPath.IsEmptyOrNull();
                    if (currentFolderPath != null)
                    {

                        currentFolderPath.CollectionChanged -= CurrentFolderPath_CollectionChanged;
                    }
                    currentFolderPath = value;
                    bool endEmpty = currentFolderPath.IsEmptyOrNull(); 
                    if (currentFolderPath != null)
                    {
                        currentFolderPath.CollectionChanged += CurrentFolderPath_CollectionChanged;
                    }
                    if (!(startEmpty && endEmpty))
                    {
                        Notify("CurrentFolderPath");
                        Notify("CurrentFolder");
                    }
                }
            }
        }

        private void CurrentFolderPath_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        [XmlIgnore]
        public MapFolder CurrentFolder
        {
            get
            {
                MapFolder folder = RootFolder;

                if (folder == null)
                {
                    RootFolder = new MapFolder();
                    RootFolder.Maps = new ObservableCollection<MapStub>();
                    folder = RootFolder;
                }

                List<int> searchPath = new List<int>();
                if (currentFolderPath != null)
                {
                    searchPath = new List<int>(currentFolderPath);
                    searchPath.PopFront();
                }
                else
                {
                    CurrentFolderPath = new ObservableCollection<int>();
                }
                while (searchPath.Count > 0)
                {
                    int next = searchPath.PopFront();
                    MapFolder nextFolder = folder.Folders.FirstOrDefault((x) => (x.Id == next));
                    if (nextFolder == null)
                    {
                        break;
                    }
                    folder = nextFolder;

                }

                return folder;

            }
        }



        [XmlIgnore]
        public ObservableCollection<MapStub> CurrentMaps
        {
            get
            {
                MapFolder folder = CurrentFolder;
                return folder.Maps;
            }
        }



        public class MapStub : SimpleNotifyClass
        {
            String name;
            String sourceFile;
            int id;
            GameMap map;
            bool cachedMap;
            List<int> folderPath;

            public MapStub() { }
            public MapStub(GameMap map)
            {
                name = map.Name;
                id = map.Id;
                this.map = map;
                this.sourceFile = map.SourceFile;
                this.cachedMap = map.CachedMap;
                this.folderPath = new List<int>(map.FolderPath);
                map.PropertyChanged += Map_PropertyChanged;

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

            public List<int> FolderPath
            {
                get
                {
                    return folderPath;
                }
                set
                {
                    if (folderPath != value)
                    {
                        folderPath = value;
                        Notify("FolderPath");
                    }
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
                        FolderPath = map.FolderPath;
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

        public class MapFolder : SimpleNotifyClass
        {
            int id;
            string name;
            ObservableCollection<MapStub> maps = new ObservableCollection<MapStub>();
            ObservableCollection<MapFolder> folders = new ObservableCollection<MapFolder>();
            ObservableCollection<int> folderPath;

            public MapFolder() { }

            public MapFolder(String name, int id, IEnumerable<int> parentFolderPath)
            {
                this.name = name;
                this.id = id;
                this.folderPath = new ObservableCollection<int>(parentFolderPath);
                folderPath.Add(id);

            }

            public String Name
            {
                get
                {
                    return name;
                }
                set
                {
                    if (name != value)
                    {
                        name = value;
                        Notify("Name");
                    }
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
                    if (id != value)
                    {
                        id = value;
                        Notify("Id");
                    }
                }
            }

            public ObservableCollection<int> FolderPath
            {
                get
                {
                    return folderPath;
                }
                set
                {
                    if (folderPath != value)
                    {
                        folderPath = value;
                        Notify("FolderPath");
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
                    if (maps != value)
                    {
                        if (maps != null)
                        {
                            maps.CollectionChanged -= Maps_CollectionChanged;
                        }
                        maps = value;
                        if (maps != null)
                        {
                            maps.CollectionChanged += Maps_CollectionChanged;
                        }
                        Notify("Maps");
                    }
                }
            }

            private void Maps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
            }

            public ObservableCollection<MapFolder> Folders
            {
                get
                {
                    return folders;
                }
                set
                {
                    if (folders != value)
                    {
                        if (folders != null)
                        {
                            folders.CollectionChanged -= Folders_CollectionChanged;
                        }
                        folders = value;
                        if (folders != null)
                        {
                            folders.CollectionChanged += Folders_CollectionChanged;
                        }
                        Notify("Folders");

                    }
                }
            }

            private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                Notify("Folders");
            }
        }

    }


}

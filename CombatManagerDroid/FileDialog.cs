using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Security.AccessControl;

namespace CombatManagerDroid
{
    public class FileDialog : Dialog
    {
        bool _Load;

        Context _Context;
        String _Folder;
        List<String> _Extensions;
        FileViewAdapter adapter;

        public string Text{ get; set; }

        public class FileDialogEventArgs
        {
            public string Filename { get; set;}
        }
        public event EventHandler<FileDialogEventArgs> DialogComplete;

        public FileDialog(Context context, List<string> ext, bool load) : base (context)
        {
            _Load = load;
            _Context = context;
            _Extensions = ext;

            SetContentView(Resource.Layout.FileDialog);
            SetTitle(_Load?"Open File":"Save File");
            SetCanceledOnTouchOutside(true);

            Button okB = (Button)FindViewById(Resource.Id.okButton);
            (okB).Click += 
                (object sender, EventArgs e) => {OkClicked();};
            okB.Text = _Load?"Open":"Save";

            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
                (object sender, EventArgs e) => {Dismiss();};
            
            //EditText t = FindViewById<EditText>(Resource.Id.fileName);

            
            Button b = FindViewById<Button>(Resource.Id.folderUpButton);
            b.Click += (object sender, EventArgs e) => { MoveUpFolder();};

            
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }


            
            TextView tv = FindViewById<TextView>(Resource.Id.folderNameText);
            tv.Text = Folder;


            BuildList();

        }

        void MoveUpFolder()
        {
            DirectoryInfo info = new DirectoryInfo(Folder);
            if (info.Parent != null)
            {
                _Folder = info.Parent.FullName;
                UpdateFolderDisplay();
            }
        }

        void UpdateFolderDisplay()
        {
            
            TextView tv = FindViewById<TextView>(Resource.Id.folderNameText);
            tv.Text = Folder;


            adapter.UpdateFolder(Folder);
        }

        public string Folder
        {
            get
            {
                if (_Folder == null)
                {
                    string docpath = "/sdcard";

                    string folder = Path.Combine(docpath, "CombatManager");
                    _Folder = folder;
                }

                return _Folder;
            }
        }

        private void OkClicked()
        {
            EditText t = FindViewById<EditText>(Resource.Id.fileName);

            if (_Load)
            {
                if (File.Exists(Path.Combine(Folder, t.Text.Trim())))
                {
                    Text = t.Text.Trim();
                    Dismiss();
                    if (DialogComplete != null)
                    {
                        DialogComplete(this, new FileDialogEventArgs() { Filename = Text });
                    }
                }
            }
            else
            {
                if (t.Text.Trim().Length > 0)
                {
                    Text = t.Text.Trim();
                    Dismiss();
                    if (DialogComplete != null)
                    {
                        DialogComplete(this, new FileDialogEventArgs() { Filename = Text });
                    }
                }
            }
        }

        private void BuildList()
        {

            ListView v = FindViewById<ListView>(Resource.Id.fileList);

            FileViewAdapter ad = new FileViewAdapter(this, _Context, Folder);
            adapter = ad;

            v.SetAdapter(ad);
            v.ItemClick += (object sender, AdapterView.ItemClickEventArgs e)  => 
            {
                EditText t = FindViewById<EditText>(Resource.Id.fileName);
                FileSystemInfo item = ad.GetFile(e.Position);
                if (item is FileInfo)
                {
                    
                    t.Text = item.Name;
                }
                else
                {
                    if (CanAccessFolder(item.FullName))
                    {
                        _Folder = item.FullName;
                        UpdateFolderDisplay();
                    }
                    else
                    {
                        //maybe do a popup?
                    }
                }
            };
        }

        bool CanAccessFolder(string folder)
        {
            try
            {
                foreach (var f in Directory.EnumerateDirectories(folder))
                {              
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        bool MatchesExtension(FileInfo f)
        {
            return this._Extensions.Contains(f.Extension);
        }


        class FileViewAdapter : BaseAdapter
        {
            static List<FileSystemInfo> _Items;
            Context _Context;
            FileDialog _Parent;

            public FileViewAdapter(FileDialog parent, Context context, string folder)
            {
                _Context = context;
                _Parent = parent;

                UpdateFolder(folder);

            }

            public void UpdateFolder(string folder)
            {
                
                _Items =  new List<FileSystemInfo>();
                foreach (var f in Directory.EnumerateDirectories(folder))
                {
                    var d = new DirectoryInfo(f);
                   
                    _Items.Add(d);
                }

                foreach (var f in Directory.EnumerateFiles(folder))
                {
                    FileInfo file = new FileInfo(f);
                    if (_Parent.MatchesExtension(file))
                    {
                        _Items.Add(file);
                    }
                }
                NotifyDataSetChanged();
            }

            public override int Count
            {
                get
                {
                    return _Items.Count;
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return position;
            }
            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                TextView t = (TextView)convertView;
                if (t == null)
                {
                    t = new TextView(_Context);
                }
                t.TextSize = 18;
                if (_Items[position] is FileInfo)
                {

                    t.Text = ((FileInfo)_Items[position]).Name;                
                    t.SetCompoundDrawablesWithIntrinsicBounds(_Context.Resources.GetDrawable(Resource.Drawable.file16), null, null, null);

                }
                if (_Items[position] is DirectoryInfo)
                {

                    t.Text = ((DirectoryInfo)_Items[position]).Name;                
                    t.SetCompoundDrawablesWithIntrinsicBounds(_Context.Resources.GetDrawable(Resource.Drawable.folder16), null, null, null);

                }
               
                return t;
            }

            public FileSystemInfo GetFile(int position)
            {
                return _Items[position];
            }
        }

    }
}


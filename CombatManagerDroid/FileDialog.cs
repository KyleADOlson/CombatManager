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

namespace CombatManagerDroid
{
    public class FileDialog : Dialog
    {
        bool _Load;

        List<string> _FileList;
        Context _Context;

        public string Text{ get; set; }

        public class FileDialogEventArgs
        {
            public string Filename { get; set;}
        }
        public event EventHandler<FileDialogEventArgs> DialogComplete;

        public FileDialog(Context context, bool load) : base (context)
        {
            _Load = load;
            _Context = context;

            SetContentView(Resource.Layout.FileDialog);
            SetTitle(_Load?"Open File":"Save File");

            Button okB = (Button)FindViewById(Resource.Id.okButton);
            (okB).Click += 
                (object sender, EventArgs e) => {OkClicked();};
            okB.Text = _Load?"Open":"Save";

            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
                (object sender, EventArgs e) => {Dismiss();};
            
            EditText t = FindViewById<EditText>(Resource.Id.fileName);


            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            BuildList();

        }

        public static string Folder
        {
            get
            {
                string docpath = "/sdcard";

                string folder = Path.Combine(docpath, "CombatManager");

                return folder;
            }
        }

        private void OkClicked()
        {
            EditText t = FindViewById<EditText>(Resource.Id.fileName);

            if (_Load)
            {
                if (_FileList.Contains(t.Text.Trim()))
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
            ArrayAdapter<String> ad = new ArrayAdapter<String>(_Context, Android.Resource.Layout.SelectDialogItem);
            _FileList = new List<string>();
            foreach (var f in Directory.EnumerateFiles(Folder))
            {
                _FileList.Add(new FileInfo(f).Name);
            }
            ad.AddAll(_FileList);

            v.SetAdapter(ad);
            v.ItemClick += (object sender, AdapterView.ItemClickEventArgs e)  => 
            {
                EditText t = FindViewById<EditText>(Resource.Id.fileName);
                t.Text = _FileList[e.Position];
            };
        }

    }
}


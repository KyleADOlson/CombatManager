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
using CombatManager;

namespace CombatManagerDroid
{
    class ImportExportDialog : Dialog
    {
        ExportData exportData;

        Button monstersButton;
        Button spellsButton;
        Button featsButton;
        Button conditionsButton;

        ListView listView;

        List<Button> buttonList;

        List<List<CheckBoxItemInfo>> iteminfo = new List<List<CheckBoxItemInfo>>();

        int selectedButton = 0;

        Context context;
        public event EventHandler<ImportExportDialogEventArgs> DialogComplete;


        public ImportExportDialog(Context context, ExportData data, bool import) : base(context)
        {
            exportData = data;
            this.context = context;

            RequestWindowFeature((int)WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.ImportExportDialog);
            SetCanceledOnTouchOutside(true);

            ((Button)FindViewById(Resource.Id.cancelButton)).Click +=
            (object sender, EventArgs e) => { Dismiss(); };

            ((Button)FindViewById(Resource.Id.okButton)).Click +=
            (object sender, EventArgs e) => { OkClicked(); };

            monstersButton = FindViewById<Button>(Resource.Id.monstersTabButton);
            spellsButton = FindViewById<Button>(Resource.Id.spellsTabButton);
            featsButton = FindViewById<Button>(Resource.Id.featsTabButton);
            conditionsButton = FindViewById<Button>(Resource.Id.conditionsTabButton);

            listView = FindViewById<ListView>(Resource.Id.itemListView);

            if (!import)
            {
                FindViewById<TextView>(Resource.Id.titleTextView).Text = "Select Items to Export";
            }
            FindViewById<Button>(Resource.Id.selectAllButton).Click +=
                (sender, e) =>
                {
                    foreach (CheckBoxItemInfo ii in iteminfo[selectedButton])
                    {
                        ii.State = true;
                    }
                    BuildAdapter();
                };

            buttonList = new List<Button>
            {
                monstersButton,
                spellsButton,
                featsButton,
                conditionsButton
            };

            monstersButton.Selected = true;

            int index = 0;
            foreach (Button b in buttonList)
            {
                int current = index;
                index++;
                b.Click += (sender, e) =>
                     {
                         selectedButton = current;
                         UpdateSelectedButton();
                         BuildAdapter();
                     };
            }

            BuildCheckBoxInfo();

            BuildAdapter();
        }

        void UpdateSelectedButton()
        {
            for (int i = 0; i < 3; i++)
            {
                buttonList[i].Selected = (selectedButton == i);
            }
        }

        void BuildCheckBoxInfo()
        {
            List<CheckBoxItemInfo> info;

            info = new List<CheckBoxItemInfo>();
            iteminfo.Add(info);
            foreach (Monster m in exportData.Monsters)
            {
                info.Add(new CheckBoxItemInfo(m.Name, false));
            }
            info = new List<CheckBoxItemInfo>();
            iteminfo.Add(info);
            foreach (Spell m in exportData.Spells)
            {
                info.Add(new CheckBoxItemInfo(m.Name, false));
            }
            info = new List<CheckBoxItemInfo>();
            iteminfo.Add(info);
            foreach (Feat m in exportData.Feats)
            {
                info.Add(new CheckBoxItemInfo(m.Name, false));
            }
            info = new List<CheckBoxItemInfo>();
            iteminfo.Add(info);
            foreach (Condition m in exportData.Conditions)
            {
                info.Add(new CheckBoxItemInfo(m.Name, false));
            }

        }

        void OkClicked()
        {
            Dismiss();

            ExportData outputData = new ExportData();

            for (int i=0; i<iteminfo[0].Count; i++)
            {
                CheckBoxItemInfo info = iteminfo[0][i];
                if (info.State)
                {
                    outputData.Monsters.Add(exportData.Monsters[i]);
                }
            }
            for (int i = 0; i < iteminfo[1].Count; i++)
            {
                CheckBoxItemInfo info = iteminfo[1][i];
                if (info.State)
                {
                    outputData.Spells.Add(exportData.Spells[i]);
                }
            }
            for (int i = 0; i < iteminfo[2].Count; i++)
            {
                CheckBoxItemInfo info = iteminfo[2][i];
                if (info.State)
                {
                    outputData.Feats.Add(exportData.Feats[i]);
                }
            }
            for (int i = 0; i < iteminfo[3].Count; i++)
            {
                CheckBoxItemInfo info = iteminfo[3][i];
                if (info.State)
                {
                    outputData.Conditions.Add(exportData.Conditions[i]);
                }
            }


            DialogComplete(this, new ImportExportDialogEventArgs() { Data = outputData });
        }

        void BuildAdapter()
        {
            listView.Adapter = new CheckBoxListAdapter(context, iteminfo[selectedButton]);
        }

        class CheckBoxItemInfo
        {
            String text;
            bool state;

            public CheckBoxItemInfo()
            {

            }

            public CheckBoxItemInfo(String text, bool state)
            {
                this.text = text;
                this.state = state;
            }

            public bool State { get => state; set => state = value; }
            public string Text { get => text; set => text = value; }
        }


        public class ImportExportDialogEventArgs : EventArgs
        {
            public ExportData Data {get; set;}
        }

        class CheckBoxListAdapter : BaseAdapter
        {
            Context context;
            List<CheckBoxItemInfo> list;

            public CheckBoxListAdapter(Context context, List<CheckBoxItemInfo> list)
            {
                this.context = context;
                this.list = list;
            }

            public override int Count
            {
                get
                {
                    return list.Count;
                }
            }

            public List<CheckBoxItemInfo> List
            {
                get
                {
                    return list;
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
                CheckBox c = new CheckBox(context);
                c.LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                c.Text = list[position].Text;
                c.Checked = list[position].State;
                c.CheckedChange += (sender, e)=>
                {
                    list[position].State = c.Checked;
                };

                return c;
            }
        }
    }
}
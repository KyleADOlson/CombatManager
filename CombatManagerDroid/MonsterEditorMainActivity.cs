
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
using System.Reflection;


namespace CombatManagerDroid
{
    [Activity (Label = "Monster Editor - Main", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorMainActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditor;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            //Simple Text Fields
            AttachEditTextString(EditMonster, Resource.Id.nameText, "Name");
            AttachEditTextString(EditMonster, Resource.Id.sensesText, "Senses");
            AttachEditTextString(EditMonster.Adjuster, Resource.Id.subtypesText, "Subtype");
            AttachEditTextString(EditMonster, Resource.Id.raceText, "Race");
            AttachEditTextString(EditMonster, Resource.Id.classText, "Class");

            //CR
            List<String> CRList = new List<string> (){"1/8", "1/6", "1/4", "1/3", "1/2"};
            for (int i=1;i<51;i++)
            {
                CRList.Add(i.ToString());
            }

            AttachButtonStringList(EditMonster.Adjuster, Resource.Id.crButton, "CR", CRList);

            //Alignment
            List<String> alignmentList = new List<string> { "LG", "NG", "CG",
                                                        "LN", "N", "CN",
                                                        "LE", "NE", "CE"};

            AttachButtonStringList(EditMonster, Resource.Id.alignmentButton, "Alignment", alignmentList);

            //Size
            List<String> sizesList = new List<string>();
            foreach (CombatManager.MonsterSize size in Enum.GetValues(typeof(CombatManager.MonsterSize)))
            {
                sizesList.Add(CombatManager.SizeMods.GetSizeText(size));
            }

            //CreatureType
            AttachButtonStringList(EditMonster, Resource.Id.sizeButton, "Size", sizesList);

            List<String>typesList = new List<string>(CombatManager.CreatureTypeInfo.GetTypes());
            AttachButtonStringList(EditMonster, Resource.Id.typeButton, "Type", typesList);

            //Ability Fields
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.strText, "Strength");
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.dexText, "Dexterity");
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.conText, "Constitution");
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.intText, "Intelligence");
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.wisText, "Wisdom");
            AttachEditTextIntNull(EditMonster.Adjuster, Resource.Id.chaText, "Charisma");

        }


    }
}


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
    public class FeatDialog : Dialog
    {
        Feat feat;

        public delegate void FeatCompleteDelegate(FeatDialog sender, Feat feat);

        public event FeatCompleteDelegate FeatComplete;

        public FeatDialog(Context context, Feat editFeat) : base(context)
        {
            feat = (Feat)editFeat.Clone();

            RequestWindowFeature((int)WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.FeatDialog);
            SetCanceledOnTouchOutside(true);

            OKButton.Click += (sender, e) =>
            {
                Dismiss();
                FeatComplete?.Invoke(this, feat);
            };

            CancelButton.Click += (sender, e) =>
            {

                Dismiss();
            };

            NameText.AttachEditTextString(feat, "Name");
            TypesButton.AttachButtonStringList(feat, "Type", new List<string>( Feat.FeatTypes));
            PrerequisitesText.AttachEditTextString(feat, "Prerequistites");
            BenefitText.AttachEditTextString(feat, "Benefit");
            NormalText.AttachEditTextString(feat, "Normal");
            SpecialText.AttachEditTextString(feat, "Special");

            feat.PropertyChanged += Feat_PropertyChanged;

            EnableOK();
        }

        private void Feat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EnableOK();
        }
        void EnableOK()
        {
            OKButton.Enabled = !(feat.Type.StringEmptyOrNull() || feat.Name.StringEmptyOrNull());
        }

        public Button OKButton
        {
            get => FindViewById<Button>(Resource.Id.okButton);
        }

        public Button CancelButton
        {
            get => FindViewById<Button>(Resource.Id.cancelButton);
        }
        public Button TypesButton
        {
            get => FindViewById<Button>(Resource.Id.typesButton);
        }

        public EditText NameText
        {
            get => FindViewById<EditText>(Resource.Id.nameText);
        }
        public EditText PrerequisitesText
        {
            get => FindViewById<EditText>(Resource.Id.prerequisitesText);
        }
        public EditText BenefitText
        {
            get => FindViewById<EditText>(Resource.Id.benefitText);
        }
        public EditText NormalText
        {
            get => FindViewById<EditText>(Resource.Id.normalText);
        }
        public EditText SpecialText
        {
            get => FindViewById<EditText>(Resource.Id.specialText);
        }
    }
}
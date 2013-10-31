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
using Android.Webkit;
using System.IO;

namespace CombatManagerDroid
{
    class AboutDialog : Dialog
    {
        public AboutDialog (Context context) : base (context)
        {
            SetTitle("About");

            SetContentView(Resource.Layout.AboutDialog);
            SetCanceledOnTouchOutside(true);


            //load html
            Stream s = ((Activity)context).Assets.Open("AboutHtml.txt");


            String str = new StreamReader(s).ReadToEnd();
            s.Close();
        
            str = str.Replace("%VERSION%", ((Activity)context).Application.PackageManager.GetPackageInfo(((Activity)context).PackageName, 0).VersionName);

            //display html
            WebView wv = FindViewById<WebView>(Resource.Id.webView);
            wv.LoadDataWithBaseURL(null, str, "text/html", "utf-8", null);
               

            Button b = FindViewById<Button>(Resource.Id.closeButton);
            b.Click += (object sender, EventArgs e) => {Dismiss();};
        }
    }
}


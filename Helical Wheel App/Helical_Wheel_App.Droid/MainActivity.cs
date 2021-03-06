﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Xml.Linq;

namespace Helical_Wheel_App.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Helical Wheel", Icon = "@drawable/WheelIcon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            FileStream.PathApp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var docmentPath = Path.Combine(FileStream.PathApp, FileStream.FileName);
            try
            {
                XDocument doc = XDocument.Load(docmentPath);
            }
            catch
            {
                XDocument doc = new XDocument(new XElement(FileStream.RootName));
                doc.Save(docmentPath);
            }
            App.ScreenWidth = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density;
            App.ScreenHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;
            LoadApplication(new App());
        }
    }
}


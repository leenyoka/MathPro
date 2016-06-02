using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathPro.Menu;

namespace MathPro
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashScreen : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			//RequestWindowFeature(WindowFeatures.NoTitle);
			//SetContentView(Resource.Layout.SplashView);

			Thread.Sleep(3000);
			var i = new Intent(this, typeof (MainMenu));
			StartActivity(i);
		}
	}
}
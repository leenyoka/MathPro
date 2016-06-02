using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MathPro.Geometry;
using MathPro.Graphs;

namespace MathPro.Menu
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class MainMenu: Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main_Menu);
			(FindViewById<Button>(Resource.Id.btnMainEquaions)).Click += Equations;
			(FindViewById<Button>(Resource.Id.btnMainGeometry)).Click += Geometry;
			(FindViewById<Button>(Resource.Id.btnMainGraphs)).Click += Graphs;
			(FindViewById<Button>(Resource.Id.btnMainHelp)).Click += Help;
			(FindViewById<Button>(Resource.Id.btnMainSettings)).Click += Settings;

		}
		private void Equations(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(EquationsMenu));
			StartActivity(i);
		}

		private void Geometry(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(GeometryMenu));
			StartActivity(i);
			
		}

		private void Graphs(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(GraphsMenu));
			StartActivity(i);
		}
		private void Help(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(Help));
			StartActivity(i);
		}
		private void Settings(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(Settings));
			StartActivity(i);
		}
		
	}

}
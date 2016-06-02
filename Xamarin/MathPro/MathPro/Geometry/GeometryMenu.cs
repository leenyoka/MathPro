using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace MathPro.Geometry
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class GeometryMenu:Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Geometry_Menu);
			(FindViewById<Button>(Resource.Id.btnPlotGeoShape)).Click += Plot;
			(FindViewById<Button>(Resource.Id.btnTransformationGeometry)).Click += TransformationGeometry;

		}
		private void Plot(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(PlotGeoShape));
			StartActivity(i);
		}

		private void TransformationGeometry(object sender, EventArgs e)
		{
			Toast.MakeText(this, "Coming soon!",ToastLength.Short).Show();
			//var i = new Intent(this, typeof(TransformationGeoPage));
			//StartActivity(i);
		}
	}
}
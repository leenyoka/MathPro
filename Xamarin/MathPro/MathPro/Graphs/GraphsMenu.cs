using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MathPro.Geometry;

namespace MathPro.Graphs
{
	 [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class GraphsMenu:Activity
	{
		 readonly Helper _helper = new Helper();
		 protected override void OnCreate(Bundle bundle)
		 {
			 base.OnCreate(bundle);
			 SetContentView(Resource.Layout.Graphs_Menu);


			 (FindViewById<Button>(Resource.Id.btnGraphEquation)).Click += Equations;
			 (FindViewById<Button>(Resource.Id.btnGraphStraight)).Click += Straight;
			 (FindViewById<Button>(Resource.Id.btnGraphParabola)).Click += Curve;
		 }
		 private void Equations(object sender, EventArgs e)
		 {
			 _helper.SetPreference("PlotFromEq", "true");
			 var i = new Intent(this, typeof(GraphBuilder));
			 StartActivity(i);
		 }

		 private void Straight(object sender, EventArgs e)
		 {
			 _helper.SetPreference("PlotFromEq", "false");
			 _helper.SetPreference("Straight","true");
			 var i = new Intent(this, typeof(EnterStraightOrCurve));
			 StartActivity(i);
		 }
		 private void Curve(object sender, EventArgs e)
		 {
			 _helper.SetPreference("PlotFromEq", "false");
			 _helper.SetPreference("Straight", "false");
			 var i = new Intent(this, typeof(EnterStraightOrCurve));
			 StartActivity(i);
		 }
	}
}
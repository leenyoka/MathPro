using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using MathPro.Graphs;

namespace MathPro.Geometry
{
	 [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class GeometryPlotter:Activity
	{
		 protected override void OnCreate(Bundle bundle)
		 {
			 base.OnCreate(bundle);
			 SetContentView(Resource.Layout.Straight_line);

			 var surface = FindViewById<RelativeLayout>(Resource.Id.surface);
			 surface.AddView(new FilledPolygon(this, GetPoints()));

		 }

		 private Point[] GetPoints()
		 {
			 var food = new Helper().ReadPreferent("PlotGeoShape").Split('_');

			 return food.Select(rawPoint => rawPoint.Split(',')).Select
				 (xAndY => new Point(int.Parse(xAndY[0]), int.Parse(xAndY[1]))).ToArray();
		 }
	}
}
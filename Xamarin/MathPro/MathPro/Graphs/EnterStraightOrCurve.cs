using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using MathPro.Graphs;

namespace MathPro.Geometry
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class EnterStraightOrCurve : Activity
	{
		List<PointView> _points;
		Helper _helper = new Helper();

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.PlotGeoShape);
			_points = new List<PointView>();
			AddClicked("Point 1");
			AddClicked("Point 2");
			if (_helper.ReadPreferent("Straight")!= "true")
			{
				AddClicked("Turning point");
			}
			
			(FindViewById<Button>(Resource.Id.btnPlotShapeGeoPage)).Click += Plot1;
		}

		public void Plot1(object sender, EventArgs e)
		{
			var food = (_points.Select(point => point.GetPoint()).Aggregate("", 
				(current1, current) => current1 + (current.X + "," + current.Y + "_"))).Trim('_');

			new Helper().SetPreference("PlotGeoShape",food);

			var i = new Intent(this, typeof(StraighLine));
			StartActivity(i);
		}

		private void AddClicked(string name)
		{
			_points.Add(new PointView(name,this));
			ShowPoints();
		}
		private void ShowPoints()
		{
			var pointsList = FindViewById<LinearLayout>(Resource.Id.hostessPlotGeo);
			pointsList.RemoveAllViews();
			foreach (var point in _points)
			{
				pointsList.AddView(point);
				pointsList.AddView(SpaceWaster());
			}

		}
		private RelativeLayout SpaceWaster()
		{
			var grid = new RelativeLayout(this) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20))};
			return grid;
		}
		public List<Point> GetPoints()
		{
			return _points.Select(point => point.GetPoint()).ToList();
		}
	}
}
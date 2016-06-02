using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MathPro.Geometry
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class TransformationGeoPage:Activity
	{
		private readonly Helper _helper = new Helper();
		List<PointView> _points;
		int _number = 1;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Transformation_Geo);
			_points = new List<PointView>();
			AddClicked(this, null);
			AddClicked(this, null);
			AddClicked(this, null);
			(FindViewById<Button>(Resource.Id.btnPlotShapeGeoPage)).Click += Plot1;
		}
		public override ScreenOrientation RequestedOrientation
		{
			get
			{
				return ScreenOrientation.ReverseLandscape;
			}
			set
			{
				base.RequestedOrientation = ScreenOrientation.ReverseLandscape;
			}
		}
		public void Plot1(object sender, EventArgs e)
		{
			var food = (_points.Select(point => point.GetPoint()).Aggregate("",
				(current1, current) => current1 + (current.X + "," + current.Y + "_"))).Trim('_');

			new Helper().SetPreference("PlotGeoShape", food);

			var i = new Intent(this, typeof(GeometryPlotter));
			StartActivity(i);
		}
		private RelativeLayout GetAddButton()
		{
			var grid = new RelativeLayout(this) { LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(200), _helper.Factor(260)) };
			grid.SetBackgroundResource(Resource.Drawable.border);

			var button = new Button(this) { Text = "Add" };
			button.Click += AddClicked;
			button.LayoutParameters = new RelativeLayout.LayoutParams(_helper.Factor(180), _helper.Factor(80)) { TopMargin = 90, LeftMargin = 7 };
			button.Gravity = GravityFlags.Center;
			grid.AddView(button);
			return grid;
		}
		private void AddClicked(Object sender, EventArgs e)
		{
			_points.Add(new PointView(_number, this));
			_number++;
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
			pointsList.AddView(GetAddButton());

		}
		private RelativeLayout SpaceWaster()
		{
			var grid = new RelativeLayout(this) { LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20)) };
			return grid;
		}
		public List<Point> GetPoints()
		{
			return _points.Select(point => point.GetPoint()).ToList();
		}
	}
}
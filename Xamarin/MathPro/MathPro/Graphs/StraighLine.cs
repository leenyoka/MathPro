using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using MathBase;

namespace MathPro.Graphs
{
	 [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class StraighLine: Activity
	{
		 private readonly Helper _helper = new Helper();
		 protected override void OnCreate(Bundle bundle)
		 {
			 base.OnCreate(bundle);
			 SetContentView(Resource.Layout.Graphs_View);



			 if (_helper.ReadPreferent("PlotFromEq") == "true")
			 {
				 FromEquation();
			 }
			 else
			 {
				 FromPoints();
			 }


			 

		 }

		 private void FromEquation()
		 {
			 var surface = FindViewById<RelativeLayout>(Resource.Id.surface);
			 var problemDefinition = _helper.GetProblemDefinition(SolveForXStepType.Equation, Action.Solve, this);

			 if (problemDefinition == null || problemDefinition.GetStepType() != SolveForXStepType.Equation) return;
			 var mathG = new MathGraphs((Equation)problemDefinition);

			 if (mathG.Canvas == null) return;

			 if (mathG.Canvas.Count() == 2)
				 surface.AddView(new FilledPolygon(this, mathG.Canvas));
			 if (mathG.Canvas.Count() == 3)
				 surface.AddView(new FilledPolygon(this, mathG.Canvas, true));
		 }

		 private void FromPoints()
		 {
			 var surface = FindViewById<RelativeLayout>(Resource.Id.surface);
			 var points = GetPoints();
			 var mathG = _helper.ReadPreferent("Straight") == "true" ? 
				 new MathGraphs(points[0],points[1]) : new MathGraphs(points[0], points[1], points[2]);
			

			 if (mathG.Canvas == null) return;

			 if (mathG.Canvas.Count() == 2)
				 surface.AddView(new FilledPolygon(this, mathG.Canvas));
			 if (mathG.Canvas.Count() == 3)
				 surface.AddView(new FilledPolygon(this, mathG.Canvas, true));
		 }
		 private Point[] GetPoints()
		 {
			 var food = new Helper().ReadPreferent("PlotGeoShape").Split('_');

			 return food.Select(rawPoint => rawPoint.Split(',')).Select
				 (xAndY => new Point(int.Parse(xAndY[0]), int.Parse(xAndY[1]))).ToArray();
		 }


	}
}
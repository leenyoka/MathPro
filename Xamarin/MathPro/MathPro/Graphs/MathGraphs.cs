using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Graphics;
using MathBase;

namespace MathPro.Graphs
{
	public sealed class MathGraphs 
	{
		#region Properties

		List<Point> _points = new List<Point>();
		List<Point> _straightLineTruePoints = new List<Point>();
		public Point[] Canvas { get; set; }
		public List<Point> Points
		{
			get { return _points; }
			set { _points = value; }
		}

		#endregion Properties

		#region Constructor

		public MathGraphs(Equation eq )
			//:base(context)
		{
			//LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			try
			{
				Prepare(eq);
			}
			catch
			{
			}
		}

		public MathGraphs(Point point1, Point point2)
			//: base(context)
		{
			//LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			try
			{
				Prepare(point1, point2);
			}
			catch (Exception ex)
			{
			}
		}

		public MathGraphs(Point point1, Point point2, Point turningPoint)
			//:base(context)
		{
			//LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			try
			{
				Prepare(point1, point2, turningPoint);
			}
			catch (Exception ex)
			{
			}
		}

		#endregion Constructor

		#region Methods

		private void Prepare(Equation eq)
		{
			var eqLeft = new SimpificationEquation(new SimplificationExpression((Expression)eq.Left[0]));

			if (eq.Right.Count > 0)
			{
				var eqRight = new SimpificationEquation(new SimplificationExpression((Expression)eq.Right[0]));

				var expOne = (Expression)eqLeft.Solution[eqLeft.Solution.Count - 1][0];
				var expTwo = (Expression)eqRight.Solution[eqRight.Solution.Count - 1][0];

				expOne.Numerator.Sort();
				expTwo.Numerator.Sort();

				if (IsStraightLine(ChopOfFofX(expOne, expTwo)))
				{
					_points = GetPointsForStraightLine(ChopOfFofX(expOne, expTwo));
					Canvas = DrawStraightLine(new List<Point>(_points));
					ShowStraightLinePoints();
				}
				else if (IsParabola(ChopOfFofX(expOne, expTwo)))
				{
					_points = GetPointsForParabola(ChopOfFofX(expOne, expTwo));
					new List<Point>(_points);
				 Canvas=DrawParabola(_points);
					ShowParabolaPoints();
				}
			}
			else
			{
				var expOne = (Expression)eqLeft.Solution[eqLeft.Solution.Count - 1][0];

				expOne.Numerator.Sort();

				if (IsStraightLine(expOne))
				{
					_points = GetPointsForStraightLine(expOne);
					Canvas = _points.ToArray();
					//DrawStraightLine(new List<Point>(_points));
					//ShowStraightLinePoints();
				}
				else if (IsParabola(expOne))
				{
					_points = GetPointsForParabola(expOne);
					//Canvas = _points.ToArray();
					new List<Point>(_points);
					Canvas = DrawParabola(_points);
					//ShowParabolaPoints();
				}
			}
		}
		private void Prepare(Point point1, Point point2)
		{
		 Canvas= DrawStraightLine(new List<Point>(new[] { point1, point2 }));
			_straightLineTruePoints = new List<Point>(new[] { point1, point2 });
			ShowStraightLinePoints();
			var x = 0;
		}
		private void Prepare(Point point1, Point point2, Point turningPoint)
		{
			_points = new List<Point>(new[] { point1, point2, turningPoint });
			new List<Point>(_points);
			Canvas = DrawParabola(_points);
			ShowParabolaPoints();
		}
		private Expression ChopOfFofX(Expression one, Expression two)
		{
			if (((Term)one.Numerator[0]).TermBase == 'f' || one.Numerator.Count <= 1)
				return two;
			return one;
		}

		private bool IsStraightLine(Expression exp)
		{
			exp.Numerator.Sort();

			if (((Term)exp.Numerator[0]).Power == 1 && exp.Numerator.Count == 2)
				return true;
			 return false;
		}
		private bool IsParabola(Expression exp)
		{
			exp.Numerator.Sort();

			if (((Term)exp.Numerator[0]).Power == 2 && exp.Numerator.Count == 3)
				return true;
		 return false;
		}
		private List<Point> GetPointsForStraightLine(Expression exp)
		{
			var points = new List<Point>();
			var yIntercept = 0;

			var eq = new Equation(new iEquationPiece[] { exp }, SignType.equal, new iEquationPiece[] { new Expression(new IExpressionPiece[] { new Term(0) }, null) });
			var x = new SolveForX(eq);

			var xThing = ((Term)((Expression)((Equation)x.Solution[x.Solution.Count - 1]).Right[0]).Numerator[0]);
			var xIntercept = int.Parse(xThing.Sign + "1");
			xIntercept *= xThing.CoEfficient;
			_straightLineTruePoints.Add(new Point(xIntercept, 0));


			exp.Numerator[0] = new Term(0);
			var eq2 = new Equation(new iEquationPiece[] { new Expression(new IExpressionPiece[] { new Term('y') }, null) }, SignType.equal,
				new iEquationPiece[] { exp });
			var y = new SolveForX(eq2);


			var yThing = (Term)((Expression)((Equation)y.Solution[y.Solution.Count - 1]).Right[0]).Numerator[0];


			yIntercept = int.Parse(yThing.Sign + "1");
			yIntercept *= yThing.CoEfficient;
			_straightLineTruePoints.Add(new Point(0, yIntercept));
			yIntercept = (yIntercept > 0) ? yIntercept + 2 : yIntercept - 2;

			points.Add(new Point(xIntercept, (yIntercept > 0) ? -1 : 1));
			points.Add(new Point((xIntercept > 0) ? -1 : 1, yIntercept));


			return points;
		}

		private List<Point> GetPointsForParabola(Expression exp)
		{
			int xValue1 = 0, xValue2 = 0;

			var sol = new SolveForX(new Equation(new iEquationPiece[] { exp }, SignType.equal, new iEquationPiece[] { new Expression(new MathBase.IExpressionPiece[] { new Term(0) }, null) }));

			var eqFac = (FactorEquations)sol.Solution[sol.Solution.Count - 1];

			xValue1 = ((Term)((Expression)((Equation)eqFac.FactoringSolutions[0].Solution
				[eqFac.FactoringSolutions[0].Solution.Count - 1]).Right[0]).Numerator[0]).CoEfficient
				* int.Parse((((Term)((Expression)((Equation)eqFac.FactoringSolutions[0].Solution
				[eqFac.FactoringSolutions[0].Solution.Count - 1]).Right[0]).Numerator[0]).Sign + "1"));

			xValue2 = ((Term)((Expression)((Equation)eqFac.FactoringSolutions[1].Solution
				[eqFac.FactoringSolutions[1].Solution.Count - 1]).Right[0]).Numerator[0]).CoEfficient
			   * int.Parse((((Term)((Expression)((Equation)eqFac.FactoringSolutions[1].Solution
				[eqFac.FactoringSolutions[1].Solution.Count - 1]).Right[0]).Numerator[0]).Sign + "1"));

			var points = new List<Point> {new Point(xValue1, 0), new Point(xValue2, 0)};

			//x = -b / (2a)

			double turningPointX = (-1 * int.Parse((((Term)exp.Numerator[1]).Sign + ((Term)exp.Numerator[1]).CoEfficient.ToString(CultureInfo.InvariantCulture)))) /
				(2 * int.Parse((((Term)exp.Numerator[0]).Sign + ((Term)exp.Numerator[0]).CoEfficient.ToString(CultureInfo.InvariantCulture))));


			double turningPointY = (int.Parse((((Term)exp.Numerator[0]).Sign + ((Term)exp.Numerator[0]).CoEfficient)) * Math.Pow(turningPointX, 2))
								  + (int.Parse((((Term)exp.Numerator[1]).Sign + ((Term)exp.Numerator[1]).CoEfficient)) * turningPointX)
								  + int.Parse((((Term)exp.Numerator[2]).Sign + ((Term)exp.Numerator[2]).CoEfficient));

			points.Add(new Point(int.Parse(turningPointX.ToString(CultureInfo.InvariantCulture)), int.Parse(turningPointY.ToString(CultureInfo.InvariantCulture))));



			return points;
		}
		private List<Point> GetStartingAndEndPointsParabola(List<Point> points)
		{
			for (var i = 0; i < points.Count - 1; i++)
			{
				if (points[i].X > 0)
					points[i] = new Point(points[i].X + 1, points[i].Y);
				else
					points[i] = new Point(points[i].X - 1, points[i].Y);

				if (points[2].Y < 0)
					points[i] = new Point(points[i].X, points[i].Y + 3);
				else
					points[i] = new Point(points[i].X, points[i].Y - 3);
				//what if its pointing downwards?
			}
			//if (points[2].Y < 0)
			//    points[2] = new Point(points[2].X, points[2].Y - 2);
			//else
			//    points[2] = new Point(points[2].X, points[2].Y + 2);

			return points;
		}
		private Point[] GetPolygon(IEnumerable<Point> points)
		{
			var straightPoints = points.Select(GetPointFromCertasian).ToList();
			
			//canvas.DrawPath(path, paint);

			return straightPoints.ToArray();
			//return path;
		}
		public Point[] DrawStraightLine(List<Point> points)
		{
			return GetPolygon(points.ToArray());
		}
		private void ShowStraightLinePoints()
		{
			//List<Point> marginDirector = new List<Point>();

			//foreach (Point point in _straightLineTruePoints)
			//	marginDirector.Add(GetPointFromCertasian(point));

			//Grid host = new Grid();
			//host.Height = 480;
			//host.Width = 480;

			//for (int i = 0; i < _straightLineTruePoints.Count; i++)
			//{
			//	TextBlock txt = new TextBlock();
			//	txt.Foreground = new SolidColorBrush(Colors.Black);
			//	txt.Text = "(" + _straightLineTruePoints[i].X + "," + _straightLineTruePoints[i].Y + ")";
			//	txt.Margin = new Thickness(marginDirector[i].X, marginDirector[i].Y, 0, 0);
			//	host.Children.Add(txt);
			//}
			//this.Children.Add(host);
			/*
			 * <Grid Width="480" Height="480">
				<TextBlock Margin="260,380,0,0" Foreground="Black" Text="(x,y)"></TextBlock>
			</Grid>
			 */

		}
		private void ShowParabolaPoints()
		{
			//List<Point> marginDirector = new List<Point>();

			//foreach (Point point in _parabolaTruePoints)
			//	marginDirector.Add(GetPointFromCertasian(point));

			//Grid host = new Grid();
			//host.Height = 480;
			//host.Width = 480;

			//for (int i = 0; i < _parabolaTruePoints.Count; i++)
			//{
			//	TextBlock txt = new TextBlock();
			//	txt.Foreground = new SolidColorBrush(Colors.Black);
			//	txt.Text = "(" + _parabolaTruePoints[i].X + "," + _parabolaTruePoints[i].Y + ")";
			//	txt.Margin = new Thickness(marginDirector[i].X, marginDirector[i].Y, 0, 0);
			//	host.Children.Add(txt);
			//}
			//this.Children.Add(host);
		}


		public Point[] DrawParabola(List<Point> points)
		{

			points = GetStartingAndEndPointsParabola(new List<Point>(points));
			points[0] = GetPointFromCertasian(points[0]);
			points[1] = GetPointFromCertasian(points[1]);
			points[2] = GetPointFromCertasian(points[2]);

			return points.ToArray();
		}

		private Point GetPointFromCertasian(Point cartesian)
		{
			return cartesian;
			//return new Point(GetXValue(cartesian.X), GetYValue(cartesian.Y));
		}
		private int GetXValue(double value1)
		{
			var value = int.Parse(value1.ToString(CultureInfo.InvariantCulture));
			switch (value)
			{
				case 0: return 238;
				case 1: return 260;
				case 2: return 280;
				case 3: return 300;
				case 4: return 320;
				case 5: return 340;
				case 6: return 360;
				case 7: return 380;
				case 8: return 400;
				case 9: return 420;
				case 10: return 440;
				case -1: return 222;
				case -2: return 200;
				case -3: return 180;
				case -4: return 160;
				case -5: return 140;
				case -6: return 119;
				case -7: return 99;
				case -8: return 78;
				case -9: return 57;
				default: return 36;
			}
		}
		private int GetYValue(double value1)
		{
			var value = int.Parse((value1).ToString(CultureInfo.InvariantCulture));
			switch (value)
			{
				case 0: return 238;
				case 1: return 219;
				case 2: return 200;
				case 3: return 180;
				case 4: return 158;
				case 5: return 138;
				case 6: return 117;
				case 7: return 96;
				case 8: return 72;
				case 9: return 56;
				case 10: return 36;
				case -1: return 260;
				case -2: return 280;
				case -3: return 300;
				case -4: return 320;
				case -5: return 340;
				case -6: return 360;
				case -7: return 380;
				case -8: return 400;
				case -9: return 420;
				default: return 440;
			}
		}
		#endregion Methods


	}
}
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace MathPro.Graphs
{
	public class FilledPolygon : View
	{
		private readonly Point[] _points;
		private readonly bool _parabola;
		private readonly Color _color;
		private readonly bool _colorSet;
		public int YValue(int y)
		{
			return ConvertY(y) - 110;
		}

		private int ConvertY(int y)
		{
			switch (y)
			{
				case 0:
					return 500;
				case 1:
					return 455;
				case 2:
					return 413;
				case 3:
					return 375;
				case 4:
					return 332;
				case 5:
					return 287;
				case 6:
					return 245;
				case 7:
					return 200;
				case 8:
					return 155;
				case 9:
					return 113;
				case 10:
					return 745;
				case -1:
					return 545;
				case -2:
					return 587;
				case -3:
					return 631;
				case -4:
					return 673;
				case -5:
					return 719;
				case -6:
					return 761;
				case -7:
					return 805;
				case -8:
					return 848;
				case -9:
					return 889;
				case -10:
					return 933;
				default:
					return y > 10 ? 755 : 945;
			}
		}
		public int XValue(int x)
		{
			return ConvertX(x) - 100;
		}

		private int ConvertX(int x)
		{
			switch (x)
			{
				case 0:
					return 400;
				case 1:
					return 435;
				case 2:
					return 470;
				case 3:
					return 500;
				case 4:
					return 535;
				case 5:
					return 570;
				case 6:
					return 605;
				case 7:
					return 640;
				case 8:
					return 675;
				case 9:
					return 710;
				case 10:
					return 745;
				case -1:
					return 366;
				case -2:
					return 327;
				case -3:
					return 295;
				case -4:
					return 262;
				case -5:
					return 230;
				case -6:
					return 200;
				case -7:
					return 165;
				case -8:
					return 125;
				case -9:
					return 95;
				case -10:
					return 55;
				default:
					return x > 10 ? 755 : 45;
			}
		}

		public FilledPolygon(Context context, Point[] points)
			: base(context)
		{
			_points = points;
		}
		public FilledPolygon(Context context, Point[] points, Color color)
			: base(context)
		{
			_points = points;
			_color = color;
			_colorSet = true;
		}
		public FilledPolygon(Context context, Point[] points, bool parabola)
			: base(context)
		{
			_points = points;
			_parabola = parabola;
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			if (_parabola)
				canvas.DrawPath(GetCurvePath(), CurvePaint());
			else
				canvas.DrawPath(GetShapePath(), ShapePaint());

			//canvas.DrawText("(3,4)", 100, 100, TextPaint());
			//canvas.DrawText("(3,4)", 200, 150, TextPaint());
			var textPaint = TextPaint();
			var toDraw = GetPointsToDraw();

			foreach (var point in toDraw)
			{
				canvas.DrawText(string.Format("({0},{1})", point._.X, point._.Y), point.X, point.Y, textPaint);
			}
		}

		private IEnumerable<PointToDraw> GetPointsToDraw()
		{
			var sweetPoints = new List<SweetPoint>();
			for(var i =0;i< _points.Count();i++)
			{
				sweetPoints.Add(new SweetPoint(i,_points.ToList()));
			}

			var points = new List<PointToDraw>();

			foreach (var sweetPoint in sweetPoints)
			{
				var x = (sweetPoint.X);
				var y = (sweetPoint.Y);

				foreach (var uniquess in sweetPoint.Uniqueness)
				{
					if (uniquess == Uniqueness.BiggestX)
						x += 1;
					if (uniquess == Uniqueness.BiggestY)
						y += 1;
					if (uniquess == Uniqueness.SmallestX)
						x -= 1;
					if (uniquess == Uniqueness.SmallestY)
						y -= 1;
				}
				points.Add(new PointToDraw(new Point(XValue(x), YValue(y)), sweetPoint));
			}
			return points;
		}
		private Path GetShapePath()
		{
			var path = new Path();
			path.MoveTo(XValue(_points[0].X), YValue(_points[0].Y));
			for (var i = 1; i < _points.Length; i++)
			{

				path.LineTo(XValue(_points[i].X), YValue(_points[i].Y));
			}
			path.LineTo(XValue(_points[0].X), YValue(_points[0].Y));


			return path;
		}

		private Path GetCurvePath()
		{
			_points[0] = FixCurvePoint(_points[0]);
			_points[1] = FixCurvePoint(_points[1]);
			_points[2] = FixCurvePoint(_points[2]);
			var point1 =(new Point(XValue(_points[0].X), YValue(_points[0].Y)));
			var turningPoint = FixPoint(new Point(XValue(_points[2].X), YValue(_points[2].Y)));
			var point2 =( new Point(XValue(_points[1].X), YValue(_points[1].Y)));

			var path = new Path();
			path.MoveTo(point1.X, point1.Y);
			path.QuadTo(turningPoint.X, turningPoint.Y, point2.X, point2.Y);


			return path;
		}

		private Point FixPoint(Point oldPoint)
		{
			//if (oldPoint.Y > 0)
			//	oldPoint.Y = oldPoint.Y + 200;
			//if (oldPoint.Y < 0)
			//	oldPoint.Y = oldPoint.Y - 200;

			return oldPoint;
		}
		private Point FixCurvePoint(Point oldPoint)
		{
			//if (oldPoint.X > 0)
			//	oldPoint.X = oldPoint.X + 2;
			//if (oldPoint.X < 0)
			//	oldPoint.X = oldPoint.X - 2;

			//if (oldPoint.Y > 0)
			//	oldPoint.Y = oldPoint.Y + 2;
			//if (oldPoint.Y < 0)
			//	oldPoint.Y = oldPoint.Y - 2;

			return oldPoint;
		}

		private Paint ShapePaint()
		{
			var paint = new Paint
			{
				Color =  _colorSet ? _color: Color.Black,
				StrokeWidth = 3.0f
			};
			paint.SetStyle(Paint.Style.Stroke);

			return paint;
		}
		private Paint TextPaint()
		{
			var paint = new Paint
			{
				Color = _colorSet ? _color : Color.Black,
				StrokeWidth = 1.5f,
				TextSize = 20
			};
			paint.SetStyle(Paint.Style.Stroke);

			return paint;
		}
		private Paint CurvePaint()
		{
			
			var paint = new Paint();
			paint.SetStyle(Paint.Style.Stroke);
			paint.StrokeCap = Paint.Cap.Round;
			paint.StrokeWidth = 3.0f;
			paint.AntiAlias = true;
			paint.Color = Color.Black;
			return paint;
		}

	}

	public enum Uniqueness
	{
		BiggestX,BiggestY,SmallestX,SmallestY
	}

	public class PointToDraw : Point
	{
		public SweetPoint _;

		public PointToDraw(Point point, SweetPoint pointTrue)
		{
			X = point.X;
			Y = point.Y;

			_ = pointTrue;
		}
	}
	public class SweetPoint : Point
	{
		public List<Uniqueness> Uniqueness = new List<Uniqueness>(); 
		public SweetPoint(int index, List<Point> points)
		{
			Y = points[index].Y;
			X = points[index].X;

			if (GetMaxX(points) == X)
				Uniqueness.Add(Graphs.Uniqueness.BiggestX);
			if (GetMinX(points) == X)
				Uniqueness.Add(Graphs.Uniqueness.SmallestX);
			if (GetMaxY(points) == Y)
				Uniqueness.Add(Graphs.Uniqueness.BiggestY);
			if (GetMinY(points) == Y)
				Uniqueness.Add(Graphs.Uniqueness.SmallestY);
		}

		public int GetMaxX(List<Point> points)
		{
			return (points.Select(point => point.X).ToList()).Max();
		}
		public int GetMaxY(List<Point> points)
		{
			return (points.Select(point => point.Y).ToList()).Max();
		}
		public int GetMinX(List<Point> points)
		{
			return (points.Select(point => point.X).ToList()).Min();
		}
		public int GetMinY(List<Point> points)
		{
			return (points.Select(point => point.Y).ToList()).Min();
		}
	}
}
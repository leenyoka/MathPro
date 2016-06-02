using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace MathPro.Geometry
{
    public class TransformationGeometry: Grid
    {
        #region Properties

        List<Point[]> _history =new List<Point[]>();

        #endregion Properties

        #region Constructor

        public TransformationGeometry(params Point [] points)
        {
            _history.Add(points);
            this.Children.Add(GetPolygon(1, points));

        }
        public TransformationGeometry(Geometry geo)
        {
            _history.Add(geo.Points.ToArray());
            this.Children.Add(GetPolygon(1, geo.Points.ToArray()));

            if(geo.Properties != null)
            for (int i = 0; i < geo.Properties.Count; i++)
            {
                if (geo.Properties[i].TransType == TransformationType.Reflection)
                    Reflection(geo.Properties[i].ReflectionType);
                else if (geo.Properties[i].TransType == TransformationType.Rotation)
                    Rotation(geo.Properties[i].RotationAngle, geo.Properties[i].Direction);
                else if (geo.Properties[i].TransType == TransformationType.Translation)
                    Translation(geo.Properties[i].UnitsHorizotantl, geo.Properties[i].UnitVertical);
                else if (geo.Properties[i].TransType == TransformationType.Enlargement)
                    Enlargement(geo.Properties[i].Factor);
            }
        }
        #endregion Constructor

        #region Methods
        private Polygon GetPolygon(int number, Point[] points)
        {
            Polygon poly = new Polygon();
            poly.Stroke = GetColor(number);
            //poly.Fill = new SolidColorBrush(Colors.White);
            poly.Width = 480;
            poly.Height = 480;
            poly.StrokeThickness = 2;

            for (int i = 0; i < points.Length; i++)
                poly.Points.Add(GetPointFromCertasian(points[i]));

            return poly;
        }
        private SolidColorBrush GetColor(int number)
        {
            switch (number)
            {
                case 1: return new SolidColorBrush(Colors.Black);
                case 2: return new SolidColorBrush(Colors.Blue);
                case 3: return new SolidColorBrush(Colors.Red);
                case 4: return new SolidColorBrush(Colors.Orange);
                case 5: return new SolidColorBrush(Colors.Yellow);
                case 6: return new SolidColorBrush(Colors.Purple);
                case 7: return new SolidColorBrush(Colors.Gray);
                case 8: return new SolidColorBrush(Colors.Cyan);
                default: return new SolidColorBrush(Colors.Green);
            }
        }
        private Point GetPointFromCertasian(Point cartesian)
        {
            return new Point(GetXValue(cartesian.X), GetYValue(cartesian.Y));
        }
        private int GetXValue(double value1)
        {
            int value = int.Parse(value1.ToString());
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
            int value = int.Parse((value1).ToString());
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
        public void Reflection(ReflectionType type)
        {
            List<Point> start = new List<Point>(this._history[this._history.Count - 1]);
            List<Point> newPoints = new List<Point>();

            for (int i = 0; i < start.Count; i++)
            {
                if (type == ReflectionType.xAxis)
                    newPoints.Add(new Point(start[i].X, start[i].Y * -1));
                else if (type == ReflectionType.yAxis)
                    newPoints.Add(new Point(start[i].X * -1, start[i].Y));
                else
                    newPoints.Add(new Point(start[i].Y, start[i].X));
            }

            _history.Add(newPoints.ToArray());
            this.Children.Add(GetPolygon(_history.Count, newPoints.ToArray()));
        }
        public void Rotation(RotationAngle angle, Direction direction)
        {
            List<Point> start = new List<Point>(this._history[this._history.Count - 1]);
            List<Point> newPoints = new List<Point>();

            for (int i = 0; i < start.Count; i++)
            {
                if (angle == RotationAngle.Ninty)
                {
                    if (direction == Direction.Clockwise)
                        newPoints.Add(new Point(start[i].Y, start[i].X * -1));
                    else newPoints.Add(new Point(start[i].Y * -1, start[i].X));
                }
                else
                {
                    if (direction == Direction.Clockwise)
                        newPoints.Add(new Point(start[i].X * -1, start[i].Y * -1));
                }
            }

            _history.Add(newPoints.ToArray());
            this.Children.Add(GetPolygon(_history.Count, newPoints.ToArray()));
        }
        public void Translation(int unitsHorizontal, int unitsVertical)
        {
            List<Point> start = new List<Point>(this._history[this._history.Count - 1]);
            List<Point> newPoints = new List<Point>();

            for (int i = 0; i < start.Count; i++)
            {
                newPoints.Add(new Point(start[i].X + unitsHorizontal, start[i].Y + unitsVertical));
            }

            _history.Add(newPoints.ToArray());
            this.Children.Add(GetPolygon(_history.Count, newPoints.ToArray()));
        }
        public void Enlargement(int factor)
        {
            List<Point> start = new List<Point>(this._history[this._history.Count - 1]);
            List<Point> newPoints = new List<Point>();

            for (int i = 0; i < start.Count; i++)
            {
                newPoints.Add(new Point(start[i].X * factor, start[i].Y * factor));
            }

            _history.Add(newPoints.ToArray());
            this.Children.Add(GetPolygon(_history.Count, newPoints.ToArray()));
        }
        #endregion Methods
    }
    public enum ReflectionType
    {
        xAxis, yAxis, XequalY, Null
    }
    public enum Direction
    {
        Clockwise, AntiClockwise, Null
    }
    public enum RotationAngle
    {
        Ninty, HundredAndEighty, Null
    }
    public enum TransformationType
    {
        Reflection, Rotation, Translation, Enlargement
    }
    public class TransformationGeometryProperties
    {
        #region Properties
        TransformationType transType;

        public TransformationType TransType
        {
            get { return transType; }
            set { transType = value; }
        }
        int unitsHorizotantl = 0;

        public int UnitsHorizotantl
        {
            get { return unitsHorizotantl; }
            set { unitsHorizotantl = value; }
        }
        int unitVertical = 0;

        public int UnitVertical
        {
            get { return unitVertical; }
            set { unitVertical = value; }
        }
        int factor = 0;

        public int Factor
        {
            get { return factor; }
            set { factor = value; }
        }
        RotationAngle rotationAngle = RotationAngle.Null;

        public RotationAngle RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }
        ReflectionType reflectionType = ReflectionType.Null;

        public ReflectionType ReflectionType
        {
            get { return reflectionType; }
            set { reflectionType = value; }
        }
        Direction direction = Direction.Null;

        public Direction Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        #endregion Properties

        #region Constructor

        public TransformationGeometryProperties()
        {

        }

        #endregion Constructor
    }
    public struct Geometry
    {
        List<TransformationGeometryProperties> properties;

        public List<TransformationGeometryProperties> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
        List<Point> points;

        public List<Point> Points
        {
            get { return points; }
            set { points = value; }
        }
    }
}

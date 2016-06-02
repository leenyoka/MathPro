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
using MathBase;

namespace MathPro.Graphs
{
    public class MathGraphs:Grid
    {
        #region Properties

        List<Point> points = new List<Point>();
        List<Point> straightLineTruePoints = new List<Point>();
        List<Point> parabolaTruePoints = new List<Point>();
        GraphType _type = GraphType.None;

        public List<Point> Points
        {
            get { return points; }
            set { points = value; }
        }

        #endregion Properties

        #region Constructor

        public MathGraphs( Equation eq)
        {
            this.Height = 480;
            this.Width = 480;
            try
            {
                Prepare(eq);
            }
            catch (Exception ex)
            {
            }
        }

        public MathGraphs( Point point1, Point point2)
        {
            this.Height = 480;
            this.Width = 480;
            try
            {
                Prepare(point1, point2);
            }
            catch (Exception ex)
            {
            }
        }

        public MathGraphs(Point point1, Point point2, Point turningPoint)
        {
            this.Height = 480;
            this.Width = 480;
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
            SimpificationEquation eqLeft = new SimpificationEquation(new SimplificationExpression((MathBase.Expression)eq.Left[0]));

            if (eq.Right.Count > 0)
            {
                SimpificationEquation eqRight = new SimpificationEquation(new SimplificationExpression((MathBase.Expression)eq.Right[0]));

                MathBase.Expression expOne = (MathBase.Expression)((SimplificationExpression)eqLeft.Solution[eqLeft.Solution.Count - 1][0]);
                MathBase.Expression expTwo = (MathBase.Expression)((SimplificationExpression)eqRight.Solution[eqRight.Solution.Count - 1][0]);

                expOne.Numerator.Sort();
                expTwo.Numerator.Sort();

                if (IsStraightLine(ChopOfFofX(expOne, expTwo)))
                {
                    points = GetPointsForStraightLine(ChopOfFofX(expOne, expTwo));
                    DrawStraightLine(new List<Point>(points));
                    ShowStraightLinePoints();
                }
                else if (IsParabola(ChopOfFofX(expOne, expTwo)))
                {
                    points = GetPointsForParabola(ChopOfFofX(expOne, expTwo));
                    parabolaTruePoints = new List<Point>(points);
                    DrawParabola(points);
                    ShowParabolaPoints();
                }
            }
            else
            {
                MathBase.Expression expOne = (MathBase.Expression)((SimplificationExpression)eqLeft.Solution[eqLeft.Solution.Count - 1][0]);

                expOne.Numerator.Sort();

                if (IsStraightLine(expOne))
                {
                    points = GetPointsForStraightLine(expOne);
                    DrawStraightLine(new List<Point>(points));
                    ShowStraightLinePoints();
                }
                else if (IsParabola(expOne))
                {
                    points = GetPointsForParabola(expOne);
                    parabolaTruePoints = new List<Point>(points);
                    DrawParabola(points);
                    ShowParabolaPoints();
                }
            }

            int x = 0;
        }
        private void Prepare(Point point1, Point point2)
        {
            DrawStraightLine(new List<Point>(new Point[] { point1, point2 }));
            straightLineTruePoints = new List<Point>(new Point[] { point1, point2 });
            ShowStraightLinePoints();
            int x = 0;
        }
        private void Prepare(Point point1, Point point2, Point turningPoint)
        {
            points = new List<Point>(new Point[] { point1, point2,turningPoint });
            parabolaTruePoints = new List<Point>(points);
            DrawParabola(points);
            ShowParabolaPoints();
        }
        private MathBase.Expression ChopOfFofX(MathBase.Expression one, MathBase.Expression two)
        {
            if (((Term)one.Numerator[0]).TermBase == 'f' || one.Numerator.Count <= 1)
                return two;
            else return one;
        }
        private bool IsStraightLine(MathBase.Expression exp)
        {
            exp.Numerator.Sort();

            if (((Term)exp.Numerator[0]).Power == 1 && exp.Numerator.Count == 2)
                return true;
            else return false;
        }
        private bool IsParabola(MathBase.Expression exp)
        {
            exp.Numerator.Sort();

            if (((Term)exp.Numerator[0]).Power == 2 && exp.Numerator.Count == 3)
                return true;
            else return false;
        }
        private List<Point> GetPointsForStraightLine(MathBase.Expression exp)
        {
            List<Point> points = new List<Point>();
            int xIntercept = 0;
            int yIntercept = 0;

            Equation eq = new Equation(new iEquationPiece[] { exp }, SignType.equal, new iEquationPiece[] 
            { new MathBase.Expression(new IExpressionPiece[] { new Term(0) }, null) });
            SolveForX x = new SolveForX(eq);

            Term xThing = ((Term)((MathBase.Expression)((Equation)x.Solution[x.Solution.Count - 1]).Right[0]).Numerator[0]);
            xIntercept = int.Parse(xThing.Sign + "1");
            xIntercept *= xThing.CoEfficient;
            straightLineTruePoints.Add(new Point(xIntercept, 0));
            int xInterceptX = (xIntercept > 0) ? xIntercept + 2 : xIntercept - 2;


            exp.Numerator[0] = new Term(0);
            Equation eq2 = new Equation(new iEquationPiece[] { new MathBase.Expression(new IExpressionPiece[] { new Term('y') }, null) }, SignType.equal,
                new iEquationPiece[] { exp });
            SolveForX y = new SolveForX(eq2);


            Term yThing = (Term)((MathBase.Expression)((Equation)y.Solution[y.Solution.Count - 1]).Right[0]).Numerator[0];


            yIntercept = int.Parse(yThing.Sign + "1");
            yIntercept *= yThing.CoEfficient;
            straightLineTruePoints.Add(new Point(0, yIntercept));
            yIntercept = (yIntercept > 0) ? yIntercept + 2 : yIntercept - 2;

            points.Add(new Point(xIntercept, (yIntercept > 0)? -1: 1));
            points.Add(new Point((xIntercept > 0)? -1: 1, yIntercept));


            return points;
        }
        private int GetRandom()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            
            return rand.Next(2, 9);

        }
        private List<Point> GetPointsForParabola(MathBase.Expression exp)
        {
            int xValue1 = 0, xValue2 = 0;

            SolveForX sol = new SolveForX(new Equation(new iEquationPiece[] { exp }, SignType.equal, new iEquationPiece[] 
            { new MathBase.Expression(new MathBase.IExpressionPiece[] { new Term(0) }, null) }));

            FactorEquations eqFac = (FactorEquations)sol.Solution[sol.Solution.Count - 1];

            xValue1 = ((Term)((MathBase.Expression)((Equation)eqFac.FactoringSolutions[0].Solution
                [eqFac.FactoringSolutions[0].Solution.Count - 1]).Right[0]).Numerator[0]).CoEfficient
                * int.Parse((((Term)((MathBase.Expression)((Equation)eqFac.FactoringSolutions[0].Solution
                [eqFac.FactoringSolutions[0].Solution.Count - 1]).Right[0]).Numerator[0]).Sign + "1"));

            xValue2 = ((Term)((MathBase.Expression)((Equation)eqFac.FactoringSolutions[1].Solution
                [eqFac.FactoringSolutions[1].Solution.Count - 1]).Right[0]).Numerator[0]).CoEfficient
               * int.Parse((((Term)((MathBase.Expression)((Equation)eqFac.FactoringSolutions[1].Solution
                [eqFac.FactoringSolutions[1].Solution.Count - 1]).Right[0]).Numerator[0]).Sign + "1"));

            List<Point> points = new List<Point>();
            points.Add(new Point(xValue1, 0));
            points.Add(new Point(xValue2, 0));

            //x = -b / (2a)

            double turningPointX = (-1 * int.Parse((((Term)exp.Numerator[1]).Sign  +((Term)exp.Numerator[1]).CoEfficient.ToString()))) /
                (2 * int.Parse((((Term)exp.Numerator[0]).Sign  +((Term)exp.Numerator[0]).CoEfficient.ToString())));


            double turningPointY = (int.Parse((((Term)exp.Numerator[0]).Sign + ((Term)exp.Numerator[0]).CoEfficient.ToString())) * Math.Pow(turningPointX, 2))
                                  + (int.Parse((((Term)exp.Numerator[1]).Sign + ((Term)exp.Numerator[1]).CoEfficient.ToString())) * turningPointX)
                                  + int.Parse((((Term)exp.Numerator[2]).Sign + ((Term)exp.Numerator[2]).CoEfficient.ToString()));

            points.Add(new Point(int.Parse(turningPointX.ToString()), int.Parse(turningPointY.ToString())));



            return points;
        }
        private List<Point> GetStartingAndEndPointsParabola(List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
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
        private void DrawStraightLine(List<Point> points)
        {
            this.Children.Add(GetPolygon(1,points.ToArray()));
        }
        private void ShowStraightLinePoints()
        {
            List<Point> marginDirector = new List<Point>();

            foreach (Point point in straightLineTruePoints)
                marginDirector.Add(GetPointFromCertasian(point));

            Grid host = new Grid();
            host.Height = 480;
            host.Width = 480;

            for (int i = 0; i < straightLineTruePoints.Count; i++)
            {
                TextBlock txt = new TextBlock();
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Text = "(" + straightLineTruePoints[i].X + "," + straightLineTruePoints[i].Y + ")";
                txt.Margin = new Thickness(marginDirector[i].X, marginDirector[i].Y, 0, 0);
                host.Children.Add(txt);
            }
            this.Children.Add(host);
            /*
             * <Grid Width="480" Height="480">
                <TextBlock Margin="260,380,0,0" Foreground="Black" Text="(x,y)"></TextBlock>
            </Grid>
             */

        }
        private void ShowParabolaPoints()
        {
            List<Point> marginDirector = new List<Point>();

            foreach (Point point in parabolaTruePoints)
                marginDirector.Add(GetPointFromCertasian(point));

            Grid host = new Grid();
            host.Height = 480;
            host.Width = 480;

            for (int i = 0; i < parabolaTruePoints.Count; i++)
            {
                TextBlock txt = new TextBlock();
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Text = "(" + parabolaTruePoints[i].X + "," + parabolaTruePoints[i].Y + ")";
                txt.Margin = new Thickness(marginDirector[i].X, marginDirector[i].Y, 0, 0);
                host.Children.Add(txt);
            }
            this.Children.Add(host);
        }
        private void DrawParabola2(List<Point> points)
        {
            //points = GetStartingAndEndPointsParabola(new List<Point>(points));
            points[0] = GetPointFromCertasian(points[0]);
            points[1] = GetPointFromCertasian(points[1]);
            points[2] = GetPointFromCertasian(points[2]);

            QuadraticBezierSegment seg = new QuadraticBezierSegment();
            if (points[2].Y > 238)
                seg.Point1 = new Point(points[2].X, points[2].Y * 2);// points[2];
            else
                seg.Point1 = new Point(points[2].X, points[2].Y - (points[2].Y / 2));
            seg.Point2 = points[1];

            PathFigure pathFig = new PathFigure();
            pathFig.StartPoint = points[0];
            pathFig.Segments.Add(seg);
            PathGeometry path = new PathGeometry();
            path.Figures.Add(pathFig);
            Path myPath = new Path();
            myPath.Data = path;
            myPath.StrokeThickness = 2;
            myPath.Stroke = new SolidColorBrush(Colors.Black);

            myPath.Width = 480;
            myPath.Height = 480;

            this.Children.Add(myPath);

        }
        private void DrawParabola(List<Point> points)
        {
            points = GetStartingAndEndPointsParabola(new List<Point>(points));
            points[0] = GetPointFromCertasian(points[0]);
            points[1] = GetPointFromCertasian(points[1]);
            points[2] = GetPointFromCertasian(points[2]);

            QuadraticBezierSegment seg = new QuadraticBezierSegment();
            if (points[2].Y > 238)
                seg.Point1 = new Point(points[2].X, points[2].Y * 2);// points[2];
            else
                seg.Point1 = new Point(points[2].X, points[2].Y - (points[2].Y / 2));
            seg.Point2 = points[1];

            PathFigure pathFig = new PathFigure();
            pathFig.StartPoint = points[0];
            pathFig.Segments.Add(seg);
            PathGeometry path = new PathGeometry();
            path.Figures.Add(pathFig);
            Path myPath = new Path();
            myPath.Data = path;
            myPath.StrokeThickness = 2;
            myPath.Stroke = new SolidColorBrush(Colors.Black);

            myPath.Width = 480;
            myPath.Height = 480;

            this.Children.Add(myPath);
           
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
        #endregion Methods


    }
}

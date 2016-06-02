using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;

namespace MathPro.Geometry
{
    public partial class PlotGeometricShape : PhoneApplicationPage
    {
        List<PointView> points;
        int number = 1;
       
        public PlotGeometricShape()
        {
            InitializeComponent();
            points = new List<PointView>();
            AddClicked(this, null);
            AddClicked(this, null);
            AddClicked(this, null);
        }

        private Grid SpaceWaster()
        {
            Grid grid = new Grid();
            grid.Width = 20;
            return grid;
        }
        private Grid GetAddButton()
        {
            Grid grid = new Grid();
            grid.Width = 200;
            grid.Height = 200;
            grid.Background = GetBackGround();

            Button button = new Button();
            button.Content = "Add";
            button.Click += AddClicked;
            button.Width = 180;
            button.Height = 80;
            grid.Children.Add(button);
            return grid;
        }
        private void ShowPoints()
        {
            PointsList.Items.Clear();
            foreach (PointView point in points)
            {
                PointsList.Items.Add(point);
                PointsList.Items.Add(SpaceWaster());
            }
            PointsList.Items.Add(GetAddButton());

        }
        private void AddClicked(Object sender, RoutedEventArgs e)
        {
            points.Add(new PointView(number));
            number++;
            ShowPoints();
        }
        private ImageBrush GetBackGround()
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/border.png", UriKind.Relative);

            if (uri == null)
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private void btnScetch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Geometry geo = new Geometry();

                try
                {
                    geo.Points = new List<Point>(getPoints());
                }
                catch
                {
                    MessageBox.Show("Input not in correct format");
                }

                App.MyGeo = geo;

                Dispatcher.BeginInvoke(() =>
                {
                    this.NavigationService.Navigate(new Uri("/Geometry/TransformationGeometrySolutionViewer.xaml", UriKind.Relative));
                });
            }
            catch
            {
                MessageBox.Show("Something went wrong and we dont know what! Please try again");
            }
        }
        //public List<Point> getPoints()
        //{
        //    List<Point> points = new List<Point>();
        //    string[] pieces = txtPoints.Text.Trim().Split(',');

        //    for (int i = 0; i < pieces.Length; i++)
        //    {
        //        string[] subPieces = pieces[i].Split(';');

        //        int xValue = 0; int yValue = 0;

        //        if (int.TryParse(subPieces[0].Trim('(', ')'), out xValue)
        //            && int.TryParse(subPieces[1].Trim('(', ')'), out yValue))
        //        {
        //            points.Add(new Point(xValue, yValue));
        //        }
        //    }

        //    return points;
        //}
        public List<Point> getPoints()
        {
            List<Point> points = new List<Point>();


            foreach (PointView point in this.points)
                points.Add(point.GetPoint());

            return points;
        }
    }
}
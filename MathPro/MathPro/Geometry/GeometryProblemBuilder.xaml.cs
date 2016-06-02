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
    public partial class GeometryProblemBuilder : PhoneApplicationPage
    {
        List<TransformationGeometryProperties> properties = new List<TransformationGeometryProperties>();
        List<PointView> points;
        int number = 1;
        public GeometryProblemBuilder()
        {
            InitializeComponent();
            rdXAxis.IsChecked = true;
            rdReflection.IsChecked = true;
            rdNinty.IsChecked = true;
            rdClockwise.IsChecked = true;
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
        public void TranslationTypeChanged(object sender, RoutedEventArgs e)
        {
            panelEnlargement.Visibility = System.Windows.Visibility.Collapsed;
            panelReflection.Visibility = System.Windows.Visibility.Collapsed;
            panelRotation.Visibility = System.Windows.Visibility.Collapsed;
            panelTranslation.Visibility = System.Windows.Visibility.Collapsed;

            if (rdEnlargement.IsChecked == true)
                panelEnlargement.Visibility = System.Windows.Visibility.Visible;
            else if (rdReflection.IsChecked == true)
                panelReflection.Visibility = System.Windows.Visibility.Visible;
            else if (rdRotation.IsChecked == true)
                panelRotation.Visibility = System.Windows.Visibility.Visible;
            else if (rdTranslation.IsChecked == true)
                panelTranslation.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdEnlargement.IsChecked == true)
                {
                    int value = 0;
                    if (txtEnlargement.Text.Trim() != "" && int.TryParse(txtEnlargement.Text.Trim(), out value))
                    {
                        TransformationGeometryProperties property = new TransformationGeometryProperties();
                        property.TransType = TransformationType.Enlargement;
                        property.Factor = value;
                        properties.Add(property);
                    }

                    txtAdded.Text += " Enlargement,";
                }
                else if (rdReflection.IsChecked == true)
                {
                    TransformationGeometryProperties property = new TransformationGeometryProperties();
                    property.TransType = TransformationType.Reflection;
                    if (rdXAxis.IsChecked == true)
                    {
                        property.ReflectionType = ReflectionType.xAxis;
                    }
                    else if (rdYAxis.IsChecked == true)
                    {
                        property.ReflectionType = ReflectionType.yAxis;
                    }
                    else
                    {
                        property.ReflectionType = ReflectionType.XequalY;
                    }
                    properties.Add(property);
                    txtAdded.Text += " Reflection,";
                }
                else if (rdRotation.IsChecked == true)
                {
                    TransformationGeometryProperties property = new TransformationGeometryProperties();
                    property.TransType = TransformationType.Rotation;

                    if (rdNinty.IsChecked == true)
                        property.RotationAngle = RotationAngle.Ninty;
                    else
                        property.RotationAngle = RotationAngle.HundredAndEighty;

                    if (rdClockwise.IsChecked == true)
                        property.Direction = Direction.Clockwise;
                    else
                        property.Direction = Direction.AntiClockwise;
                    properties.Add(property);

                    txtAdded.Text += " Rotation,";
                }
                else if (rdTranslation.IsChecked == true)
                {
                    int vertical = 0, horizontal = 0;

                    if (((txtHorizontal.Text.Trim() != "" && int.TryParse(txtHorizontal.Text.Trim(), out horizontal))
                        || (txtVertical.Text.Trim() != "" && int.TryParse(txtVertical.Text.Trim(), out vertical))))
                    {
                        TransformationGeometryProperties property = new TransformationGeometryProperties();
                        property.TransType = TransformationType.Translation;
                        property.UnitVertical = vertical;
                        property.UnitsHorizotantl = horizontal;
                        properties.Add(property);

                        txtAdded.Text += " Translation,";
                    }
                }

                txtAdded.Text = txtAdded.Text.Trim(',');
            }
            catch
            {
                MessageBox.Show("Something went wrong and we dont know what! Please try again");
            }
        }

        private void btnScetch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Geometry geo = new Geometry();

                if (properties.Count > 0)
                    geo.Properties = new List<TransformationGeometryProperties>(properties);

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
        public List<Point> getPoints()
        {
            List<Point> points = new List<Point>();


            foreach (PointView point in this.points)
                points.Add(point.GetPoint());

            return points;
        }
    }
}
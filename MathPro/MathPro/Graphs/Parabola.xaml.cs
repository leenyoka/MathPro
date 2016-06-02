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

namespace MathPro.Graphs
{
    public partial class Parabola : PhoneApplicationPage
    {
        public Parabola()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            var values = new string[] { "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "-1", "-2", "-3", "-4", "-5",
                "-6", "-7", "-8", "-9"};

            pointOneX.ItemsSource = values;
            pointOneY.ItemsSource = values;
            pointTwoX.ItemsSource = values;
            pointTwoY.ItemsSource = values;
            turningX.ItemsSource = values;
            turningY.ItemsSource = values;
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            int point1x = int.Parse(pointOneX.SelectedItem.ToString());
            int point2x = int.Parse(pointTwoX.SelectedItem.ToString());
            int point1y = int.Parse(pointOneY.SelectedItem.ToString());
            int point2y = int.Parse(pointTwoY.SelectedItem.ToString());
            int turningPX = int.Parse(turningX.SelectedItem.ToString());
            int turninPY = int.Parse(turningY.SelectedItem.ToString());

            App.GraphPoints = new List<Point>(new Point[] { new Point(point1x, point1y),
                new Point(point2x, point2y) , new Point(turningPX, turninPY)});
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Graphs/MathGraphViewer.xaml", UriKind.Relative));
            });
        }
    }
}
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

namespace MathPro.Menu
{
    public partial class GraphMenu : PhoneApplicationPage
    {
        public GraphMenu()
        {
            InitializeComponent();
        }

        private void btnFormula_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Plot);
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Graphs/MathGraphBuilder.xaml", UriKind.Relative));
            });
        }

        private void btnStraight_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Plot);
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Graphs/Straight.xaml", UriKind.Relative));
            });
        }

        private void btnParabolic_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Plot);
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Graphs/Parabola.xaml", UriKind.Relative));
            });
        }
    }
}
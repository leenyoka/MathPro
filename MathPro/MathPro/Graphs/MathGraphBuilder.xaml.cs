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
using MathBase;
using System.Windows.Threading;

namespace MathPro.Graphs
{
    public partial class MathGraphBuilder : PhoneApplicationPage
    {
        // Constructor
        Calculator calc = new Calculator();
        DispatcherTimer timer;
        EventHandler _handler;
        public MathGraphBuilder()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
            App.Builder.Timer = timer;
            this.ContentPanel.Children.Add(App.Builder);
            App.Builder.Draw(ref timer);
            _handler = new EventHandler(ErrorBuilding);
            App.Builder.ErrorHandler = _handler;
            btnTest.Content = App.Builder.Action.ToString();
        }
        private void ErrorBuilding(object sender, EventArgs e)
        {
            MessageBox.Show("Not allowed");
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Simplify);
           
            App.ProblemDefinition = App.Builder.GetEquation(); 
            Go();
        }
        private void Go()
        {
            App.GraphPoints = new List<Point>();
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Graphs/MathGraphViewer.xaml", UriKind.Relative));
            });
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ContentPanel.Children.Remove(App.Builder);
        }
    }
}
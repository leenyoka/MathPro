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
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace MathPro.ProblemBuilder
{
    public partial class LogarithmicBuilderViewer : PhoneApplicationPage
    {
        MathPro.ProblemBuilder.LogarithimsBuilder builder;
        DispatcherTimer _timer;
        EventHandler _handler;
        public LogarithmicBuilderViewer()
        {
            InitializeComponent();

            this._timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Start();
            builder = new LogarithimsBuilder(ref _timer);
            this.ContentPanel.Children.Add(builder);
            builder.Draw(ref _timer);
            _handler = new EventHandler(ErrorBuilding);
            builder.ErrorHandler = _handler;
            //btnTest.Content = App.Builder.Action.ToString();
        }
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.ProblemDefinition = builder.GetEquation();
                App.Builder = new GeneralBuilder(MathBase.EquationType.Logarithmic, MathBase.Action.Solve);
                App.problemBuilt = new string[] { builder.Top, builder.Bot };
            }
            catch
            {
                MessageBox.Show("Something went wrong and we dont know what! Please try again");
            }
            //App.CurrentViewPlayerItem = sol;

            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/SolutionViewer/BasicSolutionPlayer.xaml", UriKind.Relative));
            });
        }
        private void ErrorBuilding(object sender, EventArgs e)
        {
            MessageBox.Show("Not allowed");
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            WriteableBitmap bmp = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            bmp.Render(this, null);
            bmp.Invalidate();

            Capture.Capturer capturer = new Capture.Capturer();
            capturer.SaveImage(bmp);
        }
        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            if (this.SupportedOrientations == SupportedPageOrientation.Landscape)
            {
                this.SupportedOrientations = SupportedPageOrientation.Portrait;
                this.Orientation = PageOrientation.Portrait;
            }
            else
            {
                this.SupportedOrientations = SupportedPageOrientation.Landscape;
                this.Orientation = PageOrientation.Landscape;
            }
        }
    }
}
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

namespace MathPro.Graphs
{
    public partial class MathGraphViewer : PhoneApplicationPage
    {
        MathGraphs graph;
        public MathGraphViewer()
        {
            InitializeComponent();

            if (App.GraphPoints.Count == 0)
                graph = new MathGraphs((MathBase.Equation)App.ProblemDefinition);
            else if (App.GraphPoints.Count == 2)
                graph = new MathGraphs(App.GraphPoints[0], App.GraphPoints[1]);
            else
                graph = new MathGraphs(App.GraphPoints[0], App.GraphPoints[1], App.GraphPoints[2]);

            this.ContentPanel.Children.Add(graph);
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            WriteableBitmap bmp = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            bmp.Render(this, null);
            bmp.Invalidate();

            Capture.Capturer capturer = new Capture.Capturer();
            capturer.SaveImage(bmp);
            //capturer.SendImage(bmp);
        }
    }
}
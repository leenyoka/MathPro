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
    public partial class TransformationGeometrySolutionViewer : PhoneApplicationPage
    {
        public TransformationGeometrySolutionViewer()
        {
            InitializeComponent();

            try
            {

                TransformationGeometry geo = new TransformationGeometry(App.MyGeo);

                this.ContentPanel.Children.Add(geo);
            }
            catch
            {
                MessageBox.Show("Something went wrong and we dont know what! Please try again");
            }
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

    /*"Menu/EquationsMenu.xaml"/>*/
}
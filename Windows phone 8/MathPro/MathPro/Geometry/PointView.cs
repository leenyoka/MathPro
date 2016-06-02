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
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace MathPro.Geometry
{
    public class PointView:Grid
    {
        #region Properties
        private int[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -2, -3, -4, -5, -6, -7, -8, -9 };
        public ListPicker xValue { get; set; }
        public ListPicker yValue { get; set; }
        #endregion Properties

        #region Constructor

        public PointView(int number)
        {
            Width = 200;
            Height = 200;
            Background = GetBackGround();
            this.Children.Add(GetLabel(number));
            SetX();
            SetY();
        }
        #endregion Constructor

        #region Methods
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

        private TextBlock GetLabel(int number)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(65, -150, 0, 0);
            block.Text = "Point " + number;
            block.Foreground = new SolidColorBrush(Colors.White);
            block.Height = 40;
            return block;
        }
        private void SetX()
        {
            xValue = new ListPicker();
            xValue.Header = "X Value";
            xValue.Margin = new Thickness(0, 30, 0, 0);
            xValue.ListPickerMode = ListPickerMode.Full;
            xValue.Width = 180;
            xValue.ItemsSource = values;
            this.Children.Add(xValue);
        }
        private void SetY()
        {
            yValue = new ListPicker();
            yValue.Header = "Y Value";
            yValue.Margin = new Thickness(0, 110, 0, 0);
            yValue.ListPickerMode = ListPickerMode.Full;
            yValue.Width = 180;
            yValue.ItemsSource = values;
            this.Children.Add(yValue);
        }

        public Point GetPoint()
        {
            int x = int.Parse(xValue.SelectedItem.ToString());
            int y = int.Parse(yValue.SelectedItem.ToString());

            return new Point(x, y);
        }
        #endregion Methods
    }
}

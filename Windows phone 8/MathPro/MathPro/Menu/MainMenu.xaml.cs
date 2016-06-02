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
using MathPro.Graphs;
using MathBase;
using System.Windows.Media.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.Phone.Tasks;

namespace MathPro.Menu
{
    public partial class MainMenu : PhoneApplicationPage
    {
        SettingsAssistant assistant = new SettingsAssistant();
        public MainMenu()
        {
            InitializeComponent();
            Initialize();
        }
        public void Initialize()
        {
            if (FirstTimeUser())
            {
                ThanksForUsingApp();
                SaveDefaultSettings();
            }
            else// if ( == "10")
            {
                string value = assistant.getValuePairFromSettings("UsageCount").Value;

                if (value == "10" || value == "20" || value == "30")
                {
                    if (!assistant.ValuePairExists("Reviewed"))
                    {
                        PleaseReviewOurApp();
                    }
                }
                else
                {
                    viewThanks.Visibility = System.Windows.Visibility.Collapsed;
                    viewOptions.Visibility = System.Windows.Visibility.Visible;
                    viewReview.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        public void PleaseReviewOurApp()
        {
            viewThanks.Visibility = System.Windows.Visibility.Collapsed;
            viewOptions.Visibility = System.Windows.Visibility.Collapsed;
            viewReview.Visibility = System.Windows.Visibility.Visible;

        }
        public void SaveDefaultSettings()
        {
            assistant.changeSetting(new valuePair("Orientation", "landscape"));
            //assistant.changeSetting(new valuePair("NumberOfSavedBasic", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedLogarithmic", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedInequalities", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedExponential", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedSolve", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedSimplify", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedFactor", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedDistribute", "0"));
            assistant.changeSetting(new valuePair("NumberOfSavedInequalities", "0"));


        }
        public void ThanksForUsingApp()
        {
            viewThanks.Visibility = System.Windows.Visibility.Visible;
            viewOptions.Visibility = System.Windows.Visibility.Collapsed;
            viewReview.Visibility = System.Windows.Visibility.Collapsed;
        }
        public Boolean FirstTimeUser()
        {
                  
            valuePair pair;

            if (assistant.ValuePairExists("UsageCount"))
            {
                pair = assistant.getValuePairFromSettings("UsageCount");
                pair.Value = (int.Parse(pair.Value) + 1).ToString();
                assistant.changeSetting(pair);
                return false;
            }
            else
            {
                assistant.changeSetting(new valuePair("UsageCount", "1"));
                return true;
            }
        
        }

        private void btnEquations_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Menu/EquationsMenu.xaml", UriKind.Relative));
            });
        }

        private void btnGraphs_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Menu/GraphMenu.xaml", UriKind.Relative));
            });
        }

        private void btnGeometry_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Menu/GeometryMenu.xaml", UriKind.Relative));
            });
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
            });
        }
        private void pdfStuff()
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();

            // Create an empty page
            PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

            XImage xImg = XImage.FromFile("");

            gfx.DrawImage(xImg, new Point());

            // Draw the text
            gfx.DrawString("Hello, World!", font, XBrushes.Black,
              new XRect(0, 0, page.Width, page.Height), XStringFormat.Center);

            // Save the document...
            string filename = "HelloWorld.pdf";
            document.Save(filename);
            // ...and start a viewer.
            //Process.Start(filename);
        }

        private void btnScreen_Click(object sender, RoutedEventArgs e)
        {
            //Capture the screen and set it to the internal picture box
            WriteableBitmap bmp = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            bmp.Render(this, null);
            bmp.Invalidate();

            Capture.Capturer capturer = new Capture.Capturer();
            capturer.SaveImage(bmp);
            //Grid myImage = new Grid();
            //myImage.Width = 200;
            //myImage.Height = 200;
           

            //ImageBrush brush = new ImageBrush();
            //brush.ImageSource = bmp; 
            //myImage.Background = brush;


            //ImageHolder.Children.Add(myImage);



        }

        private void btnNoThanks_Click(object sender, RoutedEventArgs e)
        {
            viewOptions.Visibility = System.Windows.Visibility.Visible;
            viewReview.Visibility = System.Windows.Visibility.Collapsed;
            viewThanks.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            viewOptions.Visibility = System.Windows.Visibility.Visible;
            viewReview.Visibility = System.Windows.Visibility.Collapsed;
            viewThanks.Visibility = System.Windows.Visibility.Collapsed;
            assistant.changeSetting(new valuePair("Reviewed", "true"));

            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
            throw new NotImplementedException();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            viewOptions.Visibility = System.Windows.Visibility.Visible;
            viewReview.Visibility = System.Windows.Visibility.Collapsed;
            viewThanks.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnBasicCalculator_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/ProblemBuilder/BasicCalculator.xaml", UriKind.Relative));
            });
        }
    }
}
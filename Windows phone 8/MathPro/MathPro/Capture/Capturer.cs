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
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Phone.Tasks;

namespace MathPro.Capture
{
    public class Capturer
    {
        #region properties

        #endregion properties

        #region Constructor

        #endregion Constructor

        #region Methods

        public void SaveImage(WriteableBitmap wr)
        {
            Random rand = new Random();
            string fileName =string.Format("mathPro_{0}_{1}_{2}{3}", 
                DateTime.Now.ToShortDateString().Replace("/","_").Replace(" ", "_").Replace(",", "_"),
                DateTime.Now.ToShortTimeString().Replace(",", "_").Replace(":","_"), 
                rand.Next(0,10000).ToString(), ".jpg").Replace("__", "_").Replace(" ", "");
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(fileName))
            {
                myStore.DeleteFile(fileName);
            }

            IsolatedStorageFileStream myFileStream = myStore.CreateFile(fileName);

            wr.SaveJpeg(myFileStream, wr.PixelWidth, wr.PixelHeight, 0, 85);
            myFileStream.Close();

            // Create a new stream from isolated storage, and save the JPEG file to the media library on Windows Phone.
            myFileStream = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            MediaLibrary library = new MediaLibrary();
            //byte[] buffer = ToByteArray(qrImage);
            library.SavePicture(fileName, myFileStream);

            MessageBox.Show("Image saved :)", "Saved", MessageBoxButton.OK);


        }
        //public void SendImage(WriteableBitmap wr)
        //{
        //    try
        //    {
        //        EmailComposeTask emailcomposer = new EmailComposeTask();
        //        emailcomposer.To = "<a href=\"mailto:leenyoka@gmail.com\" target=\"_blank\" rel =\"nofollow\">leenyoka@gmail.com</a>";
        //        emailcomposer.Subject = "MathPro screenshot";
        //        emailcomposer.Body = "Find Attachment";
        //        emailcomposer.Show();
        //    }
        //    catch(Exception ex)
        //    {
        //    }

        //}

        #endregion Methods

    }
}

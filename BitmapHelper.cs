using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace stego_metro
{
    static class BitmapHelper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToBitmapImage(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource retval = null;
            try
            {
                //var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                //var bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                retval = (ImageSource)Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            return retval;
        }

        public static Bitmap ToBitmap(this BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

/*        public static Bitmap ToGrayscale(this Bitmap bmp)
        {



            //for (int i = 0; i < bmp.Width; i++)
            //{
            //    for (int j = 0; j < bmp.Height; j++)
            //    {
            //        System.Drawing.Color col = bmp.GetPixel(i, j);
            //        System.Drawing.Color n  = System.Drawing.Color.FromArgb(col.R, 0, 0);
            //        bmp.SetPixel(i, j, n);
            //    }
            //}
            //return bmp;

            Bitmap newBitmap = new Bitmap(bmp.Width, bmp.Height);

            Graphics g = Graphics.FromImage(newBitmap);

            //grayscale colormatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f,.3f,.3f,0,0},
                    new float[] {.59f,.59f,.59f,0,0},
                    new float[] {.11f,.11f,.11f,0,0},
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}
                });

            //image attributes
            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }
        */

    }
}

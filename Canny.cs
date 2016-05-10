using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stego_metro
{

    /***********************************************************
     **                                                       **
     **             Canny Edge Detection                      **
     **                                                       **
     ***********************************************************/          
    public static class Canny
    {


        private static int[,] kernel = {{2,4,2},
                                       {4,6,4},
                                       {2,4,2}};

        private static int KernelSize = 3;
        private static int Weight = 30;
        private static int Width, Height;
        private static int[,] EdgeMap;
        private static int[,] VisitedMap;
        private static int[,] EdgePoints;


        /***********************************************************
        **                                                       **
        **             Convert Bitmap to int[,]                  **
        **                                                       **
        ***********************************************************/

        private static int[,] ReadImage(Bitmap Obj)
        {
            int i, j;
           int[,]  GreyImage = new int[Obj.Width, Obj.Height];  //[Row,Column]
            Bitmap image = Obj;
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        GreyImage[j, i] = (int)((imagePointer1[0] + imagePointer1[1] + imagePointer1[2]) / 3.0);

                        imagePointer1 += 4;  //4 bytes per pixel
                    }

                    imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4); //4 bytes per pixel
                }
            }
            image.UnlockBits(bitmapData1);
            return GreyImage;
        }



        /***********************************************************
        **                                                       **
        **             Convert int[,] to Bitmap                  **
        **                                                       **
        ***********************************************************/
        private static Bitmap DisplayImage(int[,] GreyImage)
        {
            int i, j;
            int W, H;
            W = GreyImage.GetLength(0);
            H = GreyImage.GetLength(1);
            Bitmap image = new Bitmap(W, H);
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, W, H),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {

                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        imagePointer1[0] = (byte)GreyImage[j, i];
                        imagePointer1[1] = (byte)GreyImage[j, i];
                        imagePointer1[2] = (byte)GreyImage[j, i];
                        imagePointer1[3] = (byte)255;
                        imagePointer1 += 4;
                    }
                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }
            }
            image.UnlockBits(bitmapData1);
            return image;
        }

        /***********************************************************
         **                                                       **
         **             Apply GaussianFilter to image             **
         **                                                       **
         ***********************************************************/
        private static int[,] GaussianFilter(Bitmap bmp,int[,] Data)
        {
            //  GenerateGaussianKernel(KernelSize, Sigma, out KernelWeight);
            Width = bmp.Width;
            Height = bmp.Height;
            
            int[,] Output = new int[Width, Height];
            int i, j, k, l;
            int Limit = KernelSize / 2;

            float Sum = 0;


            Output = Data; 

            for (i = Limit; i <= ((Width - 1) - Limit); i++)
            {
                for (j = Limit; j <= ((Height - 1) - Limit); j++)
                {
                    Sum = 0;
                    for (k = -Limit; k <= Limit; k++)
                    {

                        for (l = -Limit; l <= Limit; l++)
                        {
                            Sum = Sum + ((float)Data[i + k, j + l] * kernel[Limit + k, Limit + l]);

                        }
                    }
                    Output[i, j] = (int)(Math.Round(Sum / (float)Weight));
                }

            }


            return Output;
        }

        /***********************************************************
        **                                                       **
        **             Embed the text into cover image           **
        **             using canny edge detection algo           **
        **                                                       **
        ***********************************************************/ 

        public static Bitmap EmbedText(Bitmap bmp , string text)
        {
            //Detect Canny Edges First

            int[,] rimg = ReadImage(bmp);
            int[,] filtered = GaussianFilter(bmp, rimg);
            DetectCannyEdges(filtered);
            
            
            //Compare edges with cover image and put data at the edges of cover image
            int height = bmp.Height;
            int width = bmp.Width;
            string data = string.Empty;
            int red = 0, green = 0, blue = 0;
            int dataIndex = 0;
            int fullTrips;
            int extraBits;
            int count = 0;

            System.Drawing.Color color = new System.Drawing.Color();

            data = text.StringToBinary();
            fullTrips = data.Length / 3;
            extraBits = data.Length % 3;
            if (height * width * 3 < data.Length)
            {
                return null;
            }
            for (int i = 0; i < height; i++)
            {

                for (int j = 0; j < width; j++)
                {
                    if ( i<EdgeMap.GetLength(0) && j<EdgeMap.GetLength(1) &&  EdgeMap[i,j] == 255)
                    {
                        color = bmp.GetPixel(j, i);
                        if (fullTrips != 0)
                        {
                            count = count + 3;
                            //red
                            if (data[dataIndex].Equals('1'))
                            {
                                red = color.R | 1;
                                dataIndex++;
                            }
                            else
                            {
                                red = color.R & 254;
                                dataIndex++;
                            
                            }
                            //green
                            if (data[dataIndex].Equals('1'))
                            {
                                green = color.G | 1;
                                dataIndex++;
                            
                            }
                            else
                            {
                                green = color.G & 254;
                                dataIndex++;
                            

                            }

                            //blue
                            if (data[dataIndex].Equals('1'))
                            {
                                blue = color.B | 1;
                                dataIndex++;
                            

                            }
                            else
                            {
                                blue = color.B & 254;
                                dataIndex++;
                            

                            }
                            fullTrips--;

                        }
                        else if (extraBits == 1)
                        {
                            count++;
                            //red
                            if (data[dataIndex].Equals('1'))
                            {
                                red = color.R | 1;
                                dataIndex++;
                            
                            }
                            else
                            {
                                red = color.R & 254;
                                dataIndex++;
                            
                            }

                            //green+blue both 0
                            green = color.G & 254;
                            blue = color.G & 254;

                            extraBits = -1;
                        }
                        else if (extraBits == 2)
                        {
                            count += 2;

                            //red
                            if (data[dataIndex].Equals('1'))
                            {
                                red = color.R | 1;
                                dataIndex++;
                                
                            }
                            else
                            {
                                red = color.R & 254;
                                dataIndex++;
                                
                            }

                            //green
                            if (data[dataIndex].Equals('1'))
                            {
                                green = color.G | 1;
                                dataIndex++;
                                
                            }
                            else
                            {
                                green = color.G & 254;
                                dataIndex++;
                                

                            }

                            //blue zero
                            blue = color.G & 254;


                            extraBits = -1;
                        }
                        else
                        {
                            count += 3;
                            red = color.R - color.R % 2;
                            green = color.G - color.G % 2;
                            blue = color.B - color.B % 2;
                        }

                        bmp.SetPixel(j, i, System.Drawing.Color.FromArgb(red, green, blue));
                        
                    }
                }
              
            }
           
            return bmp;

        }

        /***********************************************************
        **                                                       **
        **             Extract the text from stego image         **
        **             using canny edge detection algo           **
        **                                                       **
        ***********************************************************/ 
        public static string ExtractText(Bitmap bmp)
        { 
            //read image in integer array
            int[,] rimg = ReadImage(bmp);
            //apply gaussianfilter
            int[,] filtered = GaussianFilter(bmp, rimg);
            //detect canny edges in filtered image
             DetectCannyEdges(filtered);
            

            //embed data in cover image
            int height = bmp.Height;
            int width = bmp.Width;
            string data = string.Empty;
            System.Drawing.Color color = new System.Drawing.Color();
            int zeroCount = 0;
            int keepBits;

            int i=0, j=0;
            int count = 0,tcount=0;
            for (i = 0; i < height; i++)
            {

                for (j = 0; j < width; j++)
                {

                    if (i < EdgeMap.GetLength(0) && j < EdgeMap.GetLength(1) && EdgeMap[i, j] == 255)
                    {

                        color = bmp.GetPixel(j, i);
                        if (zeroCount < 8)
                        {
                            data += (color.R % 2).ToString();
                            if (color.R % 2 == 0)
                            {
                                zeroCount++;
                                count++;
                            }
                            else
                            {
                                count++;
                                zeroCount = 0;
                            }

                            data += (color.G % 2).ToString();
                            if (color.G % 2 == 0)
                            {
                                count++;
                                zeroCount++;
                            }
                            else
                            {
                                count++;
                                zeroCount = 0;
                            }

                            data += (color.B % 2).ToString();
                            if (color.B % 2 == 0)
                            {
                                count++;
                                zeroCount++;
                            }
                            else
                            {
                                count++;
                                zeroCount = 0;
                            }

                        }
                        tcount++;
                    }
                }

            }
            keepBits = (data.Length / 8) * 8;
            data = data.Substring(0, keepBits);
            return data.BinaryToString();
        }


        /***********************************************************
        **                                                       **
        **             Find the Median of the int[,] of          **
        **            pixels and use it as upperthreshold        **
        **                                                       **
        ***********************************************************/

        private static float  findmedian(int[,] x)
        {
            int row = x.GetLength(0);
            int col = x.GetLength(1);
            int[] arr = new int[row * col]; 
            int  k = 0;
            
            for (int i = 0; i < row-1; i++)
            {
                for (int j = 0; j < col-1; j++)
                {
                           arr[k] = x[i,j];
                    k++;
                }
            }

            Array.Sort(arr);

            int middle = arr.Length / 2;

            if(arr.Length%2 == 1 )
                return arr[middle];
            else
                return ((arr[middle-1] + arr[middle])/2);
        }


        /***********************************************************
        **                                                       **
        **     Detect CannyEdges in gaussian Filtered Image      **
        **                                                       **
        ***********************************************************/


        private static void DetectCannyEdges(int[,] FilteredImage)   //Gaussian Filtered Input Image 

        {
            int Width = FilteredImage.GetLength(0);
            int Height = FilteredImage.GetLength(1);

            float[,] Gradient = new float[Width, Height];
            float[,] NonMax = new float[Width, Height];
            int[,] PostHysteresis = new int[Width, Height];

            float[,] DerivativeX = new float[Width, Height];
            float[,] DerivativeY = new float[Width, Height];
            EdgeMap = new int[Width,Height];
            VisitedMap = new int[Width,Height];

            //float MaxHysteresisThresh = 20F, MinHysteresisThresh = MaxHysteresisThresh / 2;


            float MaxHysteresisThresh=findmedian(FilteredImage), MinHysteresisThresh=MaxHysteresisThresh/2;

            
            //Sobel Masks
            int[,] Dx = {{1,0,-1},
                         {1,0,-1},
                         {1,0,-1}};

            int[,] Dy = {{1,1,1},
                         {0,0,0},
                         {-1,-1,-1}};


            DerivativeX = Differentiate(FilteredImage, Dx);
            DerivativeY = Differentiate(FilteredImage, Dy);

            int i, j;

            //Compute the gradient magnitude based on derivatives in x and y:
            for (i = 0; i <= (Width - 1); i++)
            {
                for (j = 0; j <= (Height - 1); j++)
                {
                    Gradient[i, j] = (float)Math.Sqrt((DerivativeX[i, j] * DerivativeX[i, j]) + (DerivativeY[i, j] * DerivativeY[i, j]));

                }

            }
            // Perform Non maximum suppression:
            // NonMax = Gradient;

            for (i = 0; i <= (Width - 1); i++)
            {
                for (j = 0; j <= (Height - 1); j++)
                {
                    NonMax[i, j] = Gradient[i, j];
                }
            }

            int Limit = KernelSize / 2;
            int r, c;
            float Tangent;


            for (i = Limit; i <= (Width - Limit) - 1; i++)
            {
                for (j = Limit; j <= (Height - Limit) - 1; j++)
                {

                    if (DerivativeX[i, j] == 0)
                        Tangent = 90F;
                    else
                        Tangent = (float)(Math.Atan(DerivativeY[i, j] / DerivativeX[i, j]) * 180 / Math.PI); //rad to degree



                    //Horizontal Edge
                    if (((-22.5 < Tangent) && (Tangent <= 22.5)) || ((157.5 < Tangent) && (Tangent <= -157.5)))
                    {
                        if ((Gradient[i, j] < Gradient[i, j + 1]) || (Gradient[i, j] < Gradient[i, j - 1]))
                            NonMax[i, j] = 0;
                    }


                    //Vertical Edge
                    if (((-112.5 < Tangent) && (Tangent <= -67.5)) || ((67.5 < Tangent) && (Tangent <= 112.5)))
                    {
                        if ((Gradient[i, j] < Gradient[i + 1, j]) || (Gradient[i, j] < Gradient[i - 1, j]))
                            NonMax[i, j] = 0;
                    }

                    //+45 Degree Edge
                    if (((-67.5 < Tangent) && (Tangent <= -22.5)) || ((112.5 < Tangent) && (Tangent <= 157.5)))
                    {
                        if ((Gradient[i, j] < Gradient[i + 1, j - 1]) || (Gradient[i, j] < Gradient[i - 1, j + 1]))
                            NonMax[i, j] = 0;
                    }

                    //-45 Degree Edge
                    if (((-157.5 < Tangent) && (Tangent <= -112.5)) || ((67.5 < Tangent) && (Tangent <= 22.5)))
                    {
                        if ((Gradient[i, j] < Gradient[i + 1, j + 1]) || (Gradient[i, j] < Gradient[i - 1, j - 1]))
                            NonMax[i, j] = 0;
                    }

                }
            }


            //PostHysteresis = NonMax;
            for (r = Limit; r <= (Width - Limit) - 1; r++)
            {
                for (c = Limit; c <= (Height - Limit) - 1; c++)
                {

                    PostHysteresis[r, c] = (int)NonMax[r, c];
                }

            }

            //Find Max and Min in Post Hysterisis
            float min, max;
            min = 100;
            max = 0;
            for (r = Limit; r <= (Width - Limit) - 1; r++)
                for (c = Limit; c <= (Height - Limit) - 1; c++)
                {
                    if (PostHysteresis[r, c] > max)
                    {
                        max = PostHysteresis[r, c];
                    }

                    if ((PostHysteresis[r, c] < min) && (PostHysteresis[r, c] > 0))
                    {
                        min = PostHysteresis[r, c];
                    }
                }

            float[,] GNH = new float[Width, Height];
            float[,] GNL = new float[Width, Height]; ;
            EdgePoints = new int[Width, Height];

            for (r = Limit; r <= (Width - Limit) - 1; r++)
            {
                for (c = Limit; c <= (Height - Limit) - 1; c++)
                {
                    if (PostHysteresis[r, c] >= MaxHysteresisThresh)
                    {

                        EdgePoints[r, c] = 1;
                        GNH[r, c] = 255;
                    }
                    if ((PostHysteresis[r, c] < MaxHysteresisThresh) && (PostHysteresis[r, c] >= MinHysteresisThresh))
                    {

                        EdgePoints[r, c] = 2;
                        GNL[r, c] = 255;

                    }

                }

            }

            HysterisisThresholding(EdgePoints);
            //EdgeMap has value 1 where edge is ,in cover image
            for (i = 0; i <= (Width - 1); i++)
                for (j = 0; j <= (Height - 1); j++)
                {
                    EdgeMap[i, j] = EdgeMap[i, j] * 255;
                }

            return;

        }


        /***********************************************************
        **                                                       **
        **     Apply Thresholding on initial edges found         **
        **                                                       **
        ***********************************************************/          

        private static  void HysterisisThresholding(int[,] Edges)
        {

            int i, j;
            int Limit = KernelSize / 2;


            for (i = Limit; i <= (Width - 1) - Limit; i++)
                for (j = Limit; j <= (Height - 1) - Limit; j++)
                {
                    if (Edges[i, j] == 1)
                    {
                        EdgeMap[i, j] = 1;

                    }

                }

            for (i = Limit; i <= (Width - 1) - Limit; i++)
            {
                for (j = Limit; j <= (Height - 1) - Limit; j++)
                {
                    if (Edges[i, j] == 1)
                    {
                        EdgeMap[i, j] = 1;
                        Travers(i, j);
                        VisitedMap[i, j] = 1;
                    }
                }
            }

            return;
        }

        private static void Travers(int X, int Y)
        {


            if (VisitedMap[X, Y] == 1)
            {
                return;
            }

            //1
            if (EdgePoints[X + 1, Y] == 2)
            {
                EdgeMap[X + 1, Y] = 1;
                VisitedMap[X + 1, Y] = 1;
                Travers(X + 1, Y);
                return;
            }
            //2
            if (EdgePoints[X + 1, Y - 1] == 2)
            {
                EdgeMap[X + 1, Y - 1] = 1;
                VisitedMap[X + 1, Y - 1] = 1;
                Travers(X + 1, Y - 1);
                return;
            }

            //3

            if (EdgePoints[X, Y - 1] == 2)
            {
                EdgeMap[X, Y - 1] = 1;
                VisitedMap[X, Y - 1] = 1;
                Travers(X, Y - 1);
                return;
            }

            //4

            if (EdgePoints[X - 1, Y - 1] == 2)
            {
                EdgeMap[X - 1, Y - 1] = 1;
                VisitedMap[X - 1, Y - 1] = 1;
                Travers(X - 1, Y - 1);
                return;
            }
            //5
            if (EdgePoints[X - 1, Y] == 2)
            {
                EdgeMap[X - 1, Y] = 1;
                VisitedMap[X - 1, Y] = 1;
                Travers(X - 1, Y);
                return;
            }
            //6
            if (EdgePoints[X - 1, Y + 1] == 2)
            {
                EdgeMap[X - 1, Y + 1] = 1;
                VisitedMap[X - 1, Y + 1] = 1;
                Travers(X - 1, Y + 1);
                return;
            }
            //7
            if (EdgePoints[X, Y + 1] == 2)
            {
                EdgeMap[X, Y + 1] = 1;
                VisitedMap[X, Y + 1] = 1;
                Travers(X, Y + 1);
                return;
            }
            //8

            if (EdgePoints[X + 1, Y + 1] == 2)
            {
                EdgeMap[X + 1, Y + 1] = 1;
                VisitedMap[X + 1, Y + 1] = 1;
                Travers(X + 1, Y + 1);
                return;
            }


            //VisitedMap[X, Y] = 1;
            return;
        }

        private static float[,] Differentiate(int[,] Data, int[,] Filter)
        {
            int i, j, k, l, Fh, Fw;

            Fw = Filter.GetLength(0);
            Fh = Filter.GetLength(1);
            float sum = 0;
            float[,] Output = new float[Width, Height];

            for (i = Fw / 2; i <= (Width - Fw / 2) - 1; i++)
            {
                for (j = Fh / 2; j <= (Height - Fh / 2) - 1; j++)
                {
                    sum = 0;
                    for (k = -Fw / 2; k <= Fw / 2; k++)
                    {
                        for (l = -Fh / 2; l <= Fh / 2; l++)
                        {
                            sum = sum + Data[i + k, j + l] * Filter[Fw / 2 + k, Fh / 2 + l];


                        }
                    }
                    Output[i, j] = sum;

                }

            }
            return Output;

        }


        static int[,] GaussianKernel;// = new int[3, 3];
        private static int GenerateGaussianKernel(int N, float S /*int Weight*/)
        {

            float Sigma = S;
            float pi;
            pi = (float)Math.PI;
            int i, j;
            int SizeofKernel = N;

            float[,] Kernel = new float[N, N];
            GaussianKernel = new int[N, N];
            float[,] OP = new float[N, N];
            float D1, D2;


            D1 = 1 / (2 * pi * Sigma * Sigma);
            D2 = 2 * Sigma * Sigma;

            float min = 1000;

            for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
            {
                for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                {
                    Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = ((1 / D1) * (float)Math.Exp(-(i * i + j * j) / D2));
                    if (Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] < min)
                        min = Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];

                }
            }
            int mult = (int)(1 / min);
            int sum = 0;
            if ((min > 0) && (min < 1))
            {

                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] * mult, 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
            else
            {
                sum = 0;
                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j], 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
            //Normalizing kernel Weight
          //  Weight = sum;
            string s = "";
            for ( i = 0; i < N; i++)
            {
                for ( j = 0; j < N; j++)
                {
                    s += " " +GaussianKernel[i, j];
                }
            }
            MessageBox.Show(s);
            return sum;
        }
    }
}

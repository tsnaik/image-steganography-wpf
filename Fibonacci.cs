using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stego_metro
{
    class Fibonacci
    {
        public static Bitmap EmbedText(Bitmap bmp, string text)
        {
            
           
            int height = bmp.Height;
            int width = bmp.Width;
            string data = string.Empty;
            int red = 0, green = 0, blue = 0;
            int dataIndex = 0;
            int fullTrips;
            int extraBits;
            int fiboIndex=0;
            int pixelCount = 0;
            int[] series;
            int seriesSize;
       //     string test = string.Empty;

            System.Drawing.Color color = new System.Drawing.Color();

            data = text.StringToBinary();

            //MessageBox.Show(data.Length.ToString());
            fullTrips = data.Length / 3;
            extraBits = data.Length % 3;
            series = GenerateSeries(height * width - 1);
            seriesSize = series.Count();

            if ((seriesSize * 3)< data.Length)
            {
                return null;
            }
         //   test += fullTrips.ToString() + extraBits.ToString();
           
                for (int i = 0; i < height; i++)
                {

                    for (int j = 0; j < width; j++, pixelCount++)
                    {
                        if (fiboIndex<series.Count()  && pixelCount == series[fiboIndex])
                        {
                            fiboIndex++;
                            color = bmp.GetPixel(j, i);
                            /*      test += color.R.ToString() + ",";
                                  test += color.G.ToString() + ",";
                                  test += color.B.ToString() + "; ";
                              */
                            if (fullTrips != 0)
                            {
                                //red
                                if (data[dataIndex].Equals('1'))
                                {
                                    red = color.R | 1;
                                    dataIndex++;
                                    //test += color.R.ToString() + "," + red.ToString()+";";
                                }
                                else
                                {
                                    red = color.R & 254;
                                    dataIndex++;
                                    //test += color.R.ToString() + "," + red.ToString() + ";";
                                }
                                //green
                                if (data[dataIndex].Equals('1'))
                                {
                                    green = color.G | 1;
                                    dataIndex++;
                                    //test += color.G.ToString() + "," + green.ToString() + ";";
                                }
                                else
                                {
                                    green = color.G & 254;
                                    dataIndex++;
                                    // test += color.G.ToString() + "," + green.ToString() + ";";

                                }

                                //blue
                                if (data[dataIndex].Equals('1'))
                                {
                                    blue = color.B | 1;
                                    dataIndex++;
                                    // test += color.B.ToString() + "," + blue.ToString() + ";";

                                }
                                else
                                {
                                    blue = color.B & 254;
                                    dataIndex++;
                                    // test += color.B.ToString() + "," + blue.ToString() + ";";

                                }
                                fullTrips--;

                            }
                            else if (extraBits == 1)
                            {
                                //red
                                if (data[dataIndex].Equals('1'))
                                {
                                    red = color.R | 1;
                                    dataIndex++;
                                    //  test += color.R.ToString() + "," + red.ToString()+";";
                                }
                                else
                                {
                                    red = color.R & 254;
                                    dataIndex++;
                                    //    test += color.R.ToString() + "," + red.ToString() + ";";
                                }

                                //green+blue both 0
                                green = color.G & 254;
                                blue = color.G & 254;

                                extraBits = -1;
                            }
                            else if (extraBits == 2)
                            {

                                //red
                                if (data[dataIndex].Equals('1'))
                                {
                                    red = color.R | 1;
                                    dataIndex++;
                                    //  test += color.R.ToString() + "," + red.ToString()+";";
                                }
                                else
                                {
                                    red = color.R & 254;
                                    dataIndex++;
                                    //    test += color.R.ToString() + "," + red.ToString() + ";";
                                }

                                //green
                                if (data[dataIndex].Equals('1'))
                                {
                                    green = color.G | 1;
                                    dataIndex++;
                                    //test += color.G.ToString() + "," + green.ToString() + ";";
                                }
                                else
                                {
                                    green = color.G & 254;
                                    dataIndex++;
                                    //test += color.G.ToString() + "," + green.ToString() + ";";

                                }

                                //blue zero
                                blue = color.G & 254;


                                extraBits = -1;
                            }
                            else
                            {
                                red = color.R - color.R % 2;
                                green = color.G - color.G % 2;
                                blue = color.B - color.B % 2;
                                /*      test += color.R.ToString() + "," + red.ToString() + ";";
                                      test += color.G.ToString() + "," + green.ToString() + ";";
                                      test += color.B.ToString() + "," + blue.ToString() + ";";
                                  */
                            }


                            bmp.SetPixel(j, i, System.Drawing.Color.FromArgb(red, green, blue));
                            //MessageBox.Show(test, "Details", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        }
                    }
                    //  test += color.B.ToString() + "," + blue.ToString() + ";";

                }
            
         
            // MessageBox.Show(test, "Details", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return bmp;

        }


        public static string ExtractText(Bitmap bmp)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            string data = string.Empty;
            System.Drawing.Color color = new System.Drawing.Color();
            int zeroCount = 0;
            int keepBits;
            int fiboIndex = 0;
            int pixelCount = 0;
            int[] series;
            int seriesSize;
            series = GenerateSeries(height * width - 1);
            seriesSize = series.Count();
            for (int i = 0; i < height; i++)
            {

                for (int j = 0; j < width; j++, pixelCount++)
                {
                    if (fiboIndex < series.Count() && series[fiboIndex] == pixelCount)
                    {
                        fiboIndex++;
                        color = bmp.GetPixel(j, i);
                        if (zeroCount < 8)
                        {
                            data += (color.R % 2).ToString();
                            if (color.R % 2 == 0)
                            {
                                zeroCount++;
                            }
                            else
                            {
                                zeroCount = 0;
                            }

                            data += (color.G % 2).ToString();
                            if (color.G % 2 == 0)
                            {
                                zeroCount++;
                            }
                            else
                            {
                                zeroCount = 0;
                            }

                            data += (color.B % 2).ToString();
                            if (color.B % 2 == 0)
                            {
                                zeroCount++;
                            }
                            else
                            {
                                zeroCount = 0;
                            }
                        }

                    }
                }
            }
            keepBits = (data.Length / 8) * 8;
            data = data.Substring(0, keepBits);

            return data.BinaryToString();

            // return data;
        }


        private static int[] GenerateSeries(int pixels)//pixels-1 (from 0)
        {
            int a = 0, b = 1, c = 0;
            var list = new List<int>();
            if (pixels == 0)
            {
                list.Add(0);
                return list.ToArray();
            }
            else if (pixels == 1)
            {
                list.Add(0);
                list.Add(1);

                return list.ToArray();
            }
            else if (pixels > 1)
            {
                list.Add(0);


                for (int i = 0; i < int.MaxValue; i++)
                {
                    c = a + b;
                    if (c > pixels)
                    {
                        return list.ToArray();
                    }
                    list.Add(c);
                    a = b;
                    b = c;
                }


            }
            
            return null;
        }
    }
}

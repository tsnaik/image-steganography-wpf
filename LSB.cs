using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stego_metro
{
    class LSB
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
          //  int count = 0;
            //MessageBox.Show("Characters : "+(text.Length).ToString());
            //string test = string.Empty;

            System.Drawing.Color color = new System.Drawing.Color();

            data = text.StringToBinary();
        //    MessageBox.Show("d : " +(data.Length/8).ToString());
            fullTrips = data.Length / 3;
            extraBits = data.Length % 3;
            if (height * width * 3 < data.Length)
            {
                return null;
            }
            //test += (((fullTrips*3) + extraBits)/8).ToString();
            //MessageBox.Show(test);
            for (int i = 0; i < height; i++)
            {

                for (int j = 0; j < width; j++)
                {
                    color = bmp.GetPixel(j, i);
                    /*      test += color.R.ToString() + ",";
                          test += color.G.ToString() + ",";
                          test += color.B.ToString() + "; ";
                      */
                    if (fullTrips != 0)
                    {
            //            count = count+3;
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
              //          count++;
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
                //        count += 2 ;

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
                  //      count += 3;
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
                //  test += color.B.ToString() + "," + blue.ToString() + ";";

            }
            // MessageBox.Show("count : "+  (count).ToString()    ,"Details", MessageBoxButton.OK, MessageBoxImage.Exclamation);

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

            int i=0, j=0;
        //    int count = 0,tcount=0;
        //    MessageBox.Show("img length :" + ((height*width*3)/8).ToString());
            for (i = 0; i < height; i++)
            {

                for (j = 0; j < width; j++)
                {
                    color = bmp.GetPixel(j, i);
                    if (zeroCount < 8)
                    {
                        data += (color.R % 2).ToString();
                        if (color.R % 2 == 0)
                        {
                            zeroCount++;
              //              count++;
                        }
                        else
                        {
                //            count++;
                            zeroCount = 0;
                        }

                        data += (color.G % 2).ToString();
                        if (color.G % 2 == 0)
                        {
                  //          count++;
                            zeroCount++;
                        }
                        else
                        {
                    //        count++;
                            zeroCount = 0;
                        }

                        data += (color.B % 2).ToString();
                        if (color.B % 2 == 0)
                        {
                      //      count++;
                            zeroCount++;
                        }
                        else
                        {
                        //    count++;
                            zeroCount = 0;
                        }

                    }
                //    tcount++;
                }

            }
          //  MessageBox.Show("imgafter length :" + ((count) / 8).ToString());

            //MessageBox.Show("dataoriginal length :" + ((tcount*3)/8 ).ToString());
            keepBits = (data.Length / 8) * 8;
           // MessageBox.Show("keep length :" + ((keepBits / 8)).ToString());

            data = data.Substring(0, keepBits);
            //MessageBox.Show("data length :"+ ((data.Length) / 8).ToString());

            return data.BinaryToString();
            
            // return data;
        }

    }
}

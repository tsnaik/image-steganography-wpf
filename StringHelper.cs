using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stego_metro
{
    public static class StringHelper
    {
        public static string BinaryToString(this string binary)
        {
            var list = new List<Byte>();


           // MessageBox.Show(((binary.Length)/8).ToString());

            for (int i = 0; i < binary.Length; i += 8)
            {

                string t = binary.Substring(i, 8);
                list.Add(Convert.ToByte(t, 2));

            }
           // MessageBox.Show(       Encoding.ASCII.GetString(list.ToArray()).Length.ToString());

            return Encoding.ASCII.GetString(list.ToArray());
        }

        public static string StringToBinary(this string str)
        {
            string ret = String.Empty;
            foreach (string letter in str.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')))
            {
                ret += (letter);
            }
            return ret;
        }
    }
}

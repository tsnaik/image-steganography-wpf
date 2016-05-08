using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace stego_metro
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int t = DateTime.Now.Millisecond % 6;
            switch (t)
            {
                case 0:
                    textBlock2.Text = "Tanmay Naik";
                    textBlock3.Text = "Prabhakar Niraula";
                    textBlock4.Text = "Darshil Parikh";
                    break;

                case 1:
                    textBlock2.Text = "Tanmay Naik";
                    textBlock3.Text = "Darshil Parikh";
                    textBlock4.Text = "Prabhakar Niraula";
                    break;
                case 2:
                    textBlock2.Text = "Prabhakar Niraula";
                    textBlock3.Text = "Tanmay Naik";
                    textBlock4.Text = "Darshil Parikh";
                    break;
                case 3:
                    textBlock2.Text = "Prabhakar Niraula";
                    textBlock3.Text = "Darshil Parikh";
                    textBlock4.Text = "Tanmay Naik";
                    break;
                case 4:
                    textBlock2.Text = "Darshil Parikh";
                    textBlock3.Text = "Prabhakar Niraula";
                    textBlock4.Text = "Tanmay Naik";
                    break;
                case 5:
                    textBlock2.Text = "Darshil Parikh";
                    textBlock3.Text = "Tanmay Naik";
                    textBlock4.Text = "Prabhakar Niraula";
                    break;

                default:
                    break;
            }
           
        }
    }
}

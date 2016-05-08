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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Drawing;
using System.Security.Cryptography;
using MahApps.Metro.Controls.Dialogs;

namespace stego_metro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Bitmap original = null;
        private BitmapImage originalImage = null;

        private Bitmap stego = null;
        private BitmapImage stegoImage = null;
        string filename="stego", fileExtension;

        string text = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog diag = new Microsoft.Win32.OpenFileDialog();
			//diag.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
			diag.Filter = "Image files| *.jpg; *.jpeg; *.png";
            // Bitmap bmp;
            //    text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;

            Nullable<bool> result = diag.ShowDialog();
            if (result == true)
            {
                filename = System.IO.Path.GetFileNameWithoutExtension(diag.FileName);
                fileExtension = System.IO.Path.GetExtension(diag.FileName);
                try
                {
                    originalImage = new BitmapImage(new Uri(diag.FileName));
                }
                catch (NotSupportedException)
                {
                    await this.ShowMessageAsync("Unsupported File", "Selected file is not supported.", MessageDialogStyle.Affirmative);

                    //     MessageBox.Show("Selected file is not supported.", "Unsupported File", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                original = stego = originalImage.ToBitmap();

                //image.Source = src;
                image.Source = originalImage;
                //imageBorder.Visibility = Visibility.Collapsed;
                imageBorder.Opacity = 1;
                imageBorder.BorderThickness = new Thickness(0);
            }
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            text = richTextBox.Text;
            // text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (original == null)
            {
                await this.ShowMessageAsync("Error", "Image is not selected. Please choose an image first.", MessageDialogStyle.Affirmative);

              //  MessageBox.Show("Image is not selected. Please choose an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (encrypt.IsChecked == true)
            {
                if (keyBox.Text != string.Empty)
                {
                    
                    text = AES.StringCipher.Encrypt(text, keyBox.Text);
                    //byte[] plain = Convert.FromBase64String(text);
                    //text = Encoding.UTF8.GetString(plain);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Provide cryptography key.", MessageDialogStyle.Affirmative);

                 //   MessageBox.Show("Provide cryptography key.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (radioLSB.IsChecked == true)
            {
                stego = LSB.EmbedText(original, text);
            }
            else if (radioFibo.IsChecked == true)
            {
                stego = Fibonacci.EmbedText(original, text);
            }
            else if (radioEdge.IsChecked == true)
            {
                stego = Canny.EmbedText(original, text);
            }
            //   image.Source = bmp.Bitmap2BitmapImage();


            //  MessageBox.Show(text);
            if (stego == null)
            {
                await this.ShowMessageAsync("Error", "Image is not large enough. Please select another image.", MessageDialogStyle.Affirmative);
              //  MessageBox.Show("Image is not large enough. Please select another image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // MessageBox.Show("Image is not large enough. Please select another image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            else
            {
                await this.ShowMessageAsync("Success", "Image successfully Generated. Now you can save.", MessageDialogStyle.Affirmative);

//                MessageBox.Show("Image successfully Generated. Now you can save.", "Success");
                //     image2.Source = stegoBmp.Bitmap2BitmapImage();

            }


        }

        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (stego == null)
            {
                await this.ShowMessageAsync("Error", "Nothing to save.", MessageDialogStyle.Affirmative);

                //    MessageBox.Show("Nothing to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = filename + "_stego";
            dlg.DefaultExt = fileExtension;
            //    MessageBox.Show(fileExtension);
            dlg.Filter = "Image File (*" + fileExtension + ")|*" + fileExtension;
            Nullable<bool> res = dlg.ShowDialog();
            if (res == true)
            {

                try
                {
                    stego.Save(dlg.FileName);
                }
                catch (Exception)
                {

                    await this.ShowMessageAsync("Error", "Error Occured. Try another file.", MessageDialogStyle.Affirmative);

                   // MessageBox.Show("Error Occured. Try another file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                await this.ShowMessageAsync("Success", "Image saved successfully.", MessageDialogStyle.Affirmative);

               // MessageBox.Show("Image saved successfully", "Success!", MessageBoxButton.OK);
            }

        }

        private void encrypt_Checked(object sender, RoutedEventArgs e)
        {
            keyBlock.Visibility = Visibility.Visible;
            keyBox.Visibility = Visibility.Visible;

        }

        private void encrypt_Unchecked(object sender, RoutedEventArgs e)
        {
            keyBlock.Visibility = Visibility.Collapsed;
            keyBox.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
              new About().Show();
            //MetroWindow window = new MetroWindow();
            //window.Show();
        }

        private async void extractButton_Click(object sender, RoutedEventArgs e)
        {
            if (stego == null)
            {
                await this.ShowMessageAsync("Error", "Image is not selected. Please choose an image first.", MessageDialogStyle.Affirmative);

                //MessageBox.Show("Image is not selected. Please choose an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            if (radioLSB.IsChecked == true)
            {
                text = LSB.ExtractText(stego);
            }
            else if (radioFibo.IsChecked == true)
            {
                text = Fibonacci.ExtractText(stego);
            }
            else if (radioEdge.IsChecked == true)
            {
                text = Canny.ExtractText(stego);
            }
            if (encrypt.IsChecked == true)
            {
                if (keyBox.Text != string.Empty)
                {
                    try
                    {
                        //richTextBox.Text = AES.StringCipher.Decrypt(text, keyBox.Text);
                        //var tmp = Encoding.UTF8.GetBytes(text.Substring(0, text.Length - 1));
                        //var base64 = Convert.ToBase64String(tmp);
                        richTextBox.Text = AES.StringCipher.Decrypt(text.Substring(0, text.Length - 1), keyBox.Text);
                    }
                    catch (FormatException ferr)
                    {
                        await this.ShowMessageAsync("Error", "Error Occured. " + ferr.Message, MessageDialogStyle.Affirmative);

                       // MessageBox.Show("Error Occured. " + ferr.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    catch (Exception err)
                    {
                        await this.ShowMessageAsync("Error", "Error Occured. Please check key.", MessageDialogStyle.Affirmative);

                        //MessageBox.Show("Error Occured. Please check key." + err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;

                    }
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Provide cryptography key.", MessageDialogStyle.Affirmative);

                    //  MessageBox.Show("Provide cryptography key.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                richTextBox.Text = text.Substring(0, text.Length - 1);
            }
            //    richTextBox.Document.Blocks.Clear();
            //richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
            //  MessageBox.Show("Successfully Generated. Now you can save.");

        }
    }
}

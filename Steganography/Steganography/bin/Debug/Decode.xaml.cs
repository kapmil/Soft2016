using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Steganography
{
    /// <summary>
    /// Interaction logic for Decode.xaml
    /// </summary>
    public partial class Decode : Page
    {
        MainWindow mw;

        public Decode()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog().Equals(true))
            {
                image.Source = new BitmapImage(new Uri(openFileDialog.FileName.ToString(), UriKind.Absolute));
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);

            
            int index = 0;
            int R = 0, G = 0, B = 0, A = 0;

            System.Drawing.Color lastPixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int size = lastPixel.G * 255 + lastPixel.B;
            var bits = new BitArray(size * size * 32);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (index + 12 <= bits.Length)
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);
                        R = pixel.R;
                        G = pixel.G;
                        B = pixel.B;
                        A = pixel.A;

                        R &= 7;
                        var b1 = 0x00;
                        b1 |= R;
                        var a1 = b1 & 1;
                        if (a1 == 1)
                            bits.Set(index, true);
                        else bits.Set(index, false);
                        var a2 = b1 & 2;
                        if(a2 == 2)
                            bits.Set(index+1, true);
                        else bits.Set(index+1, false);
                        var a3 = b1 & 4;
                        if(a3 == 4)
                            bits.Set(index + 2, true);
                        else bits.Set(index + 2, false);
                        index += 3;
                        G &= 7;
                        b1 = 0x00;
                        b1 |= G;
                        a1 = b1 & 1;
                        if (a1 == 1)
                            bits.Set(index, true);
                        else bits.Set(index, false);
                        a2 = b1 & 2;
                        if (a2 == 2)
                            bits.Set(index + 1, true);
                        else bits.Set(index + 1, false);
                        a3 = b1 & 4;
                        if (a3 == 4)
                            bits.Set(index + 2, true);
                        else bits.Set(index + 2, false);
                        index += 3;
                        B &= 7;
                        b1 = 0x00;
                        b1 |= B;
                        a1 = b1 & 1;
                        if (a1 == 1)
                            bits.Set(index, true);
                        else bits.Set(index, false);
                        a2 = b1 & 2;
                        if (a2 == 2)
                            bits.Set(index + 1, true);
                        else bits.Set(index + 1, false);
                        a3 = b1 & 4;
                        if (a3 == 4)
                            bits.Set(index + 2, true);
                        else bits.Set(index + 2, false);
                        index += 3;
                        A &= 7;
                        b1 = 0x00;
                        b1 |= A;
                        a1 = b1 & 1;
                        if (a1 == 1)
                            bits.Set(index, true);
                        else bits.Set(index, false);
                        a2 = b1 & 2;
                        if (a2 == 2)
                            bits.Set(index + 1, true);
                        else bits.Set(index + 1, false);
                        a3 = b1 & 4;
                        if (a3 == 4)
                            bits.Set(index + 2, true);
                        else bits.Set(index + 2, false);
                        index += 3;
                    }
                }
            }
                       
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);

            int h = (int)Math.Floor(Math.Sqrt(ret.Length)) / 2;
            var output = new Bitmap(h, h);                  
            var rect = new System.Drawing.Rectangle(0, 0, h, h);
            var bmpData = output.LockBits(rect,
                ImageLockMode.ReadWrite, output.PixelFormat);
            var arrRowLength = h * System.Drawing.Image.GetPixelFormatSize(output.PixelFormat) / 8;
            var ptr = bmpData.Scan0;
            for (var i = 0; i < h; i++)
            {
                Marshal.Copy(ret, i * arrRowLength, ptr, arrRowLength);
                ptr += bmpData.Stride;
            }
            output.UnlockBits(bmpData);


            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            if (saveFile.ShowDialog().Equals(true))
            {
                output.Save(saveFile.FileName.ToString());
                image1.Source = new BitmapImage(new Uri(saveFile.FileName.ToString(), UriKind.Absolute));
            }
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Page p = new Page();
            p = Application.LoadComponent(new Uri("Encode.xaml", UriKind.Relative)) as Page;
            mw = this.Parent as MainWindow;
            mw.Content = null;
            mw.AddChildOverriden(p);
        }

        // Izvlacenje teksta iz slike
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);
            String text = "";

            System.Drawing.Color lastPixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int textLength = lastPixel.R * 65025 + lastPixel.G * 255 + lastPixel.B;
            int index = 0;

            for(int i = 0; i< img.Width; i++)
            {
                for(int j=0; j< img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if (index < textLength)
                    {
                        int value = pixel.B;
                        char c = Convert.ToChar(value);
                        String letter = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });

                        text += letter;
                        index++;
                    }
                }
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Text files (*.txt)|*.txt";

            if (saveFile.ShowDialog().Equals(true))
            {
                System.IO.File.WriteAllText(saveFile.FileName, text);
                textBox1.Text = saveFile.FileName.ToString();
            }


        }


        // Izvlacenje slike iz slike preko jednog LSB po bajtu
        public void decodeImage1LSB()
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);


            int index = 0;
            int R = 0, G = 0, B = 0, A = 0;

            System.Drawing.Color lastPixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int size = lastPixel.G * 255 + lastPixel.B;
            var bits = new BitArray(size * size * 32);
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (index < bits.Length)
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);
                        if (pixel.R % 2 == 0)
                        {
                            bits.Set(index, false);
                        }
                        else
                        {
                            bits.Set(index, true);
                        }
                        index++;
                        if (pixel.G % 2 == 0)
                        {
                            bits.Set(index, false);
                        }
                        else
                        {
                            bits.Set(index, true);
                        }
                        index++;
                        if (pixel.B % 2 == 0)
                        {
                            bits.Set(index, false);
                        }
                        else
                        {
                            bits.Set(index, true);
                        }
                        index++;
                        if (pixel.A % 2 == 0)
                        {
                            bits.Set(index, false);
                        }
                        else
                        {
                            bits.Set(index, true);
                        }
                        index++;
                    }
                }
            }
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);

            int h = (int)Math.Floor(Math.Sqrt(ret.Length)) / 2;
            var output = new Bitmap(h, h);                  //proveri ove velicine slike
            var rect = new System.Drawing.Rectangle(0, 0, h, h);
            var bmpData = output.LockBits(rect,
                ImageLockMode.ReadWrite, output.PixelFormat);
            var arrRowLength = h * System.Drawing.Image.GetPixelFormatSize(output.PixelFormat) / 8;
            var ptr = bmpData.Scan0;
            for (var i = 0; i < h; i++)
            {
                Marshal.Copy(ret, i * arrRowLength, ptr, arrRowLength);
                ptr += bmpData.Stride;
            }
            output.UnlockBits(bmpData);


            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            if (saveFile.ShowDialog().Equals(true))
            {
                output.Save(saveFile.FileName.ToString());
                image1.Source = new BitmapImage(new Uri(saveFile.FileName.ToString(), UriKind.Absolute));
            }
        }
    }
}

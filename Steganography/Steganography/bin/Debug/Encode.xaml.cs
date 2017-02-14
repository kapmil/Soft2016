using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Steganography
{
    /// <summary>
    /// Interaction logic for Encode.xaml
    /// </summary>
    public partial class Encode : Page
    {
        MainWindow mw;

        public Encode()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg;|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                image.Source = new BitmapImage(new Uri(openFileDialog.FileName.ToString(), UriKind.Absolute));
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg;|Text files (*.txt)|*.txt;";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                textBox.Text = openFileDialog.FileName.ToString();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.Contains(".jp") || textBox.Text.Contains(".png")) 
            {
                hidePicture();
            }
            else if (textBox.Text.Contains(".txt"))
            {
                hideText();
            }
           
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Page p = new Page();
            p = Application.LoadComponent(new Uri("Decode.xaml", UriKind.Relative)) as Page;
            mw = this.Parent as MainWindow;
            mw.Content = null;
            mw.AddChildOverriden(p);
        }

        // Sakrivanje slike pomocu 3 najmanje znacajna bita po bajtu
        private void hidePicture()
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);  //slika u koju se sakriva informacija

            BitmapSource img2 = new BitmapImage(new Uri(textBox.Text, UriKind.Absolute));    // slika koja se sakriva

            //provera da li je slika koja se sakriva manja od cover slike 
              if(img2.Width*img2.Height*3 > img.Width * img.Height)
              {
                MessageBox.Show("Slika koju ste izabrali je prevelika za sakrivanje unutar odabrane slike.");
                return;
              }

            int stride = (int)img2.PixelWidth * (img2.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)img2.PixelHeight * stride];

            img2.CopyPixels(pixels, stride, 0);
            var bits = new BitArray(pixels);
            int index = 0;

            int R = 0, G = 0, B = 0, A = 0;
            bool poslednji = true;


             for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (index + 12 < bits.Length)
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);
                        R = pixel.R;
                        G = pixel.G;
                        B = pixel.B;
                        A = pixel.A;

                        R &= 0xF8;
                        G &= 0xF8;
                        B &= 0xF8;
                        A &= 0xF8;

                        var bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        byte[] b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        int bb = BitConverter.ToInt32(b, 0);
                        R |= bb;

                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        G |= bb;

                        bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        B |= bb;

                        bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        A |= bb;
                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));
                    }
                    else if (poslednji)
                    {
                        int size = (int)Math.Floor(Math.Sqrt(bits.Length / 32));
                        img.SetPixel(img.Width - 1, img.Height - 1, System.Drawing.Color.FromArgb(0, 0, size / 255, size % 255));
                        System.Drawing.Color pixel = img.GetPixel(j, i);
                        R = pixel.R;
                        G = pixel.G;
                        B = pixel.B;
                        A = pixel.A;

                        R &= 0xF8;
                        G &= 0xF8;
                        B &= 0xF8;
                        A &= 0xF8;
                        
                        if(index+2 >= bits.Length)
                        {
                            continue;
                        }

                        var bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        byte[] b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        int bb = BitConverter.ToInt32(b, 0);
                        R |= bb;

                        if (index + 2 >= bits.Length)
                        {
                            continue;
                        }
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        G |= bb;

                        if (index + 2 >= bits.Length)
                        {
                            continue;
                        }
                        bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        B |= bb;

                        if (index + 2 >= bits.Length)
                        {
                            continue;
                        }
                        bits1 = new BitArray(3);
                        bits1.Set(0, bits.Get(index));
                        bits1.Set(1, bits.Get(index + 1));
                        bits1.Set(2, bits.Get(index + 2));
                        b = new byte[8];
                        bits1.CopyTo(b, 0);
                        index += 3;

                        bb = BitConverter.ToInt32(b, 0);
                        A |= bb;
                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));
                        poslednji = false;
                        
                    }
                    else
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);

                        R = pixel.R - pixel.R % 2;
                        G = pixel.G - pixel.G % 2;
                        B = pixel.B - pixel.B % 2;
                        A = pixel.A - pixel.A % 2;
                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));
                        
                    }
                }
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            if (saveFile.ShowDialog().Equals(true))
            {
                //mw.textBox1.Text = saveFile.FileName.ToString();
                img.Save(saveFile.FileName.ToString());
                image1.Source = new BitmapImage(new Uri(saveFile.FileName.ToString(), UriKind.Absolute));
            }
        }

        private void hideText()
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);  //slika u koju sakrivamo

            string text = System.IO.File.ReadAllText(textBox.Text);

            int index = 0;

            //provera da li je tekst koji se sakriva manji od cover slike 
            if (text.Length >= img.Width * img.Height || text.Length >= 16646650)
            {
                MessageBox.Show("Tekst koji ste izabrali je preveliki za sakrivanje unutar odabrane slike.");
                return;
            }

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if(index < text.Length)
                    {
                        char letter = Convert.ToChar(text.Substring(j, 1));
                        int value = Convert.ToInt32(letter);
                        img.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, value));
                        index++;
                    }
                    if(i == img.Width-1 && j == img.Height - 1) // u poslednji pixel upisuje se duzina teksta
                    {
                        img.SetPixel(i, j, System.Drawing.Color.FromArgb(text.Length/65025, text.Length/255, text.Length%255));
                    }
                }
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            if (saveFile.ShowDialog().Equals(true))
            {
                //mw.textBox1.Text = saveFile.FileName.ToString();
                img.Save(saveFile.FileName.ToString());
                image1.Source = new BitmapImage(new Uri(saveFile.FileName.ToString(), UriKind.Absolute));
            }

        }

        // Sakrivanje slike pomocu pomocu least significant beat-a
        public void hidePic1LSB()
        {
            Bitmap img = new Bitmap(((BitmapImage)image.Source).UriSource.AbsolutePath);  //slika u koju se sakriva informacija

            BitmapSource img2 = new BitmapImage(new Uri(textBox.Text, UriKind.Absolute));    // slika koja se sakriva

            //provera da li je slika koja se sakriva manja od cover slike 
            if (img2.Width * img2.Height * 8 > img.Width * img.Height)
            {
                MessageBox.Show("Slika koju ste izabrali je prevelika za sakrivanje unutar odabrane slike.");
                return;
            }

            int stride = (int)img2.PixelWidth * (img2.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)img2.PixelHeight * stride];

            img2.CopyPixels(pixels, stride, 0);
            var bits = new BitArray(pixels);
            int index = 0;

            int R = 0, G = 0, B = 0, A = 0;
            bool poslednji = true;

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (bits.Length >= index + 4)
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);

                        R = pixel.R - pixel.R % 2;
                        G = pixel.G - pixel.G % 2;
                        B = pixel.B - pixel.B % 2;
                        A = pixel.A - pixel.A % 2;

                        if (bits.Get(index))
                            R += 1;
                        index++;
                        if (bits.Get(index))
                            G += 1;
                        index++;
                        if (bits.Get(index))
                            B += 1;
                        index++;
                        if (bits.Get(index))
                            A += 1;
                        index++;

                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));

                    }
                    else if (poslednji)
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);

                        R = pixel.R - pixel.R % 2;
                        G = pixel.G - pixel.G % 2;
                        B = pixel.B - pixel.B % 2;
                        A = pixel.A - pixel.A % 2;

                        R += 1;
                        index++;
                        G += 1;
                        index++;
                        B += 1;
                        index++;
                        A += 1;
                        index++;

                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));
                        poslednji = false;
                    }
                    else
                    {
                        System.Drawing.Color pixel = img.GetPixel(j, i);

                        R = pixel.R - pixel.R % 2;
                        G = pixel.G - pixel.G % 2;
                        B = pixel.B - pixel.B % 2;
                        A = pixel.A - pixel.A % 2;
                        img.SetPixel(j, i, System.Drawing.Color.FromArgb(A, R, G, B));

                    }
                }
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            if (saveFile.ShowDialog().Equals(true))
            {
                //mw.textBox1.Text = saveFile.FileName.ToString();
                img.Save(saveFile.FileName.ToString());
                image1.Source = new BitmapImage(new Uri(saveFile.FileName.ToString(), UriKind.Absolute));
            }
        }
    }
}

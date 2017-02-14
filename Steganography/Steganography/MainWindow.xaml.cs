using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Steganography
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button ButtoninXAML;


        public string SourceUri
        {
            get
            {
                String p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on).ToString();
                p = p.Replace('\\', '/');
                return Path.Combine(p, "images/b9.jpg");
            }
        }

        public string SourceUri2
        {
            get
            {
                String p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on).ToString();
                p = p.Replace('\\', '/');
                return Path.Combine(p, "images/icon1.png");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Page p = new Page();
            p = Application.LoadComponent(new Uri("Encode.xaml", UriKind.Relative)) as Page;
            this.Content = null;
            this.AddChild(p);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Page p = new Page();
            p = Application.LoadComponent(new Uri("Decode.xaml", UriKind.Relative)) as Page;
            this.Content = null;
            this.AddChild(p);
        }

        public void AddChildOverriden(Page p)
        {
            this.AddChild(p);
        }
    }
}

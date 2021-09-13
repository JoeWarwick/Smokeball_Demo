using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Smokeball_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string placeholderText = "www.smokeball.com.au";

        public MainWindow()
        {
            InitializeComponent();
            Url.Text = placeholderText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wc = new WebClient();
            var dataUrl = string.Format("https://www.google.com/search?q={0}&num=100", Url.Text);
            var req = (HttpWebRequest)WebRequest.Create(dataUrl);
            List<string> Urls;
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                using (Stream stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions { 
                        
                    });
                    Urls = FindResults(responseString);
                }
                var positions = Urls.Select((url, index) => new { url, index })
                    .Where(u => u.url.EndsWith(Url.Text)).ToList();
                Results.ItemsSource = positions.Select(p => p.index + 1);
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Content = ex.Message;
            }
        }

        private List<string> FindResults(string source)
        {
            Regex regex = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w%&=])?$");
            MatchCollection Collection = regex.Matches(source);
            List<string> Urls = new List<string>();
            foreach (Match match in Collection)
            {
                Urls.Add(match.ToString());
            }
            return Urls.ToList();
        }
    }
}

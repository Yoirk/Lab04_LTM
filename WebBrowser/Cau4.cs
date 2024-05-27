using CefSharp;
using CefSharp.WinForms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class Cau4 : Form
    {
        private ChromiumWebBrowser browser;
        private string saveFolderPath = @"D:\Test";

        public Cau4()
        {
            InitializeComponent();
            InitializeChromium();

            txtUrl.KeyDown += new KeyEventHandler(txtUrl_KeyDown);
            btnDownload.Click += new EventHandler(btnDownload_Click);

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }
        }

        private void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            browser = new ChromiumWebBrowser("https://www.google.com")
            {
                Dock = DockStyle.Fill,
            };
            panelBrowser.Controls.Add(browser);

            browser.AddressChanged += Browser_AddressChanged;
        }
        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                txtUrl.Text = e.Address;
                //Console.WriteLine("Navigated to: " + e.Address);
            }));
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NavigateToUrl();
                e.SuppressKeyPress = true; // Ngăn chặn âm thanh 'ding' khi nhấn Enter
            }
        }
        private void NavigateToUrl()
        {
            var url = txtUrl.Text;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                browser.Load(url);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập Url hợp lệ!");
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                await DownloadWebPageAsync(url);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập Url hợp lệ!");
            }
        }
        private async Task DownloadWebPageAsync(string url)
        {
            string html = await browser.GetSourceAsync();
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            // Save HTML
            string fileName = Path.Combine(saveFolderPath, "index.html");
            File.WriteAllText(fileName, document.DocumentNode.OuterHtml);

            // Download images and other resources
            var imgNodes = document.DocumentNode.SelectNodes("//img[@src]");
            var cssNodes = document.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
            var scriptNodes = document.DocumentNode.SelectNodes("//script[@src]");

            await DownloadResources(imgNodes, url);
            await DownloadResources(cssNodes, url);
            await DownloadResources(scriptNodes, url);

            MessageBox.Show("Đã tải!");
        }

        private async Task DownloadResources(HtmlNodeCollection nodes, string baseUrl)
        {
            if (nodes == null) return;

            using (var client = new WebClient())
            {
                foreach (var node in nodes)
                {
                    string src = node.GetAttributeValue("src", null) ?? node.GetAttributeValue("href", null);
                    if (!string.IsNullOrEmpty(src))
                    {
                        string absoluteUrl = new Uri(new Uri(baseUrl), src).AbsoluteUri;
                        string localPath = Path.Combine(saveFolderPath, Path.GetFileName(absoluteUrl));

                        try
                        {
                            await client.DownloadFileTaskAsync(new Uri(absoluteUrl), localPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error downloading " + absoluteUrl + ": " + ex.Message);
                        }
                    }
                }
            }
        }

        private async void btnViewSource_Click(object sender, EventArgs e)
        {
            string htmlSource = await browser.GetSourceAsync();
            ViewSource sourceViewForm = new ViewSource(htmlSource);
            sourceViewForm.Show();
        }
    }
}


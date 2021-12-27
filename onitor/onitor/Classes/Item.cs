using System;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using Windows.Web.Http;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace Onitor
{
    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _title;
        public string Title
        {
            get
            {
                if (_title.Length >= 10)
                {
                    return string.Format("{0}...", _title.Substring(0, 10));
                }
                else
                {
                    return _title;
                }
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string FullTitle
        {
            get
            {
                return _title;
            }
        }

        public WebViewPivotItem PivotItem { get; set; }

        public HttpClient appHttpClient = new HttpClient();

        public async Task<ContentDialogResult?> ShowCertInfoDialog()
        {
            HttpRequestMessage aReq = new HttpRequestMessage(HttpMethod.Get, PivotItem.WebViewCore.WebView.Source);
            HttpResponseMessage x = await appHttpClient.SendRequestAsync(aReq);

            if (x.IsSuccessStatusCode && aReq.TransportInformation.ServerCertificate != null)
            {
                ContentDialog SiteInfoDialog = new ContentDialog()
                {
                    Title = "Certficate Information",
                    PrimaryButtonText = "OK"
                };

                Certificate cert = aReq.TransportInformation.ServerCertificate;
                StackPanel ContentStackPanel = new StackPanel();
                ContentStackPanel.Children.Add(new TextBlock() { Text = cert.Subject, FontSize = 17 });

                ContentStackPanel.Children.Add(new TextBlock() { Text = "Issued by: " + cert.Issuer });
                ContentStackPanel.Children.Add(new TextBlock() { Text = "Valid from: " + cert.ValidFrom.DateTime });
                ContentStackPanel.Children.Add(new TextBlock() { Text = "Valid to: " + cert.ValidTo.DateTime });
                ContentStackPanel.Children.Add(new TextBlock() { Text = "Algorithm: " + cert.SignatureHashAlgorithmName });

                SiteInfoDialog.Content = ContentStackPanel;

                return await SiteInfoDialog.ShowAsync();
            }

            return null;
        }

        public Image Favicon
        {
            get
            {
                Image image = new Image();

                BitmapImage bitmapImage = new BitmapImage();
                if (PivotItem.WebViewCore.WebView.Source.AbsoluteUri.StartsWith("http://") || PivotItem.WebViewCore.WebView.Source.AbsoluteUri.StartsWith("https://"))
                {
                    bitmapImage.UriSource = new Uri("http://" + PivotItem.WebViewCore.WebView.Source.Host + "/favicon.ico");
                }
                else if (PivotItem.WebViewCore.WebView.Source.AbsoluteUri.StartsWith("ms-appx-web://71330982-ba82-4d35-b5cb-3488eefb31ed/PagesHTML"))
                {
                    bitmapImage.UriSource = new Uri("ms-appx-web://71330982-ba82-4d35-b5cb-3488eefb31ed/PagesHTML/Assets/favicon.ico");
                }
                else
                {
                    bitmapImage.UriSource = null;
                }

                image.Source = bitmapImage;

                return image;
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

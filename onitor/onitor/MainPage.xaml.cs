using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace onitor
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SystemNavigationManager currentView = 
            SystemNavigationManager.GetForCurrentView();

        public WebView currentWebView;

        ApplicationDataContainer localSettings =
            ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();

            currentView.BackRequested += CurrentView_BackRequested;

            var AppView = ApplicationView.GetForCurrentView();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            //PC customization
            var titleBar = AppView.TitleBar;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                if (titleBar != null)
                {
                    ContentGrid.Margin = new Thickness(0, 32.4, 0, 0);
                    FindName(nameof(LeftAppTitleBar));
                    FindName(nameof(MiddleAppTitleBar));
                }
            }

            coreTitleBar.LayoutMetricsChanged += coreTitleBar_LayoutMetricsChanged;

            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 100;

                    ContentGrid.Margin = new Thickness(0, 0, 0, 0);
                }
            }

            titleBar.ButtonBackgroundColor = Resources["SystemAccentColor"] as Color?;

            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            AddTab();

            TitleTextBlock.Text = Package.Current.DisplayName;

            string homePage = localSettings.Values["homePage"].ToString();
            Navigate(homePage);
        }

        private void coreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            MiddleAppTitleBar.Margin = new Thickness(64, 0, 0, 0);
            MiddleAppTitleBar.Height = sender.Height;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            InputPane pane = InputPane.GetForCurrentView();
            var appView = ApplicationView.GetForCurrentView();
            if (appView.IsFullScreenMode)
            {
                appView.ExitFullScreenMode();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        #region "Event Handlers"
        private void AddressAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Navigate(AddressAutoSuggestBox.Text);
            currentWebView.Focus(FocusState.Programmatic);
        }

        private void CloseTabAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            PivotMain.CloseCurrentTab();
        }

        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            currentWebView.GoBack();
        }

        private void ForwardAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            currentWebView.GoForward();
        }

        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            currentWebView.Refresh();
        }

        private void StopAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            currentWebView.Stop();
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void PivotMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotMain.SelectedWebViewItem.Margin = new Thickness(
                0, 0, 0, 0);

            currentWebView = PivotMain.SelectedWebView;

            if(currentWebView.Source == null)
            {
                AddressAutoSuggestBox.Text = "";
            }
            else
            {
                AddressAutoSuggestBox.Text = currentWebView.Source.ToString();
            }

            CheckIfCanGoBack();
            CheckIfCanGoForward();
        }

        private async void currentWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string DeviceVersion = localSettings.Values["DeviceVersion"].ToString();
            if (DeviceVersion == "Desktop")
            {
                onitor.UserAgent.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17133");
            }
            else if (DeviceVersion == "Mobile")
            {
                onitor.UserAgent.SetUserAgent("Mozilla/5.0 (Windows Phone 10.0; Android 8.0; ARM) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Mobile Safari/537.36 EdgA/41.0.0.1726");
            }

            // WebView native object must be inserted in the OnNavigationStarting event handler
            KeyHandler winRTObject = new KeyHandler();
            // Expose the native WinRT object on the page's global object
            currentWebView.AddWebAllowedObject("NotifyApp", winRTObject);

            MainProgressRing.IsActive = true;
            MainProgressRing.Visibility = Visibility.Visible;

            MainFavicon.Visibility = Visibility.Collapsed;

            PivotMain.SelectedWebViewItem.HeaderTextBlock.Text = currentWebView.Source.DnsSafeHost;

            StopAppBarButton.Visibility = Visibility.Visible;
            RefreshAppBarButton.Visibility = Visibility.Collapsed;
        }

        private void currentWebView_ContentLoading(FrameworkElement sender, WebViewContentLoadingEventArgs args)
        {
            MainProgressRing.IsActive = true;
            MainProgressRing.Visibility = Visibility.Visible;

            StopAppBarButton.Visibility = Visibility.Visible;
            RefreshAppBarButton.Visibility = Visibility.Collapsed;

            AddressAutoSuggestBox.Text = currentWebView.Source.ToString();
        }

        private void currentWebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {     
            RefreshAppBarButton.Visibility = Visibility.Visible;
            StopAppBarButton.Visibility = Visibility.Collapsed;

            MainProgressRing.IsActive = false;
            MainProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void currentWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var header =
                PivotMain.SelectedWebViewItem.HeaderTextBlock;

            if (currentWebView.DocumentTitle.Length <= 15)
            {
                header.Text = currentWebView.DocumentTitle;
            }
            else
            {
                header.Text = string.Format("{0}...", currentWebView.DocumentTitle.Substring(0, 15));
            }

            ToolTipService.SetToolTip(header, currentWebView.DocumentTitle);

            RefreshAppBarButton.Visibility = Visibility.Visible;
            StopAppBarButton.Visibility = Visibility.Collapsed;

            CheckIfCanGoBack();
            CheckIfCanGoForward();

            MainProgressRing.IsActive = false;
            MainProgressRing.Visibility = Visibility.Collapsed;

            BitmapImage bitmapImage = new BitmapImage();
            if(!args.Uri.AbsoluteUri.StartsWith("about:"))
            { 
                bitmapImage.UriSource = new Uri("http://" + args.Uri.Host + "/favicon.ico");
            }

            MainFavicon.Visibility = Visibility.Visible;
            MainFavicon.Source = bitmapImage;

            await sender.InvokeScriptAsync("eval", new string[] { "window.alert = function(AlertMessage) { window.external.notify('typeAlert:' + AlertMessage) }" });
            await sender.InvokeScriptAsync("eval", new string[] { "window.confirm = function(confirmMessage) { window.external.notify('typeConfirm:' + confirmMessage); }" });
            await sender.InvokeScriptAsync("eval", new string[] { "window.prompt = function(promptMessage) { window.external.notify('typePrompt:' + promptMessage); }" });
        }

        private void currentWebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            Navigate(args.Uri.ToString());

            args.Handled = true;
        }

        private void currentWebView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement && !applicationView.IsFullScreenMode)
            {
                AddressAutoSuggestBox.Visibility = Visibility.Collapsed;
                MainCommandBar.Visibility = Visibility.Collapsed;
                applicationView.TryEnterFullScreenMode();
                ContentGrid.Margin = new Thickness(0, -80, 0, 0);
            }
            else
            {
                AddressAutoSuggestBox.Visibility = Visibility.Visible;
                MainCommandBar.Visibility = Visibility.Visible;
                applicationView.ExitFullScreenMode();
                ContentGrid.Margin = new Thickness(0, 32, 0, 0);
            }
        }

        private async void currentWebView_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            if (args.PermissionRequest.PermissionType == WebViewPermissionType.WebNotifications)
            {
                string WebNotifyPermission = localSettings.Values["WebNotificationPermission"].ToString();
                if(WebNotifyPermission == "1")
                {
                    PermissionTextBlock.Text =
                        string.Format("\"{0}\" wants to send notification. Do you want to allow?",
                        args.PermissionRequest.Uri.Host);

                    var result = await PermissionContentDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        args.PermissionRequest.Allow();
                    }
                    else
                    {
                        args.PermissionRequest.Deny();
                    }
                }
                else if(WebNotifyPermission == "2")
                {
                    args.PermissionRequest.Allow();
                }
                else if(WebNotifyPermission == "3")
                {
                    args.PermissionRequest.Deny();
                }
            }

            if (args.PermissionRequest.PermissionType == WebViewPermissionType.Geolocation)
            {
                string LocationPermission = localSettings.Values["LocationPermission"].ToString();
                if (LocationPermission == "1")
                {
                    PermissionTextBlock.Text =
                        string.Format("\"{0}\" wants to access your location. Do you want to allow?",
                        args.PermissionRequest.Uri.Host);

                    var result = await PermissionContentDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        args.PermissionRequest.Allow();
                    }
                    else
                    {
                        args.PermissionRequest.Deny();
                    }
                }
                else if (LocationPermission == "2")
                {
                    args.PermissionRequest.Allow();
                }
                else if (LocationPermission == "3")
                {
                    args.PermissionRequest.Deny();
                }
            }

            if (args.PermissionRequest.PermissionType == WebViewPermissionType.Media)
            {
                string MediaPermission = localSettings.Values["MediaPermission"].ToString();
                if (MediaPermission == "1")
                {
                    PermissionTextBlock.Text =
                        string.Format("\"{0}\" wants to access your camera or microphone. Do you want to allow?",
                        args.PermissionRequest.Uri.Host);

                    var result = await PermissionContentDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        args.PermissionRequest.Allow();
                    }
                    else
                    {
                        args.PermissionRequest.Deny();
                    }
                }
                else if (MediaPermission == "2")
                {
                    args.PermissionRequest.Allow();
                }
                else if (MediaPermission == "3")
                {
                    args.PermissionRequest.Deny();
                }
            }
        }

        private async void currentWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string[] messageArray = e.Value.Split(':');
            string message;
            string type;

            if (messageArray.Length > 1)
            {
                message = messageArray[1];
                type = messageArray[0];
            }
            else
            {
                message = e.Value;
                type = "typeAlert";
            }

            if (type.Equals("typeAlert"))
            {
                AlertTextBlock.Text = message;
                var result = await AlertContentDialog.ShowAsync();
            }
            else if (type.Equals("typeConfirm"))
            {
                ConfirmTextBlock.Text = message;
                var result = await ConfirmContentDialog.ShowAsync();
            }
            else if (type.Equals("typePrompt"))
            {
                PromptTextBlock.Text = message;
                var result = await PromptContentDialog.ShowAsync();
            }
        }
        #endregion

        #region "Methods"

        private void AddTab()
        {
            var newTab = PivotMain.AddTab();
            newTab.Tag = PivotMain.SelectedWebViewItem;

            currentWebView = PivotMain.SelectedWebView;
            ToolTipService.SetToolTip(newTab.HeaderTextBlock, newTab.HeaderTextBlock.Text);

            currentWebView.NavigationStarting += currentWebView_NavigationStarting;
            currentWebView.NavigationCompleted += currentWebView_NavigationCompleted;
            currentWebView.ContentLoading += currentWebView_ContentLoading;
            currentWebView.DOMContentLoaded += currentWebView_DOMContentLoaded;
            currentWebView.NewWindowRequested += 
                new TypedEventHandler<WebView, WebViewNewWindowRequestedEventArgs>(currentWebView_NewWindowRequested);
            currentWebView.ContainsFullScreenElementChanged += currentWebView_ContainsFullScreenElementChanged;
            currentWebView.PermissionRequested += currentWebView_PermissionRequested;
            currentWebView.ScriptNotify += currentWebView_ScriptNotify;
        }

        private void Navigate(string address)
        {
            if (address.StartsWith("ms-appx-web:///"))
            {
                currentWebView.Navigate(new Uri(address));
            }
            else if (address.Contains(".com") || address.Contains(".us")
                || address.Contains(".org") || address.Contains(".net") 
                || address.Contains(".am") || address.Contains(".ru") || address.Contains(".io"))
            {
                if (address.StartsWith("http://") || address.StartsWith("https://")
                    || address.StartsWith("ftp://"))
                {
                    currentWebView.Navigate(new Uri(address));
                }
                else
                {
                    currentWebView.Navigate(new Uri("http://" + address));
                }
            }
            else
            {
                if (address.StartsWith("about:"))
                {
                    currentWebView.Navigate(new Uri(address));
                }
                else
                {
                    string SearchEngine = localSettings.Values["SearchEngine"].ToString();
                    if (SearchEngine == "Bing")
                    {
                        currentWebView.Navigate(new Uri("https://www.bing.com/search?q=" + address));
                    }
                    else if (SearchEngine == "Google")
                    {
                        currentWebView.Navigate(new Uri("https://www.google.am/search?q=" + address));
                    }
                    else if (SearchEngine == "Yahoo")
                    {
                        currentWebView.Navigate(new Uri("https://search.yahoo.com/search?p=" + address));
                    }
                }
            }
        } 

        private void CheckIfCanGoBack()
        {
            bool canGoBack = currentWebView.CanGoBack;

            BackAppBarButton.IsEnabled = canGoBack;
        }

        private void CheckIfCanGoForward()
        {
            bool canGoForward = currentWebView.CanGoForward;

            ForwardAppBarButton.IsEnabled = canGoForward;
        }
        #endregion
    }
}
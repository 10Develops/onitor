using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;

namespace onitor
{
    public sealed partial class SettingsPage : Page
    {
        SystemNavigationManager currentView = 
            SystemNavigationManager.GetForCurrentView();

        ApplicationDataContainer localSettings =
            ApplicationData.Current.LocalSettings;

        public SettingsPage()
        {
            InitializeComponent();

            currentView.BackRequested += CurrentView_BackRequested;

            var AppView = ApplicationView.GetForCurrentView();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.LayoutMetricsChanged += coreTitleBar_LayoutMetricsChanged;

            //PC customization
            var titleBar = AppView.TitleBar;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                if (titleBar != null)
                {
                    ContentGrid.Margin = new Thickness(0, 32.4, 0, 0);
                    Window.Current.SetTitleBar(MiddleAppTitleBar);
                    MiddleAppTitleBar.Visibility = Visibility.Visible;
                    LeftAppTitleBar.Visibility = Visibility.Visible;
                }
            }

            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            On_BackRequested();
            e.Handled = true;
        }

        private void coreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            MiddleAppTitleBar.Margin = new Thickness(64, 0, 0, 0);
            MiddleAppTitleBar.Height = sender.Height;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Window.Current.SetTitleBar(MiddleAppTitleBar);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            Window.Current.SetTitleBar(null);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private void AboutBlankSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HomePageSettingsTextBox.Text = "about:blank";
        }

        private void HomePageSettingsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["homePage"] = HomePageSettingsTextBox.Text;
        }

        private void SettingsGridMain_Loaded(object sender, RoutedEventArgs e)
        {
            string homePage = localSettings.Values["homePage"].ToString();
            HomePageSettingsTextBox.Text = homePage;

            string SearchEngine = localSettings.Values["SearchEngine"].ToString();
            if (SearchEngine == "Bing")
            {
                BingRadioButton.IsChecked = true;
            }
            else if (SearchEngine == "Google")
            {
                GoogleRadioButton.IsChecked = true;
            }
            else if (SearchEngine == "Yahoo")
            {
                YahooRadioButton.IsChecked = true;
            }

            string DeviceVersion = localSettings.Values["DeviceVersion"].ToString();
            if(DeviceVersion == "Desktop")
            {
                DeviceVersionComboBox.SelectedItem = DesktopComboBoxItem;
            }
            else if(DeviceVersion == "Mobile")
            {
                DeviceVersionComboBox.SelectedItem = MobileComboBoxItem;
            }

            string WebNotifyPermission = localSettings.Values["WebNotificationPermission"].ToString();
            if(WebNotifyPermission == "1")
            {
                AlwaysAskWebNotificationsRadioButton.IsChecked = true;
            }
            else if(WebNotifyPermission == "2")
            {
                AllowWebNotificationsRadioButton.IsChecked = true;
            }
            else if(WebNotifyPermission == "3")
            {
                BlockWebNotificationsRadioButton.IsChecked = true;
            }

            string LocationPermission = localSettings.Values["LocationPermission"].ToString();
            if (LocationPermission == "1")
            {
                AlwaysAskLocationRadioButton.IsChecked = true;
            }
            else if (LocationPermission == "2")
            {
                AllowLocationRadioButton.IsChecked = true;
            }
            else if (LocationPermission == "3")
            {
                BlockLocationRadioButton.IsChecked = true;
            }

            string MediaPermission = localSettings.Values["MediaPermission"].ToString();
            if (MediaPermission == "1")
            {
                AlwaysAskMediaRadioButton.IsChecked = true;
            }
            else if (MediaPermission == "2")
            {
                AllowMediaRadioButton.IsChecked = true;
            }
            else if (MediaPermission == "3")
            {
                BlockMediaRadioButton.IsChecked = true;
            }
        }

        private void BingRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SearchEngine"] = "Bing";
        }

        private void GoogleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SearchEngine"] = "Google";
        }

        private void YahooRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SearchEngine"] = "Yahoo";
        }

        private void DeviceVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DesktopComboBoxItem.IsSelected == true)
            {
                localSettings.Values["DeviceVersion"] = "Desktop";
            }
            else if(MobileComboBoxItem.IsSelected == true)
            {
                localSettings.Values["DeviceVersion"] = "Mobile";
            }
        }

        private void AlwaysAskWebNotificationsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["WebNotificationPermission"] = "1";
        }

        private void AllowWebNotificationsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["WebNotificationPermission"] = "2";
        }

        private void BlockWebNotificationsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["WebNotificationPermission"] = "3";
        }

        private void AlwaysAskLocationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["LocationPermission"] = "1";
        }

        private void AllowLocationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["LocationPermission"] = "2";
        }

        private void BlockLocationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["LocationPermission"] = "3";
        }

        private void AlwaysAskMediaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["MediaPermission"] = "1";
        }

        private void AllowMediaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["MediaPermission"] = "2";
        }

        private void BlockMediaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["MediaPermission"] = "3";
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Onitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareTargetPage : Page
    {
        ApplicationView appView = ApplicationView.GetForCurrentView();

        public ShareTargetPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IActivatedEventArgs args = e.Parameter as IActivatedEventArgs;
            if(args.Kind == ActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareArgs = args as ShareTargetActivatedEventArgs;

                Uri testAppUri = new Uri("onitor:"); // The protocol handled by the launched app
                LauncherOptions options = new LauncherOptions { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName };

                ValueSet inputData = new ValueSet();
                DataPackageView data = shareArgs.ShareOperation.Data;
                if (data.Contains(StandardDataFormats.Html))
                {
                    Uri text = await data.GetWebLinkAsync();
                    inputData["html"] = text.ToString();
                }
                else if (data.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> file = await data.GetStorageItemsAsync();
                    inputData["filePath"] = file[0].Path;
                }
                else if (data.Contains(StandardDataFormats.WebLink))
                {
                    Uri text = await data.GetWebLinkAsync();
                    inputData["link"] = text.ToString();
                }

                await Launcher.LaunchUriAsync(testAppUri, options, inputData);
                shareArgs.ShareOperation.ReportCompleted();
            }
        }
    }
}

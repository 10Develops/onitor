using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnitedCodebase.Classes;
using Windows.Foundation.Metadata;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Onitor
{
    public class DownloadManager
    {
        Uri _uri;
        private readonly string FileName;
        private readonly string FileExtension;

        DownloadOperation downloadOperation;
        CancellationTokenSource cancellationToken;
        BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        ContentDialog downloadDialog;

        DownloadAction DlAction;

        StorageFolder localDownloads;

        public DownloadManager(Uri uri)
        {
            _uri = uri;
            FileName = Path.GetFileName(uri.AbsoluteUri);
            FileExtension = Path.GetExtension(_uri.AbsoluteUri);
        }

        public async void ShowContentDialog()
        {
            downloadDialog = new ContentDialog()
            {
                Title = string.Format("What to do with \"{0}\"?", FileName),
            };

            StackPanel ContentStackPanel = new StackPanel();
            ContentStackPanel.Children.Add(new TextBlock() { Text = "File name: " + FileName });
            ContentStackPanel.Children.Add(new TextBlock() { Text = "File format: " + FileExtension });
            ContentStackPanel.Children.Add(new TextBlock() { Text = "URL: " + _uri.DnsSafeHost });

            StackPanel ActionStackPanel = new StackPanel() { Orientation = Orientation.Horizontal};
            HyperlinkButton SaveAsHyperlinkButton = new HyperlinkButton() { Content = "Save As" };
            SaveAsHyperlinkButton.Click += SaveAsHyperlinkButton_Click;
            ActionStackPanel.Children.Add(SaveAsHyperlinkButton);

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                if (FileExtension != ".exe")
                {
                    downloadDialog.PrimaryButtonText = "Open";
                    downloadDialog.SecondaryButtonText = "Save";
                }
                else
                {
                    downloadDialog.PrimaryButtonText = "Save";
                }
            }
            else
            {
                if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ContentDialog", "CloseButtonText") && FileExtension != ".exe")
                {
                    downloadDialog.PrimaryButtonText = "Open";
                    downloadDialog.SecondaryButtonText = "Save";
                    downloadDialog.CloseButtonText = "Cancel";
                }
                else
                {
                    if(FileExtension != ".exe")
                    {
                        HyperlinkButton OpenHyperlinkButton = new HyperlinkButton() { Content = "Open file" };
                        OpenHyperlinkButton.Click += OpenHyperlinkButton_Click;
                        ActionStackPanel.Children.Insert(0, OpenHyperlinkButton);
                    }

                    downloadDialog.PrimaryButtonText = "Save";
                    downloadDialog.SecondaryButtonText = "Cancel";
                }
            }

            ContentStackPanel.Children.Add(ActionStackPanel);
            downloadDialog.Content = ContentStackPanel;

            ContentDialogResult result = await downloadDialog.ShowAsync();

            if(result == ContentDialogResult.Primary && downloadDialog.PrimaryButtonText == "Open")
            {
                localDownloads = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync("Downloads") as StorageFolder;

                if (localDownloads == null)
                {
                    localDownloads = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Downloads");
                }

                StorageFile file = await localDownloads.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                DlAction = DownloadAction.Open;
                Download(file);
            }
            else if((result == ContentDialogResult.Primary && downloadDialog.PrimaryButtonText == "Save")
                || (result == ContentDialogResult.Secondary && downloadDialog.SecondaryButtonText == "Save"))
            {
                StorageFile file = await DownloadsFolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                DlAction = DownloadAction.Save;
                Download(file);
            }
        }

        private async void OpenHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFile file = await localDownloads.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
            DlAction = DownloadAction.Open;
            Download(file);

            downloadDialog.Hide();
        }

        private async void SaveAsHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("All files", new string[] { "." });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = FileName;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                DlAction = DownloadAction.Save;
                Download(file);
            }

            downloadDialog.Hide();
        }

        public async void Download(StorageFile file)
        {
            backgroundDownloader.SuccessToastNotification =
                (new UCNotification(string.Format("Downloaded \"{0}\"", file.Name), "Download completed!")).Toast;
            backgroundDownloader.FailureToastNotification =
                (new UCNotification(string.Format("Download failed \"{0}\"", file.Name), "Download failed!")).Toast;
            downloadOperation = backgroundDownloader.CreateDownload(_uri, file);
            downloadOperation.Priority = BackgroundTransferPriority.High;
            Progress<DownloadOperation> progress = new Progress<DownloadOperation>();
            progress.ProgressChanged += Progress_ProgressChanged;
            cancellationToken = new CancellationTokenSource();
            try
            {
                new UCNotification(string.Format("Downloading \"{0}\"...", file.Name), "Download started!", 
                    DateTime.Now.AddSeconds(4), new ToastAudio() { Silent = true }, ToastDuration.Short).ShowNotification();
                await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
            }
            catch (TaskCanceledException)
            {
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
            }
        }

        private async void Progress_ProgressChanged(object sender, DownloadOperation e)
        {
            double percent = 100;
            if (e.Progress.TotalBytesToReceive > 0)
            {
                percent = e.Progress.BytesReceived * 100 / e.Progress.TotalBytesToReceive;
            }
            
            if(percent == 100 && DlAction == DownloadAction.Open)
            {
                var file = e.ResultFile;

                if (file != null)
                {

                    var success = await Launcher.LaunchFileAsync(file);

                    if (success)
                    {
                        // File launched
                    }
                    else
                    {
                        // File launch failed
                    }
                }
                else
                {
                    // Could not find file
                }
            }
        }
    }

    internal enum DownloadAction
    {
        Save = 0,
        Open = 1
    }
}

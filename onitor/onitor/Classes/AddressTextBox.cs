using System;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;

namespace Onitor
{
    public class AddressTextBox : ContentControl
    {
        private readonly AutoSuggestBox _suggestBox;
        private Button _queryButton;
        public TextBox TextBox;
        private Popup popup;
        public event RoutedEventHandler TextBoxLoaded;

        public bool IsThisTarget = false;
        public bool IsFocused = false;
        public bool IsTapped = false;

        public AddressTextBox() : this(new AutoSuggestBox())
        {

        }

        public AddressTextBox(AutoSuggestBox suggestBox)
        {
            _suggestBox = suggestBox;

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            QueryIcon = new FontIcon { Glyph = "\uE721" };

            Content = suggestBox;

            Style s = new Style { TargetType = typeof(TextBox), BasedOn = Resources["AutoSuggestBoxTextBoxStyle"] as Style };
            s.Setters.Add(new Setter(TextBox.IsTextPredictionEnabledProperty, true));
            s.Setters.Add(new Setter(TextBox.InputScopeProperty, "Url"));
            suggestBox.TextBoxStyle = s;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
            {
                suggestBox.GettingFocus += SuggestBox_GettingFocus;
            }

            suggestBox.GotFocus += SuggestBox_GotFocus;
            suggestBox.Loaded += SuggestBox_Loaded;
        }
        

        public AutoSuggestBox SuggestBox
        {
            get
            {
                return _suggestBox;
            }
        }

        public Button QueryButton
        {
            get
            {
                return _queryButton;
            }
        }

        public string PlaceholderText
        {
            get
            {
                return SuggestBox.PlaceholderText;
            }
            set
            {
                SuggestBox.PlaceholderText = value;
            }
        }

        public string Text
        {
            get
            {
                return SuggestBox.Text;
            }
            set
            {
                SuggestBox.Text = value;
            }
        }

        public IconElement QueryIcon
        {
            get
            {
                return SuggestBox.QueryIcon;
            }
            set
            {
                SuggestBox.QueryIcon = value;
            }
        }


        public string SelectedText
        {
            get
            {
                return TextBox.SelectedText;
            }
            set
            {
                TextBox.SelectedText = value;
            }
        }

        public int SelectionStart
        {
            get
            {
                return TextBox.SelectionStart;
            }
            set
            {
                TextBox.SelectionStart = value;
            }
        }

        public int SelectionLength
        {
            get
            {
                return TextBox.SelectionLength;
            }
            set
            {
                TextBox.SelectionLength = value;
            }
        }

        public void TextSelect(int start, int length)
        {
            TextBox.Select(start, length);
        }

        public void TextSelectAll()
        {
            TextBox.Focus(FocusState.Programmatic);
            TextSelect(0, TextBox.Text.Length);
        }

        public ItemCollection Items
        {
            get
            {
                return SuggestBox.Items;
            }
        }

        public object ItemsSource
        {
            get
            {
                return SuggestBox.ItemsSource;
            }
            set
            {
                SuggestBox.ItemsSource = value;
            }
        }

        public bool IsSuggestionListOpen
        {
            get
            {
                return SuggestBox.IsSuggestionListOpen;
            }
            set
            {
                SuggestBox.IsSuggestionListOpen = value;
            }
        }

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged     
        {
            add
            {
                SuggestBox.TextChanged += value;
            }
            remove
            {
                SuggestBox.TextChanged -= value;
            }
        }

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
        {
            add
            {
                SuggestBox.QuerySubmitted += value;
            }
            remove
            {
                SuggestBox.QuerySubmitted -= value;
            }
        }

        public event ContextMenuOpeningEventHandler ContextMenuOpening
        {
            add
            {
                TextBox.ContextMenuOpening += value;
            }
            remove
            {
                TextBox.ContextMenuOpening -= value;
            }
        }

        private void SuggestBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsTapped)
            {
                SuggestBox.IsSuggestionListOpen = true;
            }
            else
            {
                SuggestBox.IsSuggestionListOpen = false;
            }
        }

        private void SuggestBox_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            if (args.InputDevice == FocusInputDeviceKind.Mouse || args.InputDevice == FocusInputDeviceKind.Touch || args.InputDevice == FocusInputDeviceKind.Pen)
            {
                IsTapped = true;
            }
            else
            {
                IsTapped = false;
            }
        }

        private void SuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = VisualTreeHelper.GetChild(SuggestBox, 0) as Grid;
            TextBox = grid.Children[0] as TextBox;
            TextBoxLoaded.Invoke(TextBox, null);
            popup = grid.Children[1] as Popup;

            Grid performers = VisualTreeHelper.GetChild(TextBox, 0) as Grid;

            for (int i = 0; i < performers.Children.Count; i++)
            {
                if (((FrameworkElement)performers.Children[i]).Name == "QueryButton")
                {
                    _queryButton = performers.Children[i] as Button;
                    break;
                }
            }

            ToolTipService.SetToolTip(QueryButton, ResourceLoader.GetForCurrentView().GetString("GoQueryButtonText"));
        }

        /*private void SuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Back)
            {
                Text = "";
                e.Handled = false;
            }
        }*/

        public void Cut()
        {
            Copy();
            SelectedText = string.Empty;
        }

        public void Copy()
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(SelectedText);
            Clipboard.SetContent(dataPackage);
        }

        public async void Paste()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();
                SelectedText = text;
                SelectionStart = SelectionStart + text.Length;
                SelectionLength = 0;
                SuggestBox.ItemsSource = null;
            }
        }
    }
}

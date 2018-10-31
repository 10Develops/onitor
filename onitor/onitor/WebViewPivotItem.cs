using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace onitor
{
    public class WebViewPivotItem : PivotItem
    {
        private WebView _webView;
        private TextBlock _headerTextBlock;

        public WebViewPivotItem() : this(new TextBlock(), new WebView())
        {
        }

        public WebViewPivotItem(TextBlock headerTextBlock, WebView webView)
        {
            headerTextBlock.FontSize = 18;
            Header = headerTextBlock;
            _headerTextBlock = headerTextBlock;

            Margin = new Thickness(0, 0, 0, 0);
            Content = webView;
            _webView = webView;

            webView.Margin = new Thickness(0, 0, 0, 0);
        }

        public TextBlock HeaderTextBlock
        {
            get
            {
                return _headerTextBlock;
            }
        }

        public WebView WebView
        {
            get
            {
                return _webView;
            }
        }
    }
}

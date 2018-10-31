using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace onitor
{
    public class WebViewPivot : Pivot
    {
        public WebViewPivotItem SelectedWebViewItem
        {
            get
            {
                return (WebViewPivotItem)SelectedItem;
            }
        }

        public WebView SelectedWebView
        {
            get
            {
                return SelectedWebViewItem.WebView;
            }
        }

        public WebViewPivotItem AddTab()
        {
            WebViewPivotItem item = new WebViewPivotItem();
            Items.Add(item);
            SelectedItem = item;
            return item;
        }

        public bool CloseCurrentTab()
        {
            bool canRemove = Items.Count > 1;

            if (canRemove)
            {
                WebViewPivotItem item = SelectedWebViewItem;
                Items.Remove(item);
            }

            return canRemove;
        }
    }
}

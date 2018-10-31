using System.Diagnostics;
using Windows.Foundation.Metadata;

namespace onitor
{
    //KeyHandler.cs
    [AllowForWeb]
    public sealed class KeyHandler
    {
        public void setKeyCombination(int keyPress)
        {
            Debug.WriteLine("Called from WebView! {0}", keyPress);
        }
    }
}

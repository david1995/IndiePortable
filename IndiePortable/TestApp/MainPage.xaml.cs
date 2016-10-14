using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using IndiePortable.Communication.EncryptedConnection;
using IndiePortable.Communication.UniversalWindows;
using IndiePortable.Formatter;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            RsaCryptoManager mgr;
            if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "KeyPair-Uwp.dat")))
            {
                mgr = new RsaCryptoManager(File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "KeyPair-Uwp.dat")));
            }
            else
            {
                mgr = new RsaCryptoManager();
                using (var str = File.Open(Path.Combine(ApplicationData.Current.LocalFolder.Path, "KeyPair-Uwp.dat"), FileMode.Create, FileAccess.Write))
                {
                    mgr.ExportLocalKeyPair(str);
                }
            }

            File.WriteAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-uwp.dat"), mgr.LocalPublicKey.KeyBlob);

            var remotePublicKey = new PublicKeyInfo(File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-netclassic.dat")));
            mgr.StartSession(remotePublicKey);

            var result = mgr.Decrypt(File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "NetFramework-Encrypted.dat")));
            this.tb.Text = Encoding.UTF8.GetString(result);
        }
    }
}

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
            RsaCryptoManager crypto;
            ////if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-uwp.dat")))
            ////{
            ////    crypto = new RsaCryptoManager(File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-uwp.dat")));
            ////}
            ////else
            ////{
            crypto = new RsaCryptoManager();
            File.WriteAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-uwp.dat"), crypto.LocalPublicKey.KeyBlob);
            ////}

            var remotePublicKey = new PublicKeyInfo(File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "key-netclassic.dat")));
            crypto.StartSession(remotePublicKey);

            using (var memstr = new MemoryStream())
            {
                var f = BinaryFormatter.CreateWithCoreSurrogates();
                f.Serialize(memstr, new PublicKeyInfo(new byte[] { 0xff, 0xff, 0xff, 0x7f }));
                var encryptedData = crypto.Encrypt(memstr.ToArray());

                File.WriteAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "encrypted.dat"), encryptedData);
            }
        }
    }
}

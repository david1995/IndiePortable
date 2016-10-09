using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using IndiePortable.Communication.UniversalWindows;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            var crypto = new RsaCryptoManager();

            File.WriteAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "pk-asn.dat"), crypto.LocalKey.ExportPublicKey(CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey).ToArray());

            var remoteModulus = File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "modulus.dat"));
            var remoteExponent = File.ReadAllBytes(Path.Combine(ApplicationData.Current.LocalFolder.Path, "exponent.dat"));

            crypto.StartSession(new IndiePortable.Communication.EncryptedConnection.PublicKeyInfo(remoteExponent, remoteModulus));

            var key = crypto.RemoteKey.ExportPublicKey(Windows.Security.Cryptography.Core.CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);

            this.tb.Text = $@"
Modulus: {string.Join("", crypto.LocalPublicKey.Modulus.Select(b => b.ToString("x2")))}
Length: {crypto.LocalPublicKey.Modulus.Length * 8} bits

Exponent: {string.Join("", crypto.LocalPublicKey.Exponent.Select(b => b.ToString("x2")))}
Length: {crypto.LocalPublicKey.Exponent.Length * 8} bits";
        }
    }
}

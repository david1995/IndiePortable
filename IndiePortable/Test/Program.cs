using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndiePortable.Communication.EncryptedConnection;
using IndiePortable.Communication.NetClassic;
using IndiePortable.Formatter;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RsaCryptoManager mgr;
            if (File.Exists("KeyPair-NetClassic.dat"))
            {
                mgr = new RsaCryptoManager(File.ReadAllBytes("KeyPair-NetClassic.dat"));
            }
            else
            {
                mgr = new RsaCryptoManager();
                using (var str = File.Open("KeyPair-NetClassic.dat", FileMode.Create, FileAccess.Write))
                {
                    mgr.ExportLocalKeyPair(str);
                }

                File.WriteAllBytes("Key-NetClassic.dat", mgr.LocalPublicKey.KeyBlob);
            }

            Console.WriteLine($"Key Blob: {string.Join(string.Empty, mgr.LocalPublicKey.KeyBlob.Select(b => b.ToString("x2")))}");
            Console.WriteLine($"\nLength: {mgr.LocalPublicKey.KeyBlob.Length} byte(s) ({mgr.LocalPublicKey.KeyBlob.Length * 8} bits)\n\n");
            
            Console.ReadLine();

            var remoteRSA = File.ReadAllBytes("Key-UWP.dat");
            mgr.StartSession(new PublicKeyInfo(remoteRSA));

            var encData = File.ReadAllBytes("encrypted.dat");
            var decData = mgr.Decrypt(encData);

            ////using (var str = new MemoryStream(decData, false))
            ////{
            ////    var f = BinaryFormatter.CreateWithCoreSurrogates();
            ////    var obj = f.Deserialize<PublicKeyInfo>(str);
            ////    Console.WriteLine(string.Join(" ", obj.KeyBlob.Select(b => b.ToString("x2"))));
            ////}

            var result = Encoding.UTF8.GetString(decData);
            Console.WriteLine(result);
            
            Console.ReadLine();

            mgr.Dispose();
        }
    }
}

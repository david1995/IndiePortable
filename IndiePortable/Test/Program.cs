using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            }

            File.WriteAllBytes("Key-NetClassic.dat", mgr.LocalPublicKey.KeyBlob);

            ////var remoteRSA = File.ReadAllBytes("Key-UWP.dat");
            ////mgr.StartSession(new PublicKeyInfo(remoteRSA));
            mgr.StartSession(new PublicKeyInfo(File.ReadAllBytes("Key-Uwp.dat")));

            var sw = Stopwatch.StartNew();
            ////var encData = File.ReadAllBytes("encrypted.dat");
            var encData = mgr.Encrypt(Encoding.UTF8.GetBytes(
                @"This is an en/decryption test for .Net Framework.
Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.
Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.
Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.
At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur"));
            sw.Stop();
            
            File.WriteAllBytes("NetFramework-Encrypted.dat", encData);

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

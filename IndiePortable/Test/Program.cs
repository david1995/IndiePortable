using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndiePortable.Communication.NetClassic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RsaCryptoManager mgr;
            if (File.Exists("Key-NetClassic.dat"))
            {
                mgr = new RsaCryptoManager(File.ReadAllBytes("Key-NetClassic.dat"));
            }
            else
            {
                mgr = new RsaCryptoManager();
                File.WriteAllBytes("Key-NetClassic.dat", mgr.LocalPublicKey.KeyBlob);
            }

            Console.WriteLine($"Key Blob: {string.Join(string.Empty, mgr.LocalPublicKey.KeyBlob)}");
            Console.WriteLine($"\nLength: {mgr.LocalPublicKey.KeyBlob.Length} byte(s) ({mgr.LocalPublicKey.KeyBlob.Length * 8} bits)\n\n");
            
            Console.ReadLine();

            mgr.Dispose();
        }
    }
}

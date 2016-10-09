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
            var mgr = new RsaCryptoManager();
            Console.WriteLine($"Modulus: {string.Join(string.Empty, mgr.LocalPublicKey.Modulus)}");
            Console.WriteLine($"\nLength: {mgr.LocalPublicKey.Modulus.Length} byte(s) ({mgr.LocalPublicKey.Modulus.Length * 8} bits)\n\n");

            File.WriteAllBytes("modulus.dat", mgr.LocalPublicKey.Modulus);

            Console.WriteLine($"Exponent: {string.Join(string.Empty, mgr.LocalPublicKey.Exponent)}");
            Console.WriteLine($"\nLength: {mgr.LocalPublicKey.Exponent.Length} byte(s) ({mgr.LocalPublicKey.Exponent.Length * 8} bits)\n\n");

            File.WriteAllBytes("exponent.dat", mgr.LocalPublicKey.Exponent);

            Console.ReadLine();
            mgr.Dispose();
        }
    }
}

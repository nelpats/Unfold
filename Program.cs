using dnlib.DotNet;
using dnlib.PE;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unfold
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Origami unpacker github.com/NSDCode";
            Console.CursorVisible = false;

            Console.WriteLine("Loading Assembly...");
            ModuleDefMD module = ModuleDefMD.Load(args[0]);

            var peImage = module.Metadata.PEImage;
            Console.WriteLine("Looking for .origami PE Section...");

            for (var i = 0; i < peImage.ImageSectionHeaders.Count; i++)
            {
                var section = peImage.ImageSectionHeaders[i];

                if (section.DisplayName == ".origami")
                {
                    Console.WriteLine("Section '.origami' found !");
                    var reader = peImage.CreateReader(section.VirtualAddress, section.SizeOfRawData);

                    Console.WriteLine("Extracting section...");
                    var data = Decompress(reader.ToArray());

                    Console.WriteLine("File has been extracted !");
                    File.WriteAllBytes($@"{Environment.CurrentDirectory}\Unfolded.exe", data);
                    
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            Console.WriteLine("PE Secion '.origami' not found !");
            Console.ReadLine();
        }

        private static byte[] Decompress(byte[] data)
        {
            var destination = new MemoryStream();
            using (var deflateStream = new DeflateStream(new MemoryStream(data), CompressionMode.Decompress))
            {
                deflateStream.CopyTo(destination);
            }

            return destination.ToArray();
        }
    }
}

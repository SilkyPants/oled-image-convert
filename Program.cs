using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;

namespace Image2CArrayConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            int index = 0;
            foreach (var arg in args)
            {
				Console.WriteLine("Arg {0}: {1}", index++, arg);
            }

            var inputFile = args.Last();

            if (!File.Exists(inputFile)) {
                Console.WriteLine("Cannot load file {0}", inputFile);

                return;
            }

            using (FileStream stream = File.OpenRead("Laughing_Man.png"))
            using (Image<Rgba32> image = Image.Load<Rgba32>(stream))
            {
                Console.WriteLine("Width = {0}", image.Width);
                Console.WriteLine("Height = {0}", image.Height);

                //for (int x = 0; x < image.Width; x++)
                //{
                //    for (int y = 0; y < image.Height; y++)
                //    {
                //        var pixel = image[x, y];

                //        Console.WriteLine("Pixel {0}:{1} - {2}", x, y, pixel.ToString());
                //    }
                //}
            }
        }
    }
}

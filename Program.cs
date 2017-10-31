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

            ConvertImageToArray("Laughing_Man.png", "Laughing_Man.h");

            ConvertArrayToImage("Laughing_Man.h", "Laughing_Man-Test.png");
        }

        static void ConvertImageToArray(string imagePath, string outputPath) 
        {
            using (FileStream stream = File.OpenRead(imagePath))
            using (Image<Rgba32> image = Image.Load<Rgba32>(stream))
            {
                Console.WriteLine("Width = {0}", image.Width);
                Console.WriteLine("Height = {0}", image.Height);

                // Could probably do this better
                // but for now we are simply just storing if there is a renderable pixel or not
                var bitArray = new bool[image.Width, image.Height]; 
                var cutOff = 0f;

                // Ensure the image is grayscale

                // Loop through pixels
                for (int x = 0; x < image.Width; x++)
                {
                   for (int y = 0; y < image.Height; y++)
                   {
                       var pixel = image[x, y];
                       
                       var value = pixel.Red;

                        bitArray[x, y] = value >= cutOff;
                   }
                }

                // Final array output
                var finalArray = new uint[image.Width * (image.height / 8)];
                var blockIndex = 0;

                // if Vertical
                if (true) {
                        
                    for (int x = 0; x < image.Width; x++) {
                        for (int y = 0; y < image.Height; y +=8) {

                            uint block = 0;

                            for (int segment = 0; segment < 8; segment ++) {

                                if (bitArray[x, y + segment]) {
                                    block = (uint)1 << segment;
                                }
                            }

                            finalArray[blockIndex++] = block;
                        }
                    }
                }
                else { // Horizontal
                    
                    for (int y = 0; y < image.Height; y +=8) {
                        for (int x = 0; x < image.Width; x++) {
                            uint block = 0;

                            for (int segment = 0; segment < 8; segment ++) {

                                if (bitArray[x, y + segment]) {
                                    block = (uint)1 << segment;
                                }
                            }

                            finalArray[blockIndex++] = block;
                        }
                    }
                }
            }
        }

        static void ConvertArrayToImage(string inputFile, string outputPath)
        {
            // Open file - scan for data?
            // Read in header data
            // Type (Graphic|Font)
            // Width
            // Height
            // Num Images packed
            // Format (Vertical|Horizontal) - What about LSB/MSB order?
            // Start Character (Font Only)
        }
    }
}

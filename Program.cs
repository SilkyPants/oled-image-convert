using System;
using System.Drawing;
using System.IO;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Dithering; 

namespace OledImageConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            // int index = 0;
            // foreach (var arg in args)
            // {
			// 	Console.WriteLine("Arg {0}: {1}", index++, arg);
            // }

            // var inputFile = args.Last();

            // if (!File.Exists(inputFile)) {
            //     Console.WriteLine("Cannot load file {0}", inputFile);

            //     return;
            // }

            ConvertImageToArray("tachikoma.png", "tachikoma.h");

            ConvertArrayToImage("tachikoma.h", "tachikoma-Test.png");
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
                var cutOff = 254f;

                // Ensure the image is grayscale
                image.Mutate(x => x.Grayscale().Dither(new FloydSteinbergDiffuser(), .8f));

                // Loop through pixels
                for (int x = 0; x < image.Width; x++)
                {
                   for (int y = 0; y < image.Height; y++)
                   {
                       var pixel = image[x, y];
                       
                       var value = pixel.R;
                       
                       bitArray[x, y] = value <= cutOff;
                   }
                }

                // Final array output
                var finalArray = new byte[image.Width * (image.Height / 8)];
                var blockIndex = 0;

                // if Vertical
                if (true) {
                        
                    for (int x = 0; x < image.Width; x++) {
                        for (int y = 0; y < image.Height; y +=8) {

                            byte block = 0;

                            for (byte segment = 0; segment < 8; segment ++) {

                                if (bitArray[x, y + segment]) {
                                    block |= (byte)(1 << segment);
                                }
                            }

                            finalArray[blockIndex++] = block;
                        }
                    }
                }
                else { // Horizontal
                    
                    for (int y = 0; y < image.Height; y +=8) {
                        for (int x = 0; x < image.Width; x++) {
                            byte block = 0;

                            for (byte segment = 0; segment < 8; segment ++) {

                                if (bitArray[x, y + segment]) {
                                    block |= (byte)(1 << segment);
                                }
                            }

                            finalArray[blockIndex++] = block;
                        }
                    }
                }


                using (var output = new StreamWriter(outputPath))
                {
                    bool first = true;
                    foreach(var block in finalArray) {

                        if (!first) {
                            output.Write(",");
                        }
                        else {
                            first = false;
                        }

                        output.Write("0x" + block.ToString("X2"));
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

            int imageWidth = 128;
            int imageHeight = 64;

            using (var input = new StreamReader(inputFile))
            using (FileStream output = File.OpenWrite(outputPath))
            using (Image<Rgba32> image = new Image<Rgba32>(imageWidth, imageHeight))
            {
                var blocks = input.ReadLine().Split(',');

                for (int i = 0; i < blocks.Length; i++)
                {
                    var blockString = blocks[i].Substring(2, 2);
                    var block = Convert.ToByte(blockString, 16);

                    // This only works for vertical
                    int blockHeight = imageHeight / 8;
                    int x = i / blockHeight;
                    int startY = (i - (x * blockHeight)) * 8; //??

                    for (byte segment = 0; segment < 8; segment ++) {
                        int y = startY + segment;
                        if ((block & (1 << segment)) != 0) {
                            image[x, y] = Rgba32.Black;
                        }
                        else {
                            image[x, y] = Rgba32.White;
                        }
                    }
                }

                image.SaveAsPng(output);
                Console.WriteLine("Image Saved");
            }
        }
    }
}

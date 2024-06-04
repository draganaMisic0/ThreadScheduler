using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OPOS_project;
namespace OPOS_project.Specific_jobs
{
    internal class BlurImageJob 
    {
        public string Path { get; init; } = "Blur";
        private Image image = null;
        public BlurImageJob(string path)
        {
            Path = path;
        }
        public void isJobInterrupter() {
            Console.WriteLine("metod koji treba napisati");
        }
        public Bitmap Run(Bitmap image, int blurSize)
        {
            
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // Make an exact copy of the bitmap provided
            Graphics graphics = Graphics.FromImage(blurred);
            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            

            // Look at every pixel in the blur rectangle
            for (int xx = 0; xx < blurred.Width; xx++)
            {
                for (int yy = 0; yy < blurred.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0; //number of pixels used in calculation of current pixel

                    // Average the color of the red, green, and blue for each pixel in the blur size
                    for (int x = xx; (x < xx + blurSize && x < blurred.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < blurred.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // Now that we know the average for the pixel, we need to set the pixel to the new color
                    for (int x = xx; (x < xx + blurSize && x < blurred.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < blurred.Height); y++)
                        {
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                        }
                    }
                }
            }

            return blurred;
            

        }
    }
}

using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OPOS_project;
using System.Transactions;
namespace OPOS_project.Specific_jobs
{
    internal class BlurImageJob : Job
    {
        public string Path { get; init; } = "Blur";

       // BitmapImage image=new BitmapImage();
     
        public BlurImageJob(JobCreationElements elements, int priority) : 
            base(elements, priority)
        {
           // this.image = (image == null ? new BitmapImage() : image);
        }
       
        public void Run(IStatefulJob isj)
        {
            int blurSize = 5;
           
            Bitmap blurred = new Bitmap(this.Image.PixelWidth, this.Image.PixelHeight);
               
            /*

            // Make an exact copy of the bitmap provided
            Graphics graphics = Graphics.FromImage(blurred);
            graphics.DrawImage(blurred, new Rectangle(0, 0, this.Image.PixelWidth, this.Image.PixelHeight), 
                new Rectangle(0, 0, this.Image.PixelWidth, this.Image.PixelHeight), GraphicsUnit.Pixel);
            
            */
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
                            //checkState();
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

            //return blurred;
            

        }
    }
}

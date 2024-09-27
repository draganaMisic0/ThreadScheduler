using OPOS_project.Scheduler;
using System.Drawing;
using System.Drawing.Imaging;

namespace OPOS_project.Specific_jobs
{
    internal class BlurImageJob 
    {
       /* public string Path { get; init; } = "Blur";

       
        public BlurImageJob(JobMessage elements, int priority) : base(elements, priority)
        {
        }

        public override void RunThisJob()
        {
            int blurSize = 7;
            int totalPixels = this.myJobElements.Image.Width * this.myJobElements.Image.Height;

            Bitmap original = new Bitmap(this.myJobElements.Image);
            Bitmap blurred = new Bitmap(original.Width, original.Height);

            int processed_pixels = 0;

            for (int xx = 0; xx < original.Width; xx++)
            {
                for (int yy = 0; yy < original.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0; 

                    for (int x = xx; (x < xx + blurSize && x < original.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < original.Height); y++)
                        {
                            Color pixel = original.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                            this.checkState();
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    for (int x = xx; (x < xx + blurSize && x < original.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < original.Height); y++)
                        {
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                        }
                    }

                    processed_pixels++;
                    this.Progress = (int)((double)processed_pixels / totalPixels * 100);
                }
            }
            this.Finish();
            string name = @"\" + myJobElements.Name + ".png";
            blurred.Save(RESULT_FILE_PATH + name, ImageFormat.Png);
        }
       */
    }
       
       
}
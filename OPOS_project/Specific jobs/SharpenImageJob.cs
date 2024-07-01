using OPOS_project.Scheduler;
using System.Drawing;
using System.Drawing.Imaging;

namespace OPOS_project.Specific_jobs
{
    internal class SharpenImageJob : Job
    {
        public SharpenImageJob(JobCreationElements elements, int priority) : base(elements, priority)
        {
        }

        /*  public override void RunThisJob()
          {
              throw new NotImplementedException();
          }*/

        public override void RunThisJob()
        {
            Bitmap sharpened = new Bitmap(myJobElements.Image.Width, myJobElements.Image.Height);

            int[,] kernel = { { -1, -1, -1 },
                          { -1,  9, -1 },
                          { -1, -1, -1 } };

            int kernelSize = 3;
            int kernelOffset = (kernelSize - 1) / 2;
            int kernelDivisor = 1;

            int totalPixels = (myJobElements.Image.Width - 2 * kernelOffset) * (myJobElements.Image.Height - 2 * kernelOffset);
            int processedPixels = 0;

            // Apply convolution
            for (int y = kernelOffset; y < myJobElements.Image.Height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < myJobElements.Image.Width - kernelOffset; x++)
                {
                    int r = 0, g = 0, b = 0;

                    // Apply kernel
                    for (int ky = 0; ky < kernelSize; ky++)
                    {
                        for (int kx = 0; kx < kernelSize; kx++)
                        {
                            this.checkState();
                            Color pixel = myJobElements.Image.GetPixel(x + kx - kernelOffset, y + ky - kernelOffset);
                            r += pixel.R * kernel[ky, kx];
                            g += pixel.G * kernel[ky, kx];
                            b += pixel.B * kernel[ky, kx];
                        }
                    }

                    r = Math.Min(Math.Max(r / kernelDivisor, 0), 255);
                    g = Math.Min(Math.Max(g / kernelDivisor, 0), 255);
                    b = Math.Min(Math.Max(b / kernelDivisor, 0), 255);

                    sharpened.SetPixel(x, y, Color.FromArgb(r, g, b));

                    processedPixels++;
                    this.Progress = (int)((double)processedPixels / totalPixels * 100);
                }
            }
            this.Finish();
            string name = @"\" + myJobElements.Name + ".png";
            sharpened.Save(RESULT_FILE_PATH + name, ImageFormat.Png);
        }
    }
}
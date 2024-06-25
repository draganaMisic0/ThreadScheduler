using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Specific_jobs
{
    internal class SharpenImageJob : Job
    {
        public SharpenImageJob(JobCreationElements elements, int priority): base(elements, priority) { }    
        public static Bitmap Run(Bitmap image)
        {
            Bitmap sharpened = new Bitmap(image.Width, image.Height);

            int[,] kernel = { { -1, -1, -1 },
                          { -1,  9, -1 },
                          { -1, -1, -1 } };

            int kernelSize = 3;
            int kernelOffset = (kernelSize - 1) / 2;
            int kernelDivisor = 1;

            // Apply convolution
            for (int y = kernelOffset; y < image.Height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < image.Width - kernelOffset; x++)
                {
                    int r = 0, g = 0, b = 0;

                    // Apply kernel
                    for (int ky = 0; ky < kernelSize; ky++)
                    {
                        for (int kx = 0; kx < kernelSize; kx++)
                        {
                            Color pixel = image.GetPixel(x + kx - kernelOffset, y + ky - kernelOffset);
                            r += pixel.R * kernel[ky, kx];
                            g += pixel.G * kernel[ky, kx];
                            b += pixel.B * kernel[ky, kx];
                        }
                    }

                    r = Math.Min(Math.Max(r / kernelDivisor, 0), 255);
                    g = Math.Min(Math.Max(g / kernelDivisor, 0), 255);
                    b = Math.Min(Math.Max(b / kernelDivisor, 0), 255);

                    sharpened.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return sharpened;
        }
    }
}

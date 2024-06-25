using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Specific_jobs
{
    internal class EqualizeHIstogramJob : Job
    {
        public EqualizeHIstogramJob(JobCreationElements elements, int priority): base(elements, priority) { }
        public static Bitmap Run(Bitmap image)
        {
            // Define the sharpening kernel
            double[,] kernel = {
            { -1, -1, -1 },
            { -1,  9, -1 },
            { -1, -1, -1 }
        };

            // Convolve the image with the kernel
            return Convolve(image, kernel);
        }

        public static Bitmap Convolve(Bitmap image, double[,] kernel)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);
            int kernelOffset = kernelWidth / 2;

            // Loop through each pixel in the image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    double red = 0, green = 0, blue = 0;

                    // Apply the kernel to the neighborhood of the current pixel
                    for (int ky = 0; ky < kernelHeight; ky++)
                    {
                        for (int kx = 0; kx < kernelWidth; kx++)
                        {
                            int pixelX = x + kx - kernelOffset;
                            int pixelY = y + ky - kernelOffset;

                            // Check if the pixel is within the image boundaries
                            if (pixelX >= 0 && pixelX < image.Width && pixelY >= 0 && pixelY < image.Height)
                            {
                                Color pixel = image.GetPixel(pixelX, pixelY);
                                red += pixel.R * kernel[kx, ky];
                                green += pixel.G * kernel[kx, ky];
                                blue += pixel.B * kernel[kx, ky];
                            }
                        }
                    }

                    // Clamp the color values to the range [0, 255]
                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    // Set the color of the corresponding pixel in the result image
                    result.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue));
                }
            }

            return result;
        }
    }
}

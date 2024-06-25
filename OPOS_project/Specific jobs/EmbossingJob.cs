using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Specific_jobs
{
    internal class EmbossingJob : Job
    {
        public EmbossingJob(JobCreationElements elements, int priority) : base(elements, priority) { }

        public static Bitmap Run(Bitmap image)
        {
            // Convert image to grayscale
            Bitmap grayscaleImage = ConvertToGrayscale(image);

            // Define emboss kernel
            int[,] embossKernel = {
            { -2, -1, 0 },
            { -1,  1, 1 },
            {  0,  1, 2 }
        };

            // Apply embossing convolution
            Bitmap embossedImage = ApplyConvolution(grayscaleImage, embossKernel);

            return embossedImage;
        }

        public static Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int intensity = (int)((pixel.R + pixel.G + pixel.B) / 3.0);
                    Color newPixel = Color.FromArgb(intensity, intensity, intensity);
                    grayscaleImage.SetPixel(x, y, newPixel);
                }
            }

            return grayscaleImage;
        }

        public static Bitmap ApplyConvolution(Bitmap image, int[,] kernel)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            int kHeight = kernel.GetLength(0);
            int kWidth = kernel.GetLength(1);
            int kCenterY = kHeight / 2;
            int kCenterX = kWidth / 2;

            for (int y = kCenterY; y < image.Height - kCenterY; y++)
            {
                for (int x = kCenterX; x < image.Width - kCenterX; x++)
                {
                    int newR = 128, newG = 128, newB = 128; // Start with a medium gray background

                    for (int ky = -kCenterY; ky <= kCenterY; ky++)
                    {
                        for (int kx = -kCenterX; kx <= kCenterX; kx++)
                        {
                            int pixelVal = image.GetPixel(x + kx, y + ky).R;
                            newR += pixelVal * kernel[ky + kCenterY, kx + kCenterX];
                            newG += pixelVal * kernel[ky + kCenterY, kx + kCenterX];
                            newB += pixelVal * kernel[ky + kCenterY, kx + kCenterX];
                        }
                    }

                    newR = Math.Min(255, Math.Max(0, newR));
                    newG = Math.Min(255, Math.Max(0, newG));
                    newB = Math.Min(255, Math.Max(0, newB));
                    result.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                }
            }

            return result;
        }
    }
}

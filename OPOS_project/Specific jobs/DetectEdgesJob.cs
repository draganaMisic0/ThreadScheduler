using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OPOS_project.Specific_jobs
{
    internal class DetectEdgesJob: Job
    {
        public DetectEdgesJob(JobCreationElements myJobElements, int priority) : base (myJobElements, priority) {}
        public override void RunThisJob()
        {
            // Convert image to grayscale
            Bitmap grayscaleImage = ConvertToGrayscale(myJobElements.Image);

            // Define Sobel kernels
            int[,] horizontalSobel = {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

            int[,] verticalSobel = {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
        };

            // Apply horizontal and vertical Sobel operators
            Bitmap horizontalEdges = ApplyConvolution(grayscaleImage, horizontalSobel);
            Bitmap verticalEdges = ApplyConvolution(grayscaleImage, verticalSobel);

            // Combine horizontal and vertical edges
            Bitmap combinedEdges = CombineEdgeImages(horizontalEdges, verticalEdges);

            return combinedEdges;
        }

        public static Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int intensity = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
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
                    int newColorValue = 0;

                    for (int ky = -kCenterY; ky <= kCenterY; ky++)
                    {
                        for (int kx = -kCenterX; kx <= kCenterX; kx++)
                        {
                            int pixelVal = image.GetPixel(x + kx, y + ky).R;
                            newColorValue += pixelVal * kernel[ky + kCenterY, kx + kCenterX];
                        }
                    }

                    newColorValue = Math.Min(255, Math.Max(0, newColorValue));
                    result.SetPixel(x, y, Color.FromArgb(newColorValue, newColorValue, newColorValue));
                }
            }

            return result;
        }

        public static Bitmap CombineEdgeImages(Bitmap horizontalEdges, Bitmap verticalEdges)
        {
            // Combine horizontal and vertical edge images
            int width = horizontalEdges.Width;
            int height = horizontalEdges.Height;
            Bitmap combinedEdges = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color horizontalPixel = horizontalEdges.GetPixel(x, y);
                    Color verticalPixel = verticalEdges.GetPixel(x, y);

                    int grayValue = Math.Min(255, (int)Math.Sqrt(horizontalPixel.R * horizontalPixel.R + verticalPixel.R * verticalPixel.R));
                    combinedEdges.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return combinedEdges;
        }
    }
}

using OPOS_project.Scheduler;
using System.Drawing;
using System.Drawing.Imaging;

namespace OPOS_project.Specific_jobs
{
    internal class DetectEdgesJob
    {
      /*  public DetectEdgesJob(JobMessage myJobElements, int priority) : base(myJobElements, priority)
        {
        }

        public override void RunThisJob()
        {

            int totalSteps = myJobElements.Image.Height * myJobElements.Image.Width * 3; // Grayscale + Horizontal + Vertical
            int processedSteps = 0;
            Bitmap grayscaleImage = ConvertToGrayscale(myJobElements.Image, ref processedSteps, totalSteps);
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

            Bitmap horizontalEdges = ApplyConvolution(grayscaleImage, horizontalSobel, ref processedSteps, totalSteps);
            Bitmap verticalEdges = ApplyConvolution(grayscaleImage, verticalSobel, ref processedSteps, totalSteps);

            Bitmap combinedEdges = CombineEdgeImages(horizontalEdges, verticalEdges);

            this.Finish();

            string name = @"\" + myJobElements.Name + ".png";
            combinedEdges.Save(RESULT_FILE_PATH + name, ImageFormat.Png);
        }

        public Bitmap ConvertToGrayscale(Bitmap image, ref int processedSteps, int totalSteps)
        {
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    this.checkState();
                    Color pixel = image.GetPixel(x, y);
                    int intensity = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
                    Color newPixel = Color.FromArgb(intensity, intensity, intensity);
                    grayscaleImage.SetPixel(x, y, newPixel);

                    processedSteps++;
                    this.Progress = (int)((double)processedSteps / totalSteps * 100);
                }
            }

            return grayscaleImage;
        }

        public Bitmap ApplyConvolution(Bitmap image, int[,] kernel, ref int processedSteps, int totalSteps)
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
                            this.checkState();
                            int pixelVal = image.GetPixel(x + kx, y + ky).R;
                            newColorValue += pixelVal * kernel[ky + kCenterY, kx + kCenterX];
                        }
                    }

                    newColorValue = Math.Min(255, Math.Max(0, newColorValue));
                    result.SetPixel(x, y, Color.FromArgb(newColorValue, newColorValue, newColorValue));

                    processedSteps++;
                    this.Progress = (int)((double)processedSteps / totalSteps * 100);
                }
            }

            return result;
        }

        public Bitmap CombineEdgeImages(Bitmap horizontalEdges, Bitmap verticalEdges)
        {
            int width = horizontalEdges.Width;
            int height = horizontalEdges.Height;
            Bitmap combinedEdges = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.checkState();
                    Color horizontalPixel = horizontalEdges.GetPixel(x, y);
                    Color verticalPixel = verticalEdges.GetPixel(x, y);

                    int grayValue = Math.Min(255, (int)Math.Sqrt(horizontalPixel.R * horizontalPixel.R + verticalPixel.R * verticalPixel.R));
                    combinedEdges.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            return combinedEdges;
        }
      */
    }
}
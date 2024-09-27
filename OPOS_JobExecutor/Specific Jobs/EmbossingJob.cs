using OPOS_project.Scheduler;
using System.Drawing;
using System.Drawing.Imaging;

namespace OPOS_project.Specific_jobs
{
    internal class EmbossingJob 
    {
       /* public EmbossingJob(JobMessage elements, int priority) : base(elements, priority)
        {
        }

        public override void RunThisJob()
        {
            int totalSteps = myJobElements.Image.Width * myJobElements.Image.Height * 2;
            int processedSteps = 0;
            Bitmap grayscaleImage = ConvertToGrayscale(myJobElements.Image, ref processedSteps, totalSteps);

            int[,] embossKernel = {
            { -2, -1, 0 },
            { -1,  1, 1 },
            {  0,  1, 2 }
        };

            Bitmap embossedImage = ApplyConvolution(grayscaleImage, embossKernel, ref processedSteps, totalSteps);

            this.Finish();
            string name = @"\" + myJobElements.Name + ".png";
            embossedImage.Save(RESULT_FILE_PATH + name, ImageFormat.Png);
            this.Progress = 100;
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
                    int intensity = (int)((pixel.R + pixel.G + pixel.B) / 3.0);
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
                    int newR = 128, newG = 128, newB = 128;

                    for (int ky = -kCenterY; ky <= kCenterY; ky++)
                    {
                        for (int kx = -kCenterX; kx <= kCenterX; kx++)
                        {
                            this.checkState();
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

                    processedSteps++;
                    this.Progress = (int)((double)processedSteps / totalSteps * 100);
                }
            }

            return result;
        }
       */
    }
}
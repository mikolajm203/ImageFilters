using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace lab1v2
{
    public abstract class Filter
    {
        public String Name;

        abstract public Image apply(Image image);
    }

    class InverseFilter : Filter
    {
        public InverseFilter()
        {
            Name = "InverseFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    Color invertedPixel = Color.FromArgb(originalPixel.A, 255 - originalPixel.R, 255 - originalPixel.G
                        , 255 - originalPixel.B);
                    bitmap.SetPixel(i, j, invertedPixel);
                }
            }
            return bitmap;
        }
    }
    class BrightenFilter : Filter
    {
        private const int value = 40;
        public BrightenFilter()
        {
            Name = "BrightenFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    Color brightenedPixel = Color.FromArgb(originalPixel.A, Math.Min(originalPixel.R + value, (byte)255),
                        Math.Min(originalPixel.G + value, (byte)255),
                        Math.Min(originalPixel.B + value, (byte)255));
                    bitmap.SetPixel(i, j, brightenedPixel);
                }
            }
            return bitmap;
        }
    }

    class ContrastFilter : Filter
    {
        private const int value = 60;
        public ContrastFilter()
        {
            Name = "ContrastFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    newR = calculatePixel(originalPixel.R);
                    newG = calculatePixel(originalPixel.G);
                    newB = calculatePixel(originalPixel.B);
                    Color contrastedPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, contrastedPixel);
                }
            }
            return bitmap;
        }

        private byte calculatePixel(byte pixel)
        {
            if (pixel < value)
            {
                return (byte)0;
            }
            else if (pixel > 255 - value)
            {
                return (byte)255;
            }
            else
            {
                float ret = ((float)(pixel - value) / (float)(255 - value - value)) * (255);
                return (byte)ret;
            }
        }
    }
    class GammaFilter : Filter
    {
        private const float gamma = 1.6f;
        public GammaFilter()
        {
            Name = "GammaFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    newR = (byte)(Math.Pow((double)originalPixel.R / 255, gamma) * 255);
                    newG = (byte)(Math.Pow((double)originalPixel.G / 255, gamma) * 255);
                    newB = (byte)(Math.Pow((double)originalPixel.B / 255, gamma) * 255);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
    }
    class BlurFilter : Filter
    {
        private int[,] kernel;
        private double factor = 1.0/9;
        public BlurFilter()
        {
            kernel = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    kernel[i, j] = 1;
                }
            }
            Name = "BlurFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    int Rsum = 0, Gsum = 0, Bsum = 0;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            Color consideredPixel = bitmap.GetPixel(i - 1 + x, j - 1 + y);
                            Rsum += consideredPixel.R * kernel[x, y];
                            Gsum += consideredPixel.G * kernel[x, y];
                            Bsum += consideredPixel.B * kernel[x, y];
                        }
                    }
                    newR = (byte)(Rsum * factor);
                    newG = (byte)(Gsum * factor);
                    newB = (byte)(Bsum * factor);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte secure(int min, int max, double value)
        {
            return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
        }
    }
    class GaussianBlurFilter : Filter
    {
        private double factor;
        private int[,] kernel;
        public GaussianBlurFilter()
        {
            factor = 1.0 / 16;
            kernel = new int[,] { { 1, 2, 1 },
                                { 2, 4, 2 },
                                { 1, 2, 1 } };
            Name = "GaussianBlurFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    int Rsum = 0, Gsum = 0, Bsum = 0;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            Color consideredPixel = bitmap.GetPixel(i - 1 + x, j - 1 + y);
                            Rsum += consideredPixel.R * kernel[x, y];
                            Gsum += consideredPixel.G * kernel[x, y];
                            Bsum += consideredPixel.B * kernel[x, y];
                        }
                    }
                    newR = secure(0, 255, Rsum * factor);
                    newG = secure(0, 255, Gsum * factor);
                    newB = secure(0, 255, Bsum * factor);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte secure(int min, int max, double value)
        {
            return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
        }

    }
    class SharpenFilter : Filter
    {
        private int[,] kernel;
        private double factor;
        public SharpenFilter()
        {
            factor = 1.0;
            kernel = new int[,] { { -1, -1, -1 },
                                    { -1, 9, -1 },
                                    { -1, -1, -1 } };
            Name = "SharpenFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    int Rsum = 0, Gsum = 0, Bsum = 0;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            Color consideredPixel = bitmap.GetPixel(i - 1 + x, j - 1 + y);
                            Rsum += consideredPixel.R * kernel[x, y];
                            Gsum += consideredPixel.G * kernel[x, y];
                            Bsum += consideredPixel.B * kernel[x, y];
                        }
                    }
                    newR = secure(0, 255, Rsum * factor);
                    newG = secure(0, 255, Gsum * factor);
                    newB = secure(0, 255, Bsum * factor);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte secure(int min, int max, double value)
        {
            return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
        }
    }
    class EdgeFilter : Filter
    {
        private int[,] kernel;
        private double factor;
        public EdgeFilter()
        {
            factor = 1.0;
            kernel = new int[,] { { -1, 0, 0 },
                                    { 0, 1, 0 },
                                    { 0, 0, 0 } };
            Name = "EdgeFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    int Rsum = 0, Gsum = 0, Bsum = 0;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            Color consideredPixel = bitmap.GetPixel(i - 1 + x, j - 1 + y);
                            Rsum += consideredPixel.R * kernel[x, y];
                            Gsum += consideredPixel.G * kernel[x, y];
                            Bsum += consideredPixel.B * kernel[x, y];
                        }
                    }
                    newR = secure(0, 255, Rsum * factor);
                    newG = secure(0, 255, Gsum * factor);
                    newB = secure(0, 255, Bsum * factor);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte secure(int min, int max, double value)
        {
            return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
        }
    }
    class EmbossFilter : Filter
    {
        private int[,] kernel;
        private double factor;
        public EmbossFilter()
        {
            factor = 1.0;
            kernel = new int[,] { { -1, -1, -1 },
                                    { 0, 1, 0 },
                                    { 1, 1, 1 } };
            Name = "EmbossFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte newR, newG, newB;
                    int Rsum = 0, Gsum = 0, Bsum = 0;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            Color consideredPixel = bitmap.GetPixel(i - 1 + x, j - 1 + y);
                            Rsum += consideredPixel.R * kernel[x, y];
                            Gsum += consideredPixel.G * kernel[x, y];
                            Bsum += consideredPixel.B * kernel[x, y];
                        }
                    }
                    newR = secure(0, 255, Rsum * factor);
                    newG = secure(0, 255, Gsum * factor);
                    newB = secure(0, 255, Bsum * factor);
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte secure(int min, int max, double value)
        {
            return (value < min) ? (byte)min : (value > max) ? (byte)max : (byte)value;
        }
    }
    class HFilter : Filter
    {
        public HFilter()
        {
            Name = "HFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte min = getMin(originalPixel.R, originalPixel.G, originalPixel.B);
                    byte max = getMax(originalPixel.R, originalPixel.G, originalPixel.B);
                    byte newR, newG, newB;
                    double Hprim;
                    if(min == max)
                    {
                        newR = 0;
                        newG = 0;
                        newB = 0;
                    }
                    else if(originalPixel.R == max)
                    {
                        Hprim = ((double)(originalPixel.G - originalPixel.B) / (max - min));
                        if(Hprim < 0)
                        {
                            Hprim += 6;
                        }
                        newR = (byte)(((double)Hprim / 6) * 255);
                        newG = newR;
                        newB = newR;
                    }
                    else if(originalPixel.G == max)
                    {
                        Hprim = (double)(originalPixel.B - originalPixel.R) / (max - min) + 2;
                        newR = (byte)(((double)Hprim / 6) * 255);
                        newG = newR;
                        newB = newR;
                    }
                    else
                    {
                        Hprim = (double)(originalPixel.R - originalPixel.G) / (max - min) + 4;
                        newR = (byte)(((double)Hprim / 6) * 255);
                        newG = newR;
                        newB = newR;
                    }
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte getMax(byte R, byte G, byte B)
        {
            if(R > G && R > B)
            {
                return R;
            }
            else if(G > R && G > B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
        private byte getMin(byte R, byte G, byte B)
        {
            if (R < G && R < B)
            {
                return R;
            }
            else if (G < R && G < B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
    }
    class SFilter : Filter
    {
        public SFilter()
        {
            Name = "SFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte min = getMin(originalPixel.R, originalPixel.G, originalPixel.B);
                    byte max = getMax(originalPixel.R, originalPixel.G, originalPixel.B);
                    byte newR, newG, newB;
                    if (max == 0)
                    {
                        newR = 0;
                        newG = 0;
                        newB = 0;
                    }
                    else
                    {
                        newR = (byte)(255 * ((double)(max - min) / max));
                        newG = newR;
                        newB = newR;
                    }
                    Color newPixel = Color.FromArgb(originalPixel.A, newR, newG, newB);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte getMax(byte R, byte G, byte B)
        {
            if (R > G && R > B)
            {
                return R;
            }
            else if (G > R && G > B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
        private byte getMin(byte R, byte G, byte B)
        {
            if (R < G && R < B)
            {
                return R;
            }
            else if (G < R && G < B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
    }
    class VFilter : Filter
    {
        public VFilter()
        {
            Name = "VFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    Color originalPixel = bitmap.GetPixel(i, j);
                    byte max = getMax(originalPixel.R, originalPixel.G, originalPixel.B);
                    Color newPixel = Color.FromArgb(originalPixel.A, max, max, max);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
            return bitmap;
        }
        private byte getMax(byte R, byte G, byte B)
        {
            if (R > G && R > B)
            {
                return R;
            }
            else if (G > R && G > B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
        private byte getMin(byte R, byte G, byte B)
        {
            if (R < G && R < B)
            {
                return R;
            }
            else if (G < R && G < B)
            {
                return G;
            }
            else
            {
                return B;
            }
        }
    }
    class GrayFilter : Filter
    {
        public GrayFilter()
        {
            this.Name = "GrayFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            for (int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    int value = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                    bitmap.SetPixel(i, j, Color.FromArgb(pixel.A, value, value, value));
                }
            }
            return bitmap;
        }
    }
    class AverageDitheringFilter : Filter
    {
        int R, G, B;
        int[] Rvalues, Gvalues, Bvalues;
        public static bool isApplied= false;
        bool isInShades;
        int K;
        int[] values;
        public AverageDitheringFilter(int K)
        {
            isInShades = true;
            this.Name = "AverageDitheringFilter";
            this.K = K;
            values = new int[K];
            for (int i = 0; i < K; i++)
            {
                values[i] = (255 / (R - 1)) * i;
            }
        }
        public AverageDitheringFilter(int R, int G, int B)
        {
            isInShades = false;
            this.Name = "AverageDitheringFilter";
            this.R = R;
            this.G = G;
            this.B = B;
            Rvalues = new int[R];
            Gvalues = new int[G];
            Bvalues = new int[B];
            for(int i = 0; i < R; i++)
            {
                Rvalues[i] =  (i * 255 / (R - 1));
            }
            for (int i = 0; i < G; i++)
            {
                Gvalues[i] = (i * 255 / (G - 1));
            }
            for (int i = 0; i < B; i++)
            {
                Bvalues[i] = (i * 255 / (B - 1));
            }
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            double[] averages = Averages(bitmap);

            for(int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    if (!isInShades)
                    {
                        int newRed = computeNewColor(pixel.R, averages[0], Rvalues);
                        int newGreen = computeNewColor(pixel.G, averages[1], Gvalues);
                        int newBlue = computeNewColor(pixel.B, averages[2], Bvalues);
                        Color newPixel = Color.FromArgb(pixel.A, newRed, newGreen, newBlue);
                        bitmap.SetPixel(i, j, newPixel);
                    }
                    else
                    {
                        int oldShade = (pixel.R + pixel.G + pixel.B) / 3;
                        int newShade = computeNewColor(oldShade, averages[0], this.values);
                        Color newPixel = Color.FromArgb(pixel.A, newShade, newShade, newShade);
                        bitmap.SetPixel(i, j, newPixel);
                    }
                }
            }
            return bitmap;
        }
        private double[] Averages(Bitmap bitmap)
        {
            if (!isInShades)
            {
                int Rsum = 0;
                int Gsum = 0;
                int Bsum = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        Color pixel = bitmap.GetPixel(i, j);
                        Rsum += pixel.R;
                        Gsum += pixel.G;
                        Bsum += pixel.B;
                    }
                }
                double Ravg = (double)(Rsum / (bitmap.Width * bitmap.Height)) / 255;
                double Gavg = (double)(Gsum / (bitmap.Width * bitmap.Height)) / 255;
                double Bavg = (double)(Bsum / (bitmap.Width * bitmap.Height)) / 255;
                return new double[] { Ravg, Gavg, Bavg };
            }
            else
            {
                int sum = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        Color pixel = bitmap.GetPixel(i, j);
                        sum += pixel.R;
                        sum += pixel.G;
                        sum += pixel.B;
                    }
                }
                double avg = (double)(sum / (bitmap.Width * bitmap.Height * 3)) / 255;
                return new double[] { avg };
            }
        }
        private int computeNewColor(int oldColor, double avg, int[] values)
        {
            int range = values[1] - values[0];
            int min = 0, max = 255;
            for(int i = 0; i < values.Length; i++)
            {
                if(values[i] > oldColor)
                {
                    min = values[i - 1];
                    max = values[i];
                    break;
                }
            }
            if (oldColor > min && oldColor < min + avg * range)
            {
                return min;
            }
            else
            {
                return max;
            }
        }
    }

    class YCbCrDitheringFilter : Filter
    {
        int R, G, B;
        int[] Rvalues, Gvalues, Bvalues;
        public static bool isApplied = false;
        bool isInShades;
        int K;
        int[] values;
        public YCbCrDitheringFilter(int K)
        {
            isInShades = true;
            this.Name = "YCbCrDitheringFilter";
            this.K = K;
            values = new int[K];
            for (int i = 0; i < K; i++)
            {
                values[i] = (255 / (R - 1)) * i;
            }
        }
        public YCbCrDitheringFilter(int R, int G, int B)
        {
            isInShades = false;
            this.Name = "YCbCrDitheringFilter";
            this.R = R;
            this.G = G;
            this.B = B;
            Rvalues = new int[R];
            Gvalues = new int[G];
            Bvalues = new int[B];
            for (int i = 0; i < R; i++)
            {
                Rvalues[i] = (255 * i)/ (R - 1);
            }
            for (int i = 0; i < G; i++)
            {
                Gvalues[i] = (255 * i)/ (G - 1);
            }
            for (int i = 0; i < B; i++)
            {
                Bvalues[i] = (255 * i)/ (B - 1);
            }
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();

            convertToYCbCr(bitmap);

            double[] averages = Averages(bitmap);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    if (!isInShades)
                    {
                        int newRed = computeNewColor(pixel.R, averages[0], Rvalues);
                        int newGreen = computeNewColor(pixel.G, averages[1], Gvalues);
                        int newBlue = computeNewColor(pixel.B, averages[2], Bvalues);
                        Color newPixel = Color.FromArgb(pixel.A, newRed, newGreen, newBlue);
                        bitmap.SetPixel(i, j, newPixel);
                    }
                    else
                    {
                        int oldShade = (pixel.R + pixel.G + pixel.B) / 3;
                        int newShade = computeNewColor(oldShade, averages[0], this.values);
                        Color newPixel = Color.FromArgb(pixel.A, newShade, newShade, newShade);
                        bitmap.SetPixel(i, j, newPixel);
                    }
                }
            }
            convertToRGB(bitmap);
            return bitmap;
        }
        private void convertToRGB(Bitmap bitmap)
        {
            for(int i = 0; i < bitmap.Width; i++)
            {
                for(int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    double R = Math.Clamp(pixel.R + 1.402 * (pixel.B - 128), 0, 255);
                    double G = Math.Clamp(pixel.R - 0.334 * (pixel.G - 128) - 0.714 * (pixel.B - 128), 0, 255);
                    double B = Math.Clamp(pixel.R + 1.772 * (pixel.G - 128), 0, 255);
                    Color newPixel = Color.FromArgb(255, (int)R, (int)G, (int)B);
                }
            }
        }
        private void convertToYCbCr(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    double Yprim = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                    double Cb = 128 - 0.169 * pixel.R - 0.331 * G + 0.5 * pixel.B;
                    double Cr = 128 + 0.5 * R - 0.419 * pixel.G - 0.081 * pixel.B;
                    Color newPixel = Color.FromArgb(255, (int)Yprim, (int)Cb, (int)Cr);
                    bitmap.SetPixel(i, j, newPixel);
                }
            }
        }
        private double[] Averages(Bitmap bitmap)
        {
            if (!isInShades)
            {
                int Rsum = 0;
                int Gsum = 0;
                int Bsum = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        Color pixel = bitmap.GetPixel(i, j);
                        Rsum += pixel.R;
                        Gsum += pixel.G;
                        Bsum += pixel.B;
                    }
                }
                double Ravg = (double)(Rsum / (bitmap.Width * bitmap.Height)) / 255;
                double Gavg = (double)(Gsum / (bitmap.Width * bitmap.Height)) / 255;
                double Bavg = (double)(Bsum / (bitmap.Width * bitmap.Height)) / 255;
                return new double[] { Ravg, Gavg, Bavg };
            }
            else
            {
                int sum = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        Color pixel = bitmap.GetPixel(i, j);
                        sum += pixel.R;
                        sum += pixel.G;
                        sum += pixel.B;
                    }
                }
                double avg = (double)(sum / (bitmap.Width * bitmap.Height * 3)) / 255;
                return new double[] { avg };
            }
        }
        private int computeNewColor(int oldColor, double avg, int[] values)
        {
            int range = values[1] - values[0];
            int min = 0, max = 255;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > oldColor)
                {
                    min = values[i - 1];
                    max = values[i];
                    break;
                }
            }
            if (oldColor < min + avg * range)
            {
                return min;
            }
            else
            {
                return max;
            }
        }
    }

    class MedianCutFilter : Filter
    {
        int colors;
        static bool isActive = false;
        public MedianCutFilter(int colors)
        {
            this.colors = colors;
            this.Name = "MedianCutFilter";
        }
        public override Image apply(Image image)
        {
            Bitmap bitmap = (Bitmap)image.Clone();
            List<PixelClass> initialBucket = new List<PixelClass>();
            for(int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    initialBucket.Add(new PixelClass(bitmap.GetPixel(i, j), i, j));
                }
            }
            splitBuckets((int)Math.Log2((double)colors), initialBucket, bitmap);
            return bitmap;
        }
        private void splitBuckets(int depth, List<PixelClass> bucket, Bitmap bitmap)
        {
            if(depth == 0)
            {
               quantize(bucket, bitmap);
               return;
            }
            //Find max range
            int Rmin = 255;
            int Rmax = 0;
            int Gmin = 255;
            int Gmax = 0;
            int Bmin = 255;
            int Bmax = 0;
            foreach(PixelClass item in bucket)
            {
                if(Rmin > item.pixel.R)
                {
                    Rmin = item.pixel.R;
                }
                if (Rmax < item.pixel.R)
                {
                    Rmax = item.pixel.R;
                }
                if (Gmin > item.pixel.G)
                {
                    Gmin = item.pixel.G;
                }
                if (Gmax < item.pixel.G)
                {
                    Gmax = item.pixel.G;
                }
                if (Bmin > item.pixel.B)
                {
                    Bmin = item.pixel.B;
                }
                if (Bmax < item.pixel.B)
                {
                    Bmin = item.pixel.B;
                }
            }
            // 1-red 2-green 3-blue
            int maxRangeColor;

            if(Bmax - Bmin >= Gmax - Gmin && Bmax - Bmin >= Rmax - Rmin)
            {
                maxRangeColor = 3;
            }
            else if(Gmax - Gmin > Rmax - Rmin)
            {
                maxRangeColor = 2;
            }
            else
            {
                maxRangeColor = 1;
            }
            //Sort the bucket
            if(maxRangeColor == 1)
            {
                bucket = bucket.OrderBy(item => item.pixel.R).ToList<PixelClass>();
            }
            else if(maxRangeColor == 2)
            {
                bucket = bucket.OrderBy(item => item.pixel.G).ToList<PixelClass>();
            }
            else
            {
                bucket = bucket.OrderBy(item => item.pixel.B).ToList<PixelClass>();
            }
            //Split the bucket
            List<PixelClass> bucket1 = new List<PixelClass>();
            List<PixelClass> bucket2 = new List<PixelClass>();
            for(int i = 0; i < bucket.Count/2; i++)
            {
                bucket1.Add(bucket[i]);
            }
            for(int i = bucket.Count/2; i < bucket.Count; i++)
            {
                bucket2.Add(bucket[i]);
            }
            //Call recursively
            splitBuckets(depth - 1, bucket1, bitmap);
            splitBuckets(depth - 1, bucket2, bitmap);
        }
        private void quantize(List<PixelClass> bucket, Bitmap bitmap)
        {
            int Rsum = 0;
            int Gsum = 0;
            int Bsum = 0;
            foreach(PixelClass item in bucket)
            {
                Rsum += item.pixel.R;
                Gsum += item.pixel.G;
                Bsum += item.pixel.B;
            }
            int Ravg = Rsum / bucket.Count;
            int Gavg = Gsum / bucket.Count;
            int Bavg = Bsum / bucket.Count;
            foreach(PixelClass item in bucket)
            {
                Color newPixel = Color.FromArgb(bitmap.GetPixel(item.width, item.height).A, Ravg, Gavg, Bavg);
                bitmap.SetPixel(item.width, item.height, newPixel);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace lab1v2
{
    class PixelClass
    {
        public Color pixel;
        public int width;
        public int height;
        public PixelClass(Color pixel, int width, int height)
        {
            this.pixel = pixel;
            this.width = width;
            this.height = height;
        }
    }
}

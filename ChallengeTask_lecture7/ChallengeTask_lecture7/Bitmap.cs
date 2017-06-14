using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;


namespace ChallengeTask_lecture7
{

    
        public class Bitmap
        {
            private PixelFormat format16bppRgb555;
            private double height;
            private double width;

            public Bitmap(double width, double height, PixelFormat format16bppRgb555)
            {
                this.width = width;
                this.height = height;
                this.format16bppRgb555 = format16bppRgb555;
            }
        }
    }

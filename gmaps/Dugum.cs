using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmaps
{
    public class Dugum
    {
        public char bilgi;
        public Dugum guneyDogu;
        public Dugum guneyBati;
        public Dugum kuzeyBati;
        public Dugum kuzeyDogu;
        public Dugum ata;
        public int xMin, yMin, xMax, yMax;
        public int x, y;

        public Dugum(int x, int y, char bilgi, Dugum ata, int xMin, int yMin, int xMax, int yMax)
        {
            this.x = x;
            this.y = y;
            this.bilgi = bilgi;
            this.ata = ata;
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;

        }
    }
}

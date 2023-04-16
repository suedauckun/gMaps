using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace gmaps
{
    class Agac
    {
        public Dugum kok;
        public Dugum eklenecekDugum = null;

        public void ekle(Dugum n, Dugum ata, int x, int y, int bilgi)
        {
            //Dugum eklenecekDugum = null;
            if (ata == null)
            {
                eklenecekDugum = new Dugum(x, y, 'A', null, 0, 0, 512, 512);
                kok = eklenecekDugum;

            }
            else if (n.x > ata.x && n.y > ata.y)
            {
                eklenecekDugum = new Dugum(n.x, n.y, (char)bilgi, ata, ata.x, ata.y, ata.xMax, ata.yMax);
                ata.guneyDogu = eklenecekDugum;

            }
            else if (n.x < ata.x && n.y > ata.y)
            {
                eklenecekDugum = new Dugum(n.x, n.y, (char)bilgi, ata, ata.xMin, ata.y, ata.x, ata.yMax);
                ata.guneyBati = eklenecekDugum;

            }
            else if (n.x < ata.x && n.y < ata.y)
            {
                eklenecekDugum = new Dugum(n.x, n.y, (char)bilgi, ata, ata.xMin, ata.yMin, ata.x, ata.y);
                ata.kuzeyBati = eklenecekDugum;

            }
            else if (n.x > ata.x && n.y < ata.y)
            {
                eklenecekDugum = new Dugum(n.x, n.y, (char)bilgi, ata, ata.x, ata.yMin, ata.xMax, ata.y);
                ata.kuzeyDogu = eklenecekDugum;

            }
        }

        public Dugum AtayıBul(int x, int y)
        {
            Dugum simdikiDugum = kok;
            while (simdikiDugum != null)
            {
                if (x > simdikiDugum.x && y > simdikiDugum.y)
                {
                    if (simdikiDugum.guneyDogu == null)
                    {
                        break;
                    }
                    simdikiDugum = simdikiDugum.guneyDogu;

                }
                else if (x < simdikiDugum.x && y > simdikiDugum.y)
                {
                    if (simdikiDugum.guneyBati == null)
                    {
                        break;
                    }
                    simdikiDugum = simdikiDugum.guneyBati;

                }
                else if (x < simdikiDugum.x && y < simdikiDugum.y)
                {
                    if (simdikiDugum.kuzeyBati == null)
                    {
                        break;
                    }
                    simdikiDugum = simdikiDugum.kuzeyBati;

                }
                else if (x > simdikiDugum.x && y < simdikiDugum.y)
                {
                    if (simdikiDugum.kuzeyDogu == null)
                    {
                        break;
                    }
                    simdikiDugum = simdikiDugum.kuzeyDogu;

                }
            }
            return simdikiDugum;
        }
    }
}

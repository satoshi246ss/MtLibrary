using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtLibrary
{
    public class common
    {
        //---------------------------------------------------------------------------
        //  地平座標系の角距離の計算
        //  IN  2組のaz,alt [deg]
        //  OUT 角距離 [deg]
        // 2004/2/20 作成
        //
        public double Cal_Distance(double az1, double alt1, double az2, double alt2)
        {
            double path_length;
            double r1, u1, v1, r2, u2, v2, pl2;
            double azrad, altrad;
            double RAD = Math.PI / 180.0;

            // 経路長[deg]
            azrad = az1 * RAD;
            altrad = alt1 * RAD;
            r1 = Math.Cos(azrad) * Math.Cos(altrad);
            u1 = Math.Sin(azrad) * Math.Cos(altrad);
            v1 = Math.Sin(altrad);

            azrad = az2 * RAD;
            altrad = alt2 * RAD;
            r2 = Math.Cos(azrad) * Math.Cos(altrad);
            u2 = Math.Sin(azrad) * Math.Cos(altrad);
            v2 = Math.Sin(altrad);

            pl2 = r1 * r2 + u1 * u2 + v1 * v2;
            if (pl2 > 1) pl2 = 1;
            if (pl2 < -1) pl2 = -1;
            //    path_length = acos( pl2 )/RAD ;

            path_length = Math.Acos(pl2);
            path_length /= RAD;

            return (path_length);
        }
    }
}

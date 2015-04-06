using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MtLibrary
{
    /********************************************/
    /***                                      ***/
    /***  惑星の位置計算をするためのヘッダー  ***/
    /***        Last Update 24 July 2002      ***/
    /***                    by SAKURAI        ***/
    /***                                      ***/
    /********************************************/
    public class Planet
    {
        //---------------------------------------------------------------------------
        // ベッセル年
        // In:時刻 t(JST)
        /***   1900年0月0日0時ETからの経過時間を　約36525日単位で示したもの     ***/
        //---------------------------------------------------------------------------
        public static double t_bessel_JST(DateTime t_jst)
        {
            DateTime t_ut = t_jst.AddHours(-9);
            return t_bessel_UT(t_ut);
        }
        public static double t_bessel_UT(DateTime t_ut)
        {
            DateTime te0 = new DateTime(1899, 12, 31, 12, 00, 00); //UTH
            double te_mjd0 = JulianDay.DateTimeToModifiedJulianDay(te0);
            double N=t_ut.Year-1900;
            N = 50;
            int g = ((int)N-1) % 4 ;
            double t50 = 1.06351646 - 0.0078012183 * N - 0.000000039270 * N * N + g / 4.0;
            //MJD
            double mjd = JulianDay.DateTimeToModifiedJulianDay(t_ut);
             
            // 文字列から DateTime の値に変換する
            DateTime dtBirth = new DateTime(1949, 12, 31, 00, 00, 00); //UTH
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth) + t50 ;

            double ans = (mjd - mjd0) / 36524.2194;  // JSTにするため、時差9hを引く 9/24h=0.375
            return ans;
        }
        //---------------------------------------------------------------------------
        // 時刻引数のラッパ
        // In:時刻 t(JST)
        /***   1975年1月0日0時ET(1974/12/31 00:00:00 ET)からの経過時間を　365.25日単位で示したもの     ***/
        //---------------------------------------------------------------------------
        public static double planet_time_jst_datetime(DateTime t_jst)
        {
            //MJD
            double mjd = JulianDay.DateTimeToModifiedJulianDay(t_jst);

            // 文字列から DateTime の値に変換する
            DateTime dtBirth = new DateTime(1974,12,31,00,00,00); //UT "1974/12/31 00:00:00" - 9H
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            double ans = (mjd - (mjd0+0.375)) / 365.25;  // JSTにするため、時差9hを引く 9/24h=0.375
            return ans;
        }
        public static double planet_time_to_mjd(double pt)
        {
            //planet time
            //  1975年1月0日0時ET(1974/12/31 00:00:00 ET)からの経過時間を　365.25日単位で示したもの     
            double pt_mdj0 = planet_time(1974, 12, 31, 0, 0, 0);
            double dpt = pt - pt_mdj0;

            DateTime dtBirth = new DateTime(1974, 12, 31, 00, 00, 00); //UT "1974/12/31 00:00:00" - 9H
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            double ans = mjd0 + dpt * 365.25; // (mjd - (mjd0 + 0.375)) / 365.25;  // JSTにするため、時差9hを引く 9/24h=0.375
            return ans;
        }

        public static double planet_time_mjd(double pt)
        {
            //planet time
            //1975年1月0日0時ET(1974/12/31 00:00:00 ET)からの経過時間を　365.25日単位で示したもの     
            DateTime dtBirth = new DateTime(1974, 12, 31, 00, 00, 00); //UT "1974/12/31 00:00:00" - 9H
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            double ans = 365.25 * pt + mjd0 ; 
            return ans;
        }
        /************************************************************/
        /*** 惑星（太陽・月も）の位置計算に必要な時刻引数を求める ***/
        /***   ただし、ＵＴ(JST-9H)を代入すること                 ***/
        /***   1975年1月0日0時ET(1974/12/31 00:00:00 ET)からの経過時間を　365.25日単位で示したもの     ***/
        /************************************************************/
        public static double planet_time(int year, int month, int day, double hour_ut, double min, double sec)
        {
            double W, F, Z, J, t, T;
            int X, R, S;
            long Y;

            //経過日数を求める準備
            W = ((double)year - 1900) / 4;
            F = W - Math.Floor(W);	//Wの小数部分をとりだす
            Y = (long)(1461 * W);
            X = (int)(((double)month + 7) / 10);
            R = (int)(1 - F);
            S = (int)(0.44 * ((double)month + 4.4));
            //観測日の０時までの経過日数ｚ
            Z = (double)Y + 31 * (double)month + (double)day + ((double)X - 1) * (double)R - (double)X * (double)S - 27424;
            //観測日の端数ｊ
            J = hour_ut / 24 + min / (24 * 60) + sec / (24 * 60 * 60);
            //世界時で表現した経過時間ｔ
            t = (Z + J) / 365.25;
            //歴表時による表現Ｔ
            T = t + (0.0317 * t + 1.43) * Math.Pow(10, -6);

            //値を返す
            return (T);
        }

        /*************************************************************/
        /*** 太陽の幾何学的黄経λｓ[deg]、地心距離ｒｓ[AU]を求める ***/
        /***   Ｔ：時刻引数                                        ***/
        /***   長沢　工「天体の位置計算」　Ｐ．２０５参照          ***/
        /*************************************************************/
        public static void sun(double T, out double lambda_s, out double r_s)
        {
            //必要なパラメータを求める
            double q;
            double PI = Math.PI;

            //太陽の視黄経λｓ’[deg]
            lambda_s = 279.0358 + 360.00769 * T
                    + (1.9159 - 0.00005 * T) * Math.Sin((356.531 + 359.991 * T) * PI / 180)
                    + 0.0200 * Math.Sin((353.06 + 719.981 * T) * PI / 180)
                    - 0.0048 * Math.Sin((248.64 - 19.341 * T) * PI / 180)
                    + 0.0020 * Math.Sin((258.0 + 329.64 * T) * PI / 180)
                    + 0.0018 * Math.Sin((334.2 - 4452.67 * T) * PI / 180)

                    + 0.0018 * Math.Sin((293.7 - 0.20 * T) * PI / 180)
                    + 0.0015 * Math.Sin((242.4 + 450.37 * T) * PI / 180)
                    + 0.0013 * Math.Sin((211.1 + 225.18 * T) * PI / 180)
                    + 0.0008 * Math.Sin((208.0 + 659.29 * T) * PI / 180)
                    + 0.0007 * Math.Sin((53.5 + 90.38 * T) * PI / 180)

                    + 0.0007 * Math.Sin((12.1 - 30.35 * T) * PI / 180)
                    + 0.0006 * Math.Sin((239.1 + 337.18 * T) * PI / 180)
                    + 0.0005 * Math.Sin((10.1 - 1.50 * T) * PI / 180)
                    + 0.0005 * Math.Sin((99.1 - 22.81 * T) * PI / 180)
                    + 0.0004 * Math.Sin((294.8 + 315.56 * T) * PI / 180)

                    + 0.0004 * Math.Sin((233.8 + 299.30 * T) * PI / 180)
                    - 0.0004 * Math.Sin((198.1 + 720.02 * T) * PI / 180)
                    + 0.0003 * Math.Sin((349.6 + 1079.97 * T) * PI / 180)
                    + 0.0003 * Math.Sin((241.2 - 44.43 * T) * PI / 180);

            //太陽の幾何学的黄経[deg]に直す
            lambda_s += 0.0057;
            //λｓを０から３６０度の範囲に
            for (; lambda_s < 0 || lambda_s > 360; )
            {
                if (lambda_s > 360) { lambda_s -= 360; }
                else if (lambda_s < 0) { lambda_s += 360; }
            }

            //地心距離を求める準備
            q = (-0.007261 + 0.0000002 * T) * Math.Cos((356.53 + 359.991 * T) * PI / 180) + 0.000030
                - 0.000091 * Math.Cos((353.1 + 719.98 * T) * PI / 180)
                + 0.000013 * Math.Cos((205.8 + 4452.67 * T) * PI / 180)
                + 0.000007 * Math.Cos((62 + 450.4 * T) * PI / 180)
                + 0.000007 * Math.Cos((105 + 329.6 * T) * PI / 180);
            //地心距離[AU]
            r_s = Math.Pow(10, q);
        }

        /************************************************************/
        /*** 日心黄道座標から黄道座標への変換                     ***/
        /***   時刻引数Ｔ                                         ***/
        /***   惑星の日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU] ***/
        /***   地球から見た惑星の黄経λ[deg]、黄緯β[deg]         ***/
        /***   長沢　工「天体の位置計算」　Ｐ．２１４参照         ***/
        /************************************************************/
        public static void geocentric(double T, double lambda, double B, double r, out double lam, out double beta)
        {
            double lambda_s, r_s;	//太陽の幾何学的黄経λｓ[deg]、地心距離ｒｓ[AU]
            double Ac, Bc, Cc;		//地心黄道直交座標系による惑星の位置
            double R;				//地心から惑星までの距離Ｒ
            double PI = Math.PI;

            //太陽の幾何学的黄経λｓ[deg]、地心距離ｒｓ[AU]を求める
            sun(T, out lambda_s, out r_s);

            //地心黄道直交座標系による惑星の位置
            Ac = r * Math.Cos(B * PI / 180) * Math.Cos(lambda * PI / 180) + r_s * Math.Cos(lambda_s * PI / 180);
            Bc = r * Math.Cos(B * PI / 180) * Math.Sin(lambda * PI / 180) + r_s * Math.Sin(lambda_s * PI / 180);
            Cc = r * Math.Sin(B * PI / 180);

            //地心から惑星までの距離Ｒ
            R = Math.Sqrt(Ac * Ac + Bc * Bc + Cc * Cc);

            //黄経λ[deg]を求める
            lam = Math.Atan(Bc / Ac);	//ここではrad単位
            //λは第Ⅱ，第Ⅲ象限にある
            if (Ac < 0) { lam += PI; }
            //λを０から２πの範囲に入れる
            if (lam > 2 * PI) { lam -= 2 * PI; }
            else if (lam < 0) { lam += 2 * PI; }
            //λをdeg単位に直す
            lam *= 180 / PI;
            /*****　このあと惑星光行差による補正が必要　*****/
            /*****各惑星の計算をするときに補正をするべし*****/


            //黄緯β[deg]を求める
            beta = Math.Asin(Cc / R);	//ここではrad単位
            //βをdeg単位に直す
            beta *= 180 / PI;
        }


        //黄道傾角ε[deg]を求める
        //Ｔ：時間引数
        public static double obliquity_of_the_ecliptic(double T)
        {
            double epsilon;
            double PI = Math.PI;
            epsilon = 23.44253 - 0.00013 * T
                    + 0.00256 * Math.Cos((249 - 19.3 * T) * PI / 180)
                    + 0.00015 * Math.Cos((198 + 720 * T) * PI / 180);
            return (epsilon);
        }

        /*************************************************************/
        /*** 月の視黄経λｓ[deg]、視黄緯βｓ[deg]、地心距離ｒｓ[km]を求める ***/
        /***   Ｔ：時刻引数                                        ***/
        /***   長沢　工「天体の位置計算」　Ｐ．２０５参照          ***/
        /*************************************************************/
        public static void moon(double T, out double lambda_s, out double beta_s, out double r_s)
        {
            //必要なパラメータを求める
            double RAD = Math.PI / 180.0;

            //【7-2表】月の位置の略算式（海上保安庁水路部による）
            //月の視黄経λｓ’[deg]
            double A = 0.0040 * Math.Sin((93.8 - 1.33 * T) * RAD)
                     + 0.0020 * Math.Sin((248.6 - 19.34 * T) * RAD)
                     + 0.0006 * Math.Sin((66 + 0.2 * T) * RAD)
                     + 0.0006 * Math.Sin((249 - 19.3 * T) * RAD);

            lambda_s = 124.8754 + 4812.67881 * T
+ 6.2887 * Math.Sin((338.915 + 4771.9886 * T + A) * RAD)
+ 1.2740 * Math.Sin((107.248 - 4133.3536 * T) * RAD)
+ 0.6583 * Math.Sin((51.668 + 8905.3422 * T) * RAD)
+ 0.2136 * Math.Sin((317.831 + 9543.9773 * T) * RAD)
+ 0.1856 * Math.Sin((176.531 + 359.9905 * T) * RAD)
+ 0.1143 * Math.Sin((292.463 + 9664.0404 * T) * RAD)
+ 0.0588 * Math.Sin((86.16 + 638.635 * T) * RAD)
+ 0.0572 * Math.Sin((103.78 - 3773.363 * T) * RAD)
+ 0.0533 * Math.Sin((30.58 + 13677.331 * T) * RAD)
+ 0.0459 * Math.Sin((124.86 - 8545.352 * T) * RAD)
+ 0.0410 * Math.Sin((342.38 + 4411.998 * T) * RAD)
+ 0.0348 * Math.Sin((25.83 + 4452.671 * T) * RAD)
+ 0.0305 * Math.Sin((155.45 + 5131.979 * T) * RAD)
+ 0.0153 * Math.Sin((240.79 + 758.698 * T) * RAD)
+ 0.0125 * Math.Sin((271.38 + 14436.029 * T) * RAD)
+ 0.0110 * Math.Sin((226.45 - 4892.052 * T) * RAD)
+ 0.0107 * Math.Sin((55.58 - 13038.696 * T) * RAD)
+ 0.0100 * Math.Sin((296.75 + 14315.966 * T) * RAD)
+ 0.0085 * Math.Sin((34.5 - 8266.71 * T) * RAD)
+ 0.0079 * Math.Sin((290.7 - 4493.34 * T) * RAD)
+ 0.0068 * Math.Sin((228.2 + 9265.33 * T) * RAD)
+ 0.0052 * Math.Sin((133.1 + 319.32 * T) * RAD)
+ 0.0050 * Math.Sin((202.4 + 4812.66 * T) * RAD)
+ 0.0048 * Math.Sin((68.6 - 19.34 * T) * RAD)
+ 0.0040 * Math.Sin((34.1 + 13317.34 * T) * RAD)
+ 0.0040 * Math.Sin((9.5 + 18449.32 * T) * RAD)
+ 0.0040 * Math.Sin((93.8 - 1.33 * T) * RAD)
+ 0.0039 * Math.Sin((103.3 + 17810.68 * T) * RAD)
+ 0.0037 * Math.Sin((65.1 + 5410.62 * T) * RAD)
+ 0.0027 * Math.Sin((321.3 + 9183.99 * T) * RAD)
+ 0.0026 * Math.Sin((174.8 - 13797.39 * T) * RAD)
+ 0.0024 * Math.Sin((82.7 + 998.63 * T) * RAD)
+ 0.0024 * Math.Sin((4.7 + 9224.66 * T) * RAD)
+ 0.0022 * Math.Sin((121.4 - 8185.36 * T) * RAD)
+ 0.0021 * Math.Sin((134.4 + 9903.97 * T) * RAD)
+ 0.0021 * Math.Sin((173.1 + 719.98 * T) * RAD)
+ 0.0021 * Math.Sin((100.3 - 3413.37 * T) * RAD)
+ 0.0020 * Math.Sin((248.6 - 19.34 * T) * RAD)
+ 0.0018 * Math.Sin((98.1 + 4013.29 * T) * RAD)
+ 0.0016 * Math.Sin((344.1 + 18569.38 * T) * RAD)
+ 0.0012 * Math.Sin((52.1 - 12678.71 * T) * RAD)
+ 0.0011 * Math.Sin((250.3 + 19208.02 * T) * RAD)
+ 0.0009 * Math.Sin((81 - 8586.0 * T) * RAD)
+ 0.0008 * Math.Sin((207 + 14037.3 * T) * RAD)
+ 0.0008 * Math.Sin((31 - 7906.7 * T) * RAD)
+ 0.0007 * Math.Sin((346 + 4052.0 * T) * RAD)
+ 0.0007 * Math.Sin((294 - 4853.3 * T) * RAD)
+ 0.0007 * Math.Sin((90 + 278.6 * T) * RAD)
+ 0.0006 * Math.Sin((237 + 1118.7 * T) * RAD)
+ 0.0005 * Math.Sin((82 + 22582.7 * T) * RAD)
+ 0.0005 * Math.Sin((276 + 19088.0 * T) * RAD)
+ 0.0005 * Math.Sin((73 - 17450.7 * T) * RAD)
+ 0.0005 * Math.Sin((112 + 5091.3 * T) * RAD)
+ 0.0004 * Math.Sin((116 - 398.7 * T) * RAD)
+ 0.0004 * Math.Sin((25 - 120.1 * T) * RAD)
+ 0.0004 * Math.Sin((181 + 9584.7 * T) * RAD)
+ 0.0004 * Math.Sin((18 + 720.0 * T) * RAD)
+ 0.0003 * Math.Sin((60 - 3814.0 * T) * RAD)
+ 0.0003 * Math.Sin((13 - 3494.7 * T) * RAD)
+ 0.0003 * Math.Sin((13 + 18089.3 * T) * RAD)
+ 0.0003 * Math.Sin((152 + 5492.0 * T) * RAD)
+ 0.0003 * Math.Sin((317 - 40.7 * T) * RAD)
+ 0.0003 * Math.Sin((348 + 23221.3 * T) * RAD);

            //月の視黄緯β[deg]
            double B = 0.0267 * Math.Sin((68.64 - 19.341 * T) * RAD)
            + 0.0043 * Math.Sin((342.0 - 19.36 * T) * RAD)
            + 0.0040 * Math.Sin((93.8 - 1.33 * T) * RAD)
            + 0.0020 * Math.Sin((248.6 - 19.34 * T) * RAD)
            + 0.0005 * Math.Sin((358 - 19.4 * T) * RAD);

            beta_s = 5.1282 * Math.Sin((236.231 + 4832.0202 * T + B) * RAD)
                + 0.2806 * Math.Sin((215.147 + 9604.0088 * T) * RAD)
+ 0.2777 * Math.Sin((77.316 + 60.0316 * T) * RAD)
+ 0.1732 * Math.Sin((4.563 - 4073.3220 * T) * RAD)
+ 0.0554 * Math.Sin((308.98 + 8965.374 * T) * RAD)
+ 0.0463 * Math.Sin((343.48 + 698.667 * T) * RAD)
+ 0.0326 * Math.Sin((287.90 + 13737.362 * T) * RAD)
+ 0.0172 * Math.Sin((194.06 + 14375.997 * T) * RAD)
+ 0.0093 * Math.Sin((25.6 - 8845.31 * T) * RAD)
+ 0.0088 * Math.Sin((98.4 - 4711.96 * T) * RAD)
+ 0.0082 * Math.Sin((1.1 - 3713.33 * T) * RAD)
+ 0.0043 * Math.Sin((322.4 + 5470.66 * T) * RAD)
+ 0.0042 * Math.Sin((266.8 + 18509.35 * T) * RAD)
+ 0.0034 * Math.Sin((188.0 - 4433.31 * T) * RAD)
+ 0.0025 * Math.Sin((312.5 + 8605.38 * T) * RAD)
+ 0.0022 * Math.Sin((291.4 + 13377.37 * T) * RAD)
+ 0.0021 * Math.Sin((340.0 + 1058.66 * T) * RAD)
+ 0.0019 * Math.Sin((218.6 + 9244.02 * T) * RAD)
+ 0.0018 * Math.Sin((291.8 - 8206.68 * T) * RAD)
+ 0.0018 * Math.Sin((52.8 + 5192.01 * T) * RAD)
+ 0.0017 * Math.Sin((168.7 + 14496.06 * T) * RAD)
+ 0.0016 * Math.Sin((73.8 + 420.02 * T) * RAD)
+ 0.0015 * Math.Sin((262.1 + 9284.69 * T) * RAD)
+ 0.0015 * Math.Sin((31.7 + 9964.00 * T) * RAD)
+ 0.0014 * Math.Sin((260.8 - 299.96 * T) * RAD)
+ 0.0013 * Math.Sin((239.7 + 4472.03 * T) * RAD)
+ 0.0013 * Math.Sin((30.4 + 379.35 * T) * RAD)
+ 0.0012 * Math.Sin((304.9 + 4812.68 * T) * RAD)
+ 0.0012 * Math.Sin((12.4 - 4851.36 * T) * RAD)
+ 0.0011 * Math.Sin((173.0 + 19147.99 * T) * RAD)
+ 0.0010 * Math.Sin((312.9 - 12978.66 * T) * RAD)
+ 0.0008 * Math.Sin((1 + 17870.7 * T) * RAD)
+ 0.0008 * Math.Sin((190 + 9724.1 * T) * RAD)
+ 0.0007 * Math.Sin((22 + 13098.7 * T) * RAD)
+ 0.0006 * Math.Sin((117 + 5590.7 * T) * RAD)
+ 0.0006 * Math.Sin((47 - 13617.3 * T) * RAD)
+ 0.0005 * Math.Sin((22 - 8485.3 * T) * RAD)
+ 0.0005 * Math.Sin((150 + 4193.4 * T) * RAD)
+ 0.0004 * Math.Sin((11 - 9483.9 * T) * RAD)
+ 0.0004 * Math.Sin((246 + 23281.3 * T) * RAD)
+ 0.0004 * Math.Sin((30 + 10242.6 * T) * RAD)
+ 0.0004 * Math.Sin((126 + 9325.4 * T) * RAD)
+ 0.0004 * Math.Sin((104 + 14097.4 * T) * RAD)
+ 0.0003 * Math.Sin((340 + 22642.7 * T) * RAD)
+ 0.0003 * Math.Sin((270 + 18149.4 * T) * RAD)
+ 0.0003 * Math.Sin((358 - 3353.3 * T) * RAD)
+ 0.0003 * Math.Sin((148 + 19268.0 * T) * RAD);

            double sinpi = 0.9507
            + 0.0518 * Math.Cos((338.92 + 4771.989 * T) * RAD)
            + 0.0095 * Math.Cos((287.2 - 4133.35 * T) * RAD)
            + 0.0078 * Math.Cos((51.7 + 8905.34 * T) * RAD)
            + 0.0028 * Math.Cos((317.8 + 9543.98 * T) * RAD)
            + 0.0009 * Math.Cos((31 + 13677.3 * T) * RAD)
            + 0.0005 * Math.Cos((305 - 8545.4 * T) * RAD)
            + 0.0004 * Math.Cos((284 - 3773.4 * T) * RAD)
            + 0.0003 * Math.Cos((342 + 4412.0 * T) * RAD);

            double a_earth = 6378.140; //[km]
            r_s = a_earth / ( sinpi * RAD ) ;

            lambda_s = lambda_s % 360;
            beta_s = beta_s % 360;
        }
        

        /****************************************/
        /*** 土星の位置を計算する             ***/
        /***   Ｔ：時刻引数                   ***/
        /***   α：赤経[deg]    δ：赤緯[deg] ***/
        /****************************************/
        public static void saturn(double T, out double alpha, out double delta)
        {
            double lambda, B, r;	//日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            double N, f, V, q_, r_;	//必要なパラメータ[deg]
            double lam, beta;	///黄道座標λ[deg]，β[deg]
            double d_lam, lambda_s, r_s;	//惑星光行差による補正
            double epsilon;		//黄道傾角ε[deg]
            double PI = Math.PI;


            /*****日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]を求める*****/
            //必要なパラメータを求める[deg]
            //Ｎ
            N = 12.3042 + 12.22117 * T
                + (0.0934 + 0.00075 * T) * Math.Sin((250.29 + 12.221 * T) * PI / 180) + 0.0008
                + (0.0057 + 0.00005 * T) * Math.Sin((265.8 - 11.81 * T) * PI / 180)
                + (0.0049 + 0.00004 * T) * Math.Sin((162.7 + 0.38 * T) * PI / 180)
                + (0.0019 + 0.00002 * T) * Math.Sin((262.0 + 24.44 * T) * PI / 180)
                + (0.8081) * Math.Sin((342.74 + 0.385 * T) * PI / 180)

                + (0.1900) * Math.Sin((3.57 - 11.813 * T) * PI / 180)
                + (0.1173) * Math.Sin((224.52 - 5.907 * T) * PI / 180)
                + (0.0093) * Math.Sin((176.6 + 6.31 * T) * PI / 180)
                + (0.0089) * Math.Sin((218.5 - 36.26 * T) * PI / 180)
                + (0.0080) * Math.Sin((10.4 - 0.23 * T) * PI / 180)

                + (0.0078) * Math.Sin((56.8 + 0.63 * T) * PI / 180)
                + (0.0074) * Math.Sin((325.4 + 0.77 * T) * PI / 180)
                + (0.0073) * Math.Sin((209.4 - 24.03 * T) * PI / 180)
                + (0.0064) * Math.Sin((202.0 - 11.59 * T) * PI / 180)
                - (0.0048) * Math.Sin((248.6 - 19.34 * T) * PI / 180)

                + (0.0034) * Math.Sin((105.2 - 30.35 * T) * PI / 180)
                + (0.0034) * Math.Sin((23.6 - 15.87 * T) * PI / 180)
                + (0.0025) * Math.Sin((348.4 - 11.41 * T) * PI / 180)
                + (0.0022) * Math.Sin((102.5 - 7.94 * T) * PI / 180)
                + (0.0021) * Math.Sin((53.5 - 3.65 * T) * PI / 180)

                + (0.0020) * Math.Sin((220.4 - 18.13 * T) * PI / 180)
                + (0.0018) * Math.Sin((326.7 - 54.38 * T) * PI / 180)
                + (0.0017) * Math.Sin((173.0 - 5.50 * T) * PI / 180)
                + (0.0014) * Math.Sin((165.5 - 5.91 * T) * PI / 180)
                + (0.0013) * Math.Sin((307.9 - 42.16 * T) * PI / 180)

                + (0.0009) * Math.Sin((292 - 29.9 * T) * PI / 180)
                + (0.0009) * Math.Sin((287 - 17.7 * T) * PI / 180)
                + (0.0008) * Math.Sin((299 - 48.5 * T) * PI / 180)
                + (0.0007) * Math.Sin((146 + 24.4 * T) * PI / 180)
                + (0.0007) * Math.Sin((155 + 12.2 * T) * PI / 180)

                + (0.0007) * Math.Sin((237 + 12.6 * T) * PI / 180)
                + (0.0005) * Math.Sin((199.7 - 12.4 * T) * PI / 180)
                + (0.0005) * Math.Sin((146 - 10.0 * T) * PI / 180)
                + (0.0005) * Math.Sin((6 + 12.6 * T) * PI / 180)
                + (0.0005) * Math.Sin((75 - 72.5 * T) * PI / 180)

                + (0.0004) * Math.Sin((57 - 60.3 * T) * PI / 180)
                + (0.0004) * Math.Sin((137 - 23.8 * T) * PI / 180)
                + (0.0004) * Math.Sin((187 - 23.6 * T) * PI / 180)
                - (0.0004) * Math.Sin((198 + 720.0 * T) * PI / 180)
                + (0.0003) * Math.Sin((255 - 0.2 * T) * PI / 180)

                + (0.0003) * Math.Sin((202 - 7.3 * T) * PI / 180)
                + (0.0003) * Math.Sin((182 + 4.3 * T) * PI / 180)
                + (0.0003) * Math.Sin((122 - 7.9 * T) * PI / 180)
                + (0.0003) * Math.Sin((87 + 6.3 * T) * PI / 180)
                + (0.0003) * Math.Sin((116 - 24.0 * T) * PI / 180)

                + (0.0003) * Math.Sin((111 - 20.1 * T) * PI / 180);

            //ｆ
            f = N + 6.4215 * Math.Sin(N * PI / 180) + 0.2248 * Math.Sin(2 * N * PI / 180)
                + 0.0109 * Math.Sin(3 * N * PI / 180) + 0.0006 * Math.Sin(4 * N * PI / 180);
            //Ｖ
            V = 0.0272 * Math.Sin((2 * f + 135.53) * PI / 180);
            //日心黄経Λ
            lambda = f + V + 91.8560 + 0.01396 * T;
            //日心黄緯Ｂ
            B = 180 / PI *  Math.Asin(0.043519 * Math.Sin((f + 337.763) * PI / 180))	//deg単位にしておく
                + (0.0286 + 0.00023 * T) * Math.Sin((f + 77.06) * PI / 180)
                + (0.0024) * Math.Sin((3.9 - 11.81 * T) * PI / 180)
                + (0.0008) * Math.Sin((269 - 5.9 * T) * PI / 180)
                + (0.0005) * Math.Sin((135 - 30.3 * T) * PI / 180);
            //ｑ’
            q_ = (0.000354 + 0.0000028 * T) *  Math.Cos((70.28 + 12.22 * T) * PI / 180) + 0.000183
                + (0.000021 + 0.0000002 * T) *  Math.Cos((265.8 - 11.81 * T) * PI / 180)
                + (0.000701) *  Math.Cos((3.43 - 11.813 * T) * PI / 180)
                + (0.000378) *  Math.Cos((110.54 - 18.128 * T) * PI / 180)
                + (0.000244) *  Math.Cos((219.13 - 5.907 * T) * PI / 180)

                + (0.000114) *  Math.Cos((158.22 + 0.383 * T) * PI / 180)
                + (0.000064) *  Math.Cos((218.1 - 36.26 * T) * PI / 180)
                + (0.000042) *  Math.Cos((215.8 - 24.03 * T) * PI / 180)
                + (0.000024) *  Math.Cos((201.8 - 11.59 * T) * PI / 180)
                + (0.000024) *  Math.Cos((1.3 + 6.31 * T) * PI / 180)

                + (0.000019) *  Math.Cos((307.7 + 12.22 * T) * PI / 180)
                + (0.000015) *  Math.Cos((326.3 - 54.38 * T) * PI / 180)
                + (0.000010) *  Math.Cos((311.1 - 42.16 * T) * PI / 180)
                + (0.000010) *  Math.Cos((83.2 + 24.44 * T) * PI / 180)
                + (0.000009) *  Math.Cos((348 - 11.4 * T) * PI / 180)

                + (0.000008) *  Math.Cos((129 - 30.3 * T) * PI / 180)
                + (0.000006) *  Math.Cos((295 - 29.9 * T) * PI / 180)
                + (0.000006) *  Math.Cos((148 - 48.5 * T) * PI / 180)
                + (0.000006) *  Math.Cos((103 - 7.9 * T) * PI / 180)
                + (0.000005) *  Math.Cos((318 + 24.4 * T) * PI / 180)

                + (0.000005) *  Math.Cos((24 - 15.9 * T) * PI / 180);

            //ｒ’
            r_ = Math.Pow(10, q_);
            //動径ｒ
            r = r_ * 9.508863 / (1 + 0.056061 *  Math.Cos(f * PI / 180));


            /*****黄道座標λ[deg]，β[deg]へ変換する*****/
            geocentric(T, lambda, B, r, out lam, out beta);
            //惑星光行差による補正
            sun(T, out lambda_s, out r_s);
            //補正項Δλを求める
            d_lam = -0.0057 / r_s *  Math.Cos((lam - lambda_s) * PI / 180)
                    - 0.0174 / r *  Math.Cos((lam - lambda) * PI / 180);
            //実際に補正を行う
            lam += d_lam;


            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = ( Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) *  Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / ( Math.Cos(beta * PI / 180) *  Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if ( Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta =  Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) *  Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta =  Math.Asin(delta);

            alpha /= (Math.PI / 180.0);
            delta /= (Math.PI / 180.0);
        }


        /****************************************/
        /*** 木星の位置を計算する             ***/
        /***   Ｔ：時刻引数                   ***/
        /***   α：赤経[deg]    δ：赤緯[deg] ***/
        /****************************************/
        public static void jupiter(double T, out double alpha, out double delta)
        {
            double lambda, B, r;	//日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            double N, f, V, q_, r_;	//必要なパラメータ[deg]
            double lam, beta;	///黄道座標λ[deg]，β[deg]
            double d_lam, lambda_s, r_s;	//惑星光行差による補正
            double epsilon;		//黄道傾角ε[deg]
            double PI = Math.PI;


            /*****日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]を求める*****/
            //必要なパラメータを求める[deg]
            //Ｎ
            N = 341.5208 + 30.34907 * T
                + (0.0350 + 0.00028 * T) * Math.Sin((245.94 - 30.349 * T) * PI / 180) + 0.0004
                - (0.0019 + 0.00002 * T) * Math.Sin((162.78 + 0.38 * T) * PI / 180)
                + (0.3323) * Math.Sin((162.78 + 0.385 * T) * PI / 180)
                + (0.0541) * Math.Sin((38.46 - 36.256 * T) * PI / 180)
                + (0.0447) * Math.Sin((293.42 - 29.941 * T) * PI / 180)

                + (0.0342) * Math.Sin((44.50 - 5.907 * T) * PI / 180)
                + (0.0230) * Math.Sin((201.25 - 24.035 * T) * PI / 180)
                + (0.0222) * Math.Sin((109.99 - 18.128 * T) * PI / 180)
                - (0.0048) * Math.Sin((248.6 - 19.34 * T) * PI / 180)
                + (0.0047) * Math.Sin((184.6 - 11.81 * T) * PI / 180)

                + (0.0045) * Math.Sin((150.1 - 54.38 * T) * PI / 180)
                + (0.0042) * Math.Sin((130.7 - 42.16 * T) * PI / 180)
                + (0.0039) * Math.Sin((7.6 + 6.31 * T) * PI / 180)
                + (0.0031) * Math.Sin((163.2 + 12.22 * T) * PI / 180)
                + (0.0031) * Math.Sin((145.6 + 0.77 * T) * PI / 180)

                + (0.0024) * Math.Sin((191.3 - 0.23 * T) * PI / 180)
                + (0.0019) * Math.Sin((148.4 + 24.44 * T) * PI / 180)
                + (0.0017) * Math.Sin((197.9 - 29.941 * T) * PI / 180)
                + (0.0010) * Math.Sin((307.9 + 36.66 * T) * PI / 180)
                + (0.0010) * Math.Sin((252.6 - 72.51 * T) * PI / 180)

                + (0.0010) * Math.Sin((269.0 - 60.29 * T) * PI / 180)
                + (0.0010) * Math.Sin((278.7 - 29.53 * T) * PI / 180)
                + (0.0008) * Math.Sin((52 - 66.6 * T) * PI / 180)
                + (0.0008) * Math.Sin((24 - 35.8 * T) * PI / 180)
                + (0.0005) * Math.Sin((356 - 5.5 * T) * PI / 180)

                + (0.0005) * Math.Sin((186 - 23.6 * T) * PI / 180)
                + (0.0004) * Math.Sin((344 - 5.9 * T) * PI / 180)
                + (0.0004) * Math.Sin((222 - 48.1 * T) * PI / 180)
                - (0.0004) * Math.Sin((198 + 720.0 * T) * PI / 180)
                + (0.0004) * Math.Sin((140 - 48.5 * T) * PI / 180)

                + (0.0004) * Math.Sin((104 - 24.0 * T) * PI / 180)
                + (0.0003) * Math.Sin((317 - 30.3 * T) * PI / 180)
                + (0.0003) * Math.Sin((280 - 17.7 * T) * PI / 180)
                + (0.0003) * Math.Sin((262 - 60.7 * T) * PI / 180)
                + (0.0003) * Math.Sin((211 - 26.1 * T) * PI / 180)

                + (0.0003) * Math.Sin((209 + 42.6 * T) * PI / 180)
                + (0.0003) * Math.Sin((1 - 90.6 * T) * PI / 180);

            //ｆ
            f = N + 5.5280 * Math.Sin(N * PI / 180) + 0.1666 * Math.Sin(2 * N * PI / 180)
                + 0.0070 * Math.Sin(3 * N * PI / 180) + 0.0003 * Math.Sin(4 * N * PI / 180);
            //Ｖ
            V = 0.0075 * Math.Sin((2 * f + 5.94) * PI / 180);
            //日心黄経Λ
            lambda = f + V + 13.6526 + 0.01396 * T;
            //日心黄緯Ｂ
            B = 180 / PI *  Math.Asin(0.022889 * Math.Sin((f + 272.975) * PI / 180))	//deg単位にしておく
                + (0.0128 + 0.00010 * T) * Math.Sin((f + 35.52) * PI / 180)
                + (0.0010) * Math.Sin((291.9 - 29.94 * T) * PI / 180)
                + (0.0003) * Math.Sin((196 - 24.0 * T) * PI / 180);
            //ｑ’
            q_ = (0.000132 + 0.0000011 * T) *  Math.Cos((245.93 - 30.349 * T) * PI / 180)
                + (0.000230) *  Math.Cos((38.47 - 36.256 * T) * PI / 180)
                + (0.000168) *  Math.Cos((293.36 - 29.941 * T) * PI / 180)
                + (0.000074) *  Math.Cos((200.5 - 24.03 * T) * PI / 180)
                + (0.000055) *  Math.Cos((110.0 - 18.13 * T) * PI / 180)

                + (0.000038) *  Math.Cos((39.3 - 5.91 * T) * PI / 180)
                + (0.000024) *  Math.Cos((150.9 - 54.38 * T) * PI / 180)
                + (0.000023) *  Math.Cos((336.4 + 0.41 * T) * PI / 180)
                + (0.000019) *  Math.Cos((131.7 - 42.16 * T) * PI / 180)
                + (0.000009) *  Math.Cos((180 - 11.8 * T) * PI / 180)

                + (0.000007) *  Math.Cos((277 - 60.3 * T) * PI / 180)
                + (0.000006) *  Math.Cos((330 + 24.4 * T) * PI / 180)
                + (0.000006) *  Math.Cos((53 - 66.6 * T) * PI / 180)
                + (0.000006) *  Math.Cos((188 + 6.3 * T) * PI / 180)
                + (0.000006) *  Math.Cos((251 - 72.5 * T) * PI / 180)

                + (0.000006) *  Math.Cos((198 - 29.9 * T) * PI / 180)
                + (0.000005) *  Math.Cos((353.5 + 12.22 * T) * PI / 180);

            //ｒ’
            r_ = Math.Pow(10, q_);
            //動径ｒ
            r = r_ * 5.190688 / (1 + 0.048254 *  Math.Cos(f * PI / 180));


            /*****黄道座標λ[deg]，β[deg]へ変換する*****/
            geocentric(T, lambda, B, r, out lam, out beta);
            //惑星光行差による補正
            sun(T, out lambda_s, out r_s);
            //補正項Δλを求める
            d_lam = -0.0057 / r_s *  Math.Cos((lam - lambda_s) * PI / 180)
                    - 0.0129 / r *  Math.Cos((lam - lambda) * PI / 180);
            //実際に補正を行う
            lam += d_lam;


            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = ( Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) *  Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / ( Math.Cos(beta * PI / 180) *  Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if ( Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta =  Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) *  Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta =  Math.Asin(delta);

            alpha /= (Math.PI / 180.0);
            delta /= (Math.PI / 180.0);
        }

        /****************************************/
        /*** 金星の位置を計算する             ***/
        /***   Ｔ：時刻引数                   ***/
        /***   α：赤経[deg]    δ：赤緯[deg] ***/
        /****************************************/
        public static void venus(double T, out double alpha, out double delta)
        {
            double lamda, B, r;	    //日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            double lamda0, lamda1, q;//必要なパラメータ[deg]
            double lam, beta;	      //黄道座標λ[deg]，β[deg]
            double d_lam, lambda_s, r_s;	//惑星光行差による補正
            double epsilon;		      //黄道傾角ε[deg]
            double PI = Math.PI;


            /*****日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]を求める*****/
            //必要なパラメータを求める[deg]
            //Ｎ
            lamda1 =
                +(0.7775 - 0.00005 * T) * Math.Sin((178.954 + 585.178 * T) * PI / 180)
                + (0.0033) * Math.Sin((357.9 + 1170.35 * T) * PI / 180)
                + (0.0031) * Math.Sin((242.3 + 450.37 * T) * PI / 180)
                + (0.0020) * Math.Sin((273.5 + 675.55 * T) * PI / 180)
                + (0.0014) * Math.Sin((31.1 + 225.18 * T) * PI / 180)

                + (0.0010) * Math.Sin((233.1 + 90.38 * T) * PI / 180)
                + (0.0008) * Math.Sin((350 + 1.5 * T) * PI / 180)
                + (0.0008) * Math.Sin((136 + 554.8 * T) * PI / 180)
                + (0.0004) * Math.Sin((295 + 540.7 * T) * PI / 180)
                + (0.0004) * Math.Sin((61 - 44.4 * T) * PI / 180)

                + (0.0004) * Math.Sin((17 - 30.3 * T) * PI / 180)
                + (0.0003) * Math.Sin((125 + 900.7 * T) * PI / 180)
                + (0.0003) * Math.Sin((44 + 11.0 * T) * PI / 180);

            lamda0 = 310.1735 + 585.19212 * T
                - (0.0503) * Math.Sin((107.44 + 1170.37 * T + 2 * lamda1) * PI / 180)
                - (0.0048) * Math.Sin((248.6 - 19.34 * T) * PI / 180)
                - (0.0004) * Math.Sin((198 + 720.0 * T) * PI / 180);

            //日心黄経Λ
            lamda = lamda0 + lamda1;

            //日心黄緯Ｂ
            B = 180 / PI *  Math.Asin(0.05922 * Math.Sin((233.72 + 585.183 * T + lamda1) * PI / 180));//deg単位にしておく

            //ｑ
            q = (-0.002947 + 0.00000021 * T) *  Math.Cos((178.954 + 585.178 * T) * PI / 180) - 0.140658
                + (-0.000015) *  Math.Cos((357.9 + 1170.35 * T) * PI / 180)
                + (+0.000010) *  Math.Cos((62.3 + 450.37 * T) * PI / 180)
                + (+0.000008) *  Math.Cos((93 + 675.6 * T) * PI / 180);

            //動径ｒ
            r = Math.Pow(10, q);


            /*****黄道座標λ[deg]，β[deg]へ変換する*****/
            geocentric(T, lamda, B, r, out lam, out beta);
            //惑星光行差による補正
            sun(T, out lambda_s, out r_s);
            //補正項Δλを求める
            d_lam = -0.0057 / r_s *  Math.Cos((lam - lambda_s) * PI / 180)
                    - 0.0048 / r *  Math.Cos((lam - lamda) * PI / 180);
            //実際に補正を行う
            lam += d_lam;


            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = ( Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) *  Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / ( Math.Cos(beta * PI / 180) *  Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if ( Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta =  Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) *  Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta =  Math.Asin(delta);

            alpha /= (Math.PI / 180.0);
            delta /= (Math.PI / 180.0);
        }

        /****************************************/
        /*** 火星の位置を計算する             ***/
        /***   Ｔ：時刻引数                   ***/
        /***   α：赤経[deg]    δ：赤緯[deg] ***/
        /****************************************/
        public static void mars(double T, out double alpha, out double delta)
        {
            double lamda, B, r;	    //日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            double lamda0, lamda1, q;//必要なパラメータ[deg]
            double lam, beta;	      //黄道座標λ[deg]，β[deg]
            double d_lam, lambda_s, r_s;	//惑星光行差による補正
            double epsilon;		      //黄道傾角ε[deg]
            double PI = Math.PI;


            /*****日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]を求める*****/
            //必要なパラメータを求める[deg]
            //Ｎ
            lamda1 =
                +(10.6886 + 0.00010 * T) * Math.Sin((273.768 + 191.399 * T) * PI / 180)
                + (0.6225) * Math.Sin((187.54 + 382.797 * T) * PI / 180)
                + (0.0503) * Math.Sin((101.31 + 574.196 * T) * PI / 180)
                + (0.0146) * Math.Sin((62.31 + 0.198 * T) * PI / 180)
                + (0.0071) * Math.Sin((71.8 + 161.05 * T) * PI / 180)

                + (0.0061) * Math.Sin((230.2 + 130.71 * T) * PI / 180)
                + (0.0046) * Math.Sin((15.1 + 765.59 * T) * PI / 180)
                + (0.0045) * Math.Sin((147.5 + 322.11 * T) * PI / 180)
                + (0.0039) * Math.Sin((279.3 - 22.81 * T) * PI / 180)
                + (0.0024) * Math.Sin((207.7 + 168.59 * T) * PI / 180)

                + (0.0020) * Math.Sin((140.1 + 145.78 * T) * PI / 180)
                + (0.0018) * Math.Sin((224.7 + 10.98 * T) * PI / 180)
                + (0.0014) * Math.Sin((221.8 - 45.62 * T) * PI / 180)
                + (0.0010) * Math.Sin((91.4 - 30.34 * T) * PI / 180)
                + (0.0009) * Math.Sin((268 + 100.4 * T) * PI / 180)

                + (0.0009) * Math.Sin((343 + 352.5 * T) * PI / 180)
                + (0.0007) * Math.Sin((71 + 123.0 * T) * PI / 180)
                + (0.0007) * Math.Sin((203 + 291.8 * T) * PI / 180)
                + (0.0006) * Math.Sin((62 + 513.5 * T) * PI / 180)
                + (0.0005) * Math.Sin((289 + 957.0 * T) * PI / 180)

                + (0.0005) * Math.Sin((13 + 167.0 * T) * PI / 180)
                + (0.0004) * Math.Sin((318 - 60.7 * T) * PI / 180)
                + (0.0004) * Math.Sin((318 + 179.2 * T) * PI / 180)
                + (0.0004) * Math.Sin((85 + 8.9 * T) * PI / 180)
                + (0.0004) * Math.Sin((57 + 483.2 * T) * PI / 180)

                + (0.0004) * Math.Sin((7 - 214.2 * T) * PI / 180)
                + (0.0003) * Math.Sin((1 + 100.2 * T) * PI / 180);

            lamda0 = 249.3542 + 191.41696 * T
                - (0.0149) * Math.Sin((40.01 + 382.819 * T + 2 * lamda1) * PI / 180)
                - (0.0048) * Math.Sin((248.6 - 19.34 * T) * PI / 180)
                - (0.0004) * Math.Sin((198 + 720.0 * T) * PI / 180);

            //日心黄経Λ
            lamda = lamda0 + lamda1;

            //日心黄緯Ｂ
            B = 180 / PI *  Math.Asin(0.03227 * Math.Sin((200.00 + 191.409 * T + lamda1) * PI / 180));//deg単位にしておく

            //ｑ
            q = -(0.040421 + 0.00000039 * T) *  Math.Cos((273.768 + 191.399 * T) * PI / 180) + 0.183844
                + (-0.002825) *  Math.Cos((187.54 + 382.797 * T) * PI / 180)
                + (-0.000249) *  Math.Cos((101.31 + 574.196 * T) * PI / 180)
                + (-0.000024) *  Math.Cos((15.1 + 765.59 * T) * PI / 180)
                + (+0.000023) *  Math.Cos((251.7 + 161.05 * T) * PI / 180)

                + (+0.000022) *  Math.Cos((327.6 + 322.11 * T) * PI / 180)
                + (+0.000017) *  Math.Cos((50.2 + 130.71 * T) * PI / 180)
                + (+0.000007) *  Math.Cos((27 + 168.6 * T) * PI / 180)
                + (+0.000006) *  Math.Cos((320 + 145.8 * T) * PI / 180);

            //動径ｒ
            r = Math.Pow(10, q);


            /*****黄道座標λ[deg]，β[deg]へ変換する*****/
            geocentric(T, lamda, B, r, out lam, out beta);
            //惑星光行差による補正
            sun(T, out lambda_s, out r_s);
            //補正項Δλを求める
            d_lam = -0.0057 / r_s *  Math.Cos((lam - lambda_s) * PI / 180)
                    - 0.0071 / r *  Math.Cos((lam - lamda) * PI / 180);
            //実際に補正を行う
            lam += d_lam;


            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = ( Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) *  Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / ( Math.Cos(beta * PI / 180) *  Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if ( Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta =  Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) *  Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta =  Math.Asin(delta);

            alpha /= (Math.PI / 180.0);
            delta /= (Math.PI / 180.0);
        }

        //****************************************/
        //*** 太陽の位置を計算する             ***/
        //***   Ｔ：時刻引数                   ***/
        //***   α：赤経[rad]    δ：赤緯[rad] ***/
        public static void sunRADEC(double T, out double alpha, out double delta)
        {
            //	double lambda,B,r;	    //日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            //	double lamda0,lamda1,q ;//必要なパラメータ[deg]
            double lam, beta;	    //黄道座標λ[deg]，β[deg]
            double lambda_s, r_s;	//惑星光行差による補正
            double epsilon;		    //黄道傾角ε[deg]
            double PI = Math.PI;

            //惑星光行差による補正
            sun(T, out lambda_s, out r_s);
            //補正項Δλを求める
            //	d_lam = - 0.0057/r_s *  Math.Cos( (lam - lambda_s)*PI/180 )
            //			- 0.0071/r   *  Math.Cos( (lam - lambda  )*PI/180 );
            //実際に補正を行う
            lam = lambda_s - 0.0057;
            beta = 0;

            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = ( Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) *  Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / ( Math.Cos(beta * PI / 180) *  Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if ( Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta =  Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) *  Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta =  Math.Asin(delta);
        }
        //****************************************/
        //*** 月の位置を計算する   　          ***/
        //***   Ｔ：時刻引数                   ***/
        //***   α：地心赤経[deg]　δ：地心赤緯[deg] ***/
        public static void moonGeoRADEC(double T, out double alpha, out double delta, out double r_s)
        {
            //	double lambda,B,r;	    //日心黄経λ[deg]、日心黄緯Ｂ[deg]、動径ｒ[AU]
            //	double lamda0,lamda1,q ;//必要なパラメータ[deg]
            double lam, beta;	    //黄道座標λ[deg]，β[deg]
            double lambda_s, beta_s;	//惑星光行差による補正
            double epsilon;		    //黄道傾角ε[deg]
            double PI = Math.PI;
            double RAD = Math.PI / 180.0;

            moon(T, out lambda_s, out beta_s, out r_s);
            //惑星光行差による補正
            //補正項Δλを求める
            //	d_lam = - 0.0057/r_s *  Math.Cos( (lam - lambda_s)*PI/180 )
            //			- 0.0071/r   *  Math.Cos( (lam - lambda  )*PI/180 );
            //実際に補正を行う
            lam = lambda_s; // -0.0057; //仮
            beta = beta_s;

            /*****赤道座標α[rad]，δ[rad]へ変換する*****/
            //黄道傾角ε[deg]を求める
            epsilon = obliquity_of_the_ecliptic(T);
            //赤経α[rad]を求める
            alpha = (Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Cos(epsilon * PI / 180) - Math.Sin(beta * PI / 180) * Math.Sin(epsilon * PI / 180))
                        / (Math.Cos(beta * PI / 180) * Math.Cos(lam * PI / 180));	//atanをとる前の値
            alpha = Math.Atan(alpha);	//α[rad]
            //αは第Ⅱ，第Ⅲ象限にある
            if (Math.Cos(lam * PI / 180) < 0) { alpha += PI; }
            //αを０から２πの範囲に
            if (alpha > 2 * PI) { alpha -= 2 * PI; }
            else if (alpha < 0) { alpha += 2 * PI; }
            //赤緯δ[rad]を求める
            delta = Math.Cos(beta * PI / 180) * Math.Sin(lam * PI / 180) * Math.Sin(epsilon * PI / 180) + Math.Sin(beta * PI / 180) * Math.Cos(epsilon * PI / 180);	// Math.Asinをとる前の値
            delta = Math.Asin(delta);

            alpha /= RAD;
            delta /= RAD;
        }
       //****************************************/
        //*** 月の位置を計算する   　          ***/
        //***   Ｔ：時刻引数                   ***/
        //***   α：赤経[deg]　δ：赤緯[deg] ***/
        public static void moonTopoRADEC(double T, out double alpha, out double delta)
        {
            moonTopoRADEC(T, 139.531555556, 35.788889, 0.080, out alpha, out delta) ; // 自宅専用
        }
        public static void moonTopoRADEC(double T, double lon_deg, double lat_deg, double height_km, out double alpha, out double delta)
        {
            //double RAD = Math.PI / 180.0;
            var obs_point = geographic2eq_km(lon_deg, lat_deg, height_km);
            double gsd = JulianDay.GSD_MJD(planet_time_to_mjd(T));
            var rz = Rotate_Z(gsd);
            obs_point = (Vector)rz.Multiply(obs_point);

            double alpha_ec, delta_ec, r_s;
            moonGeoRADEC(T, out alpha_ec, out delta_ec, out r_s);
            var moon_point = r_s * eq_directional_cosine(alpha_ec, delta_ec);
            var vmp = moon_point - obs_point;

            var vmp1 = vmp.Normalize(2);
            eq_rev_directional_cosine((Vector)vmp1, out alpha, out delta);
        }

        /// <summary>
        /// 緯度経度　->　赤道座標「ｋｍ」
        /// 経度　longitude　[deg]
        /// 緯度　latitude   [deg]
        /// 標高　height     [km]
        /// </summary>
        public static Vector geographic2eq_km(double lon_deg, double lat_deg, double height_km=0.0)
        {
            var v = Vector.Build.Dense(3);
            //DenseVector ve = ve.Dense(10);
            const double RAD = Math.PI / 180.0;
            double ae = 6377.397155;
            double e2 = 0.006674372230614;
            double N = ae / ( Math.Sqrt(1.0-e2*Math.Sin(lat_deg*RAD)*Math.Sin(lat_deg*RAD)));


            // 赤道座標の方向余弦
            v[0] = (N + height_km) * Math.Cos(lat_deg * RAD) * Math.Cos(lon_deg * RAD);
            v[1] = (N + height_km) * Math.Cos(lat_deg * RAD) * Math.Sin(lon_deg * RAD);
            v[2] = (N*(1-e2) + height_km) * Math.Sin(lat_deg * RAD);

            return (Vector)v;
        }
        
        // 以下、自作ライブリラ

        /// <summary>
        /// 赤道座標->方向余弦
        /// </summary>
        public static Vector eq_directional_cosine(double alfa, double delta)
        {
            var v = Vector.Build.Dense(3);
            //DenseVector ve = ve.Dense(10);
            const double RAD = Math.PI / 180.0;

            // 赤道座標の方向余弦
            v[0] = Math.Cos(delta * RAD) * Math.Cos(alfa * RAD);
            v[1] = Math.Cos(delta * RAD) * Math.Sin(alfa * RAD);
            v[2] = Math.Sin(delta * RAD);

            return (Vector)v;
        }
        /// <summary>
        /// 赤道座標<-方向余弦
        /// </summary>
        public static void eq_rev_directional_cosine(Vector v, out double alfa, out double delta)
        {
            const double RAD = Math.PI / 180.0;

            delta = Math.Asin(v[2]) / RAD;
            alfa = 0;
            if (Math.Abs(v[0]) < 1e-9)
            {
                if (v[1] >= 0) alfa = 90;
                if (v[1] < 0) alfa = -90;
            }
            else
            {
                alfa = Math.Atan2(v[1], v[0]) / RAD;
            }

            while (alfa < 0) alfa += 360;
            while (alfa >= 360) alfa -= 360;
        }
        /// <summary>
        /// 地平座標->方向余弦
        /// </summary>
        public static Vector hori_directional_cosine(double az, double alt)
        {
            var v = Vector.Build.Dense(3);
            //DenseVector ve = ve.Dense(10);
            const double RAD = Math.PI / 180.0;

            // 地平座標の方向余弦
            v[0] = Math.Cos(alt * RAD) * Math.Cos(az * RAD);
            v[1] = -Math.Cos(alt * RAD) * Math.Sin(az * RAD);
            v[2] = Math.Sin(alt * RAD);

            return (Vector)v;
        }
        /// <summary>
        /// 地平座標<-方向余弦
        /// </summary>
        public static void hori_rev_directional_cosine(Vector v, out double az, out double alt)
        {
            const double RAD = Math.PI / 180.0;

            alt = Math.Asin(v[2]) / RAD;
            az = 0;
            if (Math.Abs(v[0]) < 1e-9)
            {
                if (-v[1] >= 0) az = 90;
                if (-v[1] < 0) az = -90;
            }
            else
            {
                az = Math.Atan2(-v[1], v[0]) / RAD;
            }

            while (az < 0) az += 360;
            while (az >= 360) az -= 360;
        }
        /// <summary>
        /// X軸回転
        /// </summary>
        public static Matrix Rotate_X(double theta)
        {
            var m = Matrix.Build.Dense(3, 3);
            const double RAD = Math.PI / 180.0;
            double sinth = Math.Sin(theta * RAD);
            double costh = Math.Cos(theta * RAD);

            m[0, 0] = 1;
            m[0, 1] = 0;
            m[0, 2] = 0;

            m[1, 0] = 0;
            m[1, 1] = costh;
            m[1, 2] = -sinth;

            m[2, 0] = 0;
            m[2, 1] = sinth;
            m[2, 2] = costh;

            return (Matrix)m;
        }
        /// <summary>
        /// Y軸回転
        /// </summary>
        public static Matrix Rotate_Y(double theta)
        {
            var m = Matrix.Build.Dense(3, 3);
            const double RAD = Math.PI / 180.0;
            double sinth = Math.Sin(theta * RAD);
            double costh = Math.Cos(theta * RAD);

            m[0, 0] = costh;
            m[0, 1] = 0;
            m[0, 2] = sinth;

            m[1, 0] = 0;
            m[1, 1] = 1;
            m[1, 2] = 0;

            m[2, 0] = -sinth;
            m[2, 1] = 0;
            m[2, 2] = costh;

            return (Matrix)m;
        }
        /// <summary>
        /// Z軸回転 [deg]
        /// </summary>
        public static Matrix Rotate_Z(double theta)
        {
            var m = Matrix.Build.Dense(3, 3);
            const double RAD = Math.PI / 180.0;
            double sinth = Math.Sin(theta * RAD);
            double costh = Math.Cos(theta * RAD);

            m[0, 0] = costh;
            m[0, 1] = -sinth;
            m[0, 2] = 0;

            m[1, 0] = sinth;
            m[1, 1] = costh;
            m[1, 2] = 0;

            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = 1;

            return (Matrix)m;
        }
        /// <summary>
        /// 地平座標->赤道座標 用行列
        /// </summary>
        public static Matrix AzAlt2EqMat(double theta, double fai)
        {
            var m = Matrix.Build.Dense(3, 3);
            var m2 = Matrix.Build.Dense(3, 3);
            const double RAD = Math.PI / 180.0;
            double sinth = Math.Sin(theta * RAD);
            double costh = Math.Cos(theta * RAD);
            m[0, 0] = costh;
            m[0, 1] = sinth;
            m[0, 2] = 0;

            m[1, 0] = -sinth;
            m[1, 1] = costh;
            m[1, 2] = 0;

            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = 1;

            sinth = Math.Sin(fai * RAD);
            costh = Math.Cos(fai * RAD);
            m2[0, 0] = sinth;
            m2[0, 1] = 0;
            m2[0, 2] = -costh;

            m2[1, 0] = 0;
            m2[1, 1] = 1;
            m2[1, 2] = 0;

            m2[2, 0] = costh;
            m2[2, 1] = 0;
            m2[2, 2] = sinth;

            return (Matrix)(m2.Multiply(m));
        }
        /// <summary>
        /// 歳差補正
        /// </summary>
        public static void Precession_JST(double ra1, double dec1, DateTime t_jst, out double ra2, out double dec2)
        {
            DateTime t_ut = t_jst.AddHours(-9);
            Precession_UT(ra1, dec1, t_ut, out ra2, out dec2);
        }
        public static void Precession_UT(double ra1, double dec1, DateTime t_ut, out double ra2, out double dec2)
        {
            double tb = t_bessel_UT(t_ut);
            double tue   = 2304.948 * tb + 0.302 * tb * tb + 0.018 * tb * tb * tb;
            double z     = 2304.948 * tb + 1.093 * tb * tb + 0.019 * tb * tb * tb;
            double theta = 2004.2555 * tb - 0.426 * tb * tb - 0.042 * tb * tb * tb;

            var m1 = Rotate_Z(-(90 - tue / 3600));
            var m2 = Rotate_X(-theta/3600);
            var m3 = Rotate_Z(+(90 + z / 3600));

            var ve = eq_directional_cosine(ra1, dec1);
            var v  = m3.Multiply( m2.Multiply( m1.Multiply(ve)));
            eq_rev_directional_cosine((Vector)v, out ra2, out dec2);
        }

        /// <summary>
        /// 赤道座標->地平座標
        /// </summary>
        public static void Eq2AzAlt_Yokohama(double ra, double dec, DateTime t_jst, out double az, out double alt)
        {
            DateTime t_ut = t_jst.AddHours(-9);
            Eq2AzAlt_UT(ra, dec, 139.563054, 35.355091, t_ut, out az, out alt);
        }
        public static void Eq2AzAlt_JST(double ra, double dec, double lon, double fai, DateTime t_jst, out double az, out double alt)
        {
            DateTime t_ut = t_jst.AddHours(-9);
            Eq2AzAlt_UT(ra, dec, lon, fai, t_ut, out az, out alt);
        }
        public static void Eq2AzAlt_UT(double ra, double dec, double lon, double fai, DateTime t_ut, out double az, out double alt)
        {
            double theta = JulianDay.SiderealTimeUT(t_ut, lon);
            var m = Eq2AzAltMat(theta, fai);
            var ve = eq_directional_cosine(ra, dec);
            //var v = Vector.Build.Dense(3);

            var v = m.Multiply(ve);
            hori_rev_directional_cosine((Vector)v, out az, out alt);
        }

        /// <summary>
        /// 赤道座標->地平座標
        /// </summary>
        public static Matrix Eq2AzAltMat(double theta, double fai)
        {
            var m = Matrix.Build.Dense(3, 3);
            var m2 = Matrix.Build.Dense(3, 3);
            const double RAD = Math.PI / 180.0;
            double sinth = Math.Sin(theta * RAD);
            double costh = Math.Cos(theta * RAD);
            m[0, 0] = costh;
            m[0, 1] = sinth;
            m[0, 2] = 0;

            m[1, 0] = -sinth;
            m[1, 1] = costh;
            m[1, 2] = 0;

            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = 1;

            sinth = Math.Sin(fai * RAD);
            costh = Math.Cos(fai * RAD);
            m2[0, 0] = sinth;
            m2[0, 1] = 0;
            m2[0, 2] = -costh;

            m2[1, 0] = 0;
            m2[1, 1] = 1;
            m2[1, 2] = 0;

            m2[2, 0] = costh;
            m2[2, 1] = 0;
            m2[2, 2] = sinth;

            return (Matrix)(m2.Multiply(m));
        }
        /// <summary>
        /// 観測開始時刻
        /// </summary>
        public static DateTime ObsStartTime(DateTime obs_dt, double sun_alt_thresold=-5.0, double lon = 139.563054, double lat = 35.355091)
        {       
            if( sun_alt_thresold >  30.0 ) sun_alt_thresold = 30 ;
            if( sun_alt_thresold < -30.0 ) sun_alt_thresold = -30 ;
            double ra, dec, az, alt = 90.0;
            double RAD = Math.PI / 180.0;

            DateTime dtdate = obs_dt.Date;
            DateTime dt = dtdate.AddHours(15.0); // 15時から計算開始

            while (alt > sun_alt_thresold)
            {
                dt = dt.AddMinutes(1); // 刻み　1分
                double pt1 = Planet.planet_time_jst_datetime(dt);

                Planet.sunRADEC(pt1, out ra, out dec);
                Planet.Eq2AzAlt_JST(ra / RAD, dec / RAD, lon, lat, dt, out az, out alt);
            }
            return dt;
        }
        /// <summary>
        /// 観測終了時刻
        /// </summary>
        public static DateTime ObsEndTime(DateTime obs_dt, double sun_alt_thresold = -6.0, double lon = 139.563054, double lat = 35.355091)
        {
            if (sun_alt_thresold > 30.0) sun_alt_thresold = 30;
            if (sun_alt_thresold < -30.0) sun_alt_thresold = -30;
            double ra, dec, az, alt = 90.0;
            double RAD = Math.PI / 180.0;

            DateTime dtdate = obs_dt.Date;
            DateTime dt = dtdate.AddHours(9.0); // 9時から計算開始

            while (alt > sun_alt_thresold)
            {
                dt = dt.AddMinutes(-1); // 刻み　1分
                double pt1 = Planet.planet_time_jst_datetime(dt);

                Planet.sunRADEC(pt1, out ra, out dec);
                Planet.Eq2AzAlt_JST(ra / RAD, dec / RAD, lon, lat, dt, out az, out alt);
            }
            return dt;
        }
    }
}

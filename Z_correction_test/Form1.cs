﻿using System;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra.Double;
using MtLibrary;

namespace Z_correction_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Az軸方向と天頂との誤差の補正
        /// Zaz：天頂から見たAz軸方位(deg)
        /// dat：天頂から見たAz軸距離(deg)
        /// </summary>
        public void z_correct(double az, double alt, double zaz, double dzt, out double az_zc, out double alt_zc)
        {
            Vector vt = Planet.hori_directional_cosine(az, alt);

            Matrix my = Planet.Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
            Matrix mz1 = Planet.Rotate_Z(zaz); // 
            Matrix mz2 = Planet.Rotate_Z(-zaz); // 

            var v1 = mz1.Multiply(vt);
            Planet.hori_rev_directional_cosine((Vector)v1, out az_zc, out alt_zc);
            var v2 = my.Multiply(v1);
            Planet.hori_rev_directional_cosine((Vector)v2, out az_zc, out alt_zc);
            var v3 = mz2.Multiply(v2);
            Planet.hori_rev_directional_cosine((Vector)v3, out az_zc, out alt_zc);
        }
        /// <summary>
        /// Az軸とAlt軸の直交誤差の補正
        /// </summary>
        public void azalt_correct(double az, double alt, double zaz, double dzt, out double az_zc, out double alt_zc)
        {
            Vector vt = Planet.hori_directional_cosine(az, alt);

            Matrix my = Planet.Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
            Matrix mz1 = Planet.Rotate_Z(zaz); // 
            Matrix mz2 = Planet.Rotate_Z(-zaz); // 
            if (comboBox1.Text == "mmEast")
            {
                //my  = Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
              //  mz1 = Rotate_Z(90  - az - zaz); // 
              //  mz2 = Rotate_Z(-(90  - az - zaz)); // 
            }
            var v1 = mz1.Multiply(vt);
            Planet.hori_rev_directional_cosine((Vector)v1, out az_zc, out alt_zc);
            var v2 = my.Multiply(v1);
            Planet.hori_rev_directional_cosine((Vector)v2, out az_zc, out alt_zc);
            var v3 = mz2.Multiply(v2);
            Planet.hori_rev_directional_cosine((Vector)v3, out az_zc, out alt_zc);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // こんな風に行列を初期化できる
            var M1 = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 9, 8 } });
            var v1 = DenseVector.OfArray(new double[] { 1, 2, 3 });
            var v2 = M1.Multiply(v1);
            var aaa = M1[0, 1];

            double mt2az = Convert.ToDouble(textBox_MT2Az.Text);
            double mt2alt = Convert.ToDouble(textBox_MT2Alt.Text);
            double mt2zaz = 185; // Convert.ToDouble(textBox_MT2ZAz.Text);
            double mt2zdt = 2.0;// Convert.ToDouble(textBox_MT2ｄZT.Text);

            double az_zc, alt_zc;
            azalt_correct(mt2az, mt2alt, mt2zaz, mt2zdt, out az_zc, out alt_zc);
            string s = string.Format("Az:{0},{1}  {2},{3}  ans:{4,0:F1},  {5,0:F1}\n", mt2az, mt2alt, mt2zaz, mt2zdt, az_zc, alt_zc);
            richTextBox1.AppendText(s);

            mt2zaz = 130; // Convert.ToDouble(textBox_MT2ZAz.Text);
            mt2zdt = 7.6;// Convert.ToDouble(textBox_MT2ｄZT.Text);
            z_correct(mt2az, mt2alt, mt2zaz, mt2zdt, out az_zc, out alt_zc);
            s = string.Format("Z correct:{0},{1}  {2},{3}  ans:{4,0:F1},  {5,0:F1}\n", mt2az, mt2alt, mt2zaz, mt2zdt, az_zc, alt_zc);
            richTextBox1.AppendText(s);

            //MJD
            DateTime t = DateTime.Now;
            double aa = JulianDay.DateTimeToModifiedJulianDay(t);
            DateTime dtBirth1 = new DateTime(1858,11,17,00,00,00);

            // カルチャ情報を設定する
            System.Globalization.CultureInfo cFormat = (
                new System.Globalization.CultureInfo("fr-FR", false)
            );
            // 文字列から DateTime の値に変換する
            DateTime dtBirth = DateTime.Parse("1858/11/17 00:00:00", cFormat);
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);
            double pt11 = Planet.planet_time_jst_datetime(dtBirth);
            double m = Planet.planet_time_to_mjd(pt11);
            s = string.Format("dt:{0}, {1} {2}\n", dtBirth, mjd0, m);
            richTextBox1.AppendText(s);


            dtBirth = DateTime.Parse("1899/12/31 12:00:00", cFormat);
            mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            dtBirth = DateTime.Parse("1978/6/10 21:20:00", cFormat); // JST  //UTC = JST-9h  
            double az, alt;
            Planet.Eq2AzAlt_JST(316.166396, 38.499750, 139.531555556, 35.788889, dtBirth, out az, out alt);
            s = string.Format("aa:{0}, {1} \n", az,alt);
            richTextBox1.AppendText(s);

            dtBirth = DateTime.Parse("1979/9/15 21:00:00", cFormat); // JST  //UTC = JST-9h  
            double pt1 = Planet.planet_time_jst_datetime(dtBirth);
            double pt2 = Planet.planet_time(1979, 9, 15, 12, 0, 0);

            dtBirth = DateTime.Parse("1978/6/10 00:00:00", cFormat); // JST  //UTC = JST-9h  
            double gsd = JulianDay.GSD_DateTime(dtBirth);

            double ra, dec;
            DateTime dt_jst = new DateTime(2014, 5, 21, 9, 0, 0);
            pt1 = Planet.planet_time_jst_datetime(dt_jst); 
            Planet.moonTopoRADEC(pt1, out ra, out dec);
            double lon = 139.531555556, lat = 35.788889;
            Planet.Eq2AzAlt_JST(ra, dec, lon, lat, dt_jst, out az, out alt);

//           //  Planet.moonGeoRADEC(pt1, out ra, out dec, out r);
            s = string.Format("PT:{0}, {1} gsd:{2}  az:{3} {4}\n", ra, dec,gsd,az,alt);
            richTextBox1.AppendText(s);

            dt_jst = new DateTime(2015, 4, 25, 18, 0, 0);
            pt1 = Planet.planet_time_jst_datetime(dt_jst);
            Planet.mercury(pt1, out ra, out dec);
            s = string.Format("mercury PT1:{0}, ra,dec:{1},{2}\n", pt1, ra, dec);
            richTextBox1.AppendText(s);
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string bs = System.IO.File.ReadAllText(@"S:\satoshi\Lib\bsc5.dat");
            string[] sad = bs.Split('\n');
            string s1, s;

            int HR, RAh, RAm,DEsign, DEd, DEm, DEs;
            double RA, DEC, RAs, Vmag;
            string Name;
            for (int i = 0; i < 9110; i++)
            {
                s1 = sad[i];
                s = s1.Substring( 0, 4); HR  = int.Parse(s);
                s = s1.Substring( 4,10); Name = s;
                s = s1.Substring(75, 2);
                if (s == "  ") { continue; } // NOVA等の無データ対応
                RAh = int.Parse(s);
                s = s1.Substring(77, 2); RAm = int.Parse(s);
                s = s1.Substring(79, 4); RAs = double.Parse(s);
                s = s1.Substring(83, 1);
                DEsign = 1; if (s[0] == '-') { DEsign = -1; }
                s = s1.Substring(84, 2); DEd = int.Parse(s);
                s = s1.Substring(86, 2); DEm = int.Parse(s);
                s = s1.Substring(88, 2); DEs = int.Parse(s);
                s = s1.Substring(102, 5); Vmag = double.Parse(s);

                RA = 15 * (RAh + RAm / 60.0 + RAs / 3600.0);
                DEC = DEsign * (DEd + DEm / 60.0 + DEs / 3600.0);

                s = HR.ToString() + " RA:" + RA.ToString() + " DEC:" + DEC.ToString() + " Vmag:" + Vmag.ToString() + " [" + Name +"]";
                richTextBox1.AppendText(s);
            }
           // s = bs.Substring(stp+5-1, 10) + "\n";
           // richTextBox1.AppendText(s);

            Star.init();

            double ra0 = 316.166396, dec0=38.49975;
            double ra1, dec1;
            DateTime dt2 = new DateTime(1978,6,10,21,20,00);
            Planet.Precession_JST(ra0, dec0, dt2, out ra1, out dec1);

            Star.ID = 6;
            Star.cal_azalt();
            int count = Star.Count;
            s = string.Format("Star count:{0} {1} {2} Az:{3} {4}\n", count, Star.ID, Star.Name, Star.Az, Star.Alt);
            richTextBox1.AppendText(s);

            double sun_alt_thresold = -6.0;  // 航海薄明
            DateTime dt = Planet.ObsStartTime(DateTime.Now, sun_alt_thresold);
            double RAD = Math.PI / 180.0;
            DateTime dtend = Planet.ObsEndTime(DateTime.Now, sun_alt_thresold);

            double pt1 = Planet.planet_time_jst_datetime(dt);

            double ra, dec, az, alt;
            double lon = 139.563054; //　経度
            double lat = 35.355091; //  緯度

            Planet.sunRADEC(pt1, out ra, out dec);
            Planet.Eq2AzAlt_JST(ra / RAD, dec / RAD, lon, lat, dt, out az, out alt);

            s = string.Format("S={2} E={3}    Sun:{0}, {1} \n", az, alt, dt, dtend);
            richTextBox1.AppendText(s);

            double fai = 35, ramda = 140, h = 100;
            var v = Planet.geographic2eq_km(ramda, fai, h);
            s = string.Format("V=({0},  {1},  {2})\n", v[0], v[1], v[2]);
            richTextBox1.AppendText(s);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s,s1;

            Common.Send_cmd_KV1000_init();
            s  = Common.Send_cmd_KV1000(Common.MT2SetPos(30, 40));
            richTextBox1.AppendText(s);

            s1 = string.Format("ST 02000\r");
            s  = Common.Send_cmd_KV1000(s1);
            richTextBox1.AppendText(s);
            Common.Send_cmd_KV1000_close();

            double az=0, alt=0, cx=5, cy=7;
            Planet.cxcy2azalt(cx, cy, 90, 30, 0, 0, 35, 0.0074, 0.0074, ref az, ref alt);
            Planet.azalt2cxcy(az, alt, 90, 30, 0, 0, 35, 0.0074, 0.0074, ref cx, ref cy);
            s = "Az,Alt=" + az.ToString() + " " + alt.ToString(); richTextBox1.AppendText(s);
            s = "cxcy  =" + cx.ToString() + " " + cy.ToString(); richTextBox1.AppendText(s);
        }

    }
}

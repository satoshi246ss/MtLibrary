using System;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra.Double;


namespace Z_correction_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 赤道座標->方向余弦
        /// </summary>
        public Vector eq_directional_cosine(double alfa, double delta)
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
        public void eq_rev_directional_cosine(Vector v, out double alfa, out double delta)
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
        public Vector hori_directional_cosine(double az, double alt)
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
        public void hori_rev_directional_cosine(Vector v, out double az, out double alt)
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
        public Matrix Rotate_X(double theta)
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
        public Matrix Rotate_Y(double theta)
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
        /// Z軸回転
        /// </summary>
        public Matrix Rotate_Z(double theta)
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
        /// 地平座標->赤道座標
        /// </summary>
        public Matrix AzAlt2EqMat(double theta, double fai)
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
        /// 赤道座標->地平座標
        /// </summary>
        public void Eq2AzAlt(double ra, double dec, double lon, double fai, DateTime t, out double az, out double alt)
        {
            double theta = JulianDay.SiderealTime(t, lon);
            var m = Eq2AzAltMat(theta, fai);
            var ve = eq_directional_cosine(ra, dec);
            var v = Vector.Build.Dense(3);

            v = m.Multiply(ve);
            hori_rev_directional_cosine((Vector)v, out az, out alt);
        }
        
        /// <summary>
        /// 赤道座標->地平座標
        /// </summary>
        public Matrix Eq2AzAltMat(double theta, double fai)
        {
            var m = Matrix.Build.Dense(3, 3);
            var m2 = Matrix.Build.Dense(3, 3);
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

            sinth = Math.Sin(fai * RAD);
            costh = Math.Cos(fai * RAD);
            m2[0, 0] = sinth;
            m2[0, 1] = 0;
            m2[0, 2] = costh;

            m2[1, 0] = 0;
            m2[1, 1] = 1;
            m2[1, 2] = 0;

            m2[2, 0] = -costh;
            m2[2, 1] = 0;
            m2[2, 2] = sinth;

            return (Matrix)(m.Multiply(m2));
        }
        /// <summary>
        /// Az軸方向と天頂との誤差の補正
        /// Zaz：天頂から見たAz軸方位(deg)
        /// dat：天頂から見たAz軸距離(deg)
        /// </summary>
        public void z_correct(double az, double alt, double zaz, double dzt, out double az_zc, out double alt_zc)
        {
            Vector vt = hori_directional_cosine(az, alt);

            Matrix my = Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
            Matrix mz1 = Rotate_Z(zaz); // 
            Matrix mz2 = Rotate_Z(-zaz); // 

            var v1 = mz1.Multiply(vt);
            eq_rev_directional_cosine((Vector)v1, out az_zc, out alt_zc);
            var v2 = my.Multiply(v1);
            eq_rev_directional_cosine((Vector)v2, out az_zc, out alt_zc);
            var v3 = mz2.Multiply(v2);
            eq_rev_directional_cosine((Vector)v3, out az_zc, out alt_zc);
        }
        /// <summary>
        /// Az軸とAlt軸の直交誤差の補正
        /// </summary>
        public void azalt_correct(double az, double alt, double zaz, double dzt, out double az_zc, out double alt_zc)
        {
            Vector vt = hori_directional_cosine(az, alt);

            Matrix my = Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
            Matrix mz1 = Rotate_Z( zaz); // 
            Matrix mz2 = Rotate_Z( -zaz); // 
            if (comboBox1.Text == "mmEast")
            {
                //my  = Rotate_Y(-dzt); // 天頂づれ補正　dzt:北側が＋
              //  mz1 = Rotate_Z(90  - az - zaz); // 
              //  mz2 = Rotate_Z(-(90  - az - zaz)); // 
            }
            var v1 = mz1.Multiply(vt);
            eq_rev_directional_cosine((Vector)v1, out az_zc, out alt_zc);
            var v2 = my.Multiply(v1);
            eq_rev_directional_cosine((Vector)v2, out az_zc, out alt_zc);
            var v3 = mz2.Multiply(v2);

            eq_rev_directional_cosine((Vector)v3, out az_zc, out alt_zc);
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            s = string.Format("Az:{0},{1}  {2},{3}  ans:{4,0:F1},  {5,0:F1}\n", mt2az, mt2alt, mt2zaz, mt2zdt, az_zc, alt_zc);
            richTextBox1.AppendText(s);

            //MJD
            DateTime t = DateTime.Now;
            double aa = JulianDay.DateTimeToModifiedJulianDay(t);

            // カルチャ情報を設定する
            System.Globalization.CultureInfo cFormat = (
                new System.Globalization.CultureInfo("fr-FR", false)
            );
            // 文字列から DateTime の値に変換する
            DateTime dtBirth = DateTime.Parse("1858/11/17 00:00:00", cFormat);
            double mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            dtBirth = DateTime.Parse("1899/12/31 12:00:00", cFormat);
            mjd0 = JulianDay.DateTimeToModifiedJulianDay(dtBirth);

            dtBirth = DateTime.Parse("1978/6/10 12:20:00", cFormat); //UT
            double az, alt;
            Eq2AzAlt(316.166396, 38.499750, 139.5315556, 35.788889, dtBirth, out az, out alt);


            s = string.Format("aa:{0}, {1} \n", az,alt);
            richTextBox1.AppendText(s);
        }

    }
}

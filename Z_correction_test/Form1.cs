using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// 地平座標->方向余弦
        /// </summary>
        public Vector eq_directional_cosine(double az, double alt)
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
        public void eq_rev_directional_cosine(Vector v, out double az, out double alt)
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
        /// Az軸方向と天頂との誤差の補正
        /// Zaz：天頂から見たAz軸方位(deg)
        /// dat：天頂から見たAz軸距離(deg)
        /// </summary>
        public void z_correct(double az, double alt, double zaz, double dzt, out double az_zc, out double alt_zc)
        {
            Vector vt = eq_directional_cosine(az, alt);

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
            Vector vt = eq_directional_cosine(az, alt);

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

        }
    }
}

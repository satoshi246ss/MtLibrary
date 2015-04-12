using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtLibrary
{
    public class Common
    {
        private static System.Net.Sockets.UdpClient udpc_s = null;

        //---------------------------------------------------------------------------
        //  地平座標系の角距離の計算
        //  IN  2組のaz,alt [deg]
        //  OUT 角距離 [deg]
        // 2004/2/20 作成
        //
        public static double Cal_Distance(double az1, double alt1, double az2, double alt2)
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
        /// <summary>
        /// Udp 送信 ルーチン(KV1000　上位リンク)
        /// </summary>
        public static void Send_cmd_KV1000_init(int Port = 8501)
        {
            // ソケット生成
            udpc_s = new System.Net.Sockets.UdpClient(Port);
        }
        public static void Send_cmd_KV1000_close()
        {
            if (udpc_s != null)
            {
                udpc_s.Close();
            }
        }
        /// <summary>
        /// PID cmd送信ルーチン(KV1000　上位リンク)
        /// </summary>
        public static string Send_cmd_KV1000(string s1, string remoteHost = "192.168.1.10", int remotePort = 8501)
        {
            // CMD data send for cmd

            //送信するデータを読み込む
            //string s1 = string.Format("STWRS DM937 3 {0} {1} {2}\r", (ushort)id, (ushort)udpkv.PIDPV_makedata(daz), (ushort)udpkv.PIDPV_makedata(dalt));
            byte[] sendBytes = Encoding.ASCII.GetBytes(s1);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc_s.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                //this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
            string s = "S:" + remoteHost + "(" + remotePort.ToString() + ") "+s1+"\n" ;
            return s;
        }

        /// <summary>
        /// KV1000用 エンディアン変換
        /// </summary>
        public Int32 EndianChange(Int32 obj)
        {
            //int size = Marshal.SizeOf(obj);
            //IntPtr ptr = Marshal.AllocHGlobal(size);
            //Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = (BitConverter.GetBytes(obj));
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// KV1000用 エンディアン変換
        /// </summary>
        public UInt32 EndianChange(UInt32 obj)
        {
            //int size = Marshal.SizeOf(obj);
            //IntPtr ptr = Marshal.AllocHGlobal(size);
            //Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = (BitConverter.GetBytes(obj));
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        /// <summary>
        /// KV1000用 エンディアン変換
        /// </summary>
        public static short EndianChange(short obj)
        {
            byte[] bytes = (BitConverter.GetBytes(obj));
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        public static ushort EndianChange(ushort obj)
        {
            byte[] bytes = (BitConverter.GetBytes(obj));
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }
        /// <summary>
        // 速度データをmmdeg整数化（KV-1000に送信用）
        // 戻り：0.001deg/sec単位の整数
        /// </summary>
        public int round_d2i(double x)
        {
            if (x > 0.0)
            {
                return (int)(x * 1000 + 0.5);
            }
            else
            {
                return (-1 * (int)(-x * 1000 + 0.5));
            }
        }
        /// <summary>
        // doubleデータを0.001単位の非負整数化（KV-1000に送信用）、ushort変換
        // 戻り：0.001deg/sec単位の整数
        /// </summary>
        public ushort double2KV1000data_us(double daz0)
        {
            double daz = daz0;
            // 条件チェック
            const double vmax = 32.765;
            if (daz < -vmax) daz = -vmax;
            if (daz > vmax) daz = vmax;

            int upos = round_d2i(daz);

            ushort p4b;
            // p4a = (unsigned short)(upos>>16) ;
            p4b = (ushort)(0xffff & upos);

            return p4b;
        }


    }
}

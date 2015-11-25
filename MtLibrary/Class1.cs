using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtLibrary
{
    public class Common
    {
        enum MT_Reion
        {
            mmWest,
            mmEast,
            mmSouth,
            mmNorth
        }
        enum Return_code
        {
            mmSuccess,
            mmFail
        }

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
        /// UInt32 -> UShort_U, UShort_L 分解
        /// </summary>
        public void TUInt2UShortUShor(UInt32 pos, out UInt16 pos_U, out UInt16 pos_L)
        {
            pos_U = (UInt16)(pos >> 16);  // >>16 ->1/256*256
            pos_L = (UInt16)(pos & Convert.ToUInt32("FFFF", 16));
        }
        /// <summary>
        /// UInt32 -> UShort_U 分解
        /// </summary>
        public static UInt16 TUInt2UShort_U(UInt32 pos)
        {
            return (UInt16)(pos >> 16);  // >>16 ->1/256*256
        }
        /// <summary>
        /// UInt32 -> UShort_U 分解
        /// </summary>
        public static UInt16 TUInt2UShort_L(UInt32 pos)
        {
            return (UInt16)(pos & Convert.ToUInt32("FFFF", 16));
        }

        /// <summary>
        // 速度データをmmdeg整数化（KV-1000に送信用）
        // 戻り：0.001deg/sec単位の整数
        /// </summary>
        public static int round_d2i(double x)
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

        /// <summary>
        //---------------------------------------------------------------------------
        // 地平座標 -> MT2 モータAz,Alt座標 変換
        // 入力 double az  : deg 単位   0 ～ 360
        //       double alt : deg 単位  -90 ～ 90
        // 出力  double maz : deg 0:南  90：西 180：北  270：東
        //     double malt: deg 0:天底 90：西 180：天頂 270：東
        // 戻り int エラーコード
        // 2006/09/06
        //---------------------------------------------------------------------------
        /// <summary>
        public static int AzAlt2MA_X1Y1(double az, double alt, ref double maz, ref double malt, ref int region_mode)
        {
            // region check
            if (az >= 0 && az <= 180) region_mode = (int)MT_Reion.mmWest;
            else
                if (az >= 180 && az <= 360) region_mode = (int)MT_Reion.mmEast;
                else return -5;

            if (region_mode == (int)MT_Reion.mmEast)
            {
                maz = az - 90;
                malt = 270 - alt;
            }
            else if (region_mode == (int)MT_Reion.mmWest)
            {
                maz = az + 90;
                malt = alt + 90;
            }
            else return -4;

            return (int)Return_code.mmSuccess;
        }

        public static string MT2SetPos(double az, double alt)
        {
            double maz = 0, malt = 0;
            int region_mode=0;
            AzAlt2MA_X1Y1(az, alt, ref maz, ref malt, ref region_mode);

            UInt32 xpos = (UInt32) round_d2i(maz );
            UInt32 ypos = (UInt32) round_d2i(malt);

            //("WRS DM%05d 4 %05d %05d %05d %05d\r\n"  [39]:WRS DM00006 4 46022 00003 11612 00003\r\n
            // DM6 Az,Alt位置決めデータ
            string s1 = string.Format("WRS DM00006 4 {0:00000} {1:00000} {2:00000} {3:00000}\r", TUInt2UShort_L(xpos), TUInt2UShort_U(xpos), TUInt2UShort_L(ypos), TUInt2UShort_U(ypos));
            return s1;
        }

    }
}

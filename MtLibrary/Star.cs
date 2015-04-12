using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtLibrary
{
    public struct Star_Data
    {
        public double RA;
        public double DEC;
        public double Mag;
        public string Name;
   }

    public class Star
    {
//        static int id;
        static List<Star_Data> star_data = new List<Star_Data>();
        static Star_Data sd = new Star_Data();
        static DateTime last_update = new DateTime();

        // propaty
        public static int ID{get; set;}
        public static double Az{get; set;}
        public static double Alt{get; set;}

        // Listのデータ数
        public static int Count
        {
            get { return star_data.Count; }
        }
        // IDのdata
        public static Star_Data StarData
        {
            get { return star_data[ID]; }
        }
        // IDのRA
        public static double RA
        {
            get { return star_data[ID].RA ; }
        }
        // IDのDEC
        public static double DEC
        {
            get { return star_data[ID].DEC; }
        }
        // IDのMag
        public static double Mag
        {
            get { return star_data[ID].Mag; }
        }
        // IDのName
        public static string Name
        {
            get { return star_data[ID].Name; }
        }

        // member
        public static void init()
        {
            init_planet(); //0-5

            // B1950
            //[6] シリウス
            sd.RA = 100.736308; // deg
            sd.DEC = -16.646211; // deg
            sd.Mag = -1.5;
            sd.Name = "Sirius";
            Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC);
            star_data.Add(sd);

            //[7] ベガ
            sd.RA = 278.811063; // deg
            sd.DEC = +38.736022; // deg
            sd.Mag = 0.2;
            sd.Name = "Vega";
            Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC);
            star_data.Add(sd);

            // 一等星(２５個）
            // i=5 ;
            sd.RA = 54.235344; sd.DEC = -28.1371; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 95.987703; sd.DEC = -52.695836; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 101.288567; sd.DEC = -16.712758; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 198.239786; sd.DEC = -59.821683; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 219.923378; sd.DEC = -60.835583; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 279.234; sd.DEC = 38.782869; sd.Mag = 0.05; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.05
            sd.RA = 78.634453; sd.DEC = -8.201653; sd.Mag = 0.1; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.1
            sd.RA = 79.171994; sd.DEC = 45.9992; sd.Mag = 0.1; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.1
            sd.RA = 213.918533; sd.DEC = 19.187969; sd.Mag = 0.1; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.1
            sd.RA = 219.989489; sd.DEC = -60.836983; sd.Mag = 0.35; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.35
            sd.RA = 114.827444; sd.DEC = 5.227856; sd.Mag = 0.4; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.4
            sd.RA = 24.427244; sd.DEC = -57.2365; sd.Mag = 0.5; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.5
            sd.RA = 210.956203; sd.DEC = -60.372836; sd.Mag = 0.6; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.6
            sd.RA = 297.694089; sd.DEC = 8.867306; sd.Mag = 0.6; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.8
            sd.RA = 68.979972; sd.DEC = 16.509794; sd.Mag = 0.8; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.9
            sd.RA = 88.792828; sd.DEC = 7.406992; sd.Mag = 0.9; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	0.9
            sd.RA = 201.298078; sd.DEC = -11.161233; sd.Mag = 1.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1
            sd.RA = 247.351869; sd.DEC = -26.431828; sd.Mag = 1.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1
            sd.RA = 116.330911; sd.DEC = 28.026317; sd.Mag = 1.1; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.1
            sd.RA = 154.992122; sd.DEC = 19.841778; sd.Mag = 1.2; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.2
            sd.RA = 344.411667; sd.DEC = -29.621792; sd.Mag = 1.2; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.2
            sd.RA = 191.930953; sd.DEC = -59.688586; sd.Mag = 1.3; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.3
            sd.RA = 310.357411; sd.DEC = 45.280428; sd.Mag = 1.3; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.3
            sd.RA = 152.093578; sd.DEC = 11.967111; sd.Mag = 1.4; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.4
            sd.RA = 219.917; sd.DEC = -60.840083; sd.Mag = 1.4; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.4

            // ２等星(６５個）
            // i=6 ;
            sd.RA = 81.282536; sd.DEC = 6.349686; sd.Mag = 1.6; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 104.656411; sd.DEC = -28.972086; sd.Mag = 1.6; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 113.650544; sd.DEC = 31.888758; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 186.650203; sd.DEC = -63.098861; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 187.791244; sd.DEC = -57.112444; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 263.402083; sd.DEC = -37.103592; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.6
            sd.RA = 81.572667; sd.DEC = 28.607944; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.7
            sd.RA = 84.053036; sd.DEC = -1.201936; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.7
            sd.RA = 138.3011; sd.DEC = -69.717511; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.7
            sd.RA = 332.057467; sd.DEC = -46.960611; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.7
            sd.RA = 51.080328; sd.DEC = 49.861425; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 85.189661; sd.DEC = -1.942631; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 107.097869; sd.DEC = -26.393228; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 122.383244; sd.DEC = -47.336678; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 165.933119; sd.DEC = 61.751217; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 193.506328; sd.DEC = 55.959886; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 276.043078; sd.DEC = -34.383839; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.8
            sd.RA = 89.882578; sd.DEC = 44.947439; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 99.427786; sd.DEC = 16.399444; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 125.628828; sd.DEC = -59.509672; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 206.886036; sd.DEC = 49.313372; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 252.165703; sd.DEC = -69.027475; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 264.329619; sd.DEC = -42.997819; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 306.411661; sd.DEC = -56.734472; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	1.9
            sd.RA = 31.792761; sd.DEC = 23.462822; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2
            sd.RA = 37.956661; sd.DEC = 89.264197; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2
            sd.RA = 95.674494; sd.DEC = -17.956014; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2
            sd.RA = 131.175661; sd.DEC = -54.707814; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2
            sd.RA = 141.896786; sd.DEC = -8.658856; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2
            sd.RA = 2.096375; sd.DEC = 29.090194; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 10.896692; sd.DEC = -17.9867; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 17.432428; sd.DEC = 35.620858; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 86.939119; sd.DEC = -9.669636; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 177.266211; sd.DEC = 14.572278; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 186.652994; sd.DEC = -63.099361; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 200.980161; sd.DEC = 54.925292; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 211.672089; sd.DEC = -36.368639; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 222.676203; sd.DEC = 74.155253; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 263.733311; sd.DEC = 12.560667; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 283.815828; sd.DEC = -26.296353; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 326.046161; sd.DEC = 9.875042; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.1
            sd.RA = 10.126328; sd.DEC = 56.537603; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 30.974453; sd.DEC = 42.330031; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 83.001494; sd.DEC = -0.299075; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 136.998958; sd.DEC = -43.432675; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 190.380067; sd.DEC = -48.959667; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 233.6716; sd.DEC = 26.714944; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 269.151578; sd.DEC = 51.489036; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 305.556786; sd.DEC = 40.256742; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 340.665494; sd.DEC = -46.884594; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.2
            sd.RA = 2.291458; sd.DEC = 59.150444; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 14.176911; sd.DEC = 60.716736; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 47.042203; sd.DEC = 40.955669; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 120.896328; sd.DEC = -40.003303; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 139.272744; sd.DEC = -59.275328; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 204.971994; sd.DEC = -53.466217; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 218.877083; sd.DEC = -42.157622; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 220.482244; sd.DEC = -47.388044; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 240.083453; sd.DEC = -22.6215; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 252.542978; sd.DEC = -34.29255; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.3
            sd.RA = 6.5702; sd.DEC = -42.304956; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.4
            sd.RA = 165.459619; sd.DEC = 56.382247; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.4
            sd.RA = 178.456869; sd.DEC = 53.694708; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.4
            sd.RA = 257.594328; sd.DEC = -15.725339; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.4
            sd.RA = 265.622036; sd.DEC = -39.029833; sd.Mag = 2.0; Planet.Precession_JST(sd.RA, sd.DEC, DateTime.Now, out sd.RA, out sd.DEC); star_data.Add(sd);//	2.4

        }
        public static void init_planet()
        {
            last_update = DateTime.Now;
            DateTime dt_jst = last_update;
            double pt = Planet.planet_time_jst_datetime(dt_jst);

            // [0] 月
            sd.Name = "Moon";
            Planet.moonTopoRADEC(pt, out sd.RA, out sd.DEC);
            sd.Mag = -10;
            star_data.Add(sd);

            // [1] 金星
            sd.Name = "Venus";
            Planet.venus(pt, out sd.RA, out sd.DEC);
            sd.Mag = -4.5;
            star_data.Add(sd);

            // [2] 木星
            sd.Name = "Jupiter";
            Planet.jupiter(pt, out sd.RA, out sd.DEC);
            sd.Mag = -3.0;
            star_data.Add(sd);

            // [3] 土星
            sd.Name = "Saturn";
            Planet.saturn(pt, out sd.RA, out sd.DEC);
            sd.Mag = -1.0;
            star_data.Add(sd);

            // [4] 火星
            sd.Name = "Mars";
            Planet.mars(pt, out sd.RA, out sd.DEC);
            sd.Mag = -1.0;
            star_data.Add(sd);

            // [5] 水星
            sd.Name = "Mer";
            Planet.mars(pt, out sd.RA, out sd.DEC); //未実装
            sd.Mag = -1.0;
            star_data.Add(sd);
        }
        public static void cal_azalt()
        {
            double az, alt;
            Planet.Eq2AzAlt_Yokohama(star_data[ID].RA, star_data[ID].DEC, DateTime.Now, out az, out alt);
            Az = az; Alt = alt;
        }
    }
}

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

  /*      public Star_Data(double ra = 0)
        {
            RA = 0;
            DEC = 0;
            Mag = 0;
            Name = "";
        }
   */ 
    }

    public class Star
    {
        static List< Star_Data > star_data = new List<Star_Data>();
        static Star_Data sd = new Star_Data();
        int id ;

        public static void init() 
        {
            // シリウス
            sd.RA   = 100.736308; // deg
            sd.DEC  = -16.646211; // deg
            sd.Mag  = -1.5;
            sd.Name = "Sirius";
            star_data.Add(sd);

            // ベガ
            sd.RA   = 278.811063; // deg
            sd.RA   = +38.736022; // deg
            sd.Mag  = 0.0;
            sd.Name = "Vega";
            star_data.Add(sd);

        }

        public int ID
        {
            set { id = value; }
            get { return id; }
        }
    }
}

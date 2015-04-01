using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtLibrary
{
    public class JulianDay
    {
        private const long DayOfTicks = 864000000000;       // 一日は 864000000000 Ticks
        private const double FirstDayOfJulianDay = 1721425.5;   // 西暦1年1月1日0時0分0秒はユリウス日で 1721425.5
        private const double ModifiedValue = 2400000.5;      // 修正ユリウス日の補正値

        // ユリウス日
        // ユリウス日から DateTime に変換
        public static DateTime JulianDayToDateTime(double julianDay)
        {
            return (new DateTime((long)((julianDay - FirstDayOfJulianDay) * DayOfTicks)));
        }
        // DateTime からユリウス日に変換
        public static double DateTimeToJulianDay(DateTime dateTime)
        {
            return ((dateTime.Ticks + (FirstDayOfJulianDay * DayOfTicks)) / DayOfTicks);
        }

        // 修正ユリウス日
        // 修正ユリウス日から DateTime に変換
        public static DateTime ModifiedJulianDayToDateTime(double julianDay)
        {
            return (JulianDayToDateTime(julianDay + ModifiedValue));
        }

        // DateTime から修正ユリウス日に変換
        public static double DateTimeToModifiedJulianDay(DateTime dateTime)
        {
            return (DateTimeToJulianDay(dateTime) - ModifiedValue);
        }

        // Chronological Julian Day
        // Chronological Julian Day から DateTime に変換
        static DateTime ChronologicalJulianDayToDateTime(double julianDay)
        {
            return (JulianDayToDateTime(julianDay - 0.5));
        }
        // DateTime から Chronological Julian Day に変換
        static double DateTimeToChronologicalJulianDay(DateTime dateTime)
        {
            return (DateTimeToJulianDay(dateTime) + 0.5);
        }


        ///---------------------------------------------------------------------------
        /// 恒星時
        /// In:時刻t(JST) 経度lon(deg)     Out:恒星時(deg)
        ///---------------------------------------------------------------------------
        public static double SiderealTime(DateTime td, double lon)
        {
            double gsd; //グリニジ恒星時
            double sd;  //恒星時
            double tu;  //
            double ti;  //その日の９時からの経過時間(day)
            double d0;  //その日の９時のTDateTime値

            double mjd = DateTimeToModifiedJulianDay(td);
            double t= (int)mjd -15019.5 ; 
            d0 = (int)(t);
            ti = mjd - (int)mjd - 0.375; // 時差9hを引く 9/24h=0.375
            tu = t / 36525;
            gsd = ((0.2769193981 + 100.0021359 * tu + 1.075e-6 * tu * tu) * 360) % 360 ;
            sd = gsd + lon + 1.00273791 * ti * 360;
            return (sd % 360);//(fmod(sd,360)) ;
        }
        public static double GSD(DateTime td)
        {
            double gsd; //グリニジ恒星時
            double tu;  //
            double d0;  //その日の９時のTDateTime値

            double mjd = DateTimeToModifiedJulianDay(td);
            double t = mjd - 15019.5; 

            d0 = (int)(t);
            tu = (d0 - 1.5) / 36525;
            gsd = (0.2769193981 + 100.0021359 * tu + 1.075e-6 * tu * tu) * 360;
            return (gsd % 360);
        }

    }
}

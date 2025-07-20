using UnityEngine;

namespace Scripts.EcoSystem.Calendar
{
    public static class CalendarConsts
    {
        public static readonly string[] MonthNames =
        {
            "JANUARY",
            "FEBRUARY",
            "MARCH",
            "APRIL",
            "MAY",
            "JUNE",
            "JULY",
            "AUGUST",
            "SEPTEMBER",
            "OCTOBER",
            "NOVEMBER",
            "DECEMBER",
        };

        public static string GetMonthName(int month)
        {
            return MonthNames[Mathf.Clamp(month - 1, 0, 11)];
        }
        
        public static readonly string[] MiniMonthNames =
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Seb",
            "Oct",
            "Nov",
            "Dec",
        };
        
        public static string GetMiniMonthName(int month)
        {
            return MiniMonthNames[Mathf.Clamp(month - 1, 0, 11)];
        }

        public static readonly string[] MiniDayNames =
        {
            "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"
        };

        public static string GetMiniDayName(int absoluteDayNumber)
        {
            int index = (absoluteDayNumber - 1) % 7;
            return MiniDayNames[index];
        }
        
        public static readonly int[] DaysInMonths = 
        {
            31, // Январь
            28, // Февраль
            31, // Март
            30, // Апрель
            31, // Май
            30, // Июнь
            31, // Июль
            31, // Август
            30, // Сентябрь
            31, // Октябрь
            30, // Ноябрь
            31  // Декабрь
        };

    }
}
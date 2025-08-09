using UnityEngine;

namespace Scripts.EcoSystem.Calendar
{
    public static class CalendarExtentions
    {
        public static readonly string[] MonthNames =
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
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
        
        public static int GetDayOfWeek(int day, int month, int year)
        {
            System.DateTime date = new System.DateTime(year, month, day);
            int dow = (int)date.DayOfWeek;
            return dow == 0 ? 6 : dow - 1;
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
        public static int GetDaysInMonth(int month, int year)
        {
            if (month < 1 || month > 12)
            {
                Debug.LogWarning($"[CalendarUtils] Invalid month: {month}");
                return 30;
            }

            return DaysInMonths[month - 1];
        }


    }
}
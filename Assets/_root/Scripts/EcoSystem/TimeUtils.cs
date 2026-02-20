using UnityEngine;

public static class TimeUtils
{
    public static void NormalizedToClock(
        float day01,
        out int hours,
        out int minutes)
    {
        day01 = Mathf.Clamp01(day01);

        float totalMinutes = day01 * 24 * 60f;

        hours = Mathf.FloorToInt(totalMinutes / 60f);
        minutes = Mathf.FloorToInt(totalMinutes % 60f);
    }
    
    public static float ClockToNormalized(
        int hours,
        int minutes)
    {
        hours = Mathf.Clamp(hours, 0, 24 - 1);
        minutes = Mathf.Clamp(minutes, 0, 59);

        float totalMinutes = hours * 60f + minutes;
        float maxMinutes = 24 * 60f;

        return Mathf.Clamp01(totalMinutes / maxMinutes);
    }
}
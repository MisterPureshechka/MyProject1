using System.Collections.Generic;
using _root.Notification;
using UnityEngine;

namespace Scripts.Messenger
{
    public class ComeBackStore
    {
        private const string Key = "ComeBackEvents_v1";

        [System.Serializable]
        private class Wrapper { public List<CalendarEvent> Items = new(); }

        public static void Save(List<CalendarEvent> list)
        {
            var json = JsonUtility.ToJson(new Wrapper { Items = list });
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }

        public static List<CalendarEvent> Load()
        {
            if (!PlayerPrefs.HasKey(Key)) return new();
            var json = PlayerPrefs.GetString(Key);
            var w = JsonUtility.FromJson<Wrapper>(json);
            return w?.Items ?? new();
        }
    }
}
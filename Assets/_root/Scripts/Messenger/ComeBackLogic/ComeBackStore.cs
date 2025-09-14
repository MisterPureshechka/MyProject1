using System;
using System.Collections.Generic;
using _root.Notification;
using UnityEngine;

namespace Scripts.Messenger.ComeBackLogic
{
    public class ComeBackStore : IComeBackStore
    {
        private const string Key = "ComeBackRecordsV1";

        [Serializable] private class Wrapper { public List<ComeBackRecord> items = new(); }

        public void Append(ComeBackRecord record)
        {
            var list = Load();
            list.Add(record);
            Save(list);
        }

        public List<ComeBackRecord> PullAll()
        {
            var list = Load();
            PlayerPrefs.DeleteKey(Key);
            return list;
        }

        private List<ComeBackRecord> Load()
        {
            var json = PlayerPrefs.GetString(Key, JsonUtility.ToJson(new Wrapper()));
            var wrap = JsonUtility.FromJson<Wrapper>(json);
            return wrap?.items ?? new List<ComeBackRecord>();
        }

        private void Save(List<ComeBackRecord> list)
        {
            var wrap = new Wrapper { items = list };
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(wrap));
            PlayerPrefs.Save();
        }
    }
}
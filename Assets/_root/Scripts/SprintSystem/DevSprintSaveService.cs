using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Tasks
{
    public class DevSprintSaveService
    {
        private const string Key = "Sprints.Dev.Tasks";
        private readonly ProgressDataAdapter _adapter;
        private readonly TaskLibrary _taskLibrary;

        public DevSprintSaveService(ProgressDataAdapter adapter, TaskLibrary taskLibrary)
        {
            _adapter = adapter;
            _taskLibrary = taskLibrary;
        }

        public void Save(List<ITask> tasks)
        {
            var data = new DevSprintSaveData();

            foreach (var t in tasks)
            {
                if (t is IDevTask dev)
                {
                    data.Tasks.Add(new DevTaskSnapshot
                    {
                        DevType = dev.Type.ToString(),
                        Title   = t.Title,
                        Progress = t.Progress,
                        IsCompleted = t.IsCompleted
                    });
                }
            }

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            _adapter.SaveCustomJson(Key, json);
        }

        // Загрузить Dev задачи, восстановив их через TaskLibrary
        public List<ITask> Load()
        {
            var result = new List<ITask>();
            var json = _adapter.LoadCustomJson(Key);
            if (string.IsNullOrEmpty(json)) return result;

            DevSprintSaveData data;
            try { data = JsonConvert.DeserializeObject<DevSprintSaveData>(json); }
            catch (Exception e)
            {
                Debug.LogError($"DevSprintSaveService.Load error: {e.Message}");
                return result;
            }

            if (data?.Tasks == null) return result;

            var allDev = _taskLibrary.GetAlLDevTasks(); // DevTaskType -> List<IDevTask> 

            foreach (var s in data.Tasks)
            {
                if (!Enum.TryParse(s.DevType, out DevTaskType type)) continue;
                if (!allDev.TryGetValue(type, out var list)) continue;

                var proto = list.Find(t => t.Title == s.Title);
                if (proto == null) continue;

                var clone = proto.Clone();                 // у DevTask клон переносит Id, это ок для рантайма 
                clone.Progress = s.Progress;
                if (s.IsCompleted && !clone.IsCompleted)
                    clone.ApplyProgress(float.MaxValue);   // добить до completed без эвентов, если ок для тебя

                result.Add(clone);
            }

            return result;
        }

        public void Clear()
        {
            _adapter.SaveCustomJson(Key, "{}");
        }
    }
}
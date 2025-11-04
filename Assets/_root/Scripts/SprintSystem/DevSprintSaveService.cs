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
            _adapter      = adapter;
            _taskLibrary  = taskLibrary;
        }

        /// <summary>
        /// Сохранение всех текущих Dev-задач (только Dev).
        /// </summary>
        public void Save(List<ITask> tasks)
        {
            var data = new DevSprintSaveData();

            foreach (var t in tasks)
            {
                // сохраняем только DevTask, чтобы забирать полный снапшот
                if (t is DevTask dt)
                {
                    data.Tasks.Add(dt.ToSnapshot());
                }
                else if (t is IDevTask dev) // на всякий случай — минимальный фолбэк
                {
                    data.Tasks.Add(new DevTaskSnapshot
                    {
                        DevType       = dev.Type.ToString(),
                        Title         = t.Title,
                        Progress      = t.Progress,
                        MaxProgress   = t.MaxProgress,
                        IsCompleted   = t.IsCompleted,
                        // bug-поля не известны, если это не DevTask
                    });
                }
            }

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            _adapter.SaveCustomJson(Key, json);
        }

        /// <summary>
        /// Загрузка Dev-задач: клонируем из TaskLibrary и накатываем снапшот БЕЗ событий.
        /// </summary>
        public List<ITask> Load()
        {
            var result = new List<ITask>();
            var json   = _adapter.LoadCustomJson(Key);
            if (string.IsNullOrEmpty(json)) return result;

            DevSprintSaveData data;
            try
            {
                data = JsonConvert.DeserializeObject<DevSprintSaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"DevSprintSaveService.Load error: {e.Message}");
                return result;
            }

            if (data?.Tasks == null || data.Tasks.Count == 0)
                return result;

            // TaskLibrary: DevTaskType -> List<IDevTask> (прототипы)
            var allDev = _taskLibrary.GetAlLDevTasks();

            foreach (var s in data.Tasks)
            {
                if (string.IsNullOrEmpty(s.DevType) || string.IsNullOrEmpty(s.Title))
                    continue;

                if (!Enum.TryParse(s.DevType, out DevTaskType type))
                    continue;

                if (!allDev.TryGetValue(type, out var list) || list == null)
                    continue;

                // Находим прототип по Title
                var proto = list.Find(t => t.Title == s.Title);
                if (proto == null) continue;

                // Клонируем, чтобы получить реальную задачу текущего рантайма
                var clone = proto.Clone();

                // Восстановление: только прямые присваивания (без событий)
                if (clone is DevTask dt)
                {
                    dt.RestoreFromSnapshot(s);
                }
                else
                {
                    // минимальное восстановление для других реализаций IDevTask
                    clone.Progress    = s.Progress;
                    // clone.IsCompleted — менять корректно можно только в конкретной реализации
                    // поэтому оставляем как есть (или доведёшь вручную в своей реализации)
                }

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
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Progress;
using Scripts.Tasks;

namespace Scripts.GameDev
{
    public class GameDevProgress
    {
        private const string Key = "Dev.Progress";

        private readonly ProgressDataAdapterOLD _adapterOld;                 
        private readonly Dictionary<string, GameProgressData> _games = new();

        public GameDevProgress(ProgressDataAdapterOLD adapterOld)
        {
            _adapterOld = adapterOld;
            Load(); 
        }

        public void CreateOrSelectGame(string gameName)
        {
            if (!_games.ContainsKey(gameName))
                _games[gameName] = new GameProgressData(gameName);
        }

        public void CompleteTask(string gameName, IDevTask task, bool countByTitle = true)
        {
            if (!_games.TryGetValue(gameName, out var gameData))
            {
                gameData = new GameProgressData(gameName);
                _games[gameName] = gameData;
            }

            // учёт
            if (countByTitle)
            {
                if (gameData.CompletedByTitle.ContainsKey(task.Title))
                    gameData.CompletedByTitle[task.Title] += task.Result;
                else
                    gameData.CompletedByTitle[task.Title] = task.Result;
            }

            gameData.CompletedByType[task.Type] += task.Result;

            Save(); // сразу сохраняем снапшот в Custom
        }

        public GameProgressData GetGameProgress(string gameName)
        {
            _games.TryGetValue(gameName, out var gameData);
            return gameData;
        }

        public Dictionary<string, GameProgressData> GetAllGames() => _games;

        // ---------- Persist ----------

        private void Save()
        {
            var snap = new GameDevProgressSnapshot();

            foreach (var (name, data) in _games)
            {
                var gs = new GameProgressSnapshot { GameName = name };

                // Title -> count
                foreach (var kv in data.CompletedByTitle)
                    gs.CompletedByTitle[kv.Key] = kv.Value;

                // DevTaskType -> count (как string)
                foreach (var kv in data.CompletedByType)
                    gs.CompletedByType[kv.Key.ToString()] = kv.Value;

                snap.Games[name] = gs;
            }

            string json = JsonConvert.SerializeObject(snap, Formatting.Indented);
            _adapterOld.SaveCustomJson(Key, json);
        }

        private void Load()
        {
            string json = _adapterOld.LoadCustomJson(Key);
            if (string.IsNullOrEmpty(json)) return;

            GameDevProgressSnapshot snap;
            try { snap = JsonConvert.DeserializeObject<GameDevProgressSnapshot>(json); }
            catch { return; }

            if (snap?.Games == null) return;

            _games.Clear();

            foreach (var (name, gs) in snap.Games)
            {
                var data = new GameProgressData(gs.GameName ?? name);

                // Title -> count
                if (gs.CompletedByTitle != null)
                    foreach (var kv in gs.CompletedByTitle)
                        data.CompletedByTitle[kv.Key] = kv.Value;

                // string -> enum
                if (gs.CompletedByType != null)
                {
                    foreach (var kv in gs.CompletedByType)
                    {
                        if (Enum.TryParse<DevTaskType>(kv.Key, out var type))
                            data.CompletedByType[type] = kv.Value;
                    }
                }

                _games[data.GameName] = data;
            }
        }
    }
}
using System;
using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.Progress;
using Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Rooms.SlotLogic
{
    public sealed class RoomSlotFiller : IController
    {
        private readonly RoomLogic _logic;
        private readonly RoomItemDatabase _dataBase;
        private readonly ProgressData _progressData;

        public RoomSlotFiller(RoomLogic logic, RoomItemDatabase dataBase, ProgressData progressData)
        {
            _logic = logic;
            _dataBase = dataBase;
            _progressData = progressData;
            
            LoadProgress();
        }

        public void LoadProgress()
        {
            var progress = _progressData;
            
            if (progress == null)
            {
                Debug.LogWarning("ProgressFiller: progress is null");
                return;
            }

            if (progress.Items == null || progress.Items.Count == 0)
                return;

            foreach (var p in progress.Items)
            {
                if (string.IsNullOrEmpty(p.ItemId))
                    continue;

                var itemData = _dataBase.GetById(p.ItemId);
                if (itemData == null)
                {
                    Debug.LogWarning($"ProgressFiller: ItemId '{p.ItemId}' not found in database");
                    continue;
                }

                _logic.PlaceItem(p.Column, new RoomItem(itemData));
            }
        }
    }
}
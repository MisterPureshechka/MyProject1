using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Rooms.RoomItems;
using Scripts.Rooms.SlotLogic;

namespace Scripts.Rooms.RoomItems
{
    public class RoomItemViewFactory
    {
        public RoomItemView Create(RoomItem item, Transform parent)
        {
            var config = item.Config;

            if (config == null)
            {
                Debug.LogError("RoomItem has null Config");
                return null;
            }

            if (config.Prefab == null)
            {
                Debug.LogError($"RoomItemConfig {config.name} has no Prefab assigned");
                return null;
            }

            var view = Object.Instantiate(config.Prefab, parent).GetComponent<RoomItemView>();
            view.Init(item);

            return view;
        }
    }

}
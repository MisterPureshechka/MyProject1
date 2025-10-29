using System;
using System.Collections.Generic;
using Scripts.Rooms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Upgrade
{
    [Serializable]
    public class UpgradableData
    {
        public InteractiveObjectType IOType;
        [FormerlySerializedAs("IOSprite")] public List<UpgradableObjectItem> UpgradebleObjectData;
    }

    [Serializable]
    public class UpgradableObjectItem
    {
        public int Price;
        public Sprite Clean;
        public Sprite Dirty;
    }
}
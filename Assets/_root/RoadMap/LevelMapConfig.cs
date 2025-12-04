using System.Collections.Generic;
using UnityEngine;

namespace _root.Planning
{
    [CreateAssetMenu(fileName = "LevelMapConfig", menuName = "ScriptableObjects/LevelMapConfig")]
    public class LevelMapConfig : ScriptableObject
    {
        public string StartNodeId;
        public List<LevelNode> LevelNodes;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace _root.Planning
{
    [CreateAssetMenu(fileName = "LevelNode", menuName = "LevelNode", order = 1)]
    public class LevelNode : ScriptableObject
    {
        public string Id;
        public NodeType Type;
        public List<string> NextNodeIds;
    }
}
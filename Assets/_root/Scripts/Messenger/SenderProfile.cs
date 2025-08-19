using System;
using UnityEngine;

namespace Scripts.Messenger
{
    [CreateAssetMenu(menuName = "Messenger", fileName = "Sender Profile")]
    public class SenderProfile : ScriptableObject
    {
        [SerializeField] private string _id;
        public string Id => _id;
    
        public string DisplayName;
        public Sprite Icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
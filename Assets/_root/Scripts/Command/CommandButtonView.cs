using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class CommandButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [field: SerializeField] public Button TaskPannelButton;
        
        public void Init(string info, Action onExecute = null)
        {
            _text.text = info;
            _text.raycastTarget = false;

            if (TaskPannelButton.targetGraphic != null)
                TaskPannelButton.targetGraphic.raycastTarget = true;
            
            TaskPannelButton.onClick.RemoveAllListeners();
            TaskPannelButton.onClick.AddListener(() =>
            {
                onExecute?.Invoke();
            });
        }
    }
}
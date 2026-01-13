using TMPro;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _sprintProgress;
        
        private int _currentProgress;

        public void UpdateProgress()
        {
            _currentProgress++;
            _sprintProgress.text = _currentProgress.ToString()+" / 100";
        }
    }
}
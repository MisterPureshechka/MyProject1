using Core;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Ui
{
    public class FaderLogic : ICleanUp
    {
        private readonly LocalEvents _events;

        private FaderView _faderView;
        
        public FaderLogic(LocalEvents events)
        {
            _events = events;
            _events.OnTaskCatalogShow += ShowFader;
            _events.OnTaskCatalogHide += HideFader;
            _faderView = Object.FindObjectOfType<FaderView>();
        }

        private void HideFader(SprintType sprintType)
        {
            _faderView.Hide();
        }

        private void ShowFader(SprintType sprintType)
        {
            _faderView.Show();
        }

        public void CleanUp()
        {
            _events.OnTaskCatalogShow -= ShowFader;
            _events.OnTaskCatalogHide -= HideFader;
        }
    }
}
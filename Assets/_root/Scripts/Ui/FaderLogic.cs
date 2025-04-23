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
            _events.OnAllTaskShow += ShowFader;
            _events.OnAllTaskHide += HideFader;
            _faderView = Object.FindObjectOfType<FaderView>();
        }

        private void HideFader()
        {
            _faderView.Hide();
        }

        private void ShowFader()
        {
            _faderView.Show();
        }

        public void CleanUp()
        {
            _events.OnAllTaskShow -= ShowFader;
            _events.OnAllTaskHide -= HideFader;
        }
    }
}
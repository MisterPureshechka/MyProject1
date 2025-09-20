using Core;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using Scripts.Job;
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
            _events.OnCatalogueShow += ShowFader;
            _events.OnCatalogueHide += HideFader;
            _events.OnExitEvent += HideFader;
            _faderView = Object.FindObjectOfType<FaderView>();
        }

        private void HideFader(ICatalogue catalogue)
        {
            _faderView.Hide();
        }

        private void HideFader(ExitEvent exitEvent)
        {
            _faderView.Hide();
        }

        private void ShowFader(ICatalogue catalogue)
        {
            _faderView.Show();
        }

        public void CleanUp()
        {
            _events.OnCatalogueShow -= ShowFader;
            _events.OnCatalogueHide -= HideFader;
            _events.OnExitEvent -= HideFader;
        }
    }
}
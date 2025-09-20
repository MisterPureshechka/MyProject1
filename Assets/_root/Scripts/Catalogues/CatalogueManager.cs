using Core;
using Scripts.ClickLogic;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using UnityEngine;

namespace Scripts.Catalogues
{
    public class CatalogueManager : ICleanUp
    {
        private ICatalogue _currentCatalogue;
        private readonly LocalEvents _localEvents;

        private bool _isTransition;

        public CatalogueManager(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _localEvents.OnCatalogueShow += ShowCatalogue;
            _localEvents.OnCatalogueHide += CloseCurrentCatalogue;
            _localEvents.OnExitEvent += CloseCurrentCatalogue;
        }

        public void ShowCatalogue(ICatalogue newCatalogue)
        {
            if (_isTransition)
                return;

            if (_currentCatalogue == newCatalogue)
                return; 

            _isTransition = true;

            if (_currentCatalogue != null)
            {
                _currentCatalogue.Hide(() =>
                {
                    _localEvents.TriggerClickStateChange(ClickState.UI);
                    _currentCatalogue = newCatalogue;
                    _currentCatalogue.Show(() =>
                    {
                        _isTransition = false;
                    });
                });
            }
            else
            {
                _currentCatalogue = newCatalogue;
                _localEvents.TriggerClickStateChange(ClickState.UI);
                _currentCatalogue.Show(() =>
                {
                    _isTransition = false;
                });
            }
        }

        public void CloseCurrentCatalogue(ICatalogue catalogue)
        {
            if (_currentCatalogue == catalogue)
            {
                _isTransition = true;
                _currentCatalogue.Hide(() =>
                {
                    _localEvents.TriggerClickStateChange(ClickState.Room);
                    _currentCatalogue = null;
                    _isTransition = false;
                });
            }
        }

        public void CloseCurrentCatalogue(ExitEvent exitEvent)
        {
            if (_currentCatalogue != null)
            {
                _isTransition = true;
                _currentCatalogue.Hide(() =>
                {
                    _localEvents.TriggerClickStateChange(ClickState.Room);
                    _currentCatalogue = null;
                    _isTransition = false;
                });
            }
        }

        public void CleanUp()
        {
            _localEvents.OnCatalogueShow -= ShowCatalogue;
            _localEvents.OnCatalogueHide -= CloseCurrentCatalogue;
            _localEvents.OnExitEvent -= CloseCurrentCatalogue;
        }
    }

}

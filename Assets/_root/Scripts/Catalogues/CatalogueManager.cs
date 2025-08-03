using Core;
using Scripts.GlobalStateMachine;
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
                    _currentCatalogue = newCatalogue;
                    _currentCatalogue.Show(() =>_isTransition = false);
                });
            }
            else
            {
                _currentCatalogue = newCatalogue;
                _currentCatalogue.Show(() =>_isTransition = false);
            }
        }

        public void CloseCurrentCatalogue(ICatalogue catalogue)
        {
            if (_currentCatalogue == catalogue)
            {
                _isTransition = true;
                _currentCatalogue.Hide(() =>
                {
                    _currentCatalogue = null;
                    _isTransition = false;
                });
            }
        }

        public void CleanUp()
        {
            _localEvents.OnCatalogueShow -= ShowCatalogue;
            _localEvents.OnCatalogueHide -= CloseCurrentCatalogue;
        }
    }

}

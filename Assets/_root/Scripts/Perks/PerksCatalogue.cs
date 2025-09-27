using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Perks
{
    public class PerksCatalogue : MonoBehaviour, ICatalogue
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _perkContainer;
        [SerializeField] private PerkPrefab _perkPrefab;
        [SerializeField] private PerksSprites _perksSprites;
        [SerializeField] private Button _applyButton; 
        [SerializeField] private Button _closeButton; 

        private LocalEvents _localEvents;
        private Sequence _sequence;

        private readonly Dictionary<string, PerkData> _allPerks = new(); 
        private readonly HashSet<string> _selectedIds = new();           

        private readonly Dictionary<string, PerkPrefab> _idToView = new();

        private Vector2 _startPosition = new Vector2(0f, 0f);
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -400f);

        private bool _isVisible;
        public bool IsVisible => _isVisible;

        public event Action<List<string>> OnApplySelectedPerks;

        private void Awake()
        {
            _hidePosition = _startPosition + _offset;
            
            if (_applyButton != null)
            {
                _applyButton.onClick.RemoveAllListeners();
                _applyButton.onClick.AddListener(ApplySelection);
                _applyButton.gameObject.SetActive(false);
            }
            HideOnStart();
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
        
        public void GetPerks(Dictionary<string, PerkData> perks)
        {
            _allPerks.Clear();
            _selectedIds.Clear();
            _idToView.Clear();

            foreach (Transform child in _perkContainer)
                Destroy(child.gameObject);

            foreach (var kv in perks)
            {
                string id = kv.Key;
                PerkData perk = kv.Value;

                _allPerks[id] = perk;

                var view = Instantiate(_perkPrefab, _perkContainer);
                view.SetInfo(perk, GetSpriteByName(perk.Name));
                view.Button.onClick.RemoveAllListeners();
                view.Button.onClick.AddListener(() => TogglePerk(id));

                _idToView[id] = view;
            }

            UpdateApplyButton();
        }

        public void GetPerks(Dictionary<string, PerkData> perks, IEnumerable<string> preselectedIds)
        {
            GetPerks(perks);

            if (preselectedIds != null)
            {
                foreach (var id in preselectedIds)
                    ForceSelect(id);
            }

            UpdateApplyButton();
        }
        
        private void ForceSelect(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            _selectedIds.Add(id);
            if (_idToView.TryGetValue(id, out var view))
                view.SetSelected(true);
        }

        private void TogglePerk(string perkId)
        {
            if (string.IsNullOrEmpty(perkId)) return;

            bool nowSelected;
            if (_selectedIds.Contains(perkId))
            {
                _selectedIds.Remove(perkId);
                nowSelected = false;
            }
            else
            {
                _selectedIds.Add(perkId);
                nowSelected = true;
            }

            if (_idToView.TryGetValue(perkId, out var view))
                view.SetSelected(nowSelected);

            UpdateApplyButton();
        }

        private void UpdateApplyButton()
        {
            if (_applyButton == null) return;

            bool hasSelection = _selectedIds.Count > 0;
            _applyButton.gameObject.SetActive(true);         
            _applyButton.interactable = hasSelection;       
        }

        private Sprite GetSpriteByName(string name)
        {
            var perkData = _perksSprites.PerkSprites.Find(data => data.Name == name);
            return perkData.Sprite;
        }

        private void ApplySelection()
        {
            var ids = new List<string>(_selectedIds);
            OnApplySelectedPerks?.Invoke(ids);
        }

        private void HideOnStart()
        {
            _root.gameObject.SetActive(false);
            _root.localPosition = _hidePosition;
            _isVisible = false;
        }

        public void Show(Action onComplete = null)
        {
            Debug.LogWarning("Show perks catalogue");
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            if (_applyButton != null)
            {
                _applyButton.gameObject.SetActive(true);
                UpdateApplyButton();
            }

            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOLocalMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
                _isVisible = true;
                onComplete?.Invoke();
            });
        }

        public void Hide(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Append(_root.transform.DOLocalMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
            _sequence.OnComplete(() =>
            {
                _root.gameObject.SetActive(false);
                _isVisible = false;
                onComplete?.Invoke();
            });
        }
    }
}

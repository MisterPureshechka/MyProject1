using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Planning
{
    public class LevelNodeView : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] Image _background;
        [SerializeField] Image _completeMark;
        [SerializeField] Image _currentMark;
        [SerializeField] TextMeshProUGUI _text;
        private NodeType _nodeType;
        private LevelNode _levelNodeData;
        private LevelMapController _levelNodeController;

        public LevelNode LevelNodeData => _levelNodeData;

        public void Init(LevelNode nodeData, LevelMapController controller)
        {
            _levelNodeData = nodeData;
            _levelNodeController = controller;

            _nodeType = nodeData.Type;
            UpdateVisual(nodeData);
            
            _completeMark.gameObject.SetActive(false);
            
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _levelNodeController.OnNodeClicked(this);
        }
        
        public void SetState(NodeViewState state)
        {
            switch (state)
            {
                case NodeViewState.Locked:
                    _button.interactable = false;
                    //_background.DOFade(0.5f, 0f);
                    break;

                case NodeViewState.Available:
                    _button.interactable = true;
                    _background.DOFade(1f, 0f);
                    break;

                case NodeViewState.Current:
                    _button.interactable = false;
                    _background.DOFade(1f, 0f);
                    break;

                case NodeViewState.Completed:
                    _button.interactable = false;
                    _completeMark.gameObject.SetActive(true);
                    _background.DOFade(1f, 0f);
                    break;
            }
        }

        public void SetCurrent(bool value)
        {
        }
        
        private void UpdateVisual(LevelNode nodeData)
        {
            _text.text = nodeData.Id;
            
            switch (nodeData.Type)
            {
                case NodeType.Work:
                    _background.color = new Color(0.9f, 0.9f, 0.9f);
                    break;
                case NodeType.Hire:
                    _background.color = new Color(1f, 0.8f, 0.2f);
                    break;
                case NodeType.Build:
                    _background.color = new Color(0.2f, 0.6f, 1f);
                    break;
                case NodeType.Upgrade:
                    _background.color = new Color(1f, 0.2f, 0.7f);
                    break;
                case NodeType.Perks:
                    _background.color = new Color(0.5f, 1f, 1f);
                    break;
                case NodeType.Release:
                    _background.color = new Color(1f, 0.5f, 0.5f);
                    break;
                case NodeType.OfficeUpgrade:
                    _background.color = new Color(0.3f, 0.6f, 1f);
                    break;
            }
        }
    }
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class TaskButtonsContainerView : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private TextMeshProUGUI _text;

        [Header("Settings")]
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _xOffset = 10; 
        [SerializeField] private float _yOffset = 10;  
        [SerializeField] private int _itemsPerRow = 3;
        [SerializeField] private float _randomOffsetAmount;

        private List<GameObject> _items = new List<GameObject>();

        public void AddToRoot(GameObject item)
        {
            item.transform.SetParent(_root);
            _items.Add(item);
        }
        
        public void UpdateAllItemsPositions()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].transform.localPosition = _offset + CalculatePosition(i);
            }
        }

        private Vector3 CalculatePosition(int index)
        {
            int row = index / _itemsPerRow;
            int col = index % _itemsPerRow;
            
            return new Vector3(
                col * _xOffset + Random.Range(-_randomOffsetAmount, _randomOffsetAmount),
                -row * _yOffset + Random.Range(-_randomOffsetAmount, _randomOffsetAmount),
                0f
            );
        }

        public void ClearAll()
        {
            foreach (var item in _items)
            {
                Destroy(item);
            }
            _items.Clear();
        }
        
        public void UpdateText(string text)
        {
            _text.text = text;
        }
    }
}
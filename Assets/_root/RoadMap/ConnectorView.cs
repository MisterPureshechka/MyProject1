using UnityEngine;
using UnityEngine.UI;

namespace _root.Planning
{
    public class ConnectorView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        
        public Image Image => _image;
    }
}
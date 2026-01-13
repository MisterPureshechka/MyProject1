using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class PCView : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        
        public Transform Root => _root;
    }
}
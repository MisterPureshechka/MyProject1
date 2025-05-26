using System.Collections.Generic;
using System.Linq;
using Core;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObjectRegisterer : IInitialization, ICleanUp
    {
        private readonly Dictionary<GameObject, IInteractiveObject> _interactiveObjectHash = new();
        private readonly Dictionary<InteractiveObjectType, IInteractiveObject> _interactiveObjectByTypeHash = new();
        
        public InteractiveObjectRegisterer(List<IInteractiveObject> interactiveObjects)
        {
            RegisterAllInteractiveObjects(interactiveObjects);
        }

        private void RegisterAllInteractiveObjects(List<IInteractiveObject> interactiveObjects)
        {
            foreach (var interactiveObject in interactiveObjects)
            {
                if (interactiveObject is MonoBehaviour monoBehaviour)
                {
                    _interactiveObjectHash[monoBehaviour.gameObject] = interactiveObject;
                    _interactiveObjectByTypeHash[interactiveObject.IOType] = interactiveObject;
                }
            }
        }

        public IInteractiveObject GetInteractiveObjectByType(InteractiveObjectType type)
        {
            _interactiveObjectByTypeHash.TryGetValue(type, out var result);
            return result;
        }

        public bool IsObjectRegistered(GameObject interactiveObject)
        {
            _interactiveObjectHash.TryGetValue(interactiveObject.gameObject, out var result);
            return result != null;
        }


        public void Initialize()
        {
        }

        public IInteractiveObject GetInteractiveObject(GameObject clickedObject)
        {
            _interactiveObjectHash.TryGetValue(clickedObject, out var result);
            return result;
        }

        public List<IInteractiveObject> GetInteractiveObjects()
        {
            return _interactiveObjectHash.Values.ToList();
        }

        public void CleanUp()
        {
            _interactiveObjectHash.Clear();
            _interactiveObjectByTypeHash.Clear();
        }
    }
}
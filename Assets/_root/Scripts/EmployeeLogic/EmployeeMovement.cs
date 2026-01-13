
using System;
using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class EmployeeMovement : ICleanUp
    {
        private readonly LocalEvents _localEvents;

        public EmployeeMovement(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _localEvents.OnEmployeeWalkToItem += HandleEmployeeWalkToItem;
        }

        private void HandleEmployeeWalkToItem(Employee employee, RoomItem roomItem)
        {
            if (employee == null)
            {
                Debug.LogWarning("Cannot move employee: Employee is null.");
                return;
            }

            if (roomItem == null)
            {
                Debug.LogWarning("Cannot move employee: RoomItem is null.");
                return;
            }

            if (employee.View == null)
            {
                Debug.LogWarning($"Cannot move employee: Employee {employee.Name} does not have an associated View.");
                return;
            }

            if (roomItem.View == null)
            {
                Debug.LogWarning($"Cannot move employee: RoomItem {roomItem.Name} does not have an associated View.");
                return;
            }

            MoveEmployeeToItem(employee, roomItem);
        }

        private void MoveEmployeeToItem(Employee employee, RoomItem roomItem)
        {
            var employeeTransform = employee.View.EmployeeView.transform;
            var targetPosition = roomItem.View.WalkToTransform.position;

            employee.View.StartCoroutine(MoveTowards(employeeTransform, targetPosition, employee, roomItem));
        }
        
        public void MoveEmployeeToWorkplace(Employee employee)
        {
            if (employee == null) return;

            var pcTransform = employee.View.PCView.transform;
            var employeeTransform = employee.View.EmployeeView.transform;

            employee.View.StartCoroutine(MoveTowards(employeeTransform, pcTransform.position, 
                () =>
                {
                    Debug.Log($"{employee.Name} достиг рабочего места.");
                    employee.ChangeState(EmployeeState.Work);
                }));
        }
        
        private System.Collections.IEnumerator MoveTowards(Transform employeeTransform, Vector3 destination, Action onComplete)
        {
            float speed = 3f;
            float fixedY = employeeTransform.position.y;
            float fixedZ = employeeTransform.position.z;

            while (Mathf.Abs(employeeTransform.position.x - destination.x) > 0.1f)
            {
                employeeTransform.position = Vector3.MoveTowards(
                    employeeTransform.position,
                    new Vector3(destination.x, fixedY, fixedZ),
                    speed * Time.deltaTime);
                yield return null;
            }

            onComplete?.Invoke();
        }
        
        private System.Collections.IEnumerator MoveTowards(Transform employeeTransform, Vector3 destination, Employee employee, RoomItem roomItem)
        {
            employee.PauseWork();
            
            float speed = 3f;
            float fixedY = employeeTransform.position.y;
            float fixedZ = employeeTransform.position.z;

            while (Mathf.Abs(employeeTransform.position.x - destination.x) > 0.1f)
            {
                employeeTransform.position = Vector3.MoveTowards(
                    employeeTransform.position,
                    new Vector3(destination.x, fixedY, fixedZ),
                    speed * Time.deltaTime
                );

                yield return null;
            }

            OnReachedTarget(employee, roomItem);
        }
        
        

        private void OnReachedTarget(Employee employee, RoomItem roomItem)
        {
            employee.InteractWithItemAsync(roomItem, () => MoveEmployeeToWorkplace(employee));
        }
        
        public void CleanUp()
        {
            _localEvents.OnEmployeeWalkToItem -= HandleEmployeeWalkToItem;
        }
    }
}
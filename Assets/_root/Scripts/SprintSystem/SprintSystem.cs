using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp, IExecute
    {
        private readonly SprintUI _sprintUI;
        private readonly LocalEvents _localEvents;
        private readonly TaskLibrary _taskLibrary;
        private readonly SprintView _sprintView;
        
        private readonly List<ITask> _pendingTasks = new();
        private readonly List<ITask> _activeTasks = new();
        
        private readonly Dictionary<DevTaskType, List<IDevTask>> _allDevTasks = new();
        
        public event Action<IDevTask> OnTaskUpdated;

        public SprintSystem(SprintView sprintView, SprintUI sprintUI, LocalEvents localEvents)
        {
            _sprintView = sprintView;
            _sprintUI = sprintUI;
            _localEvents = localEvents;
            
            _sprintView.Init(_localEvents);
            
            LoadSprint();

            _localEvents.OnApplyProgressToSprint += ApplyProgressToSprint;

            //_allDevTasks = _taskLibrary.GetAlLDevTasks();
        }

        private void LoadSprint()
        {
            
        }
        
        public void ApplyProgressToSprint()
        {
            _sprintUI.UpdateProgress();
            //OnTaskUpdated?.Invoke();
        }

        public void CleanUp()
        {
            _localEvents.OnApplyProgressToSprint -= ApplyProgressToSprint;
        }

        public void Execute(float deltatime)
        {
            if (Input.GetKeyDown(KeyCode.K))    
            {
                _localEvents.TriggerApplyProgressToSprint();
            } 
        }
    }
}
using System.Collections.Generic;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandManager 
    {
        private readonly LocalEvents _localEvents;
        public Dictionary<SprintType, List<Command>> Commands = new();
        
        private List<Command> _devCommands = new();

        private Command _createSprintCommand;
        private Command _continueSprintCommand;

        public CommandManager(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            LoadAllCommands();
            _localEvents.OnActiveState += SwitchSprintCommandState;
            _localEvents.OnSprintClosed += SprintCloseListener;
        }

        private void SprintCloseListener(SprintType obj)
        {
            Debug.Log("Sprint closed");
            _devCommands.Remove(_continueSprintCommand);
            _devCommands.Add(_createSprintCommand);
        }

        private void LoadAllCommands()
        {
            CreateDevCommands();

            var eatCommands = new List<Command>
            {
                new Command { 
                    CommandName = "Перекусить", 
                },
                new Command { 
                    CommandName = "Закончить перерыв"
                }
            };
            Commands.Add(SprintType.Eat, eatCommands);

            var chillCommands = new List<Command>
            {
                new Command
                {
                    CommandName = "Отдохнусть",
                    OnExecute = () => _localEvents.TriggerCreateAutoSprint(SprintType.Chill),
                }
            };
            Commands.Add(SprintType.Chill, chillCommands);
        }

        private void CreateDevCommands()
        {
            _createSprintCommand = new Command
            {
                CommandName = "Create Sprint",
                OnExecute = _localEvents.TriggerAllTaskShow,
            };
            _devCommands.Add(_createSprintCommand);
            _continueSprintCommand = new Command
            {
                CommandName = "Continue Sprint",
                OnExecute = () => _localEvents.TriggerSprintContinue(SprintType.Dev),
            };
            
            Commands.Add(SprintType.Dev, _devCommands);
        }

        public List<Command> GetCommandsForSprint(SprintType sprintType)
        {
            if (Commands.TryGetValue(sprintType, out var commands))
            {
                return commands;
            }
            return new List<Command>();
        }

        private void SwitchSprintCommandState(bool hasActiveState, SprintType sprintType)
        {
            if (sprintType == SprintType.Dev)
            {
                if (hasActiveState)
                {
                    if(_devCommands.Contains(_createSprintCommand)) _devCommands.Remove(_createSprintCommand);
                    if(!_devCommands.Contains(_continueSprintCommand)) _devCommands.Add(_continueSprintCommand);
                }
                else
                {
                    if(_devCommands.Contains(_continueSprintCommand)) _devCommands.Remove(_continueSprintCommand);
                    if(!_devCommands.Contains(_createSprintCommand)) _devCommands.Add(_createSprintCommand);
                }
            }
        }

        public void CleanUp()
        {
            _localEvents.OnActiveState -= SwitchSprintCommandState;
            _localEvents.OnSprintClosed -= SprintCloseListener;
        }

    }
}
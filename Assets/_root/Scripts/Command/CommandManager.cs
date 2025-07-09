using System.Collections.Generic;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandManager 
    {
        private readonly LocalEvents _localEvents;
        public Dictionary<InteractiveObjectType, List<Command>> Commands = new();
        
        private List<Command> _devCommands = new();

        private Command _createSprintCommand;
        private Command _continueSprintCommand;

        public CommandManager(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            LoadAllCommands();
            //_localEvents.OnActiveState += SwitchSprintCommandState;
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
            CreateReadCommands();

            var eatCommands = new List<Command>
            {
                new Command { 
                    CommandName = "Перекусить", 
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Eat),
                },
            };
            

            var chillCommands = new List<Command>
            {
                new Command
                {
                    CommandName = "Отдохнусть",
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Chill),
                }
            };

            var playCommand = new List<Command>
            {
                new Command
                {
                    CommandName = "Поиграть",
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Play),
                }
            };
            
            var toiletCommand = new List<Command>
            {
                new Command
                {
                    CommandName = "Справить нужду",
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Toilet),
                }
            };

            var bathCommand = new List<Command>
            {
                new Command()
                {
                    CommandName = "Take a shower",
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Wash),
                }

            };
            
            Commands.Add(InteractiveObjectType.Fridge, eatCommands);
            Commands.Add(InteractiveObjectType.Toilet, toiletCommand);
            Commands.Add(InteractiveObjectType.Chair, chillCommands);
            Commands.Add(InteractiveObjectType.TV, playCommand);
            Commands.Add(InteractiveObjectType.Bath, bathCommand);
        }

        private void CreateDevCommands()
        {
            _createSprintCommand = new Command
            {
                CommandName = "Create Sprint",
                OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Dev),
            };
            _devCommands.Add(_createSprintCommand);
            _continueSprintCommand = new Command
            {
                CommandName = "Continue Sprint",
                OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Dev),
            };
            
            Commands.Add(InteractiveObjectType.Pc, _devCommands);
        }

        private void CreateReadCommands()
        {
            var readCommands = new List<Command>
            {
                new Command
                {
                    CommandName = "Read Books",
                    OnExecute = () => _localEvents.TriggerWalkToIO(SprintType.Read),
                }
            };
            
            Commands.Add(InteractiveObjectType.Books, readCommands);
        }

        public List<Command> GetCommandsForSprint(InteractiveObjectType iOType)
        {
            if (Commands.TryGetValue(iOType, out var commands))
            {
                return commands;
            }
            return new List<Command>();
        }

        private void SwitchSprintCommandState(bool hasActiveState, InteractiveObjectType iOType)
        {
            if (iOType == InteractiveObjectType.Pc)
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
            //_localEvents.OnActiveState -= SwitchSprintCommandState;
            _localEvents.OnSprintClosed -= SprintCloseListener;
        }

    }
}
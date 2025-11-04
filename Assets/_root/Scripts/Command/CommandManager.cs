using System.Collections.Generic;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandManager 
    {
        private readonly LocalEvents _localEvents;
        public Dictionary<InteractiveObjectType, List<Command>> Commands = new();
        public Dictionary<InteractiveObjectType, List<PurchaseCommand>> PurchaseCommands = new();
        
        private List<Command> _devCommands = new();

        private Command _createSprintCommand;
        private Command _continueSprintCommand;
        
        private Command _cleanCommand;

        public CommandManager(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            LoadAllCommands();
            _localEvents.OnSprintClosed += SprintCloseListener;
            _localEvents.OnExitEventCreated += AddCommand;
            _localEvents.OnIODirty += AddOrRemoveCleanCommand;
            _localEvents.OnUpgradeOffer += UpgradeAvailableListener;
        }
        
        
        private static InteractiveObjectType GetPurchaseHost(InteractiveObjectType type)
        {
            switch (type)
            {
                case InteractiveObjectType.Pc:
                case InteractiveObjectType.Chair:
                    return InteractiveObjectType.Pc; 
                default:
                    return type;
            }
        }
        
        private void OnPurchaseResult(InteractiveObjectType type, bool success, int price)
        {
            if (success) return;

            var host = GetPurchaseHost(type);
            if (!PurchaseCommands.TryGetValue(host, out var list)) return;

            var command = list.Find(c => c.IoType == type);
            if (command == null) return;

            _localEvents.TriggerPurchaseFailed(type, price);
        }

        private void UpgradeAvailableListener(InteractiveObjectType type, bool available, int price)
        {
            var host = GetPurchaseHost(type);

            if (!PurchaseCommands.TryGetValue(host, out var list))
            {
                list = new List<PurchaseCommand>();
                PurchaseCommands[host] = list;
            }

            list.RemoveAll(c => c.IoType == type);

            if (!available) return;

            string title = type switch
            {
                InteractiveObjectType.Pc    => "Upgrade PC",
                InteractiveObjectType.Chair => "Upgrade Chair",
                InteractiveObjectType.TV    => "Upgrade TV",
                _                           => $"Upgrade {type}"
            };

            list.Add(new PurchaseCommand
            {
                CommandName = title,
                Price       = price,
                IoType      = type,
                OnExecute   = () =>
                {
                    _localEvents.TriggerPurchaseUpgradeRequested(type, price);
                },
            });
        }


        private void AddOrRemoveCleanCommand(InteractiveObjectType iO, bool isDirty)
        {
            if (!Commands.TryGetValue(iO, out var list))
            {
                list = new List<Command>();
                Commands[iO] = list;
            }

            list.RemoveAll(c => c.CommandName.StartsWith("Clean "));

            if (!isDirty) return;

            switch (iO)
            {
                case InteractiveObjectType.Pc:
                    list.Add(new Command {
                        CommandName = "Clean up PC",
                        OnExecute   = () => _localEvents.TriggerWalkToSprint(SprintType.CleanPc)
                    });
                    break;
                case InteractiveObjectType.Sofa:
                    list.Add(new Command {
                        CommandName = "Clean Sofa",
                        OnExecute   = () => _localEvents.TriggerWalkToSprint(SprintType.CleanFridge)
                    });
                    break;
                case InteractiveObjectType.Bath:
                    list.Add(new Command {
                        CommandName = "Clean Bath",
                        OnExecute   = () => _localEvents.TriggerWalkToSprint(SprintType.CleanBath)
                    });
                    break;
            }
        }

        private void AddCommand(ExitEvent exitEvent)
        {
            var eventCommand = new Command
            {
                CommandName = $"Go to {exitEvent.EventTitle}",
                OnExecute = () =>
                {
                    _localEvents.TriggerExitEvent(exitEvent);
                },
            };
            
            Commands[InteractiveObjectType.Door].Add(eventCommand);
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
                    CommandName = "Eat", 
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Eat),
                },
            };
            

            var chillCommands = new List<Command>
            {
                new Command
                {
                    CommandName = "Chill",
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Chill),
                }
            };

            var playCommand = new List<Command>
            {
                new Command
                {
                    CommandName = "Play games",
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Play),
                }
            };
            
            var toiletCommand = new List<Command>
            {
                new Command
                {
                    CommandName = "Use",
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Toilet),
                }
            };

            var bathCommand = new List<Command>
            {
                new Command()
                {
                    CommandName = "Take a shower",
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Shower),
                }

            };

            var bedCommand = new List<Command>
            {
                new Command()
                {
                    CommandName = "Go to sleep",
                    OnExecute = _localEvents.TriggerHeroGoToBed,
                }
            };

            var coffeeCommand = new List<Command>()
            {
                new Command()
                {
                    CommandName = "Grab a coffee",
                    OnExecute = () => _localEvents.TriggerWalkToIO(InteractiveObjectType.CoffeeMachine),
                }
            };
            
            Commands.Add(InteractiveObjectType.Fridge, eatCommands);
            Commands.Add(InteractiveObjectType.Toilet, toiletCommand);
            Commands.Add(InteractiveObjectType.Sofa, chillCommands);
            Commands.Add(InteractiveObjectType.TV, playCommand);
            Commands.Add(InteractiveObjectType.Bath, bathCommand);
            Commands.Add(InteractiveObjectType.Door, new List<Command>());
            Commands.Add(InteractiveObjectType.Bed, bedCommand);
            Commands.Add(InteractiveObjectType.CoffeeMachine, coffeeCommand);
        }

        private void CreateDevCommands()
        {
            _createSprintCommand = new Command
            {
                CommandName = "Create Sprint",
                OnExecute = () =>
                {
                    _localEvents.TriggerWalkToSprint(SprintType.Dev);
                },
                
            };
            _devCommands.Add(_createSprintCommand);
            _continueSprintCommand = new Command
            {
                CommandName = "Continue Sprint",
                OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Dev),
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
                    OnExecute = () => _localEvents.TriggerWalkToSprint(SprintType.Read),
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

        public List<PurchaseCommand> GetPurchasesForSprint(InteractiveObjectType iOType)
        {
            var host = GetPurchaseHost(iOType);
            return PurchaseCommands.TryGetValue(host, out var cmds) ? cmds : new List<PurchaseCommand>();
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
            _localEvents.OnSprintClosed -= SprintCloseListener;
            _localEvents.OnExitEventCreated -= AddCommand;
            _localEvents.OnIODirty -= AddOrRemoveCleanCommand;
            _localEvents.OnUpgradeOffer -= UpgradeAvailableListener;
        }

    }
}
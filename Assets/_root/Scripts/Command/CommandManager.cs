using System;
using System.Collections.Generic;

namespace Scripts.Tasks
{
    public class CommandManager
    {
        public Dictionary<SprintType, List<Command>> Commands = new();

        public CommandManager()
        {
            LoadAllCommands();
        }

        private void LoadAllCommands()
        {
            var devCommands = new List<Command>
            {
                new Command { 
                    CommandName = "Начать разработку", 
                },
                new Command { 
                    CommandName = "Продолжить разработку", 
                },
                new Command { 
                    CommandName = "Завершить спринт", 
                }
            };
            Commands.Add(SprintType.Dev, devCommands);

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
                    CommandName = "Отдохнусть"
                }
            };
            Commands.Add(SprintType.Chill, chillCommands);
        }

        public List<Command> GetCommandsForSprint(SprintType sprintType)
        {
            if (Commands.TryGetValue(sprintType, out var commands))
            {
                return commands;
            }
            return new List<Command>();
        }

    }

    public class Command
    {
        public string CommandName { get; set; }
        public Action OnExecute { get; set; }
    }
}
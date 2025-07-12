using System;
using Scripts.Data;
using Scripts.Tasks;
using Scripts.Ui.TaskUi;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Ui
{
    public class UiFactory
    {
        private readonly GameData _gameData;

        public UiFactory(GameData gameData)
        {
            _gameData = gameData;
        }

        public TaskView GetTaskView(Transform to)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TaskPrefab, to);

            return instance.GetComponent<TaskView>();
        }

        public TooltipView GetTooltip(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TooltipPrefab, canvasTransform);
            instance.gameObject.SetActive(false);

            return instance.GetComponent<TooltipView>();
        }

        public TooltipStatItem GetTooltipStatItem(Transform to)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TooltipPrefab, to);

            return instance.GetComponent<TooltipStatItem>();
        }
        
        public TaskPanelView GetTaskPanelView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TaskPanelPrefab, canvasTransform);

            return instance.GetComponent<TaskPanelView>();
        }

        public DevTaskCatalogue GetAllTaskView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.devTaskCatalogue, canvasTransform);

            return instance.GetComponent<DevTaskCatalogue>();
        }

        public TaskPanelButtonView GetTaskPanelButtonView(Transform to)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TaskPanelButton, to);

            return instance.GetComponent<TaskPanelButtonView>();
        }

        public SprintView GetSprintView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.SprintPrefab, canvasTransform);

            return instance.GetComponent<SprintView>();
        }


        public CommandPanelView GetCommandPanel(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.CommandPanelView, canvasTransform);

            return instance.GetComponent<CommandPanelView>(); 
        }

        public ReadTaskCatalogue GetReadTaskCatalogue(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.ReadTaskCatalogue, canvasTransform);

            return instance.GetComponent<ReadTaskCatalogue>(); 
        }
    }

    public class GameObjectFabric<T>
    {
        private readonly GameData _gameData;

        public GameObjectFabric(GameData gameData)
        {
            _gameData = gameData;
        }
        
        public T GetGameObject(GameObject prefab)
        {
            var instance = Object.Instantiate(prefab);
            var result = instance.GetComponent<T>();

            if (result != null)
            {
                return result;
            }
            
            throw new InvalidOperationException($"Can't get compoment {typeof(T)} from {prefab.name}");
        }
        
        public T GetGameObject(GameObject prefab, Transform parent)
        {
            var instance = Object.Instantiate(prefab, parent);
            var result = instance.GetComponent<T>();

            if (result != null)
            {
                return result;
            }
            
            throw new InvalidOperationException($"Can't get compoment {typeof(T)} from {prefab.name}");
        }
    }

}
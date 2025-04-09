using System;
using Scripts.Data;
using Scripts.Ui.TaskUi;
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
        
        public TaskPanelView GetTaskPanelView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TaskPanelPrefab, canvasTransform);

            return instance.GetComponent<TaskPanelView>();
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
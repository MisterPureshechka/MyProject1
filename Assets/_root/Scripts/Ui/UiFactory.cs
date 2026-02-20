using System;
using _root.Planning;
using _root.Scripts.Ui.Stats;
using Scripts.Data;
using Scripts.OnlineShop;
using Scripts.Perks;
using Scripts.Tasks;
using Scripts.Ui.EmployeeShop;
using Scripts.Ui.ItemShop;
using Scripts.Ui.SkillUpgrade;
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

        public EmployeeStats GetEmployeeStats(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.EmployeeStats, canvasTransform);
            instance.transform.SetAsFirstSibling();
            
            return instance.GetComponent<EmployeeStats>();
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

        public PerksCatalogue GetPerksCatalogue(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.PerksCatalogue, canvasTransform);
            
            return instance.GetComponent<PerksCatalogue>();
        }

        public TaskPanelButtonView GetTaskPanelButtonView(Transform to)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.TaskPanelButton, to);

            return instance.GetComponent<TaskPanelButtonView>();
        }
        
        public RoadMapView GetRoadMapView(Transform to)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.RoadMapPrefab, to);

            return instance;
        }

        public LevelNodeView GetLevelNodePrefab()
        {
            return _gameData.PrefabDataBase.LevelNodePrefab;
        }
        
        public ConnectorView GetConnectorPrefab()
        {
            return _gameData.PrefabDataBase.ConnectorPrefab;
        }

        public SprintView GetSprintView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.SprintPrefab, canvasTransform);
            instance.transform.SetAsFirstSibling();
            
            return instance.GetComponent<SprintView>();
        }


        public CommandPanelView GetCommandPanel(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.CommandPanelView, canvasTransform);

            return instance.GetComponent<CommandPanelView>(); 
        }

        public ItemShopView GetItemShopView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.ItemShopView, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<ItemShopView>(); 
        }
        
        public EmployeeShopView GetEmployeeShopView(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.EmployeeShopView, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<EmployeeShopView>(); 
        }

        public SkillUpgradeShopView GetUpgradeShop(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.UpgradeShopView, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<SkillUpgradeShopView>(); 
        }
        
        public HudView GetHud(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.Hud, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<HudView>(); 
        }
        
        public MainShopView GetShop(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.MainShop, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<MainShopView>(); 
        }
        
        public NextStateButton GetNextStateButton(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.NextStateButton, canvasTransform);
            instance.transform.SetAsFirstSibling();

            return instance.GetComponent<NextStateButton>(); 
        }

        public MilestoneResultView GetResultWindow(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.ResultWindow, canvasTransform);
            instance.transform.SetAsLastSibling();
            instance.gameObject.SetActive(false);

            return instance.GetComponent<MilestoneResultView>(); 
        }

        public ReleaseResultView GetReleaseWindow(Transform canvasTransform)
        {
            var instance = Object.Instantiate(_gameData.PrefabDataBase.ReleaseResult, canvasTransform);
            instance.transform.SetAsLastSibling();
            instance.gameObject.SetActive(false);

            return instance.GetComponent<ReleaseResultView>(); 
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
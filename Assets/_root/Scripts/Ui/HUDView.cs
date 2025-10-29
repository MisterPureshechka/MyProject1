using System;
using Scripts.Ui.TaskUi;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Ui
{
    public class HUDView : MonoBehaviour
    {
        public HealthBarView HealthBar;
        public KnowledgeBarView KnowledgeBar;
        public PassionStatBarView PassionBar;
        public SprintView SprintView;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
                
            }
        }
    }
}
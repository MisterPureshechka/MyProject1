using System.Collections.Generic;
using Core;
using Scripts.Config;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Progress
{
    public class ProjectProgressService : IController
    {
        private readonly ProgressDataAdapter _progress;
        private readonly SaveService _save;
        private readonly LocalEvents _localEvents;
        private readonly ReleaseResultService _releaseResultService;

        public ProjectProgressService(
            ProgressDataAdapter progress,
            SaveService save,
            LocalEvents localEvents,
            MilestoneRulesConfigAdapter rules)
        {
            _progress = progress;
            _save = save;
            _localEvents = localEvents;
            
            _releaseResultService = new ReleaseResultService(rules);
        }

        public bool OnMilestoneCompleted()
        {
            var data = _progress.Data;

            Debug.Log($"[PROGRESS] Before: stage={data.Stage} msIndex={data.CurrentMilestoneIndex}");

            data.CurrentMilestoneIndex++;

            bool releasedNow = false;

            if (IsStageCompleted(data))
            {
                Debug.Log($"[PROGRESS] Stage completed: stage={data.Stage} msIndex={data.CurrentMilestoneIndex}");

                if (data.Stage == ProjectStage.Polish)
                {
                    ReleaseGame(data);

                    data.PendingReleaseWindow = true;

                    releasedNow = true;
                }
                else
                {
                    AdvanceStage(data);
                }
            }

            Debug.Log($"[PROGRESS] After: stage={data.Stage} msIndex={data.CurrentMilestoneIndex} releasedNow={releasedNow}");

            _save.SaveProgress(data);
            return releasedNow;
        }

        private void ReleaseGame(ProgressData data)
        {
            data.Stage = ProjectStage.Released;

            var completedTasks = CollectCompletedTasks(data);
            var releaseResult = _releaseResultService.GenerateReleaseResult(data, completedTasks);
            data.LastReleaseResult = releaseResult;

            _localEvents.TriggerGameReleased();
            _localEvents.TriggerStageChanged(data.Stage);

            PrepareForNextGame(data);
        }

        private void PrepareForNextGame(ProgressData data)
        {
            data.GameIndex++;

            data.Stage = ProjectStage.Prototype; 
            data.CurrentMilestoneIndex = 0;

            data.MilestoneProgress = new MilestoneProgressData(); 
            //data.LastReleaseResult = null;
            //data.PendingReleaseWindow = false;

            data.MilestoneProgress.DoneTasksByType?.Clear();
            data.MilestoneProgress.DoneTasks = 0;

            _save.SaveProgress(data);

            Debug.Log($"[NEW GAME] Starting game #{data.GameIndex}");
        }
        
        private void AdvanceStage(ProgressData data)
        {
            switch (data.Stage)
            {
                case ProjectStage.Prototype:
                    data.Stage = ProjectStage.Production;
                    break;

                case ProjectStage.Production:
                    data.Stage = ProjectStage.Polish;
                    break;

                default:
                    return;
            }

            data.CurrentMilestoneIndex = 0;

            data.PendingReleaseWindow = false;

            _localEvents.TriggerStageChanged(data.Stage);
        }
        
        private Dictionary<DevTaskType, int> CollectCompletedTasks(ProgressData data)
        {
            if (data.MilestoneProgress == null || data.MilestoneProgress.DoneTasksByType == null)
            {
                return new Dictionary<DevTaskType, int>();
            }

            return new Dictionary<DevTaskType, int>(data.MilestoneProgress.DoneTasksByType);
        }


        private bool IsStageCompleted(ProgressData data)
        {
            return data.CurrentMilestoneIndex >= 1;
        }
    }
}
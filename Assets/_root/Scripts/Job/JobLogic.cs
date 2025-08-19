using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Job
{
    public class JobLogic : IExecute
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private IJob _currentJob;
        private JobLibrary _jobLibrary;
        private readonly LocalEvents _localEvents;

        public JobLogic(ProgressDataAdapter progressDataAdapter, JobLibrary jobLibrary, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _jobLibrary = jobLibrary;
            _localEvents = localEvents;
        }

        public void TryGetJob(IDevJob job)
        {
            var knowledge = GetKnowledge();
            if (job.TryGetJob(knowledge))
            {
                _currentJob = job;
                _localEvents.TriggerNewJobFound(job);
                Debug.Log($"Job found: {job.Name}");
            }
            else
            {
                Debug.LogError($"Job not found: {job.Name}");
            }
        }

        private Dictionary<DevTaskType, float> GetKnowledge()
        {
            var meta = _progressDataAdapter.GetProgressData().Metadata;
            var result = new Dictionary<DevTaskType, float>();

            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                result[type] = meta.GetValue(type.ToString());
            }

            return result;
        }


        public void Execute(float deltatime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int randomIndex = Random.Range(0, _jobLibrary.GetDevJobs().Count); 
                var job = _jobLibrary.GetDevJobs()[randomIndex];
                TryGetJob(job);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Core;
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

        public JobLogic(ProgressDataAdapter progressDataAdapter, JobLibrary jobLibrary)
        {
            _progressDataAdapter = progressDataAdapter;
            _jobLibrary = jobLibrary;
        }

        public void TryGetJob(IDevJob job)
        {
            var knowledge = GetKnowledge();
            if (job.TryGetJob(knowledge))
            {
                _currentJob = job;
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

    public class JobLibrary
    {
        private List<DevJob> _devJobs = new ();

        public JobLibrary()
        {
            LoadOrCreateDevJobs();
        }

        private void LoadOrCreateDevJobs()
        {
            var knowledgeToGetProgrammerJob = new Dictionary<DevTaskType, float>();
            knowledgeToGetProgrammerJob.Add(DevTaskType.Programming, 10f);
            knowledgeToGetProgrammerJob.Add(DevTaskType.Marketing, 1f);
            var programmer = new DevJob("Programmer", 200, "Programmer", 8, knowledgeToGetProgrammerJob);
            
            _devJobs.Add(programmer);
            
            var knowledgeToGetTechArtistJob = new Dictionary<DevTaskType, float>();
            knowledgeToGetTechArtistJob.Add(DevTaskType.Art, 50f);
            knowledgeToGetTechArtistJob.Add(DevTaskType.SoundDesign, 50f);
            knowledgeToGetTechArtistJob.Add(DevTaskType.Programming, 2f);
            var techArtist = new DevJob("Technical Artist", 600, "Technical Artist", 8, knowledgeToGetTechArtistJob);
            
            _devJobs.Add(techArtist);
            
            var knowledgeToGetSoundDesignerJob = new Dictionary<DevTaskType, float>();
            knowledgeToGetSoundDesignerJob.Add(DevTaskType.SoundDesign, 50f);
            knowledgeToGetSoundDesignerJob.Add(DevTaskType.Marketing, 5f);
            var soundDesigner = new DevJob("Sound Designer", 600, "Sound Designer", 8, knowledgeToGetSoundDesignerJob);
            
            _devJobs.Add(soundDesigner);
        }

        public List<DevJob> GetDevJobs()
        {
            return _devJobs;
        }
    }

    public interface IJob
    {
        string Name { get; }
        int Salary { get; }
        string Description { get; }
        int WorkStartTime { get; }
    }
    
    public interface IDevJob : IJob
    {
        Dictionary<DevTaskType, float> KnowledgeToGetJob { get; } 
        
        bool TryGetJob(Dictionary<DevTaskType, float> knowledgeToGetJob);
    }

    public class DevJob : IDevJob
    {
        public string Name { get; }
        public int Salary { get; }
        public string Description { get; }
        public int WorkStartTime { get; }
        public Dictionary<DevTaskType, float> KnowledgeToGetJob { get; }

        public DevJob(string name, int salary, string description, int workStartTime, Dictionary<DevTaskType, float> knowledgeToGetJob)
        {
            Name = name;
            Salary = salary;
            Description = description;
            WorkStartTime = workStartTime;
            KnowledgeToGetJob = knowledgeToGetJob;
        }

        public bool TryGetJob(Dictionary<DevTaskType, float> currentKnowledge)
        {
            foreach (var requirement in KnowledgeToGetJob)
            {

                if (!currentKnowledge.TryGetValue(requirement.Key, out var playerValue) 
                    || playerValue < requirement.Value)
                {
                    return false;
                }
            }

            return true; 
        }
    }
}
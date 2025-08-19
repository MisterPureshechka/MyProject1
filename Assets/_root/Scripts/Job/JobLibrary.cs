using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Job
{
    public class JobLibrary
    {
        private List<DevJob> _devJobs = new ();

        public JobLibrary()
        {
            LoadOrCreateDevJobs();
        }

        private void LoadOrCreateDevJobs()
        {
            // var knowledgeToGetProgrammerJob = new Dictionary<DevTaskType, float>();
            // knowledgeToGetProgrammerJob.Add(DevTaskType.Programming, 10f);
            // knowledgeToGetProgrammerJob.Add(DevTaskType.Marketing, 1f);
            // var programmer = new DevJob("Programmer", 200, "Programmer", 8, knowledgeToGetProgrammerJob);
            //
            // _devJobs.Add(programmer);
            //
            // var knowledgeToGetTechArtistJob = new Dictionary<DevTaskType, float>();
            // knowledgeToGetTechArtistJob.Add(DevTaskType.Art, 50f);
            // knowledgeToGetTechArtistJob.Add(DevTaskType.SoundDesign, 50f);
            // knowledgeToGetTechArtistJob.Add(DevTaskType.Programming, 2f);
            // var techArtist = new DevJob("Technical Artist", 600, "Technical Artist", 8, knowledgeToGetTechArtistJob);
            //
            // _devJobs.Add(techArtist);
            //
            // var knowledgeToGetSoundDesignerJob = new Dictionary<DevTaskType, float>();
            // knowledgeToGetSoundDesignerJob.Add(DevTaskType.SoundDesign, 50f);
            // knowledgeToGetSoundDesignerJob.Add(DevTaskType.Marketing, 5f);
            // var soundDesigner = new DevJob("Sound Designer", 600, "Sound Designer", 8, knowledgeToGetSoundDesignerJob);
            //
            // _devJobs.Add(soundDesigner);
        }

        public List<DevJob> GetDevJobs()
        {
            return _devJobs;
        }
    }
}
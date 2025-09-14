using System.Collections.Generic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Job
{
    public class JobLibrary
    {
        private readonly List<DevJob> _devJobs = new();

        public JobLibrary()
        {
            LoadOrCreateDevJobs();
        }

        private void LoadOrCreateDevJobs()
        {
            _devJobs.Clear();

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Junior Programmer", 300, new[] { 10, 20 }, "Fix UI bugs and simple gameplay tasks", 10,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Programming, 10f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Gameplay Programmer", 500, new[] { 5, 28 }, "Implement and maintain gameplay systems", 10,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Programming, 25f },
                    { DevTaskType.GameDesign, 10f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Tools Programmer", 550, new[] { 1, 25 }, "Editor tools and pipelines", 11,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Programming, 30f },
                    { DevTaskType.Art, 5f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Technical Artist", 600, new[] { 10, 25 }, "Shaders, VFX, content pipeline", 9,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Art, 40f },
                    { DevTaskType.Programming, 20f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "VFX Artist", 500, new[] { 12, 20 }, "Particles and real-time effects", 12,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Art, 35f },
                    { DevTaskType.Programming, 10f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "UI/UX Designer", 450, new[] { 11, 15 }, "UI flows, wireframes, assets", 11,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Art, 30f },
                    { DevTaskType.GameDesign, 15f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Level Designer", 480, new[] { 10, 28 }, "Blockouts, encounters, pacing", 10,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.GameDesign, 30f },
                    { DevTaskType.Programming, 5f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Game Designer", 620, new[] { 10, 28 }, "Systems, balance, documentation", 10,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.GameDesign, 45f },
                    { DevTaskType.Marketing, 10f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Sound Designer", 500, new[] { 13, 20 }, "SFX creation and integration", 13,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.SoundDesign, 40f },
                    { DevTaskType.Programming, 5f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Composer", 650, new[] { 13, 20 }, "Music production for levels and bosses", 13,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.SoundDesign, 55f },
                    { DevTaskType.Marketing, 5f }
                }));

            _devJobs.Add(new DevJob(
                "Journey & Journey",
                "Sarah Cole",
                "Technical Designer", 700, new[] { 9, 28 }, "Scripting + system design", 9,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.GameDesign, 35f },
                    { DevTaskType.Programming, 25f }
                }));

            _devJobs.Add(new DevJob(
                "UbiHard",
                "Sarah Cole",
                "Community Manager", 400, new[] { 12, 15 }, "Community, socials, feedback loops", 12,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Marketing, 30f },
                    { DevTaskType.GameDesign, 10f }
                }));

            _devJobs.Add(new DevJob(
                "MadTech Co.",
                "Sarah Cole",
                "Marketing Specialist", 550, new[] { 11, 25 }, "Campaigns, stores, assets", 11,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Marketing, 45f }
                }));

            _devJobs.Add(new DevJob(
                "Wild Shark Games",
                "Elice Dowson",
                "3D Artist", 520, new[] { 10, 25 }, "Modeling, UVs, materials", 10,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Art, 45f }
                }));

            _devJobs.Add(new DevJob(
                "PCMonsters",
                "John Bond",
                "Graphics Programmer", 800, new[] { 9, 28 }, "Rendering, performance, shaders", 9,
                new Dictionary<DevTaskType, float>
                {
                    { DevTaskType.Programming, 60f },
                    { DevTaskType.Art, 25f }
                }));
        }


        public List<DevJob> GetDevJobs() => _devJobs;

        public IJob GetDevJob()
        {
            return _devJobs[Random.Range(0, _devJobs.Count)];
        }
    }
}

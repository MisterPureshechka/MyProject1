using Core;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Sounds
{
    public class SoundService : ICleanUp
    {
        private LocalEvents _localEvents;
        private SoundConfig _soundConfig;
        
        private AudioSource _musicSource;
        private AudioSource _sprintSource;
        private AudioSource _backgroundSource;
        private AudioSource _sfxSource;
        private AudioSource _playerSource;

        public SoundService(LocalEvents localEvents, SoundConfig soundConfig)
        {
            _localEvents = localEvents;
            _soundConfig = soundConfig;
            
            _musicSource = new GameObject("_musicSource").AddComponent<AudioSource>();
            _sprintSource = new GameObject("_sprintSource").AddComponent<AudioSource>();
            _sprintSource.volume = 0.2f;
            _playerSource = new GameObject("_playerSource").AddComponent<AudioSource>();
            _backgroundSource = new GameObject("_backgroundSource").AddComponent<AudioSource>();
            _sfxSource = new GameObject("_sfxSource").AddComponent<AudioSource>();

            _localEvents.OnSprintCreated += PlaySoundBySprintType;
            _localEvents.OnSprintExit += StopSprintClip;
            _localEvents.OnHeroWalking += PlayWalk;
            _localEvents.OnCatalogueShow += PlayCatalogueSound;
            _localEvents.OnCatalogueHide += PlayCatalogueSound;
        }

        private void PlayCatalogueSound(ICatalogue catalogue)
        {
            PlaySFXClip(_soundConfig.OpenCatalogueClip);
        }

        private void PlayWalk(bool isWalking)
        {
            if (isWalking)
            {
                PlayPlayerClip(_soundConfig.StepClip);
            }
            else
            {
                StopPlayerClip();
            }
        }

        private void PlaySoundBySprintType(SprintType sprintType)
        {
            switch (sprintType)
            {
                case SprintType.None:
                    break;
                case SprintType.Dev:
                    PlaySprintClip(_soundConfig.DevClip);
                    break;
                case SprintType.Shower:
                    PlaySprintClip(_soundConfig.ShowerClip);
                    break;
                case SprintType.Read:
                    PlaySprintClip(_soundConfig.ReadClip);
                    break;
                case SprintType.Play:
                    PlaySprintClip(_soundConfig.PlayClip);
                    break;
            }
        }
        
        private void PlaySFXClip(AudioClip clip, bool loop = true)
        {
            _sfxSource.clip = clip;
            _sfxSource.loop = loop;
            _sfxSource.Play();
        }

        private void PlaySprintClip(AudioClip clip, bool loop = true)
        {
            _sprintSource.clip = clip;
            _sprintSource.loop = loop;
            _sprintSource.Play();
        }

        private void PlayPlayerClip(AudioClip clip, bool loop = true)
        {
            _playerSource.clip = clip;
            _playerSource.loop = loop;
            _playerSource.Play();
        }

        private void StopPlayerClip()
        {
            _playerSource.Stop();
        }

        private void StopSprintClip()
        {
            _sprintSource.Stop();
        }
        
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void CleanUp()
        {
            _localEvents.OnSprintCreated -= PlaySoundBySprintType;
            _localEvents.OnHeroWalking -= PlayWalk;
            _localEvents.OnCatalogueShow -= PlayCatalogueSound;
            _localEvents.OnCatalogueHide -= PlayCatalogueSound;
            _localEvents.OnSprintExit -= StopSprintClip;
        }
    }
}
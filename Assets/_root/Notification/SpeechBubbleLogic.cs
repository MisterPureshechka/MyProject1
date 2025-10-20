using Core;
using Scripts.GlobalStateMachine;
using Scripts.Hero;
using UnityEngine;

namespace _root.Notification
{
    public class SpeechBubbleLogic : ICleanUp
    {
        private SpeechBubbleView _speechBubbleView;
        private LocalEvents _localEvents;
        private readonly HeroView _heroView;
        private readonly Camera _camera;

        private Vector2 _bubblePosition;

        public SpeechBubbleLogic(SpeechBubbleView speechBubbleView, LocalEvents localEvents, HeroView heroView, Camera camera)
        {
            _speechBubbleView = speechBubbleView;
            _localEvents = localEvents;
            _heroView = heroView;
            _camera = camera;

            _localEvents.OnNotEnoughFood += NotEnoughFoodListener;
            _localEvents.OnNotEnoughMood += NotEnoughMoodListener;
            _localEvents.OnNotEnoughEnergy += NotEnoughEnergyListener;

            _localEvents.OnHeroWalkToSprint += HideBubble;
            _localEvents.OnHeroWalkToIO += HideBubble;
            _localEvents.OnClickEmpty += HideBubble;
        }
        
        private void UpdatePanelPosition(Vector3 worldPosition)
        {
            Vector2 screenPosition = _camera.WorldToScreenPoint(worldPosition);
            _speechBubbleView.SetPosition(screenPosition, _camera);
        }

        private void NotEnoughEnergyListener()
        {
            ShowBubble("Not Enough Energy");
        }

        private void NotEnoughMoodListener()
        {
            ShowBubble("Not Enough Mood");
        }

        private void NotEnoughFoodListener()
        {
            ShowBubble("Not Enough Food");
        }

        private void ShowBubble(string text)
        {
            UpdatePanelPosition(_heroView.transform.position);
            _speechBubbleView.ShowBubble(text);
        }

        private void HideBubble()
        {
            _speechBubbleView.HideBubble();
        }
        
        public void CleanUp()
        {
            _localEvents.OnNotEnoughFood -= NotEnoughFoodListener;
            _localEvents.OnNotEnoughMood -= NotEnoughMoodListener;
            _localEvents.OnNotEnoughEnergy -= NotEnoughEnergyListener;
            
            _localEvents.OnHeroWalkToSprint -= HideBubble;
            _localEvents.OnHeroWalkToIO -= HideBubble;
            _localEvents.OnClickEmpty -= HideBubble;
        }
    }
}
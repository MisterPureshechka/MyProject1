using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Messenger
{
    public class MessengerLogic : ICleanUp
    {
        private LocalEvents _localEvents;
        private readonly MessengerConfig _config;
        private MessengerCatalogue _messengerCatalogue;
        private MiniMessageButton _miniMessageButton;
        
        private readonly List<IMessageSender> _messages = new();
        private Dictionary<string, IMessageSender> _messageMap = new();
        private Dictionary<string, IMessageSender> _messageToDeleteMap = new();
        private int _cursor = -1;

        private readonly bool _wrapNavigation = false;     
        private readonly bool _openNewestOnShow = true;  

        public MessengerLogic(LocalEvents localEvents, MessengerConfig config)
        {
            _localEvents = localEvents;
            _config = config;

            _messengerCatalogue = Object.FindAnyObjectByType<MessengerCatalogue>(FindObjectsInactive.Include);
            if (_messengerCatalogue == null) Debug.LogError("Messenger catalogue could not be found");
            
            _miniMessageButton = Object.FindAnyObjectByType<MiniMessageButton>(FindObjectsInactive.Include);
            if (_miniMessageButton == null) Debug.LogError("MiniMessengerButton could not be found");
            
            _miniMessageButton?.ChangeMessageCount(_messages.Count);
            
            _miniMessageButton.Button.onClick.AddListener(ShowMessageCatalogue);
            _messengerCatalogue.CloseButton.onClick.AddListener(HideMessageCatalogue);
            _messengerCatalogue.NextButton.onClick.AddListener(ShowNextMessage);
            _messengerCatalogue.PreviousButton.onClick.AddListener(ShowPreviousMessage);
            
            _localEvents.OnNewMessageAddToMessenger += AddMessageToMessenger;
        }

       private void AddMessageToMessenger(IMessageSender sender)
        {
            _messages.Add(sender);
            _messageMap.Add(sender.Id, sender);

            if (_cursor == -1) _cursor = 0;

            UpdateUnreadUI();
            UpdateNavButtons();
        }

        private void ShowMessageCatalogue()
        {
            _localEvents.TriggerMessengerButtonClick();
            
            if (_messages.Count == 0) return;

            if (_openNewestOnShow) _cursor = 0;

            _localEvents.OnCatalogueShow(_messengerCatalogue);
            _messengerCatalogue.ShowMessage(_messages[_cursor]);
            
            _localEvents.TriggerMessegeReaded(_messages[_cursor].Id);
            _messageToDeleteMap.Add(_messages[_cursor].Id, _messages[_cursor]);

            UpdateNavButtons();
        }

        private void HideMessageCatalogue()
        {
            _localEvents.OnCatalogueHide(_messengerCatalogue);

            if (_messageToDeleteMap == null || _messageToDeleteMap.Count == 0) return;

            _messages.RemoveAll(m => m != null && _messageToDeleteMap.ContainsKey(m.Id));

            foreach (var kv in _messageToDeleteMap)
                _messageMap.Remove(kv.Key);

            _messageToDeleteMap.Clear();

            if (_cursor >= _messages.Count) _cursor = _messages.Count - 1;
            if (_cursor < 0 && _messages.Count > 0) _cursor = 0;

            UpdateUnreadUI();
            UpdateNavButtons();
        }

        private void ShowNextMessage()
        {
            if (_messages.Count == 0) 
            { 
                _messengerCatalogue.ShowNextButton(false); 
                _messengerCatalogue.ShowPreviousButton(false); 
                return; 
            }
            
            Debug.Log("Next pressed");

            if (_cursor < _messages.Count - 1) _cursor++;
            else if (_wrapNavigation) _cursor = 0;
            else return;

            _messengerCatalogue.ShowMessage(_messages[_cursor]);
            _localEvents.TriggerMessegeReaded(_messages[_cursor].Id);
            _messageToDeleteMap.Add(_messages[_cursor].Id, _messages[_cursor]);
            _messageMap.Remove(_messages[_cursor].Id);
            UpdateNavButtons();
        }

        private void ShowPreviousMessage()
        {
            if (_messages.Count == 0) return;

            Debug.Log("Previews pressed");
            
            if (_cursor > 0) _cursor--;
            else if (_wrapNavigation) _cursor = _messages.Count - 1;
            else return;

            _messengerCatalogue.ShowMessage(_messages[_cursor]);
            UpdateNavButtons();
        }

        private void AcceptCurrent()
        {
            if (!IsCursorValid()) return;

            _messages[_cursor].OnAccept?.Invoke();
            RemoveCurrentAndAdvance();
        }

        private void DeclineCurrent()
        {
            if (!IsCursorValid()) return;

            
            RemoveCurrentAndAdvance();
        }

        private void RemoveCurrentAndAdvance()
        {
            if (!IsCursorValid()) return;

            int removedIndex = _cursor;
            _messages.RemoveAt(removedIndex);

            if (_messages.Count == 0)
            {
                _cursor = -1;
                HideMessageCatalogue();
            }
            else
            {
                if (removedIndex >= _messages.Count)
                    _cursor = _messages.Count - 1; 

                _messengerCatalogue.ShowMessage(_messages[_cursor]);
            }

            UpdateUnreadUI();
            UpdateNavButtons();
        }
        
        private bool IsCursorValid() => _cursor >= 0 && _cursor < _messages.Count;

        private void UpdateUnreadUI()
        {
            _miniMessageButton?.ChangeMessageCount(_messages.Count);
        }

        private void UpdateNavButtons()
        {
            if (_messengerCatalogue == null) return;

            // Нет писем или курсор невалиден — обе кнопки спрятать
            if (_messages.Count <= 1 || _cursor < 0)
            {
                _messengerCatalogue.ShowPreviousButton(false);
                _messengerCatalogue.ShowNextButton(false);
                return;
            }

            // При круговой навигации показываем обе, если писем > 1
            if (_wrapNavigation)
            {
                bool show = _messages.Count > 1;
                _messengerCatalogue.ShowPreviousButton(show);
                _messengerCatalogue.ShowNextButton(show);
                return;
            }

            // Обычная навигация: прячем по краям
            bool isFirst = _cursor == 0;
            bool isLast  = _cursor == _messages.Count - 1;

            _messengerCatalogue.ShowPreviousButton(!isFirst);
            _messengerCatalogue.ShowNextButton(!isLast);
        }

        public void CleanUp()
        {
            _localEvents.OnNewMessageAddToMessenger -= AddMessageToMessenger;
            _miniMessageButton.Button.onClick.RemoveAllListeners();
            _messengerCatalogue.CloseButton.onClick.RemoveAllListeners();
        }
    }
}
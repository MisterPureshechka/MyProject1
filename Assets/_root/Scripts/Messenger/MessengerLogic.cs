using System.Collections.Generic;
using _root.Notification;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Messenger
{
    public class MessengerLogic : ICleanUp
    {
        private LocalEvents _localEvents;
        private readonly MessengerConfig _config;
        private readonly CalendarLogic _calendarLogic;
        private readonly TimeLogic _timeLogic;
        private MessengerCatalogue _messengerCatalogue;
        private MiniMessageButton _miniMessageButton;
        
        private readonly List<IMessageSender> _messages = new();
        private readonly List<IScheduleMessageSender> _scheduledMessages = new();
        private Dictionary<string, IMessageSender> _messageMap = new();
        private Dictionary<string, IMessageSender> _messageToDeleteMap = new();
        private readonly Dictionary<string, MessangerButtonState> _buttonStates = new();
        private int _cursor = -1;

        private readonly bool _wrapNavigation = false;     
        private readonly bool _openNewestOnShow = true;  

        public MessengerLogic(LocalEvents localEvents, MessengerConfig config, CalendarLogic calendarLogic, TimeLogic timeLogic)
        {
            _localEvents = localEvents;
            _config = config;
            _calendarLogic = calendarLogic;
            _timeLogic = timeLogic;

            _messengerCatalogue = Object.FindAnyObjectByType<MessengerCatalogue>(FindObjectsInactive.Include);
            if (_messengerCatalogue == null) Debug.LogError("Messenger catalogue could not be found");
            
            _miniMessageButton = Object.FindAnyObjectByType<MiniMessageButton>(FindObjectsInactive.Include);
            if (_miniMessageButton == null) Debug.LogError("MiniMessengerButton could not be found");
            
            _miniMessageButton?.ChangeMessageCount(_messages.Count);
            
            _miniMessageButton.Button.onClick.AddListener(ShowMessageCatalogue);
            _messengerCatalogue.CloseButton.onClick.AddListener(HideMessageCatalogue);
            _messengerCatalogue.NextButton.onClick.AddListener(ShowNextMessage);
            _messengerCatalogue.PreviousButton.onClick.AddListener(ShowPreviousMessage);

            _localEvents.OnNewMinute += HandleMessageTime;
            _localEvents.OnNewMessageAddToMessenger += AddMessageToMessenger;
            _localEvents.OnScheduleMessageAdded += AddScheduleMessage;
        }
        
        private void AddScheduleMessage(IScheduleMessageSender sm)
        {
            _scheduledMessages.Add(sm);
        }

        private void HandleMessageTime()
        {
            if (_scheduledMessages.Count == 0) return;

            var now = _calendarLogic.GetCurrentDate();
            int ch  = _timeLogic.CurrentHour;
            int cm  = _timeLogic.CurrentMinute;

            for (int i = _scheduledMessages.Count - 1; i >= 0; i--)
            {
                var sm = _scheduledMessages[i];
                if (IsDue(sm, now.Year, now.Month, now.Day, ch, cm))
                {
                    
                    AddMessageToMessenger(sm);

                    _scheduledMessages.RemoveAt(i);
                }
            }
        }
        
        private static bool IsDue(IScheduleMessageSender sm, int y, int m, int d, int h, int min)
        {
            if (y != sm.Year)   return y > sm.Year;
            if (m != sm.Month)  return m > sm.Month;
            if (d != sm.Day)    return d > sm.Day;
            if (h != sm.Hour)   return h > sm.Hour;
            return min >= sm.Minute;
        }
        
        private void AddMessageToMessenger(IMessageSender sender)
        {
            _messages.Add(sender);
            _messageMap.Add(sender.Id, sender);
            
            if (!_buttonStates.ContainsKey(sender.Id))
                _buttonStates[sender.Id] = MessangerButtonState.None;

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

            var msg = _messages[_cursor];
            var id = msg.Id;

            _messengerCatalogue.ShowMessage(
                msg,
                GetState(id),
                onAcceptPressed: () => { _buttonStates[id] = MessangerButtonState.Accepted; },
                onDeclinePressed: () => { _buttonStates[id] = MessangerButtonState.Declined; }
            );

            _localEvents.TriggerMessegeReaded(id);
            _messageToDeleteMap[id] = msg;

            UpdateNavButtons();
        }
        
        private MessangerButtonState GetState(string id) =>
            _buttonStates.TryGetValue(id, out var s) ? s : MessangerButtonState.None;


        private void HideMessageCatalogue()
        {
            _localEvents.OnCatalogueHide(_messengerCatalogue);
            if (_messageToDeleteMap == null || _messageToDeleteMap.Count == 0) return;

            _messages.RemoveAll(m => m != null && _messageToDeleteMap.ContainsKey(m.Id));
            foreach (var kv in _messageToDeleteMap)
            {
                _messageMap.Remove(kv.Key);
                _buttonStates.Remove(kv.Key); // <-- очистка состояния
            }
            _messageToDeleteMap.Clear();

            if (_cursor >= _messages.Count) _cursor = _messages.Count - 1;
            if (_cursor < 0 && _messages.Count > 0) _cursor = 0;

            UpdateUnreadUI();
            UpdateNavButtons();
        }


        private void ShowNextMessage()
        {
            if (_messages.Count == 0) { _messengerCatalogue.ShowNextButton(false); _messengerCatalogue.ShowPreviousButton(false); return; }
            if (_cursor < _messages.Count - 1) _cursor++;
            else if (_wrapNavigation) _cursor = 0;
            else return;

            var msg = _messages[_cursor];
            var id = msg.Id;

            _messengerCatalogue.ShowMessage(
                msg,
                GetState(id),
                onAcceptPressed: () => { _buttonStates[id] = MessangerButtonState.Accepted; },
                onDeclinePressed: () => { _buttonStates[id] = MessangerButtonState.Declined; }
            );

            _messengerCatalogue.Shake();

            _localEvents.TriggerMessegeReaded(id);
            if (!_messageToDeleteMap.ContainsKey(id)) _messageToDeleteMap[id] = msg;
            _messageMap.Remove(id);
            UpdateNavButtons();
        }

        private void ShowPreviousMessage()
        {
            if (_messages.Count == 0) return;
            if (_cursor > 0) _cursor--;
            else if (_wrapNavigation) _cursor = _messages.Count - 1;
            else return;

            var msg = _messages[_cursor];
            var id = msg.Id;

            _messengerCatalogue.ShowMessage(
                msg,
                GetState(id),
                onAcceptPressed: () => { _buttonStates[id] = MessangerButtonState.Accepted; },
                onDeclinePressed: () => { _buttonStates[id] = MessangerButtonState.Declined; }
            );
            
            _messengerCatalogue.Shake();

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

                //_messengerCatalogue.ShowMessage(_messages[_cursor]);
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

            if (_messages.Count <= 1 || _cursor < 0)
            {
                _messengerCatalogue.ShowPreviousButton(false);
                _messengerCatalogue.ShowNextButton(false);
                return;
            }

            if (_wrapNavigation)
            {
                bool show = _messages.Count > 1;
                _messengerCatalogue.ShowPreviousButton(show);
                _messengerCatalogue.ShowNextButton(show);
                return;
            }

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
            _localEvents.OnScheduleMessageAdded -= AddScheduleMessage;
        }
    }
}
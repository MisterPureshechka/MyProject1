using System;
using _root;
using _root.Notification;
using Core;
using Scripts.Catalogues;
using Scripts.ClickLogic;
using Scripts.Data;
using Scripts.Job;
using Scripts.Messenger;
using Scripts.Meta;
using Scripts.Passion;
using Scripts.Rooms;
using Scripts.Tasks;
using Scripts.Upgrade;
using Scripts.Wallet;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class LocalEvents : IController
    {
        public Action OnClosePanel { get; set; }
        public void TriggerClosePanel() => OnClosePanel?.Invoke();
        public Action<ClickState> OnClickStateChange { get; set; }
        public void TriggerClickStateChange(ClickState state) => OnClickStateChange?.Invoke(state);
        public Action OnOpenPanel { get; set; }
        public void TriggerOpenPanel() => OnOpenPanel?.Invoke();
        public Action<SprintType> OnHeroGetSprint { get; set; }
        public void TriggerHeroGetSprint(SprintType iOType) => OnHeroGetSprint?.Invoke(iOType);
        public Action<InteractiveObjectType> OnHeroGetIO { get; set; }
        public void TriggerHeroGetIO(InteractiveObjectType IOType) => OnHeroGetIO?.Invoke(IOType);
        
        public Action OnHeroGetExit { get; set; }
        public void TriggerHeroGetExit() => OnHeroGetExit?.Invoke();
        
        public Action<Vector2> OnMouseClickWorld {get; set;}
        public void TriggerMouseClickedWorld(Vector2 pos) => OnMouseClickWorld?.Invoke(pos);
        
        public Action<Vector2> OnMouseClickedUI;

        public void TriggerMouseClickedUI(Vector2 screenPos)
        {
            OnMouseClickedUI?.Invoke(screenPos);
        }

        public Action<CalendarEvent> OnSaveComeBackAction { get; set; }
        public void TriggerSaveComeBackAction() => OnSaveComeBackAction?.Invoke(null);

        public Action<InteractiveObjectType, Vector2> OnMouseClickIO { get; set; }
        public void TriggerMouseClickedIO(InteractiveObjectType iOType, Vector2 pos) => OnMouseClickIO?.Invoke(iOType, pos);
        public Action<Vector2> OnMousePositionChange { get; set; }
        public void TriggerMousePositionChange(Vector2 pos) => OnMousePositionChange?.Invoke(pos);

        public Action<bool> OnGetSupportedType { get; set; }
        public void GetSupportedTypeResult(bool isSupportedType) => OnGetSupportedType?.Invoke(isSupportedType);
        
        public Action OnTasksApply { get; set; }
        public void TriggerTasksApply() => OnTasksApply?.Invoke();
        
        public Action<Vector3> OnGetHeroPos { get; set; }
        public void TriggerGetHeroPos(Vector3 pos) => OnGetHeroPos?.Invoke(pos);
        
        public Action OnClickEmpty {get; set;}
        public void TriggerEmptyClick()
        {
            Debug.Log("EmptyClick!!!!!!!!!!!");
            OnClickEmpty?.Invoke();
        }

        public Action<SprintType> OnTaskCatalogShow { get; set; }
        public void TriggerAllTaskShow(SprintType type) => OnTaskCatalogShow?.Invoke(type);

        public Action<SprintType> OnTaskCatalogHide { get; set; }
        public void TriggerTaskCatalogHide(SprintType type) => OnTaskCatalogHide?.Invoke(type);
        
        public Action<SprintType> OnSprintCreated {get; set;}
        public void TriggerSprintCreated(SprintType type) => OnSprintCreated?.Invoke(type);

        public Action<SprintType> OnSprintContinue { get; set; }
        public void TriggerSprintContinue(SprintType sprintType) => OnSprintContinue?.Invoke(sprintType);

        public Action<bool, SprintType> OnActiveState { get; set; }
        public void TriggerActiveState(bool isActive, SprintType sprintType) => OnActiveState?.Invoke(isActive, sprintType);

        public Action OnActiveSprint { get; set; }
        
        public Action<SprintType> OnActiveSprintByType { get; set; }

        public void TriggerActiveSprintByType(SprintType sprintType)
        {
            OnActiveSprintByType?.Invoke(sprintType);
            OnActiveSprint?.Invoke();
        }
        
        public Action OnSprintExit { get; set; }
        public void TriggerSprintExit() => OnSprintExit?.Invoke();
        
        public Action<SprintType> OnSprintClosed { get; set; }
        public void TriggerSprintClosed(SprintType sprintType) => OnSprintClosed?.Invoke(sprintType);

        public Action<SprintType> OnWalkToSprint { get; set; }
        public void TriggerWalkToSprint(SprintType sprintType) => OnWalkToSprint?.Invoke(sprintType);
        
        public Action<InteractiveObjectType> OnWalkToIO { get; set; }
        public Action OnHeroWalkToIO { get; set; }
        public void TriggerWalkToIO(InteractiveObjectType ioType)
        {
            OnWalkToIO?.Invoke(ioType);
            OnHeroWalkToIO?.Invoke();
        }
        public void TriggerHeroWalkToIO() => OnHeroWalkToIO?.Invoke();
        public Action OnHeroWalkToSprint { get; set; }
        public void TriggerHeroWalkToSprint() => OnHeroWalkToSprint?.Invoke();

        public Action<SprintType> OnAutoSprintCreated { get; set; }
        public void TriggerCreateAutoSprint(SprintType type) => OnAutoSprintCreated?.Invoke(type);

        public Action<SprintType> OnSprintComplete { get; set; }
        public void TriggerSprintComplete(SprintType sprintType) => OnSprintComplete?.Invoke(sprintType);

        public Action<SprintType> OnHeroGetRootIO { get; set; }
        public void TriggerHeroGetRootIO(SprintType targetIOSprintType) => OnHeroGetRootIO?.Invoke(targetIOSprintType);

        public Action<bool> OnMouseOverSideRoom { get; set; }
        public void TriggerMouseOverSideRoom(bool sideRoomIsLeftRoom) => OnMouseOverSideRoom?.Invoke(sideRoomIsLeftRoom);
        
        public Action OnMouseOverMainRoom;
        public void TriggerMouseOverMainRoom() => OnMouseOverMainRoom?.Invoke();

        public Action OnMouseOverKitchen;
        public void TriggerMouseOverKitchen() => OnMouseOverKitchen?.Invoke();

        public Action OnMouseOverToilet;
        public void TriggerMouseOverToilet() => OnMouseOverToilet?.Invoke();
        public Action<MetaType, Vector2> OnMouseOverStat;
        public void TriggerMouseOverStat(MetaType metaType, Vector2 pos) => OnMouseOverStat?.Invoke(metaType, pos);

        public Action OnMouseExitStat;
        public void TriggerMouseExitStat() => OnMouseExitStat?.Invoke();

        public Action<DevTaskType> OnReadTaskUpdate;
        public void TriggerReadTaskUpdate(DevTaskType knowledgeToUpgrade) => OnReadTaskUpdate?.Invoke(knowledgeToUpgrade);

        public Action<float> OnDayTimeChange;
        public void TriggerOnDayTimeChange(float time) => OnDayTimeChange?.Invoke(time);

        public Action<float> OnNormilizeDayTimeChange;
        public void TriggerNormalizeDayTimeChange(float time) => OnNormilizeDayTimeChange?.Invoke(time);
        
        public Action<float> OnNormalizeNightTimeChange;
        public void TriggerNormalizeNightTimeChange(float time) => OnNormalizeNightTimeChange?.Invoke(time);

        public Action OnNewDay;
        public void TriggerNewDay() => OnNewDay?.Invoke();

        public Action OnNotEnoughFood;
        public void TriggerNotEnoughFood() => OnNotEnoughFood?.Invoke();
        
        public Action OnNotEnoughEnergy;
        public void TriggerNotEnoughEnergy() => OnNotEnoughEnergy?.Invoke();
        
        public Action OnNotEnoughMood;
        public void TriggerNotEnoughMood() => OnNotEnoughMood?.Invoke();
        
        public Action OnMiniCalendarButtonClick;
        public void TriggerMiniCalendarButtonOpen() => OnMiniCalendarButtonClick?.Invoke();
        
        public Action<ICatalogue> OnCatalogueShow {get; set;}
        public void TriggerShowCatalogue(ICatalogue catalogue) => OnCatalogueShow?.Invoke(catalogue);

        public Action<ICatalogue> OnCatalogueHide {get; set;}
        public Action OnWalletUpdate { get; set; }
        public void TriggerWalletUpdate() => OnWalletUpdate?.Invoke();
        public Action OnNewMinute {get; set;}
        public void TriggerNewMinute() => OnNewMinute?.Invoke();
        public void TriggerHideCatalogue(ICatalogue catalogue) => OnCatalogueHide?.Invoke(catalogue);
        public  Action<int, UpgradeType> OnUpgradeItem { get; set; }
        public void TriggerUpdateItem(int id, UpgradeType upgradeType) => OnUpgradeItem?.Invoke(id, upgradeType);
        public Action OnNewNotificatiom { get; set; }
        public void TriggerNewNotification() => OnNewNotificatiom?.Invoke();
        public Action<Vector2> OnMouseMoveStat { get; set; }
        public void TriggerMouseMoveStat(Vector2 eventDataPosition) => OnMouseMoveStat?.Invoke(eventDataPosition);
        public Action<MetaType> OnMouseEnterStat { get; set; }
        public void TriggerMouseEnterStat(MetaType metaType) => OnMouseEnterStat?.Invoke(metaType);
        public Action<string, int> OnCalendarNoteAdded { get; set; }
        public void TriggerCalendarNoteAdded(string note, int day) => OnCalendarNoteAdded?.Invoke(note, day);
        public Action<CalendarEvent> OnCalendarEventCreated { get; set; }
        public void TriggerCalendarEventCreated(CalendarEvent calendarEvent) => OnCalendarEventCreated?.Invoke(calendarEvent);
        public  Action<Notification> OnNewNotificationCreated { get; set; }
        public void TriggerNewNotificationCreated(Notification notification) => OnNewNotificationCreated?.Invoke(notification);
        public Action<IDevJob> OnNewJobFound { get; set; }
        public void TriggerNewJobFound(IDevJob job) => OnNewJobFound?.Invoke(job);
        public Action<IMessageSender> OnNewMessageAddToMessenger { get; set; }
        public void TriggerNewMessageAddToMassanger(IMessageSender sender) => OnNewMessageAddToMessenger?.Invoke(sender);
        public Action<IScheduleMessageSender> OnScheduleMessageAdded { get; set; }
        public void TriggerScheduleMessageAdded(IScheduleMessageSender sender) => OnScheduleMessageAdded?.Invoke(sender);
        public Action<string> OnMessageReaded { get; set; }
        public void TriggerMessegeReaded(string id) => OnMessageReaded?.Invoke(id);
        public Action OnMessangerButtonClick { get; set; }
        public void TriggerMessengerButtonClick() => OnMessangerButtonClick?.Invoke();
        public Action<int> OnHeroWalkToExit { get; set; }
        public void TriggerHeroWalkToExit(int hoursBeforeComeBack) => OnHeroWalkToExit?.Invoke(hoursBeforeComeBack);
       
        public Action<int> OnWalletAmountIncrease { get; set; }
        public void TriggerIncreaseWalletAmount(int currentJobSalary) => OnWalletAmountIncrease?.Invoke(currentJobSalary);
        public Action<int> OnPayDay { get; set; }
        public void TriggerRentPayDay(int currentFlatMonthPayment) => OnPayDay?.Invoke(currentFlatMonthPayment);
        public Action<Transaction> OnNewTransaction { get; set; }
        public void TriggerNewTransaction(Transaction transaction) => OnNewTransaction?.Invoke(transaction);

        public Action<ExitEvent> OnExitEvent { get; set; }
        public void TriggerExitEvent(ExitEvent jobExitEvent) => OnExitEvent?.Invoke(jobExitEvent);

        public Action<ExitEvent> OnExitEventWhenExit { get; set; }
        public void TriggerExitEventWhenExit(ExitEvent exitEvent) => OnExitEventWhenExit?.Invoke(exitEvent);

        public Action OnNewHour { get; set; }
        public void TriggerNewHour() => OnNewHour?.Invoke();
        
        public Action<ExitEvent> OnExitEventCreated { get; set; }
        public void TriggerExitEventCreated(ExitEvent exitEvent) => OnExitEventCreated?.Invoke(exitEvent);

        public Action OnHeroGoToBed { get; set; }
        public void TriggerHeroGoToBed() => OnHeroGoToBed?.Invoke();

        public Action OnHeroSleepState { get; set; }
        public void TriggerSleepState() => OnHeroSleepState?.Invoke();
        
        public Action<PassionIncreaseType> OnPassionIncrease { get; set; }
        public void TriggerPassionIncrease(PassionIncreaseType type) => OnPassionIncrease?.Invoke(type);
        
        public Action<DevTaskType> OnDevTaskComplete { get; set; }
        public void TriggerDevTaskComplete(DevTaskType devTaskType) => OnDevTaskComplete?.Invoke(devTaskType);

        public Action<MoodState> OnMoodStateChange { get; set; }
        public void TriggerEyesMoodChanged(MoodState currentMoodState) => OnMoodStateChange?.Invoke(currentMoodState);
        public Action<MoodState> OnBodyStateChange { get; set; }
        public void TriggerBodyMoodChanged(MoodState currentMoodState) => OnBodyStateChange?.Invoke(currentMoodState);
        public Action<CleanState> OnCleanStateChange { get; set; }
        public void TriggerCleanStateChange(CleanState currentCleanState) => OnCleanStateChange?.Invoke(currentCleanState);
        
        public Action<bool> OnTakeCoffee { get; set; }
        public void TriggerCoffee(bool hasCoffee) => OnTakeCoffee?.Invoke(hasCoffee);

        public Action OnHeroWokeUp { get; set; }
        public void TriggerHeroWokeUp() => OnHeroWokeUp?.Invoke();

        public Action<bool> OnLowEnergy { get; set; }
        public void TriggerLowEnergy(bool isLow) => OnLowEnergy?.Invoke(isLow);
        
        public Action OnBlockSprint { get; set; }
        public void TriggerBlockSprint() => OnBlockSprint?.Invoke();
    }
}
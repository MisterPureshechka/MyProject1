using Core;
using Scripts.GlobalStateMachine;

public class TimeService : IExecute
{
    public float Day01 { get; private set; } 

    private float _secondsPerDay = 240f;
    private readonly LocalEvents _localEvents;
    
    private bool _isRunning = true;

    public TimeService(LocalEvents localEvents)
    {
        _localEvents = localEvents;
        Day01 = TimeUtils.ClockToNormalized(9, 0);

        _localEvents.OnMilestoneResultWindow += MilestoneResultListener;
    }

    private void MilestoneResultListener()
    {
        _isRunning = false;
    }

    public void Execute(float deltaTime)
    {
        if (!_isRunning) return;
        
        float delta01 = deltaTime / _secondsPerDay;
        Day01 += delta01;

        if (Day01 >= 1f)
        {
            Day01 -= 1f; 
            _localEvents.TriggerDayPassed();
        }

        _localEvents.TriggerTimeUpdated(Day01);
    }
}
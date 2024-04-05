namespace AstroRaider2.Utility.Timers;

public class IntervalTimer
{
    private readonly double _intervalSeconds;

    private double _timeAccumulator;

    public IntervalTimer(double intervalSeconds)
    {
        _intervalSeconds = intervalSeconds;
    }

    public int AccumulateTimeReturnFiringCount(double delta)
    {
        // Accumulate the time...
        _timeAccumulator += delta;

        // Subtract the interval until we are back below the interval.
        // Each time through the loop represents a firing of the interval.
        int fireCount = 0;
        while (_timeAccumulator > _intervalSeconds)
        {
            _timeAccumulator -= _intervalSeconds;
            ++fireCount;
        }

        return fireCount;
    }
}
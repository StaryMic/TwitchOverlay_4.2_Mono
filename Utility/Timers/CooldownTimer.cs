using System;

namespace AstroRaider2.Utility.Timers;

public class CooldownTimer
{
    private readonly TimeSpan _cooldownTimeSpan;

    private DateTime _cooldownCompleted;

    public CooldownTimer(TimeSpan cooldownTimeSpan)
    {
        _cooldownCompleted = DateTime.UtcNow;
        _cooldownTimeSpan = cooldownTimeSpan;
    }
    
    public CooldownTimer(double cooldownTimeSeconds)
    {
        _cooldownCompleted = DateTime.UtcNow;
        _cooldownTimeSpan = TimeSpan.FromSeconds(cooldownTimeSeconds);
    }

    public void ResetCooldown()
    {
        _cooldownCompleted = DateTime.UtcNow.Add(_cooldownTimeSpan);
    }

    public bool HasCooldownElapsed()
    {
        return DateTime.UtcNow >= _cooldownCompleted;
    }
}
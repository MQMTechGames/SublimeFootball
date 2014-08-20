using UnityEngine;

public class Timer
{
    float _endTime;

    public void Wait(float seconds)
    {
        _endTime = Time.timeSinceLevelLoad + seconds;
    }

    public bool IsFinished()
    {
        return Time.timeSinceLevelLoad > _endTime;
    }

    public bool IsWaitting()
    {
        return !IsFinished();
    }
}

using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    GoalTrigger _trigger;

    void Awake()
    {
        _trigger = GameObjectUtils.FindRecursiveComponentByTypeDown<GoalTrigger>(gameObject);
        DebugUtils.Assert(_trigger != null, "_trigger != null");
    }

    public Vector3 GetRandomGoalPosition()
    {
        Bound triggerBounds = _trigger.GetTriggerDimensions();

        Vector3 randomPosition = new Vector3(
              UnityEngine.Random.Range(triggerBounds.Min.x, triggerBounds.Max.x)
            , UnityEngine.Random.Range(triggerBounds.Min.y, triggerBounds.Max.y)
            , UnityEngine.Random.Range(triggerBounds.Min.z, triggerBounds.Max.z)
            );

        return randomPosition;
    }

    public void SetOnTriggerEnterCallback(GoalTrigger.OnTriggerCallback callback)
    {
        _trigger.SetOnTriggerExitCallback(callback);
    }
    
    public void SetOnTriggerExitCallback(GoalTrigger.OnTriggerCallback callback)
    {
        _trigger.SetOnTriggerExitCallback(callback);
    }
}

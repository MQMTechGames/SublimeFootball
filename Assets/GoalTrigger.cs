using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    public delegate void OnTriggerCallback(Collider collider);

    private event OnTriggerCallback _ontriggerEnter;
    private event OnTriggerCallback _ontriggerExit;

    BoxCollider _collider;

    void Awake()
    {
        _collider = collider as BoxCollider;
    }

    public Bound GetTriggerDimensions()
    {
        Bound bound = new Bound();
        bound.Min = _collider.bounds.min;
        bound.Max = _collider.bounds.max;

        return bound;
    }

    public void SetOnTriggerEnterCallback(OnTriggerCallback callback)
    {
        _ontriggerEnter = callback;
    }

    public void SetOnTriggerExitCallback(OnTriggerCallback callback)
    {
        _ontriggerExit = callback;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(_ontriggerEnter != null)
        {
            _ontriggerEnter(collider);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(_ontriggerExit != null)
        {
            _ontriggerExit(collider);
        }
    }
}

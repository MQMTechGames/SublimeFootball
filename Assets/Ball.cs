using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody _rigidbody;
    protected new Rigidbody rigidbody
    {
        get{ return _rigidbody; }
    }

    public BaseUnit Owner{ get; set; }

    void Awake()
    {
        _rigidbody = GameObjectUtils.FindRecursiveComponentByTypeDown<Rigidbody>(gameObject);
        DebugUtils.Assert(_rigidbody != null, "_rigidbody != null");
    }

    public void KickToPosition( Vector3 destination, float maxHeight )
    {
        Owner = null;

        Vector3 vel;
        if(maxHeight == 0f)
        {
            vel = (destination - transform.position).normalized * 5f;
        }
        else
        {
            vel = ParabolicShot.CalculateVelocity(maxHeight, destination - transform.position);
        }

        _rigidbody.velocity = vel;
    }

    public void OnBeingControlled(BaseUnit owner)
    {
        Owner = owner;

        _rigidbody.velocity = Vector3.zero;
    }
}

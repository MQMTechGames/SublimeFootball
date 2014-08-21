using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    LayerMask _terrainLayer;

    [SerializeField]
    float _frictionStrength;

    bool _inTerrain = false;

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

    public void PassToPosition( Vector3 destination, float maxHeight )
    {
        Owner = null;

        Vector3 vel;
        if(Mathf.Abs(maxHeight) < 1e-1)
        {
            vel = (destination - transform.position).normalized * 65f;
        }
        else
        {
            vel = ParabolicShot.CalculateVelocity(maxHeight, destination - transform.position);
        }

        _rigidbody.velocity = vel;
    }
    public void ShootToPosition( Vector3 destination, float strength )
    {
        Owner = null;
        
        Vector3 vel = (destination - transform.position).normalized * strength;
        
        _rigidbody.velocity = vel;
    }

    void Update()
    {
        if(_inTerrain)
        {
            ReduceVelocity();
        }
    }

    public void OnBeingControlled(BaseUnit owner)
    {
        Owner = owner;

        _rigidbody.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision other)
    {
        if(CheckIsTerrain(other.gameObject))
        {
            _inTerrain = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(CheckIsTerrain(other.gameObject))
        {
            _inTerrain = false;
        }
    }

    bool CheckIsTerrain(GameObject go)
    {
        int layer = 1 << go.layer;
        int isTerrain = (layer & _terrainLayer.value);

        return isTerrain > 0;
    }

    void ReduceVelocity()
    {
        _rigidbody.velocity *= (1.0f - _frictionStrength * Time.deltaTime);
    }
}

using UnityEngine;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    [SerializeField]
    Vector3 _maxVelocity = new Vector3(100, 0, 100);

    Vector3 _velocity;

    void Update()
    {
        Vector3 newVel = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * Random.Range(0f, 1000f);
        _velocity += newVel * Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, _maxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, _maxVelocity.y);
        _velocity.z = Mathf.Min(_velocity.z, _maxVelocity.z);

        transform.position += _velocity * Time.deltaTime;
    }

    void OnCollisionEnter(Collision other)
    {
        if(1 << other.gameObject.layer == 1 << LayerMask.NameToLayer("invisibleWalls"))
        {
            Vector3 normal = other.contacts[0].normal.normalized;
            Vector3 reflVelocity = _velocity + 2f * Vector3.Dot(-_velocity, normal) * normal;

            _velocity = reflVelocity;
        }
    }
}

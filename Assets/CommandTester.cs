using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BaseUnit))]
public class CommandTester : MonoBehaviour
{
    [SerializeField]
    Vector3 targetWorldPosition;

    [SerializeField]
    bool doMove;

    BaseUnit _unit;


    void Awake()
    {
        _unit = GameObjectUtils.GetInterfaceObject<BaseUnit>(gameObject);
    }

    void Update()
    {
        if(doMove)
        {
            _unit.MoveTo(targetWorldPosition);
        }
    }
}

using UnityEngine;
using MQMTech.Unit.Mover;

[RequireComponent(typeof(BaseUnitMover))]
public class TestCustomMover : MonoBehaviour
{
    BaseUnitMover _mover;
    Match _match;

    void Awake()
    {
        _mover = GetComponent<BaseUnitMover>();

        _match = FindObjectOfType<Match>();
        DebugUtils.Assert(_match != null, "_match != null");
    }

    void Update()
    {
        Ball ball = _match.FindClosestBallToPosition(Vector3.zero);

        _mover.MoveTo(ball.transform.position);
    }
}

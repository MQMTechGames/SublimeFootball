using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class TestNavMeshAgent : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;
    Match _match;

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _match = FindObjectOfType<Match>();
        DebugUtils.Assert(_match != null, "_match != null");
    }

    void Update()
    {
        Ball ball = _match.FindClosestBallToPosition(Vector3.zero);

        _navMeshAgent.SetDestination(ball.transform.position);
    }
}

using UnityEngine;

public class SubBehaviorTreeLoader : MonoBehaviour
{
    [SerializeField]
    AttackWithoutBallTreeBuilder _attackBehaviorTreeBuilder;

    void Awake()
    {
        _attackBehaviorTreeBuilder.Create();
        BehaviorTreeManager.Add(BehaviorSubtreeNames.AttackWithoutBall, _attackBehaviorTreeBuilder.GetBehaviorTree());
    }
}

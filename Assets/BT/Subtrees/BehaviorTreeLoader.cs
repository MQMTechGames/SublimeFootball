using UnityEngine;

public class BehaviorTreeLoader : MonoBehaviour
{
    [SerializeField]
    AttackWithoutBallTreeBuilder _attackBehaviorTreeBuilder;

    void Awake()
    {
        _attackBehaviorTreeBuilder.Create();
        BehaviorTreeManager.Add(BehaviorSubtreeNames.AttackWithoutBall, _attackBehaviorTreeBuilder.GetBehaviorTree());
        BehaviorTreeManager.AddBuilder(BehaviorSubtreeNames.AttackWithoutBall, _attackBehaviorTreeBuilder);
    }
}

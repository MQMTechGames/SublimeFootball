using UnityEngine;

public class BehaviorTreeLoader : MonoBehaviour
{
    [SerializeField]
    DemoUnitBTBuilder _demoUnitBTBuilder;

    [SerializeField]
    AttackWithoutBallTreeBuilder _attackWithoutBallTreeBuilder;

    [SerializeField]
    AttackWithBallTreeBuilder _attackWithBallTreeBuilder;

    [SerializeField]
    DefensiveBallRecoverBuilder _defensiveBallRecoverBuilder;

    [SerializeField]
    NeutralBallRecoverBuilder _neutralBallRecoverBuilder;

    void Awake()
    {
        TreeLinkConfiguration attackWithoutBallLinkConfig = new TreeLinkConfiguration(TreeLinkConfiguration.LinkMemoryType.Instance, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked);
        BehaviorTreeManager.AddSubtree(BehaviortreeNames.AttackWithoutBall, _attackWithoutBallTreeBuilder, attackWithoutBallLinkConfig);

        TreeLinkConfiguration attackWithBallLinkConfig = new TreeLinkConfiguration(TreeLinkConfiguration.LinkMemoryType.Instance, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked);
        BehaviorTreeManager.AddSubtree(BehaviortreeNames.AttackWithall, _attackWithBallTreeBuilder, attackWithBallLinkConfig);

        TreeLinkConfiguration defensiveBallRecovery = new TreeLinkConfiguration(TreeLinkConfiguration.LinkMemoryType.Instance, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked);
        BehaviorTreeManager.AddSubtree(BehaviortreeNames.DefensiveBallRecover, _defensiveBallRecoverBuilder, defensiveBallRecovery);

        TreeLinkConfiguration neutralBallRecovery = new TreeLinkConfiguration(TreeLinkConfiguration.LinkMemoryType.Instance, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked, TreeLinkConfiguration.LinkMemoryType.Linked);
        BehaviorTreeManager.AddSubtree(BehaviortreeNames.NeutralBallRecover, _neutralBallRecoverBuilder, neutralBallRecovery);

        BehaviorTreeManager.AddTree(BehaviortreeNames.Footballer, _demoUnitBTBuilder);
    }
}

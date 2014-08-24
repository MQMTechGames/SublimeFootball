using UnityEngine;
using MQMTech.AI.BT;

public class AttackWithoutBallBehavior : SubtreeBehavior
{
    public AttackWithoutBallBehavior(AIMemoryKey iMoveSpeed, AIMemoryKey oMoveSpeed)
    {
        _subtreeName = BehaviorSubtreeNames.AttackWithoutBall;

        _inputs = new AIMemoryKeyPair[1];
        _outputs = new AIMemoryKeyPair[1];

        _inputs[0].Source = iMoveSpeed;
        _inputs[0].Target = UnitAIMemory.Ball;

        _outputs[0].Source = UnitAIMemory.Ball;
        _outputs[0].Target = oMoveSpeed;
    }
}

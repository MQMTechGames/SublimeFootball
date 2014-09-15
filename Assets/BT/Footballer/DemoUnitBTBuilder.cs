using UnityEngine;

using MQMTech.AI.BT;
using MQMTech.AI.Knowledge.Action;
using MQMTech.AI.Mover.Action;
using MQMTech.AI.Knowledge;

[System.Serializable]
public class DemoUnitBTBuilder : MonoBehaviour, IBehaviorWithTree, IBehaviorTreeBuilder
{
    BehaviorTree _bt;
    
    public BehaviorTree GetBehaviorTree()
    {
        return _bt;
    }
    
    public BehaviorTree Create()
    {
        Selector mainAI = new Selector();

        #region attack
        Selector chooseAttack = new Selector();
            chooseAttack.AddChild(new SubtreeBehavior(BehaviortreeNames.AttackWithall));
            chooseAttack.AddChild(new SubtreeBehavior(BehaviortreeNames.AttackWithoutBall));

        Sequence tryToAttack = new Sequence();
            tryToAttack.AddChild(new CheckAreEqualMemoryVars<BaseSquad>(UnitAIMemory.g_PossessionSquad, UnitAIMemory.Squad));
            tryToAttack.AddChild(chooseAttack);
        #endregion attack

        #region defend
        Sequence findAndSaveClosestBallProperties = new Sequence();
            findAndSaveClosestBallProperties.AddChild(new FindClosestBall(UnitAIMemory.Unit, UnitAIMemory.Ball));
            findAndSaveClosestBallProperties.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            findAndSaveClosestBallProperties.AddChild(new SavePositionFromTransform(UnitAIMemory.BallTransform, UnitAIMemory.BallPosition));

        Sequence tryToDefend = new Sequence();
            tryToDefend.AddChild(new CheckNotNullMemoryVar(UnitAIMemory.g_PossessionSquad));
            tryToDefend.AddChild(new Inverter().SetChild(new CheckAreEqualMemoryVars<BaseSquad>(UnitAIMemory.g_PossessionSquad, UnitAIMemory.Squad)));
            tryToDefend.AddChild(findAndSaveClosestBallProperties);
            tryToDefend.AddChild(new SubtreeBehavior(BehaviortreeNames.DefensiveBallRecover));
        #endregion defend

        #region recoverPossession
        Sequence checkBallIsMoving = new Sequence();
            checkBallIsMoving.AddChild(new SaveVelocityFromComponent(UnitAIMemory.Ball, UnitAIMemory.BallVelocity));
            checkBallIsMoving.AddChild(new CalculateMagnitudeFromVector(UnitAIMemory.BallVelocity, UnitAIMemory.BallVelocityMagnitude));
            checkBallIsMoving.AddChild(new SetMemoryVar<float>(UnitAIMemory.MinBallVelocityMagnitude, 1.0f));
            checkBallIsMoving.AddChild(new GreaterThan(UnitAIMemory.BallVelocityMagnitude, UnitAIMemory.MinBallVelocityMagnitude));

        Selector checkOnBallShotIsNullOrInvalid = new Selector();
            checkOnBallShotIsNullOrInvalid.AddChild(new CheckNullMemoryVar(UnitAIMemory.OnBallShot));
            checkOnBallShotIsNullOrInvalid.AddChild(new Inverter().SetChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.OnBallShot, 1f)));
            checkOnBallShotIsNullOrInvalid.AddChild(new Inverter().SetChild(checkBallIsMoving));

        Sequence tryToRecoverTheBallWithoutPossession = new Sequence();
            tryToRecoverTheBallWithoutPossession.AddChild(checkOnBallShotIsNullOrInvalid);
            tryToRecoverTheBallWithoutPossession.AddChild(new SubtreeBehavior(BehaviortreeNames.NeutralBallRecover));

        Sequence tryToRecoverPosession = new Sequence();
            tryToRecoverPosession.AddChild(new CheckNullMemoryVar(UnitAIMemory.g_PossessionSquad));
            tryToRecoverPosession.AddChild(tryToRecoverTheBallWithoutPossession);
        #endregion recoverPossession

        #region mainAI
        mainAI.AddChild(tryToAttack);
        mainAI.AddChild(tryToDefend);
        mainAI.AddChild(tryToRecoverPosession);
        #endregion mainAI

//        Sequence tryToReactOnSmash = new Sequence();
//            tryToReactOnSmash.AddChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.SmashedMessage, 1f));
//            tryToReactOnSmash.AddChild(new SetKnowledgeStatus(UnitAIMemory.SmashedMessage, KnowledgeStatus.PROCESSED));
//            tryToReactOnSmash.AddChild(new ProcessBeingSmashed(UnitAIMemory.Unit, UnitAIMemory.SmashedMessage));
//            tryToReactOnSmash.AddChild(new RemoveKnowledgeIfNotNew(UnitAIMemory.SmashedMessage));
//            tryToReactOnSmash.AddChild(unsetBallControlled);
//            tryToReactOnSmash.AddChild(unsetBallPosession);
//            tryToReactOnSmash.AddChild(new TimeWaiter(2f));

//        ActiveSelector AI = new ActiveSelector();
//            AI.AddChild(tryToReactOnSmash);
//            AI.AddChild(mainAI);

        _bt = new BehaviorTree(BehaviortreeNames.Footballer);

        Sequence setLocalVariables = new Sequence();
            setLocalVariables.AddChild(new SetMemoryVar<bool>(UnitAIMemory.TrueVar, true));
            setLocalVariables.AddChild(new SetMemoryVar<bool>(UnitAIMemory.FalseVar, false));
            setLocalVariables.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallDistance, 6f));
            setLocalVariables.AddChild(new SetMemoryVar<float>(UnitAIMemory.AttackSpeed, 66f));

        Sequence AI = new Sequence();
            AI.AddChild(setLocalVariables);
            AI.AddChild(new SaveUnitMemory(UnitAIMemory.Unit, UnitAIMemory.StartPosition));
            AI.AddChild(mainAI);
        
        _bt.Init(AI);
        return _bt;
    }
}

using UnityEngine;

using MQMTech.AI.BT;
using MQMTech.AI.Knowledge.Action;
using MQMTech.AI.Mover.Action;
using MQMTech.AI.Knowledge;

[System.Serializable]
public class DefensiveBallRecoverBuilder : MonoBehaviour, IBehaviorWithTree, IBehaviorTreeBuilder
{
    BehaviorTree _bt;
    
    public BehaviorTree GetBehaviorTree()
    {
        return _bt;
    }
    
    public BehaviorTree Create()
    {
//-- Find forward direction
        Sequence findTargetDirection = new Sequence();
            findTargetDirection.AddChild(new FindGoal(UnitAIMemory.Unit, UnitAIMemory.Goal));
            findTargetDirection.AddChild(new SavePositionFromComponent(UnitAIMemory.Goal, UnitAIMemory.GoalPosition));
            findTargetDirection.AddChild(new CalculateDirectionToPosition(UnitAIMemory.Unit, UnitAIMemory.GoalPosition, UnitAIMemory.TargetDirection));
            findTargetDirection.AddChild(new SetMemoryVar<Vector3>(UnitAIMemory.ForwardDirection, new Vector3(0.0f, 0.0f, 1.0f)));
            findTargetDirection.AddChild(new Vector3Dot(UnitAIMemory.TargetDirection, UnitAIMemory.ForwardDirection, UnitAIMemory.DotResult));
            findTargetDirection.AddChild(new Vector3MultiplyByScalar(UnitAIMemory.ForwardDirection, UnitAIMemory.DotResult, UnitAIMemory.TargetDirection));
            findTargetDirection.AddChild(new Vector3Normalize(UnitAIMemory.TargetDirection, UnitAIMemory.TargetDirection));

//        //-- MoveForward
        Sequence moveToTargetDirection = new Sequence();
            moveToTargetDirection.AddChild(new SetMemoryVar<float>(UnitAIMemory.MoveDistance, 3.0f));
            moveToTargetDirection.AddChild(new Vector3MultiplyByScalar(UnitAIMemory.TargetDirection, UnitAIMemory.MoveDistance, UnitAIMemory.TargetDirectionScaled));
            moveToTargetDirection.AddChild(new SavePositionFromComponent(UnitAIMemory.Unit, UnitAIMemory.UnitPosition));
            moveToTargetDirection.AddChild(new Vector3Add(UnitAIMemory.UnitPosition, UnitAIMemory.TargetDirectionScaled, UnitAIMemory.TargetPosition));
            moveToTargetDirection.AddChild(new CheckPositionIsInAttackingZone(UnitAIMemory.Unit, UnitAIMemory.TargetPosition));
            moveToTargetDirection.AddChild(new MoveUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.TargetPosition));

        //-- Check Ball is Close
        Sequence checkIfBallIsClose = new Sequence();
            checkIfBallIsClose.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            checkIfBallIsClose.AddChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance, 2.0f));

        //-- FindAndSave Closest Ball
        Sequence findAndSaveClosestBallProperties = new Sequence();
            findAndSaveClosestBallProperties.AddChild(new FindClosestBall(UnitAIMemory.Unit, UnitAIMemory.Ball));
            findAndSaveClosestBallProperties.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            findAndSaveClosestBallProperties.AddChild(new SavePositionFromTransform(UnitAIMemory.BallTransform, UnitAIMemory.BallPosition));

        //-- DistanceBallUnit Subtree
        Sequence checkUnitBallDistance = new Sequence();
            checkUnitBallDistance.AddChild(new SavePositionFromComponent(UnitAIMemory.Unit, UnitAIMemory.PositionA));
            checkUnitBallDistance.AddChild(new SavePositionFromComponent(UnitAIMemory.BallTransform, UnitAIMemory.PositionB));
            checkUnitBallDistance.AddChild(new SetMemoryVar<float>(UnitAIMemory.MaxUnitBallDistance, 10.0f));
            checkUnitBallDistance.AddChild(new CheckDistanceBetweenPositions(UnitAIMemory.PositionA, UnitAIMemory.PositionB, UnitAIMemory.MaxUnitBallDistance, 0f));

        Sequence setBallControlled = new Sequence();
            setBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.IsBallControlled, true));
            setBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.HaveSquadPosession, true));
            setBallControlled.AddChild(new CopyMemoryVar(UnitAIMemory.Squad, UnitAIMemory.g_PossessionSquad));
            setBallControlled.AddChild(new SetOwnerToBall(UnitAIMemory.Unit, UnitAIMemory.Ball));
            setBallControlled.AddChild(new SetMemoryVar<Vector3>(UnitAIMemory.BallVelocity, Vector3.zero));
            setBallControlled.AddChild(new SaveVelocityFromComponent(UnitAIMemory.Ball, UnitAIMemory.BallVelocity));

        #region moveToball
        // Move to Ball Transform till Close
        Parallel moveToBallTransform = new Parallel(Parallel.ParallelPolicy.SUCCESS_IF_ONE, Parallel.ParallelPolicy.FAILURE_IF_ALL);
            moveToBallTransform.AddChild(new MoveUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform));
            UntilSuccess checkDistanceTillClose = new UntilSuccess();
                checkDistanceTillClose.SetChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance, 0f));
            moveToBallTransform.AddChild(checkDistanceTillClose);

        // Move to Band End Position till Close
        Parallel moveToBallEndPosition = new Parallel(Parallel.ParallelPolicy.SUCCESS_IF_ONE, Parallel.ParallelPolicy.FAILURE_IF_ALL);
         moveToBallEndPosition.AddChild(new MoveUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.BallEndPosition));
        UntilSuccess checkDistanceToBallTransform = new UntilSuccess();
            checkDistanceToBallTransform.SetChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance, 0f));
         moveToBallEndPosition.AddChild(checkDistanceToBallTransform);

        // Move if not close
        Selector moveIfNotClose = new Selector();
            moveIfNotClose.AddChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance, 3));
            moveIfNotClose.AddChild(moveToBallTransform);

        //-- Move to Ball Distace Transform Subtree
        Sequence moveToBallTransformIfNotClose = new Sequence();
            moveToBallTransformIfNotClose.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            moveToBallTransformIfNotClose.AddChild(moveIfNotClose);

        //-- Move to target Ball position and stop if close to ball
        Sequence moveToTargetPosition = new Sequence();
            moveToTargetPosition.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            moveToTargetPosition.AddChild(moveToBallEndPosition);
        #endregion moveToball

        //-- Move with Ball Step
        #region MoveWithBall
        Sequence moveWithBallStep =  new Sequence();
            moveWithBallStep.AddChild(moveToBallTransform);
            moveWithBallStep.AddChild(new SavePositionFromTransform(UnitAIMemory.BallTransform, UnitAIMemory.BallPosition));
            moveWithBallStep.AddChild(new Vector3StepToPosition(UnitAIMemory.BallPosition, UnitAIMemory.TargetPosition, UnitAIMemory.BallEndPosition, 10));
            moveWithBallStep.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallMaxHeight, 0f));
            moveWithBallStep.AddChild(new PassBallToPosition(UnitAIMemory.Unit, UnitAIMemory.Ball, UnitAIMemory.BallEndPosition, UnitAIMemory.BallMaxHeight));

        UntilFail keepMovingWithBallStep = new UntilFail();
            keepMovingWithBallStep.SetChild(moveWithBallStep);

        ActiveSelector moveWithBallToTargetPosition = new ActiveSelector();
            moveWithBallToTargetPosition.AddChild(new CheckDistanceFromComponentToPosition(UnitAIMemory.Unit, UnitAIMemory.TargetPosition, UnitAIMemory.BallDistance, 2f));
            moveWithBallToTargetPosition.AddChild(keepMovingWithBallStep);
        
        Sequence moveForwardWithBall = new Sequence();
            moveForwardWithBall.AddChild(new SetMemoryVar<float>(UnitAIMemory.MoveDistance, 30.0f));
            moveForwardWithBall.AddChild(new Vector3MultiplyByScalar(UnitAIMemory.TargetDirection, UnitAIMemory.MoveDistance, UnitAIMemory.TargetDirectionScaled));
            moveForwardWithBall.AddChild(new SavePositionFromComponent(UnitAIMemory.Unit, UnitAIMemory.UnitPosition));
            moveForwardWithBall.AddChild(new Vector3Add(UnitAIMemory.UnitPosition, UnitAIMemory.TargetDirectionScaled, UnitAIMemory.TargetPosition));
            moveForwardWithBall.AddChild(new CheckPositionIsInAttackingZone(UnitAIMemory.Unit, UnitAIMemory.TargetPosition));
            moveForwardWithBall.AddChild(moveWithBallToTargetPosition);
        
        Sequence tryMovingWithBallToAForwardPosition = new Sequence();
            tryMovingWithBallToAForwardPosition.AddChild(findTargetDirection);
            tryMovingWithBallToAForwardPosition.AddChild(moveForwardWithBall);
        #endregion MoveWithBall

       #region defensive
        Sequence tryToSmashBallFromOwner = new Sequence();
            tryToSmashBallFromOwner.AddChild(new SaveBallOwner(UnitAIMemory.Ball, UnitAIMemory.BallOwner));
            tryToSmashBallFromOwner.AddChild(new CheckNotNullMemoryVar(UnitAIMemory.BallOwner));
            tryToSmashBallFromOwner.AddChild(new Inverter().SetChild(new CheckUnitIsTeamMate(UnitAIMemory.Unit, UnitAIMemory.BallOwner)));
            tryToSmashBallFromOwner.AddChild(new Succeder().SetChild(new SmashUnit(UnitAIMemory.Unit, UnitAIMemory.BallOwner)));
            tryToSmashBallFromOwner.AddChild(new Succeder().SetChild(new SmashBall(UnitAIMemory.Unit, UnitAIMemory.Ball)));
            tryToSmashBallFromOwner.AddChild(setBallControlled);

        Selector controllOrPushTheBallIfHasEnemyOwner = new Selector();
            controllOrPushTheBallIfHasEnemyOwner.AddChild(tryToSmashBallFromOwner);
            controllOrPushTheBallIfHasEnemyOwner.AddChild(setBallControlled);

        //-- Recover Possesion Subtree
        Sequence catchAndControllOrPushTheBall = new Sequence();
            catchAndControllOrPushTheBall.AddChild(moveToBallTransform);
            catchAndControllOrPushTheBall.AddChild(checkUnitBallDistance);
            catchAndControllOrPushTheBall.AddChild(controllOrPushTheBallIfHasEnemyOwner);

        ActiveSequence chaseControlOrPushBallIfClose = new ActiveSequence();
            chaseControlOrPushBallIfClose.AddChild(new CheckUnitDistanceBetweenComponents(UnitAIMemory.Unit, UnitAIMemory.Ball, 100f));
            chaseControlOrPushBallIfClose.AddChild(catchAndControllOrPushTheBall);
            
        Sequence tryToRecoverTheBallWhenDefensing = new Sequence();
            tryToRecoverTheBallWhenDefensing.AddChild(chaseControlOrPushBallIfClose);

        Sequence returnDefensivePosition = new Sequence();
            returnDefensivePosition.AddChild(new MoveUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.StartPosition));

        Selector chaseTheBall = new Selector();
            chaseTheBall.AddChild(tryToRecoverTheBallWhenDefensing);
            chaseTheBall.AddChild(returnDefensivePosition);

        #endregion defensive

        _bt = new BehaviorTree(BehaviortreeNames.DefensiveBallRecover);

        //_bt.Init(tryToRecoverTheBallWhenDefensing);
        _bt.Init(chaseTheBall);

        return _bt;
    }
}

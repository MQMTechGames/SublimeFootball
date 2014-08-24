using UnityEngine;

using MQMTech.AI.BT;
using MQMTech.AI.Knowledge.Action;
using MQMTech.AI.Mover.Action;
using MQMTech.AI.Knowledge;

[System.Serializable]
public class AttackWithoutBallTreeBuilder : MonoBehaviour, IBehaviorWithTree
{
    BehaviorTree _bt;
//    BaseUnit _unit;
//    Match _match;
    
//    void Awake()
//    {
//        _unit = GameObjectUtils.GetInterfaceObject<BaseUnit>(gameObject);
//        
//        _match = FindObjectOfType<Match>();
//        DebugUtils.Assert(_match!=null, "_match!=null");
//    }
    
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
        
        Sequence checkBallIsMoving = new Sequence();
        checkBallIsMoving.AddChild(new SaveVelocityFromComponent(UnitAIMemory.Ball, UnitAIMemory.BallVelocity));
        checkBallIsMoving.AddChild(new CalculateMagnitudeFromVector(UnitAIMemory.BallVelocity, UnitAIMemory.BallVelocityMagnitude));
        checkBallIsMoving.AddChild(new SetMemoryVar<float>(UnitAIMemory.MinBallVelocityMagnitude, 1.0f));
        checkBallIsMoving.AddChild(new GreaterThan(UnitAIMemory.BallVelocityMagnitude, UnitAIMemory.MinBallVelocityMagnitude));
        
        Sequence checkTransformIsGettingCloser = new Sequence();
        checkTransformIsGettingCloser.AddChild(new CalculateDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.PrevDistanceToTransform, false));
        checkTransformIsGettingCloser.AddChild(new TimeWaiter(0.1f));
        checkTransformIsGettingCloser.AddChild(new CalculateDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.DistanceToTransform, false));
        checkTransformIsGettingCloser.AddChild(new GreaterThan(UnitAIMemory.PrevDistanceToTransform, UnitAIMemory.DistanceToTransform));
        
        Sequence checkBallIsMovingAndGettingCloser = new Sequence();
        checkBallIsMovingAndGettingCloser.AddChild(checkBallIsMoving);
        checkBallIsMovingAndGettingCloser.AddChild(checkTransformIsGettingCloser);
        
        UntilSuccess keepWaittingTillBallIsClose = new UntilSuccess();
        keepWaittingTillBallIsClose.SetChild(checkIfBallIsClose);
        
        // Wait till ball is close
        ActiveSequence waitWhileBallIsComingTillIsClose = new ActiveSequence();
        waitWhileBallIsComingTillIsClose.AddChild(checkBallIsMovingAndGettingCloser);
        waitWhileBallIsComingTillIsClose.AddChild(keepWaittingTillBallIsClose);
        
        // Check On Ball Passed Notificaton is not valid
        Selector checkOnBallShotIsNullOrInvalid = new Selector();
        checkOnBallShotIsNullOrInvalid.AddChild(new CheckNullMemoryVar(UnitAIMemory.OnBallShot));
        checkOnBallShotIsNullOrInvalid.AddChild(new Inverter().SetChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.OnBallShot, 1f)));
        checkOnBallShotIsNullOrInvalid.AddChild(new Inverter().SetChild(checkBallIsMoving));
        
        Sequence setBallControlled = new Sequence();
        setBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.IsBallControlled, true));
        setBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.HaveSquadPosession, true));
        setBallControlled.AddChild(new CopyMemoryVar(UnitAIMemory.Squad, UnitAIMemory.g_PossessionSquad));
        setBallControlled.AddChild(new SetOwnerToBall(UnitAIMemory.Unit, UnitAIMemory.Ball));
        setBallControlled.AddChild(new SetMemoryVar<Vector3>(UnitAIMemory.BallVelocity, Vector3.zero));
        setBallControlled.AddChild(new SaveVelocityFromComponent(UnitAIMemory.Ball, UnitAIMemory.BallVelocity));
        
        //-- nullify the global parameter squad with posession
        Sequence nullifyPossessionSquad = new Sequence();
        nullifyPossessionSquad.AddChild(new CheckAreEqualMemoryVars<BaseSquad>(UnitAIMemory.Squad, UnitAIMemory.g_PossessionSquad));
        nullifyPossessionSquad.AddChild(new SetMemoryVar<BaseSquad>(UnitAIMemory.g_PossessionSquad, null));
        
        //-- nullify the global parameter squad with posession
        Sequence removeUnitOwnerFromBall = new Sequence();
        removeUnitOwnerFromBall.AddChild(new SaveBallOwner(UnitAIMemory.Ball, UnitAIMemory.TemporalUnit));
        removeUnitOwnerFromBall.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.TemporalUnit, UnitAIMemory.Unit));
        removeUnitOwnerFromBall.AddChild(new SetMemoryVar<BaseUnit>(UnitAIMemory.TemporalUnit, null));
        removeUnitOwnerFromBall.AddChild(new SetOwnerToBall(UnitAIMemory.TemporalUnit, UnitAIMemory.Ball));
        
        //-- removes current squad as the posession squad from local and global variables
        Sequence unsetBallControlled = new Sequence();
        unsetBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.IsBallControlled, false));
        unsetBallControlled.AddChild(new Succeder().SetChild(removeUnitOwnerFromBall));
        
        Sequence unsetBallPosession = new Sequence();
        unsetBallPosession.AddChild(new SetMemoryVar<bool>(UnitAIMemory.HaveSquadPosession, false));
        unsetBallPosession.AddChild(new SetMemoryVar<BaseSquad>(UnitAIMemory.g_PossessionSquad, null));
        
        Sequence checkOwnerVarIsNull = new Sequence();
        checkOwnerVarIsNull.AddChild(new SaveBallOwner(UnitAIMemory.Ball, UnitAIMemory.TemporalUnit));
        checkOwnerVarIsNull.AddChild(new CheckNullMemoryVar(UnitAIMemory.TemporalUnit));
        
        //-- Recover Possesion Subtree
        Sequence catchAndControllTheBall = new Sequence();
        catchAndControllTheBall.AddChild(moveToBallTransform);
        catchAndControllTheBall.AddChild(checkUnitBallDistance);
        catchAndControllTheBall.AddChild(setBallControlled);
        
        // Set Recover the ball CoolDown timer
        Sequence setRecoverTheBallCoolDownTimer = new Sequence();
        setRecoverTheBallCoolDownTimer.AddChild(new SetMemoryVar<float>(UnitAIMemory.RecoverPossessionCoolDownWaitTime, 6.0f));
        setRecoverTheBallCoolDownTimer.AddChild(new SetCoolDownTime(UnitAIMemory.RecoverPossessionCoolDownTimer, UnitAIMemory.RecoverPossessionCoolDownWaitTime));
        
        // Remove Recover the ball CoolDown timer
        Sequence removeRecoverTheBallCoolDown = new Sequence();
        removeRecoverTheBallCoolDown.AddChild(new SetMemoryVar<float>(UnitAIMemory.RecoverPossessionCoolDownWaitTime, -1.0f));
        removeRecoverTheBallCoolDown.AddChild(new SetCoolDownTime(UnitAIMemory.RecoverPossessionCoolDownTimer, UnitAIMemory.RecoverPossessionCoolDownWaitTime));
        
        Sequence tryToClearCoolDownIfPasserIsNotUnit = new Sequence();
        tryToClearCoolDownIfPasserIsNotUnit.AddChild(new Inverter().SetChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.BallPasserUnit)));
        tryToClearCoolDownIfPasserIsNotUnit.AddChild(removeRecoverTheBallCoolDown);
        
        GateMaxNumber catchAndControllTheBallGate = new GateMaxNumber(UnitAIMemory.NumUnitsToRecoverPossessionCounter, 1, GateMaxNumber.GatePolicy.FAIL_IF_UNAVAILABLE);
        catchAndControllTheBallGate.SetChild(catchAndControllTheBall);
        
        Sequence recoverClosestBall = new Sequence();
        recoverClosestBall.AddChild(new GetClosestUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit, UnitAIMemory.BallPosition));
        recoverClosestBall.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit));
        recoverClosestBall.AddChild(catchAndControllTheBallGate);
        recoverClosestBall.AddChild(setRecoverTheBallCoolDownTimer);
        
        // Cool Down to not start recovering the ball inmediately after it has been passed or shot
        CoolDown coolDownForRecoverPossession = new CoolDown(UnitAIMemory.RecoverPossessionCoolDownTimer, CoolDown.CoolDownPolicy.FAIL_IF_UNAVAILABLE);
        coolDownForRecoverPossession.SetChild(recoverClosestBall);
        
        // Try to recover the ball
        Sequence recoverTheBall = new Sequence();
        recoverTheBall.AddChild(findAndSaveClosestBallProperties);
        recoverTheBall.AddChild(checkOwnerVarIsNull);
        recoverTheBall.AddChild(new Succeder().SetChild(tryToClearCoolDownIfPasserIsNotUnit));
        recoverTheBall.AddChild(coolDownForRecoverPossession);
        
        //-- Prepare to Receive the ball to point
        Sequence faceBall = new Sequence();
        faceBall.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
        faceBall.AddChild(new FaceUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform));
        
        Sequence tryToReactToBallPassMessage = new Sequence();
        tryToReactToBallPassMessage.AddChild(new CheckKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.NEW));
        tryToReactToBallPassMessage.AddChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.OnBallPassed, 1f));
        tryToReactToBallPassMessage.AddChild(new SaveOnBallPassedProperties(UnitAIMemory.OnBallPassed, UnitAIMemory.BallEndPosition, UnitAIMemory.PossessionMateUnit, UnitAIMemory.BallTargetMateUnit, UnitAIMemory.Ball));
        tryToReactToBallPassMessage.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.BallTargetMateUnit));
        tryToReactToBallPassMessage.AddChild(removeRecoverTheBallCoolDown);
        tryToReactToBallPassMessage.AddChild(new SetKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.PROCESSED));
        tryToReactToBallPassMessage.AddChild(faceBall);
        tryToReactToBallPassMessage.AddChild(moveToTargetPosition);
        tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(waitWhileBallIsComingTillIsClose));
        tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(new RemoveKnowledgeIfNotNew(UnitAIMemory.OnBallPassed)));
        
        // Try to Move To A Passing Position
        Sequence tryMovingToAPassingPosition = new Sequence();
        tryMovingToAPassingPosition.AddChild(checkOnBallShotIsNullOrInvalid);
        tryMovingToAPassingPosition.AddChild(findTargetDirection);
        tryMovingToAPassingPosition.AddChild(moveToTargetDirection);
        
        // Checl On Ball Passed Notificaton is not valid
        Selector checkOnBallPassedIsDoneOrNullOrInvalid = new Selector();
        checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new CheckNullMemoryVar(UnitAIMemory.OnBallPassed));
        checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new CheckKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.DONE));
        checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new Inverter().SetChild(checkBallIsMoving));
        
        Sequence tryToRecoverTheBallWhenAttacking = new Sequence();
        tryToRecoverTheBallWhenAttacking.AddChild(checkOnBallPassedIsDoneOrNullOrInvalid);
        tryToRecoverTheBallWhenAttacking.AddChild(checkOnBallShotIsNullOrInvalid);
        tryToRecoverTheBallWhenAttacking.AddChild(recoverTheBall);
        
        // Choose Attack without Ball
        Selector chooseAttackWithoutBall = new Selector();
        chooseAttackWithoutBall.AddChild(tryToReactToBallPassMessage);
        chooseAttackWithoutBall.AddChild(tryToRecoverTheBallWhenAttacking);
        chooseAttackWithoutBall.AddChild(tryMovingToAPassingPosition);
        
        // Attack without ball controlled
        Sequence tryToAttackWithoutBall = new Sequence();
        tryToAttackWithoutBall.AddChild(new Inverter().SetChild(new CheckAreEqualMemoryVars<bool>(UnitAIMemory.IsBallControlled, UnitAIMemory.TrueVar)));
        tryToAttackWithoutBall.AddChild(chooseAttackWithoutBall);

        Sequence testUnitIsNotNull = new Sequence();
            testUnitIsNotNull.AddChild(new CheckNotNullMemoryVar(UnitAIMemory.Unit));
            testUnitIsNotNull.AddChild(new LogAction("Unit Is OK :)"));

        Sequence testUnitIsNull = new Sequence();
            testUnitIsNull.AddChild(new CheckNullMemoryVar(UnitAIMemory.Unit));
            testUnitIsNull.AddChild(new LogAction("Unit is NULL :("));

        Selector checkWhatHappens = new Selector();
            checkWhatHappens.AddChild(testUnitIsNull);
            checkWhatHappens.AddChild(testUnitIsNotNull);

        _bt = new BehaviorTree();

        _bt.Init(tryToAttackWithoutBall);

        return _bt;
    }
}

using UnityEngine;

using MQMTech.AI.BT;
using MQMTech.AI.Knowledge.Action;
using MQMTech.AI.Mover.Action;
using MQMTech.AI.Knowledge;

[System.Serializable]
public class DemoUnitBTBuilder : MonoBehaviour, IBehaviorWithTree
{
    BaseUnit _unit;
    BehaviorTree _bt;
    Match _match;

    [SerializeField]
    SharedMemory _squadMemory;

    [SerializeField]
    SharedMemory _matchMemory;
    
    void Awake()
    {
        _unit = GameObjectUtils.GetInterfaceObject<BaseUnit>(gameObject);
        _match = FindObjectOfType<Match>();

        DebugUtils.Assert(_match!=null, "_match!=null");
    }
    
    public BehaviorTree GetBehaviorTree()
    {
        return _bt;
    }
    
    public BehaviorTree Create()
    {
        Selector mainAI = new Selector();

        //-- Check Ball is Close
        Sequence checkIfBallIsClose = new Sequence();
            checkIfBallIsClose.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallDistance, 6.0f));
            checkIfBallIsClose.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            checkIfBallIsClose.AddChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance));

        Sequence checkIfBallIsVeryClose = new Sequence();
            checkIfBallIsVeryClose.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallDistance, 2.0f));
            checkIfBallIsVeryClose.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            checkIfBallIsVeryClose.AddChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance));

        //-- FindAndSave Closest Ball
        Sequence findAndSaveClosestBallProperties = new Sequence();
            findAndSaveClosestBallProperties.AddChild(new FindClosestBall(UnitAIMemory.Unit, UnitAIMemory.Ball));
            findAndSaveClosestBallProperties.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            findAndSaveClosestBallProperties.AddChild(new SavePositionFromTransform(UnitAIMemory.BallTransform, UnitAIMemory.BallPosition));

        //-- DistanceBallUnit Subtree
        Sequence checkUnitBallDistance = new Sequence();
            checkUnitBallDistance.AddChild(new SavePositionFromComponent<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.PositionA));
            checkUnitBallDistance.AddChild(new SavePositionFromComponent<Transform>(UnitAIMemory.BallTransform, UnitAIMemory.PositionB));
            checkUnitBallDistance.AddChild(new SetMemoryVar<float>(UnitAIMemory.MaxUnitBallDistance, 10.0f));
            checkUnitBallDistance.AddChild(new CheckDistanceBetweenPositions(UnitAIMemory.PositionA, UnitAIMemory.PositionB, UnitAIMemory.MaxUnitBallDistance));

        //-- Move to Ball Transform Subtree
        Sequence moveToTheBall = new Sequence();
        moveToTheBall.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallDistance, 6.0f));
        moveToTheBall.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            Parallel keepMovingTillClose = new Parallel(Parallel.ParallelPolicy.SUCCESS_IF_ONE, Parallel.ParallelPolicy.FAILURE_IF_ALL);
                    keepMovingTillClose.AddChild(new MoveUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform));
                    UntilSuccess checkDistanceTillClose = new UntilSuccess();
                        checkDistanceTillClose.SetChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance));
                    keepMovingTillClose.AddChild(checkDistanceTillClose);
        moveToTheBall.AddChild(keepMovingTillClose);

        //-- Move to target Ball position and stop if close to ball
        Sequence moveToTargetPosition = new Sequence();
            moveToTargetPosition.AddChild(new SetMemoryVar<float>(UnitAIMemory.BallDistance, 6.0f));
            moveToTargetPosition.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            Parallel keepMovingTillCloseToBall = new Parallel(Parallel.ParallelPolicy.SUCCESS_IF_ONE, Parallel.ParallelPolicy.FAILURE_IF_ALL);
                     keepMovingTillCloseToBall.AddChild(new MoveUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.TargetBallPosition));
                    UntilSuccess checkDistanceToBallTransform = new UntilSuccess();
                        checkDistanceToBallTransform.SetChild(new CheckUnitDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.BallDistance));
                     keepMovingTillCloseToBall.AddChild(checkDistanceToBallTransform);
        moveToTargetPosition.AddChild(keepMovingTillCloseToBall);

        Sequence checkBallIsMoving = new Sequence();
            checkBallIsMoving.AddChild(new SaveVelocityFromComponent(UnitAIMemory.Ball, UnitAIMemory.BallVelocity));
            checkBallIsMoving.AddChild(new CalculateMagnitudeFromVector(UnitAIMemory.BallVelocity, UnitAIMemory.BallVelocityMagnitude));
            checkBallIsMoving.AddChild(new SetMemoryVar<float>(UnitAIMemory.MinBallVelocityMagnitude, 0.0f));
            checkBallIsMoving.AddChild(new GreaterThan(UnitAIMemory.BallVelocityMagnitude, UnitAIMemory.MinBallVelocityMagnitude));

        Sequence checkTransformIsGettingCloser = new Sequence();
            checkTransformIsGettingCloser.AddChild(new CalculateDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.PrevDistanceToTransform, false));
            checkTransformIsGettingCloser.AddChild(new TimeWaiter(0.1f));
            checkTransformIsGettingCloser.AddChild(new CalculateDistanceToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform, UnitAIMemory.DistanceToTransform, false));
            checkTransformIsGettingCloser.AddChild(new GreaterThan(UnitAIMemory.PrevDistanceToTransform, UnitAIMemory.DistanceToTransform));

        Sequence checkBallIsMovingAndGettingCloser = new Sequence();
            checkBallIsMovingAndGettingCloser.AddChild(checkBallIsMoving);
            checkBallIsMovingAndGettingCloser.AddChild(checkTransformIsGettingCloser);

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
            removeUnitOwnerFromBall.AddChild(new SaveOwnerToBall(UnitAIMemory.Ball, UnitAIMemory.TemporalUnit));
            removeUnitOwnerFromBall.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.TemporalUnit, UnitAIMemory.Unit));
            removeUnitOwnerFromBall.AddChild(new SetMemoryVar<BaseUnit>(UnitAIMemory.TemporalUnit, null));
            removeUnitOwnerFromBall.AddChild(new SetOwnerToBall(UnitAIMemory.TemporalUnit, UnitAIMemory.Ball));

        //-- removes current squad as the posession squad from local and global variables
        Sequence unsetBallControlled = new Sequence();
            unsetBallControlled.AddChild(new SetMemoryVar<bool>(UnitAIMemory.IsBallControlled, false));
            unsetBallControlled.AddChild(new Succeder().SetChild(removeUnitOwnerFromBall));
            //unsetBallControlled.AddChild(new Succeder().SetChild(nullifyPossessionSquad));

        Sequence checkOwnerVarIsNull = new Sequence();
            checkOwnerVarIsNull.AddChild(new SaveOwnerToBall(UnitAIMemory.Ball, UnitAIMemory.TemporalUnit));
            checkOwnerVarIsNull.AddChild(new CheckNullMemoryVar(UnitAIMemory.TemporalUnit));

        //-- Recover Possesion Subtree
        Sequence catchAndControllTheBall = new Sequence();
            catchAndControllTheBall.AddChild(moveToTheBall);
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

        // Try to recover the ball
        Sequence tryToControllTheBall = new Sequence();
            tryToControllTheBall.AddChild(findAndSaveClosestBallProperties);
            tryToControllTheBall.AddChild(checkOwnerVarIsNull);
            Sequence recoverClosestBall = new Sequence();
                recoverClosestBall.AddChild(new GetClosestUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit, UnitAIMemory.BallPosition));
                recoverClosestBall.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit));
                GateMaxNumber gateMaxUnitsToRecoverBall = new GateMaxNumber(UnitAIMemory.NumUnitsToRecoverPossessionCounter, 1, GateMaxNumber.GatePolicy.FAIL_IF_UNAVAILABLE);
                gateMaxUnitsToRecoverBall.SetChild(catchAndControllTheBall);
                recoverClosestBall.AddChild(gateMaxUnitsToRecoverBall);
                recoverClosestBall.AddChild(setRecoverTheBallCoolDownTimer);
            CoolDown coolDownForRecoverPossession = new CoolDown(UnitAIMemory.RecoverPossessionCoolDownTimer, CoolDown.CoolDownPolicy.FAIL_IF_UNAVAILABLE);
                coolDownForRecoverPossession.SetChild(recoverClosestBall);
            tryToControllTheBall.AddChild(coolDownForRecoverPossession);

        //-- Send Will Pass The Ball SubTree
        Sequence sendWillPassTheBall = new Sequence();
            sendWillPassTheBall.AddChild(new SetMemoryVar<float>(UnitAIMemory.MessageValidDuration, 1.0f));
            sendWillPassTheBall.AddChild(new CreateWillPassTheBallMessage(UnitAIMemory.Unit, UnitAIMemory.TargetBallPosition, UnitAIMemory.MessageValidDuration, UnitAIMemory.CreatedMessage));
            sendWillPassTheBall.AddChild(new SendMessageToUnit(UnitAIMemory.SelectedMateUnit, UnitAIMemory.CreatedMessage, UnitAIMemory.OnWillPassTheBall.Name));
            //sendWillPassTheBall.AddChild(new LogAction("Message Sent"));

        //-- Pass The Ball Subtree
        Sequence easyBallPass = new Sequence();
            easyBallPass.AddChild(new SelectEasiestUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.SelectedMateUnit));
            easyBallPass.AddChild(new SaveTransformFromComponent<BaseUnit>(UnitAIMemory.SelectedMateUnit, UnitAIMemory.SelectedMateTransform));
            easyBallPass.AddChild(new FaceUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.SelectedMateTransform));
            easyBallPass.AddChild(new SavePositionFromTransform(UnitAIMemory.SelectedMateTransform, UnitAIMemory.TargetBallPosition));
            easyBallPass.AddChild(checkUnitBallDistance);
            easyBallPass.AddChild(sendWillPassTheBall);
            easyBallPass.AddChild(new PassBallToPosition(UnitAIMemory.Unit, UnitAIMemory.Ball, UnitAIMemory.TargetBallPosition));

        // Try to pass the ball
        Sequence tryToPassTheBall = new Sequence();
            tryToPassTheBall.AddChild(new CheckAreEqualMemoryVars<bool>(UnitAIMemory.IsBallControlled, UnitAIMemory.TrueVar));
            tryToPassTheBall.AddChild(new Succeder().SetChild(easyBallPass));
            tryToPassTheBall.AddChild(unsetBallControlled);

        //-- Prepare to Receive the ball to point
        Sequence faceBall = new Sequence();
            faceBall.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            faceBall.AddChild(new FaceUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform));

        //-- try to save ball from unit Subtree
        Sequence saveBallFromUnit = new Sequence();
            saveBallFromUnit.AddChild(new SaveBallFromPossesionUnit(UnitAIMemory.PossessionMateUnit, UnitAIMemory.TemporalBall));
            saveBallFromUnit.AddChild(new CheckNotNullMemoryVar(UnitAIMemory.TemporalBall));
            saveBallFromUnit.AddChild(new MoveMemoryVar(UnitAIMemory.TemporalBall, UnitAIMemory.Ball));

        // Wait till ball is close
        ActiveSequence waitWhileBallIsComingTillIsClose = new ActiveSequence();
                UntilSuccess keepWaittingTillBallIsClose = new UntilSuccess();
                    keepWaittingTillBallIsClose.SetChild(checkIfBallIsVeryClose); //checkIfBallIsClose
            waitWhileBallIsComingTillIsClose.AddChild(checkBallIsMovingAndGettingCloser);
            waitWhileBallIsComingTillIsClose.AddChild(keepWaittingTillBallIsClose);

        //-- tryToReactToBallPassMessage Subtree
        Sequence tryToReactToBallPassMessage = new Sequence();
            tryToReactToBallPassMessage.AddChild(new CheckKnowledgeStatus(UnitAIMemory.OnWillPassTheBall, KnowledgeStatus.NEW));
            tryToReactToBallPassMessage.AddChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.OnWillPassTheBall));
            tryToReactToBallPassMessage.AddChild(removeRecoverTheBallCoolDown);
            tryToReactToBallPassMessage.AddChild(new SetKnowledgeStatus(UnitAIMemory.OnWillPassTheBall, KnowledgeStatus.PROCESSED));
            tryToReactToBallPassMessage.AddChild(new SaveOnWillPassTheBallProperties(UnitAIMemory.OnWillPassTheBall, UnitAIMemory.TargetBallPosition, UnitAIMemory.PossessionMateUnit));
            tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(saveBallFromUnit));
            tryToReactToBallPassMessage.AddChild(faceBall);
            tryToReactToBallPassMessage.AddChild(moveToTargetPosition);
            tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(waitWhileBallIsComingTillIsClose));
            tryToReactToBallPassMessage.AddChild(new RemoveMemoryVar(UnitAIMemory.OnWillPassTheBall));

        //* What to do when team have ball posession
            // try to recover the ball if noone has it controlled
            // try to react to Ball Pass Message (Active Selector ?)
            // Do something if I have not the ball controlled
                // Choose between
                    // Try to Move to a valid position where the ball controller can pass me the ball
                    // Try to go up to the target goal
                    // Try to go down to move the ball
            // Do something if I have the ball controlled
                // Choose between
                    // try to move forward
                    // try to pass the ball
                        // easy pass
                        // long pass
                    // try to shoot to the goal
        //* What to do when the enemy have ball posession
            // try to chase the target controller if he is close enough
            // try to go back to a defensive position if I'm not in my defensive position
            // move to block a possible shot from the enemy target to the goal
        //* what to do when noone have ball posession
            // Try to recover posession
                // Try to chase the ball

        Sequence tryToAttack = new Sequence();
            tryToAttack.AddChild(new CheckAreEqualMemoryVars<BaseSquad>(UnitAIMemory.g_PossessionSquad, UnitAIMemory.Squad));
            Selector chooseAttack = new Selector();
                chooseAttack.AddChild(tryToReactToBallPassMessage);
                chooseAttack.AddChild(tryToControllTheBall);
                chooseAttack.AddChild(tryToPassTheBall);
            tryToAttack.AddChild(chooseAttack);

        Sequence tryToDefend = new Sequence();
            tryToDefend.AddChild(new Failure());

        Sequence tryToRecoverTheBall = new Sequence();
            tryToRecoverTheBall.AddChild(new CheckNullMemoryVar(UnitAIMemory.g_PossessionSquad));
            tryToRecoverTheBall.AddChild(tryToControllTheBall);

            // Main AIs subtipes
            mainAI.AddChild(tryToAttack);
            mainAI.AddChild(tryToDefend);
            mainAI.AddChild(tryToRecoverTheBall);

        _bt = new BehaviorTree();

        _bt.SetSharedMemory(_squadMemory.Memory);
        _bt.SetGlobalMemory(_matchMemory.Memory);

        DebugUtils.Assert(_bt.MemoryManager.SquadMemory != null, "_bt.SharedMemory != null");

        // Init variables
        _bt.SetMemoryObject(UnitAIMemory.Unit, _unit);
        _bt.SetMemoryObject(UnitAIMemory.Squad, _unit.Squad);
        _bt.SetMemoryObject(UnitAIMemory.TrueVar, true);
        
        _bt.Init(mainAI);
        return _bt;
    }
}

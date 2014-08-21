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

        Sequence checkOwnerVarIsNull = new Sequence();
            checkOwnerVarIsNull.AddChild(new SaveOwnerToBall(UnitAIMemory.Ball, UnitAIMemory.TemporalUnit));
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

        Sequence recoverClosestBall = new Sequence();
            recoverClosestBall.AddChild(new GetClosestUnitToPosition(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit, UnitAIMemory.BallPosition));
            recoverClosestBall.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.ClosestMateUnit));
            GateMaxNumber gateMaxUnitsToRecoverBall = new GateMaxNumber(UnitAIMemory.NumUnitsToRecoverPossessionCounter, 1, GateMaxNumber.GatePolicy.FAIL_IF_UNAVAILABLE);
                gateMaxUnitsToRecoverBall.SetChild(catchAndControllTheBall);
            recoverClosestBall.AddChild(gateMaxUnitsToRecoverBall);
            recoverClosestBall.AddChild(setRecoverTheBallCoolDownTimer);

        // Try to recover the ball
        Sequence recoverTheBall = new Sequence();
            recoverTheBall.AddChild(findAndSaveClosestBallProperties);
            recoverTheBall.AddChild(checkOwnerVarIsNull);
            CoolDown coolDownForRecoverPossession = new CoolDown(UnitAIMemory.RecoverPossessionCoolDownTimer, CoolDown.CoolDownPolicy.FAIL_IF_UNAVAILABLE);
                coolDownForRecoverPossession.SetChild(recoverClosestBall);
        recoverTheBall.AddChild(new Succeder().SetChild(tryToClearCoolDownIfPasserIsNotUnit));
        recoverTheBall.AddChild(coolDownForRecoverPossession);

        //-- Send Will Pass The Ball SubTree
        Sequence createOnBallPassedAndSaveOnSharedMemory = new Sequence();
            createOnBallPassedAndSaveOnSharedMemory.AddChild(new SetMemoryVar<float>(UnitAIMemory.MessageValidDuration, 1.0f));
            createOnBallPassedAndSaveOnSharedMemory.AddChild(new CreateWillPassTheBallMessage(UnitAIMemory.Unit, UnitAIMemory.SelectedMateUnit, UnitAIMemory.BallEndPosition, UnitAIMemory.Ball, UnitAIMemory.MessageValidDuration, UnitAIMemory.OnBallPassed));

        Sequence passBallToSelectedMateUnit = new Sequence();
            passBallToSelectedMateUnit.AddChild(new SaveTransformFromComponent<BaseUnit>(UnitAIMemory.SelectedMateUnit, UnitAIMemory.SelectedMateTransform));
            passBallToSelectedMateUnit.AddChild(new FaceUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.SelectedMateTransform));
            passBallToSelectedMateUnit.AddChild(new SavePositionFromTransform(UnitAIMemory.SelectedMateTransform, UnitAIMemory.BallEndPosition));
            passBallToSelectedMateUnit.AddChild(checkUnitBallDistance);
            passBallToSelectedMateUnit.AddChild(createOnBallPassedAndSaveOnSharedMemory);
            passBallToSelectedMateUnit.AddChild(new CalculateBallMaxHeight(UnitAIMemory.Ball, UnitAIMemory.BallEndPosition, UnitAIMemory.BallMaxHeight));
            passBallToSelectedMateUnit.AddChild(new PassBallToPosition(UnitAIMemory.Unit, UnitAIMemory.Ball, UnitAIMemory.BallEndPosition, UnitAIMemory.BallMaxHeight));
            passBallToSelectedMateUnit.AddChild(new CopyMemoryVar(UnitAIMemory.Unit, UnitAIMemory.BallPasserUnit));

        //-- Pass The Ball Subtree
        Sequence easyBallPass = new Sequence();
            easyBallPass.AddChild(new SelectEasiestUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.SelectedMateUnit));
            easyBallPass.AddChild(passBallToSelectedMateUnit);

        Sequence forwardPass = new Sequence();
            forwardPass.AddChild(findTargetDirection);
            forwardPass.AddChild(new SelectForwardUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.TargetDirection, UnitAIMemory.SelectedMateUnit));
            forwardPass.AddChild(passBallToSelectedMateUnit);

        Sequence calculateLeftDirection = new Sequence();
            calculateLeftDirection.AddChild(findTargetDirection);
            calculateLeftDirection.AddChild(new SetMemoryVar<Vector3>(UnitAIMemory.UpVector, Vector3.up));
            calculateLeftDirection.AddChild(new Vector3Cross(UnitAIMemory.TargetDirection, UnitAIMemory.UpVector, UnitAIMemory.LeftDirection));
            calculateLeftDirection.AddChild(new Vector3Normalize(UnitAIMemory.LeftDirection, UnitAIMemory.LeftDirection));

        Sequence leftPass = new Sequence();
            leftPass.AddChild(calculateLeftDirection);
            leftPass.AddChild(new SelectForwardUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.LeftDirection, UnitAIMemory.SelectedMateUnit));
            leftPass.AddChild(passBallToSelectedMateUnit);

        Sequence rightPass = new Sequence();
            rightPass.AddChild(calculateLeftDirection);
            rightPass.AddChild(new Vector3Invert(UnitAIMemory.LeftDirection, UnitAIMemory.RightDirection));
            rightPass.AddChild(new SelectForwardUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.RightDirection, UnitAIMemory.SelectedMateUnit));
            rightPass.AddChild(passBallToSelectedMateUnit);

        RandomSelector sidePass = new RandomSelector();
            sidePass.AddChild(leftPass);
            sidePass.AddChild(rightPass);

        Sequence randomBallPass = new Sequence();
            randomBallPass.AddChild(new SelectRandomUnitToPassTheBall(UnitAIMemory.Unit, UnitAIMemory.SelectedMateUnit));
            randomBallPass.AddChild(passBallToSelectedMateUnit);
        
        // Try to pass the ball
        Sequence tryToPassTheBall = new Sequence();
            tryToPassTheBall.AddChild(new CheckAreEqualMemoryVars<bool>(UnitAIMemory.IsBallControlled, UnitAIMemory.TrueVar));
            //tryToPassTheBall.AddChild(new Succeder().SetChild(easyBallPass));
            //tryToPassTheBall.AddChild(new Succeder().SetChild(forwardPass));
            //tryToPassTheBall.AddChild(new Succeder().SetChild(rightPass));
            tryToPassTheBall.AddChild(new Succeder().SetChild(randomBallPass));
            tryToPassTheBall.AddChild(unsetBallControlled);

        //-- Prepare to Receive the ball to point
        Sequence faceBall = new Sequence();
            faceBall.AddChild(new SaveTransformFromComponent<Ball>(UnitAIMemory.Ball, UnitAIMemory.BallTransform));
            faceBall.AddChild(new FaceUnitToTransform(UnitAIMemory.Unit, UnitAIMemory.BallTransform));

        // Wait till ball is close
        ActiveSequence waitWhileBallIsComingTillIsClose = new ActiveSequence();
                UntilSuccess keepWaittingTillBallIsClose = new UntilSuccess();
            keepWaittingTillBallIsClose.SetChild(checkIfBallIsClose);
            waitWhileBallIsComingTillIsClose.AddChild(checkBallIsMovingAndGettingCloser);
            waitWhileBallIsComingTillIsClose.AddChild(keepWaittingTillBallIsClose);

        //-- tryToReactToBallPassMessage Subtree
        Sequence tryToReactToBallPassMessage = new Sequence();
            tryToReactToBallPassMessage.AddChild(new CheckKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.NEW));
            tryToReactToBallPassMessage.AddChild(new CheckAIMessageIsValidOrRemove(UnitAIMemory.OnBallPassed));
            tryToReactToBallPassMessage.AddChild(new SaveOnBallPassedProperties(UnitAIMemory.OnBallPassed, UnitAIMemory.BallEndPosition, UnitAIMemory.PossessionMateUnit, UnitAIMemory.BallTargetMateUnit, UnitAIMemory.Ball));
            tryToReactToBallPassMessage.AddChild(new CheckAreEqualMemoryVars<BaseUnit>(UnitAIMemory.Unit, UnitAIMemory.BallTargetMateUnit));
            tryToReactToBallPassMessage.AddChild(removeRecoverTheBallCoolDown);
            tryToReactToBallPassMessage.AddChild(new SetKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.PROCESSED));
            tryToReactToBallPassMessage.AddChild(faceBall);
            tryToReactToBallPassMessage.AddChild(moveToTargetPosition);
            tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(waitWhileBallIsComingTillIsClose));
            tryToReactToBallPassMessage.AddChild(new Succeder().SetChild(new RemoveMessageIfNotNew(UnitAIMemory.OnBallPassed)));

        //* What to do when team have ball posession
            // Do something if I have not the ball controlled
                // Choose between
                    // try to recover the ball if noone has it controlled
                    // try to react to Ball Pass Message (Active Selector ?)
                    // Try to Move to a valid position where the ball controller can pass me the ball
                        // Try to go up to the target goal
                        // Try to go down to move the ball
                    // Go to defensive position (to cover counter attack)
            // Do something if I have the ball controlled
                // Choose between
                    // try to move forward
                    // try to pass the ball
                        // easy pass
                        // forward pass
                    // try to shoot to the goal
        //* What to do when the enemy have ball posession
            // try to chase the target controller if he is close enough
            // try to go back to a defensive position if I'm not in my defensive position
            // move to block a possible shot from the enemy target to the goal
        //* what to do when noone have ball posession
            // Try to recover posession
                // Try to chase the ball

        // Try to Move To A Passing Position
        Sequence tryMovingToAPassingPosition = new Sequence();
            tryMovingToAPassingPosition.AddChild(findTargetDirection);
            tryMovingToAPassingPosition.AddChild(moveToTargetDirection);

        // Checl On Ball Passed Notificaton is not valid
        Selector checkOnBallPassedIsDoneOrNullOrInvalid = new Selector();
            checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new CheckNullMemoryVar(UnitAIMemory.OnBallPassed));
            checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new CheckKnowledgeStatus(UnitAIMemory.OnBallPassed, KnowledgeStatus.DONE));
            checkOnBallPassedIsDoneOrNullOrInvalid.AddChild(new Inverter().SetChild(checkBallIsMoving));

        Sequence tryToRecoverTheBallWhenAttacking = new Sequence();
            tryToRecoverTheBallWhenAttacking.AddChild(checkOnBallPassedIsDoneOrNullOrInvalid);
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

        RandomSelector attackWithBall = new RandomSelector();
            attackWithBall.AddChild(tryToPassTheBall);
            attackWithBall.AddChild(tryMovingWithBallToAForwardPosition);
            //attackWithBall.AddChild(tryToShoot);

        // Attack with ball controlled
        Sequence tryToAttackWithBall = new Sequence();
            tryToAttackWithBall.AddChild(new CheckAreEqualMemoryVars<bool>(UnitAIMemory.IsBallControlled, UnitAIMemory.TrueVar));
            tryToAttackWithBall.AddChild(attackWithBall);

        // Choose attack type
        Selector chooseAttack = new Selector();
            chooseAttack.AddChild(tryToAttackWithBall);
            chooseAttack.AddChild(tryToAttackWithoutBall);

        Sequence tryToAttack = new Sequence();
            tryToAttack.AddChild(new CheckAreEqualMemoryVars<BaseSquad>(UnitAIMemory.g_PossessionSquad, UnitAIMemory.Squad));
                tryToAttack.AddChild(chooseAttack);

        Sequence tryToDefend = new Sequence();
            tryToDefend.AddChild(new Failure());

        Sequence tryToRecoverPosession = new Sequence();
            tryToRecoverPosession.AddChild(new CheckNullMemoryVar(UnitAIMemory.g_PossessionSquad));
            tryToRecoverPosession.AddChild(recoverTheBall);

            // Main AIs subtypes
            mainAI.AddChild(tryToAttack);
            mainAI.AddChild(tryToDefend);
            mainAI.AddChild(tryToRecoverPosession);

        _bt = new BehaviorTree();

        _bt.SetSharedMemory(_squadMemory.Memory);
        _bt.SetGlobalMemory(_matchMemory.Memory);

        DebugUtils.Assert(_bt.MemoryManager.SquadMemory != null, "_bt.SharedMemory != null");

        // Init variables
        _bt.SetMemoryObject(UnitAIMemory.Unit, _unit);
        _bt.SetMemoryObject(UnitAIMemory.Squad, _unit.Squad);
        _bt.SetMemoryObject(UnitAIMemory.TrueVar, true);
        _bt.SetMemoryObject(UnitAIMemory.FalseVar, false);
        _bt.SetMemoryObject(UnitAIMemory.BallDistance, 6f);
        
        _bt.Init(mainAI);
        return _bt;
    }
}

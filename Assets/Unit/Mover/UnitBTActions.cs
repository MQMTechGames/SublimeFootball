using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.AI.Mover.Action
{
    public abstract class BaseUnitAction : Behavior
    {
        AIMemoryKey _unitKey;
        protected BaseUnit Unit;
        
        public BaseUnitAction(AIMemoryKey unitKey)
        {
            _unitKey = unitKey;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _bt.GetMemoryObject(_unitKey, out Unit);
        }
    }

    [System.Serializable]
    public class MoveUnitToTransform : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        Transform _transform;

        bool _isOk;
        Timer _timer = new Timer();

        public MoveUnitToTransform(AIMemoryKey unitKey, AIMemoryKey transformKey)
            :base(unitKey)
        {
            _positionKey = transformKey;
        }
        
        public override void OnInitialize()
        {
            base.OnInitialize();
            
            _isOk = _bt.GetMemoryObject(_positionKey, out _transform);
            if(!_isOk)
            {
                return;
            }

            _timer.Wait(0.1f);
        }
        
        public override Status Update()
        {
            if(!_isOk)
            {
                return Status.FAILURE;
            }

            float distToEnd = new Vector2(_transform.position.x - Unit.transform.position.x, _transform.position.z - Unit.transform.position.z).sqrMagnitude;
            if(distToEnd < 1f)
            {
                return Status.SUCCESS;
            }

            Unit.MoveTo(_transform.position);

            return Status.RUNNING;
        }
    }

    [System.Serializable]
    public class FaceUnitToTransform : BaseUnitAction
    {
        AIMemoryKey _transformKey;
        bool _isOk;
        Timer _timer = new Timer();
        
        public FaceUnitToTransform(AIMemoryKey unitKey, AIMemoryKey transformKey)
            :base(unitKey)
        {
            _transformKey = transformKey;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            _isOk = FaceTo();
            _timer.Wait(0.1f);
        }

        public override Status Update()
        {
            if(!_isOk)
            {
                return Status.FAILURE;
            }

            if(Unit.IsMoving())
            {
                FaceTo();
                return Status.RUNNING;
            }

            if(_timer.IsWaitting())
            {
                return Status.RUNNING;
            }

            return Status.SUCCESS;
        }

        bool FaceTo()
        {
            Transform transform;
            bool isOk = _bt.GetMemoryObject(_transformKey, out transform);
            if(isOk)
            {
                Unit.FaceTo(transform.position);
            }

            return isOk;
        }
    }

    [System.Serializable]
    public class MoveUnitToPosition : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        Vector3 _position;

        bool _isOk;
        Timer _timer = new Timer();

        public MoveUnitToPosition(AIMemoryKey unitKey, AIMemoryKey positionKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            _isOk = _bt.GetMemoryObject(_positionKey, out _position);
            if(!_isOk)
            {
                return;
            }

            Unit.MoveTo(_position);
            _timer.Wait(0.1f);
        }
        
        public override Status Update()
        {
            if(!_isOk)
            {
                return Status.FAILURE;
            }

            if(_timer.IsWaitting())
            {
                return Status.RUNNING;
            }

            if(Unit.IsMoving())
            {
                return Status.RUNNING;
            }

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CheckUnitDistanceToTransform : BaseUnitAction
    {
        AIMemoryKey _transformKey;
        AIMemoryKey _distanceKey;
        float _distanceBias = 0f;
        Transform _transform;

        public CheckUnitDistanceToTransform(AIMemoryKey unitKey, AIMemoryKey transformKey, AIMemoryKey distanceKey, float distanceBias)
            :base(unitKey)
        {
            _transformKey = transformKey;
            _distanceKey = distanceKey;
            _distanceBias = distanceBias;
        }

        public override Status Update()
        {
            bool isOk = _bt.GetMemoryObject(_transformKey, out _transform);
            DebugUtils.Assert(isOk, "Transform != null");

            float distance = 9f;
            isOk &= _bt.GetMemoryObject(_distanceKey, out distance);
            DebugUtils.Assert(isOk, "distance != null: " + _distanceKey.Name + ", transformKey was: " + _transformKey.Name);

            distance += _distanceBias;
            distance *= distance;

            Vector3 dirToTransform = Unit.transform.position - _transform.position;

            return dirToTransform.sqrMagnitude < distance ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class CheckUnitDistanceBetweenComponents : Behavior
    {
        AIMemoryKey _componentAKey;
        AIMemoryKey _componentBKey;
        float _distance = 6f;
        
        public CheckUnitDistanceBetweenComponents(AIMemoryKey componentAKey, AIMemoryKey componentBKey, float distance)
        {
            _componentAKey = componentAKey;
            _componentBKey = componentBKey;

            _distance = distance * distance;
        }
        
        public override Status Update()
        {
            Component componentA;
            bool isOk = _bt.GetMemoryObject(_componentAKey, out componentA);
            DebugUtils.Assert(isOk, "componentA != null");
            
            Component componentB;
            isOk &= _bt.GetMemoryObject(_componentBKey, out componentB);
            DebugUtils.Assert(isOk, "componentB != null: ");
            
            Vector3 dirToTransform = componentA.transform.position - componentB.transform.position;
            
            return dirToTransform.sqrMagnitude < _distance ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class FindClosestBall : BaseUnitAction
    {
        AIMemoryKey _ballKey;

        public FindClosestBall(AIMemoryKey unitKey, AIMemoryKey ballKey)
            :base(unitKey)
        {
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            Ball ball = Unit.FindClosestBall();

            _bt.SetMemoryObject(_ballKey, ball);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SavePositionFromTransform : Behavior
    {
        AIMemoryKey _transformKey;
        AIMemoryKey _positionKey;

        public SavePositionFromTransform(AIMemoryKey transformKey, AIMemoryKey positionKey)
        {
            _transformKey = transformKey;
            _positionKey = positionKey;
        }
        
        public override Status Update()
        {
            Transform transform;
            bool isOk = _bt.GetMemoryObject(_transformKey, out transform);

            if(!isOk || transform == null)
            {
                return Status.FAILURE;
            }

            _bt.SetMemoryObject(_positionKey, transform.position);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SavePositionFromComponent : Behavior
    {
        AIMemoryKey _componentKey;
        AIMemoryKey _positionKey;
        
        public SavePositionFromComponent(AIMemoryKey componentKey, AIMemoryKey positionKey)
        {
            _componentKey = componentKey;
            _positionKey = positionKey;
        }
        
        public override Status Update()
        {
            Component component;
            bool isOk = _bt.GetMemoryObject(_componentKey, out component);
            
            if(!isOk || component == null)
            {
                return Status.FAILURE;
            }
            
            _bt.SetMemoryObject(_positionKey, component.transform.position);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class GetClosestUnitToPosition : BaseUnitAction
    {
        AIMemoryKey _closestUnitKey;
        AIMemoryKey _positionKey;
        
        public GetClosestUnitToPosition(AIMemoryKey unitKey, AIMemoryKey closestUnitKey, AIMemoryKey positionKey)
            :base(unitKey)
        {
            _closestUnitKey = closestUnitKey;
            _positionKey = positionKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "isOk!=true");

            BaseUnit closestUnit = Unit.GetClosestUnitToPosition(position);
            DebugUtils.Assert(closestUnit!=null, "closestUnit!=null");

            _bt.SetMemoryObject(_closestUnitKey, closestUnit);
            
            return Status.SUCCESS;
        }
    }

    // TODO: Refactor with an iterator action
    [System.Serializable]
    public class SelectEasiestUnitToPassTheBall : BaseUnitAction
    {
        AIMemoryKey _easiestUnitKey;
        
        public SelectEasiestUnitToPassTheBall(AIMemoryKey unitKey, AIMemoryKey easiestUnitKey)
            :base(unitKey)
        {
            _easiestUnitKey = easiestUnitKey;
        }
        
        public override Status Update()
        {
            BaseUnit easiestUnit = Unit.SelectEasiestUnitToPassTheBall();
            _bt.SetMemoryObject(_easiestUnitKey, easiestUnit);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SelectRandomUnitToPassTheBall : BaseUnitAction
    {
        AIMemoryKey _easiestUnitKey;
        
        public SelectRandomUnitToPassTheBall(AIMemoryKey unitKey, AIMemoryKey easiestUnitKey)
            :base(unitKey)
        {
            _easiestUnitKey = easiestUnitKey;
        }
        
        public override Status Update()
        {
            BaseUnit easiestUnit = Unit.SelectRandomUnitToPassTheBall();
            _bt.SetMemoryObject(_easiestUnitKey, easiestUnit);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SelectForwardUnitToPassTheBall : BaseUnitAction
    {
        AIMemoryKey _resultUnitKey;
        AIMemoryKey _forwardDirectionKey;
        
        public SelectForwardUnitToPassTheBall(AIMemoryKey unitKey, AIMemoryKey forwardDirectionKey, AIMemoryKey resultUnitKey)
            :base(unitKey)
        {
            _resultUnitKey = resultUnitKey;
            _forwardDirectionKey = forwardDirectionKey;
        }
        
        public override Status Update()
        {
            Vector3 direction;
            bool isOk = _bt.GetMemoryObject(_forwardDirectionKey, out direction);
            DebugUtils.Assert(isOk, "forwardDirection Not found");

            BaseUnit easiestUnit = Unit.SelectForwardUnitToPassTheBall(direction);
            _bt.SetMemoryObject(_resultUnitKey, easiestUnit);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class PassBallToPosition : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;
        AIMemoryKey _maxHeightKey;

        public PassBallToPosition(AIMemoryKey unitKey, AIMemoryKey ballKey, AIMemoryKey positionKey, AIMemoryKey maxHeightKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _ballKey = ballKey;
            _maxHeightKey = maxHeightKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            _bt.GetMemoryObject(_positionKey, out position);

            float maxHeight = 50f;
            _bt.GetMemoryObject(_maxHeightKey, out maxHeight);

            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball!=null, "ball!=null");

            Unit.PassBallToPosition(position, maxHeight, ball);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CalculateBallMaxHeight : Behavior
    {
        [SerializeField]
        float minDistaceToElevateBall  = 25f;

        [SerializeField]
        float maxDistaceToElevateBall  = 512f;

        [SerializeField]
        float _maxHeight  = 80f;

        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;
        AIMemoryKey _maxHeightKey;
        
        public CalculateBallMaxHeight(AIMemoryKey ballKey, AIMemoryKey positionKey, AIMemoryKey maxHeightKey)
        {
            _positionKey = positionKey;
            _ballKey = ballKey;
            _maxHeightKey = maxHeightKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            _bt.GetMemoryObject(_positionKey, out position);
            
            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball!=null, "ball!=null");

            Vector3 distToTargetPosition = position - ball.transform.position;
            float distNormalized = Mathf.Clamp((distToTargetPosition.magnitude - minDistaceToElevateBall) / (maxDistaceToElevateBall - minDistaceToElevateBall), 0f , 1f);
            float currentMaxHeight = distNormalized * _maxHeight;
            
            _bt.SetMemoryObject(_maxHeightKey, currentMaxHeight);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SetOwnerToBall : BaseUnitAction
    {
        AIMemoryKey _ballKey;
        
        public SetOwnerToBall(AIMemoryKey unitKey, AIMemoryKey ballKey)
            :base(unitKey)
        {
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);

            ball.Owner = Unit;
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SaveBallOwner : Behavior
    {
        AIMemoryKey _ballKey;
        AIMemoryKey _ownerVarKey;
        
        public SaveBallOwner(AIMemoryKey ballKey, AIMemoryKey ownerVarKey)
        {
            _ballKey = ballKey;
            _ownerVarKey = ownerVarKey;
        }
        
        public override Status Update()
        {
            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball!=null, "ball!=null");

            _bt.SetMemoryObject(_ownerVarKey, ball.Owner);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CreateBallPassedMessage : BaseUnitAction
    {
        AIMemoryKey _targetUnitKey;
        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;
        AIMemoryKey _messageKey;
        
        public CreateBallPassedMessage(AIMemoryKey unitKey, AIMemoryKey targetUnitKey, AIMemoryKey positionKey, AIMemoryKey ballKey, AIMemoryKey messageKey)
            :base(unitKey)
        {
            _targetUnitKey =  targetUnitKey;
            _positionKey = positionKey;
            _messageKey = messageKey;
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            BaseUnit targetUnit;
            bool isOk = _bt.GetMemoryObject(_targetUnitKey, out targetUnit);
            DebugUtils.Assert(isOk, "targetUnit not found in memory");

            Vector3 position;
            isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position not found in memory");

            Ball ball;
            isOk = _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(isOk, "ball not found in memory");

            BallPassedMessage message = new BallPassedMessage(position, Unit, targetUnit, ball);

            _bt.SetMemoryObject(_messageKey, message);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CreateBallShotMessage : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;
        AIMemoryKey _messageKey;
        
        public CreateBallShotMessage(AIMemoryKey unitKey, AIMemoryKey positionKey, AIMemoryKey ballKey, AIMemoryKey messageKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _messageKey = messageKey;
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position not found in memory");
            
            Ball ball;
            isOk = _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(isOk, "ball not found in memory");
            
            BallShotMessage message = new BallShotMessage(position, Unit, ball);
            
            _bt.SetMemoryObject(_messageKey, message);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CalculateDistanceToTransform : BaseUnitAction
    {
        [SerializeField]
        private bool _checkYAxis;

        AIMemoryKey _transformKey;
        AIMemoryKey _resultKey;

        public CalculateDistanceToTransform(AIMemoryKey unitKey, AIMemoryKey transformKey, AIMemoryKey resultKey, bool checkYAxis)
            :base(unitKey)
        {
            _transformKey = transformKey;
            _resultKey = resultKey;
            _checkYAxis = checkYAxis;
        }
        
        public override Status Update()
        {
            Transform transform;
            bool isOk = _bt.GetMemoryObject(_transformKey, out transform);
            DebugUtils.Assert(isOk, "transform not found in memory");

            float dist = 0f;
            if(!_checkYAxis)
            {
                dist = new Vector2(transform.position.x - Unit.transform.position.x, transform.position.z - Unit.transform.position.z).magnitude;
            }
            else
            {
                dist = (transform.position - Unit.transform.position).magnitude;
            }
            
            _bt.SetMemoryObject(_resultKey, dist);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class FindGoal : BaseUnitAction
    {
         AIMemoryKey _goalKey;
        
        public FindGoal(AIMemoryKey unitKey, AIMemoryKey goalKey)
            :base(unitKey)
        {
            _goalKey = goalKey;
        }
        
        public override Status Update()
        {
            Goal goal = Unit.FindGoal();
            _bt.SetMemoryObject(_goalKey, goal);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SelectGoalShootPosition : BaseUnitAction
    {
        AIMemoryKey _goalKey;
        AIMemoryKey _positionKey;
        
        public SelectGoalShootPosition(AIMemoryKey unitKey, AIMemoryKey goalKey, AIMemoryKey positionKey)
            :base(unitKey)
        {
            _goalKey = goalKey;
            _positionKey = positionKey;
        }
        
        public override Status Update()
        {
            Goal goal;
            bool isOk = _bt.GetMemoryObject(_goalKey, out goal);
            DebugUtils.Assert(isOk, "goal not found");

            Vector3 position = Unit.SelectGoalShootPosition(goal);
            _bt.SetMemoryObject(_positionKey, position);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class ShootToPosition : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;
        AIMemoryKey _shotStrength;
        
        public ShootToPosition(AIMemoryKey unitKey, AIMemoryKey ballKey, AIMemoryKey positionKey, AIMemoryKey shotStrength)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _ballKey = ballKey;
            _shotStrength = shotStrength;
        }
        
        public override Status Update()
        {
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position not found");

            Ball ball;
            isOk = _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(isOk, "ball not found");

            float shotStrength;
            isOk = _bt.GetMemoryObject(_shotStrength, out shotStrength);
            DebugUtils.Assert(isOk, "shotStrength not found");
            
            Unit.ShootToPosition(position, shotStrength, ball);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SaveOnBallPassedProperties : Behavior
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _messageKey;
        AIMemoryKey _possessionMateUnitKey;
        AIMemoryKey _targetMateUnitKey;
        AIMemoryKey _ballKey;
        
        public SaveOnBallPassedProperties(AIMemoryKey messageKey, AIMemoryKey positionKey, AIMemoryKey possessionMateUnitKey, AIMemoryKey targetMateUnitKey, AIMemoryKey ballKey)
        {
            _messageKey = messageKey;
            _positionKey = positionKey;
            _possessionMateUnitKey = possessionMateUnitKey;
            _targetMateUnitKey = targetMateUnitKey;
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            BallPassedMessage message;
            bool isOk = _bt.GetMemoryObject(_messageKey, out message);
            DebugUtils.Assert(isOk, "message not found in memory");

            DebugUtils.Assert(message.Unit!=null, "message.Unit!=null");
            DebugUtils.Assert(message.TargetUnit!=null, "message.TargetUnit!=null");
            DebugUtils.Assert(message.Ball!=null, "message.Ball!=null");
            
            _bt.SetMemoryObject(_positionKey, message.WorldPosition);
            _bt.SetMemoryObject(_possessionMateUnitKey, message.Unit);
            _bt.SetMemoryObject(_targetMateUnitKey, message.TargetUnit);
            _bt.SetMemoryObject(_ballKey, message.Ball);
            
            return Status.SUCCESS;
        }
    }


    [System.Serializable]
    public class SendMessageToUnit : Behavior
    {
        AIMemoryKey _targetUnitKey;
        AIMemoryKey _messageKey;
        string _messageName;
        
        public SendMessageToUnit(AIMemoryKey targetUnitKey, AIMemoryKey messageKey, string messageName)
        {
            _targetUnitKey = targetUnitKey;
            _messageKey = messageKey;
            _messageName = messageName;
        }
        
        public override Status Update()
        {
            BaseUnit targetUnit;
            bool isOk = _bt.GetMemoryObject(_targetUnitKey, out targetUnit);
            DebugUtils.Assert(isOk, "unit not found in memory");

            AIMessage message;
            isOk &= _bt.GetMemoryObject(_messageKey, out message);
            DebugUtils.Assert(isOk, "message not found in memory");
            
            targetUnit.SendAIMessage(_messageName, message);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SaveTransformFromComponent<T> : Behavior where T : Component
    {
        AIMemoryKey _componentKey;
        AIMemoryKey _transformKey;
        
        public SaveTransformFromComponent(AIMemoryKey componentKey, AIMemoryKey transformKey)
        {
            _componentKey = componentKey;
            _transformKey = transformKey;
        }
        
        public override Status Update()
        {
            Component component;
            bool isOk = _bt.GetMemoryObject(_componentKey, out component);
            
            if(!isOk || component == null)
            {
                return Status.FAILURE;
            }
            
            _bt.SetMemoryObject(_transformKey, component.transform);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CheckDistanceBetweenPositions : Behavior
    {
        AIMemoryKey _positionAKey;
        AIMemoryKey _positionBKey;
        AIMemoryKey _maxDistanceKey;
        float _bias = 0f;

        public CheckDistanceBetweenPositions(AIMemoryKey positionAKey, AIMemoryKey positionBKey, AIMemoryKey maxDistanceKey, float bias)
        {
            _positionAKey = positionAKey;
            _positionBKey = positionBKey;
            _maxDistanceKey = maxDistanceKey;
            _bias = bias;
        }
        
        public override Status Update()
        {
            Vector3 positionA;
            bool isOk = _bt.GetMemoryObject(_positionAKey, out positionA);

            Vector3 positionB;
            isOk &= _bt.GetMemoryObject(_positionBKey, out positionB);

            float maxDistance;
            isOk &= _bt.GetMemoryObject(_maxDistanceKey, out maxDistance);
            
            if(!isOk)
            {
                return Status.FAILURE;
            }

            maxDistance += _bias;
            maxDistance *= maxDistance;

            return (positionA - positionB).sqrMagnitude < maxDistance ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class CheckDistanceFromComponentToPosition : Behavior
    {
        AIMemoryKey _componentKey;
        AIMemoryKey _positionKey;
        AIMemoryKey _maxDistanceKey;
        float _bias = 0f;

        bool _debug;
        
        public CheckDistanceFromComponentToPosition(AIMemoryKey componentKey, AIMemoryKey positionKey, AIMemoryKey maxDistanceKey, float bias)
        {
            _componentKey = componentKey;
            _positionKey = positionKey;
            _maxDistanceKey = maxDistanceKey;
            _bias = bias;
        }

        public Behavior EnableDebug()
        {
            _debug = true;

            return this;
        }
        
        public override Status Update()
        {
            Component component;
            bool isOk = _bt.GetMemoryObject(_componentKey, out component);
            DebugUtils.Assert(isOk, "component not found");
            
            Vector3 position;
            isOk &= _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position not found for " + _positionKey.Name + ", component was: " + _componentKey.Name + " maxDistanceKey was: " + _maxDistanceKey.Name);
            
            float maxDistance;
            isOk &= _bt.GetMemoryObject(_maxDistanceKey, out maxDistance);
            DebugUtils.Assert(isOk, "maxDistance not found for " + _maxDistanceKey.Name + ", component was: " + _componentKey.Name + " position was: " + _positionKey.Name);
            
            if(!isOk)
            {
                return Status.FAILURE;
            }
            
            maxDistance += _bias;
            maxDistance *= maxDistance;

            Status status = (component.transform.position - position).sqrMagnitude < maxDistance ? Status.SUCCESS : Status.FAILURE;

            if(_debug)
            {
                Debug.Log("maxDistance: " + maxDistance + ", sqrDistance: " + (component.transform.position - position).sqrMagnitude + ", status: " + status);
            }

            return status;
        }
    }

    [System.Serializable]
    public class SaveVelocityFromComponent : Behavior
    {
        AIMemoryKey _componentKey;
        AIMemoryKey _velocityKey;
        
        public SaveVelocityFromComponent(AIMemoryKey componentKey, AIMemoryKey velocityKey)
        {
            _componentKey = componentKey;
            _velocityKey = velocityKey;
        }
        
        public override Status Update()
        {
            Component component;
            bool isOk = _bt.GetMemoryObject(_componentKey, out component);
            DebugUtils.Assert(isOk, "component is null");

            if(!isOk || component == null)
            {
                return Status.FAILURE;
            }

            _bt.SetMemoryObject(_velocityKey, component.rigidbody.velocity);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CalculateMagnitudeFromVector : Behavior
    {
        AIMemoryKey _vectorKey;
        AIMemoryKey _magnitudeKey;
        
        public CalculateMagnitudeFromVector(AIMemoryKey vectorKey, AIMemoryKey magnitudeKey)
        {
            _vectorKey = vectorKey;
            _magnitudeKey = magnitudeKey;
        }
        
        public override Status Update()
        {
            Vector3 velocity;
            bool isOk = _bt.GetMemoryObject(_vectorKey, out velocity);
            DebugUtils.Assert(isOk, "velocity is null");
            
            _bt.SetMemoryObject(_magnitudeKey, velocity.magnitude);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CalculateDirectionToPosition : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _directionKey;
        
        public CalculateDirectionToPosition(AIMemoryKey unitKey, AIMemoryKey positionKey, AIMemoryKey directionKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _directionKey = directionKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position is null");
            
            _bt.SetMemoryObject(_directionKey, (position - Unit.transform.position).normalized);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CheckPositionIsInAttackingZone : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        
        public CheckPositionIsInAttackingZone(AIMemoryKey unitKey, AIMemoryKey positionKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position is null");

            bool isAttackingZone = Unit.CheckPositionIsInAttackingZone(position);
            
            return isAttackingZone ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class Vector3Normalize : Behavior
    {
        AIMemoryKey _vectorKey;
        AIMemoryKey _resultKey;
        
        public Vector3Normalize(AIMemoryKey vectorKey, AIMemoryKey resultKey)
        {
            _vectorKey = vectorKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");
            
            _bt.SetMemoryObject(_resultKey, vector.normalized);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3Invert : Behavior
    {
        AIMemoryKey _vectorKey;
        AIMemoryKey _resultKey;
        
        public Vector3Invert(AIMemoryKey vectorKey, AIMemoryKey resultKey)
        {
            _vectorKey = vectorKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");
            
            _bt.SetMemoryObject(_resultKey, -vector);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3Dot : Behavior
    {
        AIMemoryKey _vectorAKey;
        AIMemoryKey _vectorBKey;
        AIMemoryKey _resultKey;
        
        public Vector3Dot(AIMemoryKey vectorAKey, AIMemoryKey vectorBKey, AIMemoryKey resultKey)
        {
            _vectorAKey = vectorAKey;
            _vectorBKey = vectorBKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorAKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");

            Vector3 vectorB;
            isOk = _bt.GetMemoryObject(_vectorBKey, out vectorB);
            DebugUtils.Assert(isOk, "vectorB is null");
            
            _bt.SetMemoryObject(_resultKey, Vector3.Dot(vector, vectorB));
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3Cross : Behavior
    {
        AIMemoryKey _vectorAKey;
        AIMemoryKey _vectorBKey;
        AIMemoryKey _resultKey;
        
        public Vector3Cross(AIMemoryKey vectorAKey, AIMemoryKey vectorBKey, AIMemoryKey resultKey)
        {
            _vectorAKey = vectorAKey;
            _vectorBKey = vectorBKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorAKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");
            
            Vector3 vectorB;
            isOk = _bt.GetMemoryObject(_vectorBKey, out vectorB);
            DebugUtils.Assert(isOk, "vectorB is null");
            
            _bt.SetMemoryObject(_resultKey, Vector3.Cross(vector, vectorB));
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3MultiplyByScalar : Behavior
    {
        AIMemoryKey _vectorKey;
        AIMemoryKey _scalarKey;
        AIMemoryKey _resultKey;
        
        public Vector3MultiplyByScalar(AIMemoryKey vectorKey, AIMemoryKey scalarKey, AIMemoryKey resultKey)
        {
            _vectorKey = vectorKey;
            _scalarKey = scalarKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");
            
            float scalar;
            isOk = _bt.GetMemoryObject(_scalarKey, out scalar);
            DebugUtils.Assert(isOk, "scalar is null");
            
            _bt.SetMemoryObject(_resultKey, vector * scalar);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3Add : Behavior
    {
        AIMemoryKey _vectorAKey;
        AIMemoryKey _vectorBKey;
        AIMemoryKey _resultKey;
        
        public Vector3Add(AIMemoryKey vectorAKey, AIMemoryKey vectorBKey, AIMemoryKey resultKey)
        {
            _vectorAKey = vectorAKey;
            _vectorBKey = vectorBKey;
            _resultKey = resultKey;
        }
        
        public override Status Update()
        {
            Vector3 vector;
            bool isOk = _bt.GetMemoryObject(_vectorAKey, out vector);
            DebugUtils.Assert(isOk, "vector is null");
            
            Vector3 vectorB;
            isOk = _bt.GetMemoryObject(_vectorBKey, out vectorB);
            DebugUtils.Assert(isOk, "vectorB is null");
            
            _bt.SetMemoryObject(_resultKey, vector + vectorB);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class Vector3StepToPosition : Behavior
    {
        [SerializeField]
        bool useYComponent;

        float _step = 2f;

        AIMemoryKey _srcPositionKey;
        AIMemoryKey _dstPositionKey;
        AIMemoryKey _resultKey;

        public Vector3StepToPosition(AIMemoryKey srcPositionKey, AIMemoryKey dstPositionKey, AIMemoryKey resultKey, float step)
        {
            _srcPositionKey = srcPositionKey;
            _dstPositionKey = dstPositionKey;
            _resultKey = resultKey;
            _step = step;
        }
        
        public override Status Update()
        {
            Vector3 src;
            bool isOk = _bt.GetMemoryObject(_srcPositionKey, out src);
            DebugUtils.Assert(isOk, "src is null");
            
            Vector3 dst;
            isOk = _bt.GetMemoryObject(_dstPositionKey, out dst);
            DebugUtils.Assert(isOk, "dst is null");

            Vector3 nextStepPosition = Vector3.zero;
            if(useYComponent)
            {
                Vector3 dirToDest = dst - src;
                float mag = dirToDest.magnitude;
                mag = Mathf.Min(mag, _step);

                nextStepPosition = src + dirToDest.normalized * mag;
            }
            else
            {
                Vector2 dirXZ = Vector3Utils.ToDirXZ(dst, src);
                float mag = dirXZ.magnitude;
                mag = Mathf.Min(mag, _step);

                dirXZ.Normalize();

                nextStepPosition = src + new Vector3(dirXZ.x, 0f, dirXZ.y) * mag;
            }
            
            _bt.SetMemoryObject(_resultKey, nextStepPosition);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CheckUnitIsTeamMate : BaseUnitAction
    {
        
        AIMemoryKey _testUnitKey;
        
        public CheckUnitIsTeamMate(AIMemoryKey unitKey, AIMemoryKey testUnitKey)
           : base(unitKey)
        {
            _testUnitKey = testUnitKey;
        }
        
        public override Status Update()
        {
            BaseUnit testUnit;
            bool isOk = _bt.GetMemoryObject(_testUnitKey, out testUnit);
            DebugUtils.Assert(isOk, "testUnit is null");

            bool isTeamMate = Unit.CheckOtherIsTeamMate(testUnit);
            
            return isTeamMate? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class SmashUnit : BaseUnitAction
    {
        
        AIMemoryKey _testUnitKey;
        
        public SmashUnit(AIMemoryKey unitKey, AIMemoryKey testUnitKey)
            : base(unitKey)
        {
            _testUnitKey = testUnitKey;
        }
        
        public override Status Update()
        {
            BaseUnit testUnit;
            bool isOk = _bt.GetMemoryObject(_testUnitKey, out testUnit);
            DebugUtils.Assert(testUnit, "src is null");
            
            Unit.SmashUnit(testUnit);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SmashBall : BaseUnitAction
    {
        AIMemoryKey _ballKey;
        
        public SmashBall(AIMemoryKey unitKey, AIMemoryKey ballKey)
            : base(unitKey)
        {
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            Ball ball;
            bool isOk = _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball, "src is null");

            Unit.SmashBall(ball);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class ProcessBeingSmashed : BaseUnitAction
    {
        AIMemoryKey _smashKey;
        
        public ProcessBeingSmashed(AIMemoryKey unitKey, AIMemoryKey smashKey)
            : base(unitKey)
        {
            _smashKey = smashKey;
        }
        
        public override Status Update()
        {
            SmashUnitMessage smashMsg;
            bool isOk = _bt.GetMemoryObject(_smashKey, out smashMsg);
            DebugUtils.Assert(isOk, "smashMsg is null");
            
            Unit.ProcessBeingSmashed(smashMsg.Smasher, smashMsg.SmashForce);
            
            return Status.SUCCESS;
        }
    }
}

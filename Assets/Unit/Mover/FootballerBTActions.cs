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

//        public override void OnInit(BehaviorTree bt)
//        {
//            base.OnInit(bt);
//
//            _bt.GetMemoryObject(_unitKey, out Unit);
//        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _bt.GetMemoryObject(_unitKey, out Unit);
        }
    }

   [System.Serializable]
    public class CheckSquadHavePosession : BaseUnitAction
    {
        public CheckSquadHavePosession(AIMemoryKey unitKey)
            :base(unitKey)
        {}

        public override Status Update()
        {
            return Unit.HaveSquadPosession() ? Status.SUCCESS : Status.FAILURE;
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
        Transform _transform;

        public CheckUnitDistanceToTransform(AIMemoryKey unitKey, AIMemoryKey transformKey, AIMemoryKey distanceKey)
            :base(unitKey)
        {
            _transformKey = transformKey;
            _distanceKey = distanceKey;
        }

        public override Status Update()
        {
            bool isOk = _bt.GetMemoryObject(_transformKey, out _transform);
            DebugUtils.Assert(isOk, "Transform != null");

            float distance = 9f;
            isOk &= _bt.GetMemoryObject(_distanceKey, out distance);
            DebugUtils.Assert(isOk, "distance != null");

            distance = distance * distance;

            Vector3 dirToTransform = Unit.transform.position - _transform.position;

            return dirToTransform.sqrMagnitude < distance ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class OnBallControlled : BaseUnitAction
    {
        AIMemoryKey _ballKey;

        public OnBallControlled(AIMemoryKey unitKey, AIMemoryKey ballKey)
            :base(unitKey)
        {
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball != null, "ball != null");
            Unit.OnBallControlled(ball);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class OnBallLost : BaseUnitAction
    {
        public OnBallLost(AIMemoryKey unitKey)
            :base(unitKey)
        {
        }
        
        public override Status Update()
        {
            Unit.OnBallLost();

            return Status.SUCCESS;
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
            BaseUnit easiestUnit = Unit.GetEasiestUnitToPassTheBall();
            _bt.SetMemoryObject(_easiestUnitKey, easiestUnit);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class CheckUnitHavePosession : BaseUnitAction
    {
        public CheckUnitHavePosession(AIMemoryKey unitKey)
            :base(unitKey)
        {
        }
        
        public override Status Update()
        {
            return Unit.HavePosession() ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class PassBallToPosition : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _ballKey;

        public PassBallToPosition(AIMemoryKey unitKey, AIMemoryKey ballKey, AIMemoryKey positionKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            _bt.GetMemoryObject(_positionKey, out position);

            Ball ball;
            _bt.GetMemoryObject(_ballKey, out ball);
            DebugUtils.Assert(ball!=null, "ball!=null");

            Unit.PassBallToPosition(position, ball);

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
    public class SaveOwnerToBall : Behavior
    {
        AIMemoryKey _ballKey;
        AIMemoryKey _ownerVarKey;
        
        public SaveOwnerToBall(AIMemoryKey ballKey, AIMemoryKey ownerVarKey)
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
    public class CreateWillPassTheBallMessage : BaseUnitAction
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _messageKey;
        AIMemoryKey _durationKey;
        
        public CreateWillPassTheBallMessage(AIMemoryKey unitKey, AIMemoryKey positionKey, AIMemoryKey durationKey, AIMemoryKey messageKey)
            :base(unitKey)
        {
            _positionKey = positionKey;
            _messageKey = messageKey;
            _durationKey = durationKey;
        }
        
        public override Status Update()
        {
            Vector3 position;
            bool isOk = _bt.GetMemoryObject(_positionKey, out position);
            DebugUtils.Assert(isOk, "position not found in memory");

            float duration = 1f;
            isOk &= _bt.GetMemoryObject(_durationKey, out duration);
            DebugUtils.Assert(isOk, "duration not found in memory");

            OnWillPassTheBall message = new OnWillPassTheBall(duration, position, Unit);

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
    public class SaveBallFromPossesionUnit : Behavior
    {
        AIMemoryKey _ballKey;
        AIMemoryKey _unitKey;
        
        public SaveBallFromPossesionUnit(AIMemoryKey unitKey, AIMemoryKey ballKey)
        {
            _unitKey = unitKey;
            _ballKey = ballKey;
        }
        
        public override Status Update()
        {
            BaseUnit unit;
            _bt.GetMemoryObject(_unitKey, out unit);

            Ball ball = unit.GetControlledBall();
            if(ball == null)
            {
                return Status.FAILURE;
            }

            _bt.SetMemoryObject(_ballKey, ball);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SaveOnWillPassTheBallProperties : Behavior
    {
        AIMemoryKey _positionKey;
        AIMemoryKey _messageKey;
        AIMemoryKey _possessionMateUnitKey;
        
        public SaveOnWillPassTheBallProperties(AIMemoryKey messageKey, AIMemoryKey positionKey, AIMemoryKey possessionMateUnitKey)
        {
            _messageKey = messageKey;
            _positionKey = positionKey;
            _possessionMateUnitKey = possessionMateUnitKey;
        }
        
        public override Status Update()
        {
            OnWillPassTheBall message;
            bool isOk = _bt.GetMemoryObject(_messageKey, out message);
            DebugUtils.Assert(isOk, "message not found in memory");

            DebugUtils.Assert(message.Unit!=null, "message.Unit!=null");
            
            _bt.SetMemoryObject(_positionKey, message.WorldPosition);
            _bt.SetMemoryObject(_possessionMateUnitKey, message.Unit);
            
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
    public class SavePositionFromComponent<T> : Behavior where T : Component
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
    public class CheckDistanceBetweenPositions : Behavior
    {
        AIMemoryKey _positionAKey;
        AIMemoryKey _positionBKey;
        AIMemoryKey _maxDistanceKey;

        public CheckDistanceBetweenPositions(AIMemoryKey positionAKey, AIMemoryKey positionBKey, AIMemoryKey maxDistanceKey)
        {
            _positionAKey = positionAKey;
            _positionBKey = positionBKey;
            _maxDistanceKey = maxDistanceKey;
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

            return (positionA - positionB).sqrMagnitude < (maxDistance * maxDistance) ? Status.SUCCESS : Status.FAILURE;
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
}

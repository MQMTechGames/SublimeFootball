using UnityEngine;
using MQMTech.AI.BT;

namespace MQMTech.AI.Mover.Action
{
    [System.Serializable]
    public class SaveMoveToProperties : Behavior
    {
        AIMemoryKey _moveToKey;
        AIMemoryKey _destinyTargetKey;

        public SaveMoveToProperties(AIMemoryKey moveToKey, AIMemoryKey destinyTargetKey)
        {
            _moveToKey = moveToKey;
            _destinyTargetKey = destinyTargetKey;
        }

        public override Status Update()
        {
            MoveToCommand command;
            _bt.GetMemoryObject(_moveToKey, out command);
            DebugUtils.Assert(command!=null, "command!=null");

            _bt.SetMemoryObject(_destinyTargetKey, command.WorldPos);

            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SaveFaceToProperties : Behavior
    {
        AIMemoryKey _moveToKey;
        AIMemoryKey _destinyTargetKey;
        
        public SaveFaceToProperties(AIMemoryKey moveToKey, AIMemoryKey destinyTargetKey)
        {
            _moveToKey = moveToKey;
            _destinyTargetKey = destinyTargetKey;
        }
        
        public override Status Update()
        {
            FaceToCommand command;
            _bt.GetMemoryObject(_moveToKey, out command);
            DebugUtils.Assert(command!=null, "command!=null");
            
            _bt.SetMemoryObject(_destinyTargetKey, command.WorldPos);
            
            return Status.SUCCESS;
        }
    }

    [System.Serializable]
    public class SimpleMoveToAction : Behavior
    {
        [SerializeField]
        float _speed;

        [SerializeField]
        bool enableYMovement;

        AIMemoryKey _moverKey;
        AIMemoryKey _destinyTargetKey;

        BaseMover _mover;
        Vector3 _targetWorldPos;

        bool _isOk;
        
        public SimpleMoveToAction(AIMemoryKey moverKey, AIMemoryKey destinyTargetKey)
        {
            _moverKey = moverKey;
            _destinyTargetKey = destinyTargetKey;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            _mover = null;
            _isOk = _bt.GetMemoryObject(_moverKey, out _mover);
            _isOk &= _bt.GetMemoryObject(_destinyTargetKey, out _targetWorldPos);
        }
        
        public override Status Update()
        {
            if(!_isOk)
            {
                return Status.FAILURE;
            }

            if(!enableYMovement)
            {
                _targetWorldPos.y = 0f;
            }

            float distance = _speed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(_mover.transform.position, _targetWorldPos, distance);

            _mover.transform.position = newPos;

            return newPos == _targetWorldPos ? Status.SUCCESS : Status.RUNNING;
        }
    }

    [System.Serializable]
    public class SimpleRotateToAction : Behavior
    {
        [SerializeField]
        float _speed = 1;
        
        AIMemoryKey _moverKey;
        AIMemoryKey _destinyTargetKey;
        
        BaseMover _mover;
        Vector3 _targetWorldPos;
        
        bool _isOk;
        
        public SimpleRotateToAction(AIMemoryKey moverKey, AIMemoryKey destinyTargetKey)
        {
            _moverKey = moverKey;
            _destinyTargetKey = destinyTargetKey;
        }
        
        public override void OnInitialize()
        {
            base.OnInitialize();
            
            _mover = null;
            _isOk = _bt.GetMemoryObject(_moverKey, out _mover);
            _isOk &= _bt.GetMemoryObject(_destinyTargetKey, out _targetWorldPos);
        }
        
        public override Status Update()
        {
            if(!_isOk)
            {
                return Status.FAILURE;
            }

            Vector3 targetDir = (_targetWorldPos - _mover.transform.position).normalized;
            float frontProjection = Vector3.Dot(targetDir, _mover.transform.forward);
            float rightProjection = Vector3.Dot(targetDir, _mover.transform.right);
            float radians = Mathf.Atan2(rightProjection, frontProjection);
            float angle = (180.0f / Mathf.PI) * radians;

            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);
            Quaternion newQuat = quat * _mover.transform.rotation;
            newQuat = Quaternion.Slerp(_mover.transform.rotation, newQuat, Time.deltaTime * _speed);
            _mover.transform.rotation = newQuat;

            return Mathf.Abs(angle) < 0.1f ? Status.SUCCESS : Status.RUNNING;
        }
    }
}

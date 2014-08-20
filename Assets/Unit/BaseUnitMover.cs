using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.Unit.Mover
{
    [RequireComponent(typeof(SimpleUnitMoverBuilder))]
    public class BaseUnitMover : BaseMover
    {
        [SerializeField]
        SimpleUnitMoverBuilder _btBuilder;

        BehaviorTree _bt;

        void Awake()
        {
            _btBuilder = GameObjectUtils.GetInterfaceObject<SimpleUnitMoverBuilder>(gameObject);
        }

        void Start()
        {
            _bt = _btBuilder.Create();
        }

        void Update()
        {
            _bt.Tick();
        }

        public override void MoveTo(Vector3 worldPosition)
        {
            MoveToCommand command = new MoveToCommand();
            command.WorldPos = worldPosition;
            
            _bt.SetMemoryObject(UnitAIMemory.MoverCommand, command);
        }

        public override void FaceTo(Vector3 worldPosition)
        {
            FaceToCommand command = new FaceToCommand();
            command.WorldPos = worldPosition;
            
            _bt.SetMemoryObject(UnitAIMemory.MoverCommand, command);
        }

        public override bool IsMoving()
        {
            BaseKnowledge command;
            bool isOk = _bt.GetMemoryObject(UnitAIMemory.MoverCommand, out command);

            if(!isOk)
            {
                return false;
            }

            return command.Status != MQMTech.AI.Knowledge.KnowledgeStatus.DONE ? true : false;
        }
    }
}

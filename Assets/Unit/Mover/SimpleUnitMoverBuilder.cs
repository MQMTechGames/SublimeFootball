using UnityEngine;

using MQMTech.AI.BT;
using MQMTech.AI.Knowledge.Action;
using MQMTech.AI.Mover.Action;
using MQMTech.AI.Knowledge;

[System.Serializable]
public class SimpleUnitMoverBuilder : MonoBehaviour, IBehaviorWithTree
{
    BaseMover _mover;
    BehaviorTree _bt;

    [SerializeField]
    SimpleMoveToAction _simpleMoveToAction = new SimpleMoveToAction(UnitAIMemory.Mover, UnitAIMemory.BallEndPosition);

    [SerializeField]
    SimpleRotateToAction _simpleRotateToAction = new SimpleRotateToAction(UnitAIMemory.Mover, UnitAIMemory.BallEndPosition);

    void Awake()
    {
        _mover = GameObjectUtils.GetInterfaceObject<BaseMover>(gameObject);
        DebugUtils.Assert(_mover!=null, "_mover!=null");
    }

    public BehaviorTree GetBehaviorTree()
    {
        return _bt;
    }

    public BehaviorTree Create()
    {
        Selector mainAI = new Selector();

        Sequence checkCanKeepMoving = new Sequence();
            checkCanKeepMoving.AddChild(new Inverter().SetChild(new CheckKnowledgeStatus(UnitAIMemory.MoverCommand, KnowledgeStatus.NEW)));
            checkCanKeepMoving.AddChild(new Inverter().SetChild(new CheckKnowledgeStatus(UnitAIMemory.MoverCommand, KnowledgeStatus.NEW)));

        Sequence keepMoving = new Sequence();
            Parallel moveAndRotate = new Parallel(Parallel.ParallelPolicy.SUCCESS_IF_ALL, Parallel.ParallelPolicy.FAILURE_IF_ONE);
            moveAndRotate.AddChild(_simpleMoveToAction);
            moveAndRotate.AddChild(_simpleRotateToAction);
            keepMoving.AddChild(moveAndRotate);

        ActiveSequence move = new ActiveSequence();
            move.AddChild(checkCanKeepMoving);
            move.AddChild(keepMoving);

        Sequence tryToMoveByCommand = new Sequence();
            tryToMoveByCommand.AddChild(new CheckMemoryType<MoveToCommand>(UnitAIMemory.MoverCommand));
            tryToMoveByCommand.AddChild(new SaveMoveToProperties(UnitAIMemory.MoverCommand, UnitAIMemory.BallEndPosition));
            tryToMoveByCommand.AddChild(new SetKnowledgeStatus(UnitAIMemory.MoverCommand, KnowledgeStatus.PROCESSED));
            tryToMoveByCommand.AddChild(move);
            tryToMoveByCommand.AddChild(new RemoveMemoryVar(UnitAIMemory.MoverCommand));


        ActiveSequence rotate = new ActiveSequence();
            rotate.AddChild(checkCanKeepMoving);
            rotate.AddChild(_simpleRotateToAction);

        Sequence tryToFaceToCommand = new Sequence();
            tryToFaceToCommand.AddChild(new CheckMemoryType<FaceToCommand>(UnitAIMemory.MoverCommand));
            tryToFaceToCommand.AddChild(new SaveFaceToProperties(UnitAIMemory.MoverCommand, UnitAIMemory.BallEndPosition));
            tryToFaceToCommand.AddChild(new SetKnowledgeStatus(UnitAIMemory.MoverCommand, KnowledgeStatus.PROCESSED));
            tryToFaceToCommand.AddChild(rotate);
            tryToFaceToCommand.AddChild(new RemoveMemoryVar(UnitAIMemory.MoverCommand));

        mainAI.AddChild(tryToMoveByCommand);
        mainAI.AddChild(tryToFaceToCommand);

        _bt = new BehaviorTree("UnitMover");
        _bt.SetMemoryObject(UnitAIMemory.Mover, _mover);

        _bt.Init(mainAI);
        return _bt;
    }
}

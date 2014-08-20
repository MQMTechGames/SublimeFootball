using UnityEngine;
using MQMTech.AI;
using MQMTech.AI.Knowledge;
using MQMTech.Unit.Mover;

[RequireComponent(typeof(BaseMover), typeof(BaseUnitAI))]
public class BaseUnit : MonoBehaviour
{
    BaseUnitAI _unitAI;
    BaseMover _mover;
    BaseSquad _squad;
    public BaseSquad Squad { get{ return _squad; } }

    [SerializeField] // Temporally
    bool _havePosession;

    Match _match;

    Ball _controlledBall;

    void Awake()
    {
        _unitAI = GameObjectUtils.GetInterfaceObject<BaseUnitAI>(gameObject);
        _mover = GameObjectUtils.GetInterfaceObject<BaseMover>(gameObject);

        _match = FindObjectOfType<Match>();
    }

    public virtual void Init(BaseSquad squad)
    {
        _squad = squad;
    }

    public void MoveTo(Vector3 worldPosition)
    {
        _mover.MoveTo(worldPosition);
    }

    public void FaceTo(Vector3 worldPosition)
    {
        _mover.FaceTo(worldPosition);
    }

    public void SendAIMessage<T>(string messageName, T message)
    {
        _unitAI.SendAIMessage(messageName, message);
    }

    public void OnBallControlled(Ball ball)
    {
        _havePosession = true;
        _controlledBall = ball;

        _controlledBall.OnBeingControlled(this);

        _squad.OnBallControlled();
    }

    public void OnBallLost()
    {
        _havePosession = false;
        _squad.OnBallLost();
    }

    public bool HavePosession()
    {
        return _havePosession;
    }

    public bool HaveSquadPosession()
    {
        return _squad.HaveSquadPosession();
    }

    public BaseUnit GetClosestUnitToPosition(Vector3 position)
    {
        return _squad.GetClosestUnitToBall(position);
    }

    public BaseUnit GetEasiestUnitToPassTheBall()
    {
        return _squad.GetEasiestUnitToPassTheBall(this);
    }

    public bool IsMoving()
    {
        return _mover.IsMoving();
    }

    public void PassBallToPosition(Vector3 position, Ball ball)
    {
        ball.KickToPosition(position);
    }

    public Ball GetControlledBall()
    {
        return _controlledBall;
    }

    public Ball FindClosestBall()
    {
        return _match.FindClosestBallToPosition(transform.position);
    }
}

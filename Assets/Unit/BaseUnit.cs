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

    public BaseUnit GetClosestUnitToPosition(Vector3 position)
    {
        return _squad.GetClosestUnitToBall(position);
    }

    public BaseUnit SelectEasiestUnitToPassTheBall()
    {
        return _squad.SelectEasiestUnitToPassTheBall(this);
    }

    public BaseUnit SelectRandomUnitToPassTheBall()
    {
        return _squad.SelectRandomUnitToPassTheBall(this);
    }

    public BaseUnit SelectForwardUnitToPassTheBall(Vector3 forwardDirectoin)
    {
        return _squad.SelectForwardUnitToPassTheBall(this, forwardDirectoin);
    }

    public bool IsMoving()
    {
        return _mover.IsMoving();
    }

    public void PassBallToPosition(Vector3 position, float maxHeight, Ball ball)
    {
        ball.KickToPosition(position, maxHeight);
    }

    public Ball GetControlledBall()
    {
        return _controlledBall;
    }

    public Ball FindClosestBall()
    {
        return _match.FindClosestBallToPosition(transform.position);
    }
    public Goal FindGoal()
    {
        return _squad.FindGoal();
    }

    public bool CheckPositionIsInAttackingZone(Vector3 position)
    {
        return true;
    }
}

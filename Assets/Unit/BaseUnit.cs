using UnityEngine;
using System.Collections.Generic;
using MQMTech.AI;
using MQMTech.AI.Knowledge;

[RequireComponent(typeof(BaseMover), typeof(BaseUnitAI))]
public class BaseUnit : MonoBehaviour
{
    [SerializeField]
    float _smashStrength = 68f;

    BaseUnitAI _unitAI;
    BaseMover _mover;
    BaseSquad _squad;
    public BaseSquad Squad { get{ return _squad; } }

    Match _match;

    Ball _controlledBall;
    GameZonesManager _gameZoneManager;

    void Awake()
    {
        _mover = GameObjectUtils.GetInterfaceObject<BaseMover>(gameObject);
        _unitAI = GameObjectUtils.GetInterfaceObject<BaseUnitAI>(gameObject);

        _match = FindObjectOfType<Match>();
        DebugUtils.Assert(_match!=null, "_match!=null");

        _gameZoneManager = FindObjectOfType<GameZonesManager>();
        DebugUtils.Assert(_gameZoneManager!=null, "_gameZoneManager!=null");
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

    public BaseUnit SelectForwardUnitToPassTheBall(Vector3 forwardDirection, bool discartItself)
    {
        return _squad.SelectForwardUnitToPassTheBall(this, forwardDirection, discartItself);
    }

    public bool IsMoving()
    {
        return _mover.IsMoving();
    }

    public void PassBallToPosition(Vector3 position, float maxHeight, Ball ball)
    {
        ball.PassToPosition(position, maxHeight);
    }

    public void ShootToPosition(Vector3 position, float strength, Ball ball)
    {
        ball.ShootToPosition(position, strength);
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

    public Vector3 SelectGoalShootPosition(Goal goal)
    {
        return goal.GetRandomGoalPosition();
    }

    public bool CheckOtherIsTeamMate(BaseUnit other)
    {
        return other.Squad == _squad;
    }

    public void SmashUnit(BaseUnit unit)
    {
        Vector3 smashForce = (unit.transform.position - transform.position).normalized;
        smashForce *= _smashStrength;

        SmashUnitMessage message = new SmashUnitMessage(this, smashForce);
        unit.SendAIMessage(UnitAIMemory.SmashedMessage.Name, message);
    }

    public void SmashBall(Ball ball)
    {
        Vector3 smashForce = (ball.transform.position - transform.position).normalized;
        smashForce *= _smashStrength;

        ball.OnSmashed(smashForce);
    }

    public void ProcessBeingSmashed(BaseUnit smasher, Vector3 smashForce)
    {
        //... 
    }

    public GameZone GetGameZone()
    {
        GameZone zone = _gameZoneManager.GetGameZone(transform.position, _squad.Side);
        DebugUtils.Assert(zone != null, "zone != null");

        return zone;
    }
}

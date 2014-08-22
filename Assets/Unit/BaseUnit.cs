using UnityEngine;
using System.Collections.Generic;
using MQMTech.AI;
using MQMTech.AI.Knowledge;

[RequireComponent(typeof(BaseMover), typeof(BaseUnitAI))]
public class BaseUnit : MonoBehaviour, IProbabilityBehaviorAgent
{
    [SerializeField]
    float _smashStrength = 68f;

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

    public void FillProbabilities(int selectorId, List<int> nodesIds, List<float> probabilities)
    {
        if(selectorId == AttacKWithBallSelectorKey.SelectorKey.Id)
        {
            for (int i = 0; i < probabilities.Count; ++i)
            {
                int probabilityId = nodesIds[i];

                if(probabilityId == AttacKWithBallSelectorKey.TryMovingWithBallToAForwardPosition.Id)
                {
                    probabilities[i] = 1;
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using MQMTech.AI.BT;

public interface IBehaviorTreeBuilder
{
    BehaviorTree Create();
}

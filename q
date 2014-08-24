[1mdiff --git a/Assets/BT/Subtrees/AttackBehaviorTree/AttackWithoutBallTreeBuilder.cs b/Assets/BT/Subtrees/AttackBehaviorTree/AttackWithoutBallTreeBuilder.cs[m
[1mindex 527d089..501a867 100644[m
[1m--- a/Assets/BT/Subtrees/AttackBehaviorTree/AttackWithoutBallTreeBuilder.cs[m
[1m+++ b/Assets/BT/Subtrees/AttackBehaviorTree/AttackWithoutBallTreeBuilder.cs[m
[36m@@ -9,16 +9,6 @@[m [musing MQMTech.AI.Knowledge;[m
 public class AttackWithoutBallTreeBuilder : MonoBehaviour, IBehaviorWithTree[m
 {[m
     BehaviorTree _bt;[m
[31m-//    BaseUnit _unit;[m
[31m-//    Match _match;[m
[31m-    [m
[31m-//    void Awake()[m
[31m-//    {[m
[31m-//        _unit = GameObjectUtils.GetInterfaceObject<BaseUnit>(gameObject);[m
[31m-//        [m
[31m-//        _match = FindObjectOfType<Match>();[m
[31m-//        DebugUtils.Assert(_match!=null, "_match!=null");[m
[31m-//    }[m
     [m
     public BehaviorTree GetBehaviorTree()[m
     {[m
[36m@@ -37,7 +27,7 @@[m [mpublic class AttackWithoutBallTreeBuilder : MonoBehaviour, IBehaviorWithTree[m
         findTargetDirection.AddChild(new Vector3MultiplyByScalar(UnitAIMemory.ForwardDirection, UnitAIMemory.DotResult, UnitAIMemory.TargetDirection));[m
         findTargetDirection.AddChild(new Vector3Normalize(UnitAIMemory.TargetDirection, UnitAIMemory.TargetDirection));[m
         [m
[31m-        //        //-- MoveForward[m
[32m+[m[32m        //-- MoveForward[m
         Sequence moveToTargetDirection = new Sequence();[m
         moveToTargetDirection.AddChild(new SetMemoryVar<float>(UnitAIMemory.MoveDistance, 3.0f));[m
         moveToTargetDirection.AddChild(new Vector3MultiplyByScalar(UnitAIMemory.TargetDirection, UnitAIMemory.MoveDistance, UnitAIMemory.TargetDirectionScaled));[m
[36m@@ -241,18 +231,6 @@[m [mpublic class AttackWithoutBallTreeBuilder : MonoBehaviour, IBehaviorWithTree[m
         tryToAttackWithoutBall.AddChild(new Inverter().SetChild(new CheckAreEqualMemoryVars<bool>(UnitAIMemory.IsBallControlled, UnitAIMemory.TrueVar)));[m
         tryToAttackWithoutBall.AddChild(chooseAttackWithoutBall);[m
 [m
[31m-        Sequence testUnitIsNotNull = new Sequence();[m
[31m-            testUnitIsNotNull.AddChild(new CheckNotNullMemoryVar(UnitAIMemory.Unit));[m
[31m-            testUnitIsNotNull.AddChild(new LogAction("Unit Is OK :)"));[m
[31m-[m
[31m-        Sequence testUnitIsNull = new Sequence();[m
[31m-            testUnitIsNull.AddChild(new CheckNullMemoryVar(UnitAIMemory.Unit));[m
[31m-            testUnitIsNull.AddChild(new LogAction("Unit is NULL :("));[m
[31m-[m
[31m-        Selector checkWhatHappens = new Selector();[m
[31m-            checkWhatHappens.AddChild(testUnitIsNull);[m
[31m-            checkWhatHappens.AddChild(testUnitIsNotNull);[m
[31m-[m
         _bt = new BehaviorTree();[m
 [m
         _bt.Init(tryToAttackWithoutBall);[m

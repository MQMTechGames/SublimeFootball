using UnityEngine;

public static class UnitAIMemory
{
    // Temp
    public static AIMemoryKey TemporalUnit = new AIMemoryKey("TemporalUnit");
    public static AIMemoryKey MoveDistance = new AIMemoryKey("Scalar");
    public static AIMemoryKey DotResult = new AIMemoryKey("DotResult");

    // Direction
    public static AIMemoryKey TargetDirection = new AIMemoryKey("TargetDirection");
    public static AIMemoryKey ForwardDirection = new AIMemoryKey("ForwardDirection");
    public static AIMemoryKey TargetDirectionScaled = new AIMemoryKey("TargetDirectionScaled");
    public static AIMemoryKey UpVector = new AIMemoryKey("UpVector");
    public static AIMemoryKey LeftDirection = new AIMemoryKey("LeftDirection");
    public static AIMemoryKey RightDirection = new AIMemoryKey("RightDirection");

    // Positions
    public static AIMemoryKey UnitPosition = new AIMemoryKey("UnitPosition");
    public static AIMemoryKey TargetPosition = new AIMemoryKey("TargetPosition");

    // Utils
    public static AIMemoryKey TrueVar = new AIMemoryKey("TrueVar", AIMemoryKey.ContextType.Global);
    public static AIMemoryKey FalseVar = new AIMemoryKey("FalseVar", AIMemoryKey.ContextType.Global);

    // Mover
    public static AIMemoryKey Mover = new AIMemoryKey("Mover");
    public static AIMemoryKey MoverCommand = new AIMemoryKey("MoverCommand");
    public static AIMemoryKey BallEndPosition = new AIMemoryKey("BallEndPosition");
    public static AIMemoryKey Ball = new AIMemoryKey("Ball");
    public static AIMemoryKey BallTransform = new AIMemoryKey("BallTransform");
    public static AIMemoryKey BallPosition = new AIMemoryKey("BallPosition");
    
    // Unit
    public static AIMemoryKey Unit = new AIMemoryKey("Unit");
    public static AIMemoryKey ClosestMateUnit = new AIMemoryKey("ClosestMateUnit");
    public static AIMemoryKey SelectedMateUnit = new AIMemoryKey("SelectedMateUnit");
    public static AIMemoryKey SelectedMateTransform = new AIMemoryKey("SelectedMateTransform");
    public static AIMemoryKey BallDistance = new AIMemoryKey("BallDistance");
    public static AIMemoryKey NumUnitsToRecoverPossessionCounter = new AIMemoryKey("NumUnitsToRecoverPosessionCounter", AIMemoryKey.ContextType.Shared);
    
    // Distance
    public static AIMemoryKey MaxUnitBallDistance = new AIMemoryKey("MaxUnitBallDistance");
    public static AIMemoryKey PositionA = new AIMemoryKey("PositionA");
    public static AIMemoryKey PositionB = new AIMemoryKey("PositionB");
    
    // Message
    public static AIMemoryKey MessageValidDuration = new AIMemoryKey("MessageValidDuration");
    public static AIMemoryKey MessageList = new AIMemoryKey("MessageList");
    public static AIMemoryKey OnBallPassed = new AIMemoryKey("OnWillPassTheBall", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey OnBallShot = new AIMemoryKey("OnBallShot", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey PossessionMateUnit = new AIMemoryKey("PossessionMateUnit");
    public static AIMemoryKey BallTargetMateUnit = new AIMemoryKey("BallTargetMateUnit");
    
    // Size comparison
    public static AIMemoryKey MinMessageCount = new AIMemoryKey("MinMessageCount");
    public static AIMemoryKey MessageCount = new AIMemoryKey("MessageCount");
    
    public static AIMemoryKey TemporalBall = new AIMemoryKey("TemporalBall");
    
    // Recover Possession
    public static AIMemoryKey g_PossessionSquad = new AIMemoryKey("g_PossessionSquad", AIMemoryKey.ContextType.Global);
    public static AIMemoryKey RecoverPossessionCoolDownWaitTime = new AIMemoryKey("RecoverPossessionCoolDownWaitTime");
    public static AIMemoryKey RecoverPossessionCoolDownTimer = new AIMemoryKey("RecoverPossessionCoolDownTimer");

    // Posession
    public static AIMemoryKey HaveSquadPosession = new AIMemoryKey("HaveSquadPosession", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey IsBallControlled = new AIMemoryKey("IsBallControlled");
    public static AIMemoryKey BallOwner = new AIMemoryKey("BallOwner");

    // Velocity
    public static AIMemoryKey BallVelocity = new AIMemoryKey("BallVelocity");
    public static AIMemoryKey MinBallVelocityMagnitude = new AIMemoryKey("MinBallVelocityMagnitude");
    public static AIMemoryKey BallVelocityMagnitude = new AIMemoryKey("BallVelocityMagnitude");

    // Distance
    public static AIMemoryKey PrevDistanceToTransform = new AIMemoryKey("PrevDistanceToTransform");
    public static AIMemoryKey DistanceToTransform = new AIMemoryKey("DistanceToTransform");

    // Squad
    public static AIMemoryKey Squad = new AIMemoryKey("Squad");

    // Goal
    public static AIMemoryKey Goal = new AIMemoryKey("Goal", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey GoalPosition = new AIMemoryKey("GoalPosition", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey ShootTargetPosition = new AIMemoryKey("ShootTargetPosition");
    public static AIMemoryKey ShotStrength = new AIMemoryKey("ShotStrength");

    // Ball Pass
    public static AIMemoryKey BallPasserUnit = new AIMemoryKey("BallPasserUnit", AIMemoryKey.ContextType.Shared);
    public static AIMemoryKey BallMaxHeight = new AIMemoryKey("BallMaxHeight");

    // Messages
    public static AIMemoryKey SmashedMessage = new AIMemoryKey("SmashedMessage");

    // Probability Selector
    public static AIMemoryKey TryToPassTheBallOdds = new AIMemoryKey("TryToPassTheBallOdds");
    public static AIMemoryKey TryMovingWithBallToAForwardOdds = new AIMemoryKey("TryMovingWithBallToAForwardOdds");
    public static AIMemoryKey TryToShootForwardOdds = new AIMemoryKey("TryToShootForwardOdds");

    // Zones
    public static AIMemoryKey ZoneType = new AIMemoryKey("ZoneType");

    // Test
    public static AIMemoryKey AttackSpeed = new AIMemoryKey("AttackSpeed");
}

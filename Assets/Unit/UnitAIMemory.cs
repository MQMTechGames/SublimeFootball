using UnityEngine;

public static class UnitAIMemory
{
    // Temp
    public static AIMemoryKey TemporalUnit = new AIMemoryKey("TemporalUnit");

    // Utils
    //public static AIMemoryKey TrueVar = new AIMemoryKey("TrueVar", AIMemoryKey.ContextType.Global);
    public static AIMemoryKey TrueVar = new AIMemoryKey("TrueVar");

    // Mover AI
    public static AIMemoryKey Mover = new AIMemoryKey("Mover");
    public static AIMemoryKey MoverCommand = new AIMemoryKey("MoverCommand");
    public static AIMemoryKey TargetBallPosition = new AIMemoryKey("TargetPosition");
    public static AIMemoryKey Ball = new AIMemoryKey("Ball");
    public static AIMemoryKey BallTransform = new AIMemoryKey("BallTransform");
    public static AIMemoryKey BallPosition = new AIMemoryKey("BallPosition");
    
    // Unit AI
    public static AIMemoryKey Unit = new AIMemoryKey("Unit");
    public static AIMemoryKey ClosestMateUnit = new AIMemoryKey("ClosestMateUnit");
    public static AIMemoryKey SelectedMateUnit = new AIMemoryKey("SelectedMateUnit");
    public static AIMemoryKey SelectedMateTransform = new AIMemoryKey("SelectedMateTransform");
    public static AIMemoryKey BallDistance = new AIMemoryKey("BallDistance");
    public static AIMemoryKey NumUnitsToRecoverPossessionCounter = new AIMemoryKey("NumUnitsToRecoverPosessionCounter", AIMemoryKey.ContextType.Squad);
    
    // Distance
    public static AIMemoryKey MaxUnitBallDistance = new AIMemoryKey("MaxUnitBallDistance");
    public static AIMemoryKey PositionA = new AIMemoryKey("PositionA");
    public static AIMemoryKey PositionB = new AIMemoryKey("PositionB");
    
    // Message
    public static AIMemoryKey CreatedMessage = new AIMemoryKey("CreatedMessage");
    public static AIMemoryKey MessageValidDuration = new AIMemoryKey("MessageValidDuration");
    public static AIMemoryKey MessageList = new AIMemoryKey("MessageList");
    public static AIMemoryKey OnWillPassTheBall = new AIMemoryKey("OnWillPassTheBall");
    public static AIMemoryKey PossessionMateUnit = new AIMemoryKey("PossessionMateUnit");
    
    // Size comparison
    public static AIMemoryKey MinMessageCount = new AIMemoryKey("MinMessageCount");
    public static AIMemoryKey MessageCount = new AIMemoryKey("MessageCount");
    
    public static AIMemoryKey TemporalBall = new AIMemoryKey("TemporalBall");
    
    // Recover Possession
    public static AIMemoryKey RecoverPossessionCoolDownWaitTime = new AIMemoryKey("RecoverPossessionCoolDownWaitTime");
    public static AIMemoryKey RecoverPossessionCoolDownTimer = new AIMemoryKey("RecoverPossessionCoolDownTimer");

    // Posession
    public static AIMemoryKey HaveSquadPosession = new AIMemoryKey("HaveSquadPosession", AIMemoryKey.ContextType.Squad);
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

    //---------- Global
    public static AIMemoryKey g_PossessionSquad = new AIMemoryKey("SquadWithPosession", AIMemoryKey.ContextType.Global);
}

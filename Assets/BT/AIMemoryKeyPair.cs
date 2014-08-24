public struct AIMemoryKeyPair
{
    public AIMemoryKey Source;
    public AIMemoryKey Target;

    public AIMemoryKeyPair(AIMemoryKey key, AIMemoryKey value)
    {
        Source = key;
        Target = value;
    }
}

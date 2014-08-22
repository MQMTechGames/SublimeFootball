public struct ProbabilitySelectorKey
{
    public string Name { get; set; }
    public int Id { get; set; }

    public ProbabilitySelectorKey(string name)
    {
        Name = name;
        Id = name.GetHashCode();
    }
}

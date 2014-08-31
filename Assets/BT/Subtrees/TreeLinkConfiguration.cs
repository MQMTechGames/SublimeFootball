using MQMTech.AI.BT;

public class TreeLinkConfiguration
{
    public enum LinkMemoryType
    {
        Instance,
        Linked,
        Null
    }

    public LinkMemoryType LocalMemoryType { get; set; }
    public LinkMemoryType AgentMemoryType { get; set; }
    public LinkMemoryType SharedMemoryType { get; set; }
    public LinkMemoryType GlobalMemoryType { get; set; }

    public TreeLinkConfiguration( LinkMemoryType localMemory, LinkMemoryType agentMemoryType, LinkMemoryType sharedMemoryType, LinkMemoryType globalMemoryType )
    {
        LocalMemoryType = localMemory;
        AgentMemoryType = agentMemoryType;
        SharedMemoryType = sharedMemoryType;
        GlobalMemoryType = globalMemoryType;
    }

    public void ConfigureTreeMemory(BehaviorTree subtree, BehaviorTree tree)
    {
        Memory localMemory = InitSubtreeMemory(tree.MemoryManager.LocalMemory, LocalMemoryType);
        subtree.SetLocalMemory(localMemory);

        Memory agentMemory = InitSubtreeMemory(tree.MemoryManager.AgentMemory, AgentMemoryType);
        subtree.SetAgentMemory(agentMemory);

        Memory sharedMemory = InitSubtreeMemory(tree.MemoryManager.SharedMemory, SharedMemoryType);
        subtree.SetSharedMemory(sharedMemory);

        Memory globalMemory = InitSubtreeMemory(tree.MemoryManager.GlobalMemory, GlobalMemoryType);
        subtree.SetGlobalMemory(globalMemory);
    }

    Memory InitSubtreeMemory(Memory parentMemory, LinkMemoryType type)
    {
        Memory memory;

        if(type == LinkMemoryType.Instance)
        {
            memory = new Memory();
        }
        else if(type == LinkMemoryType.Linked)
        {
            memory = parentMemory;
        }
        else
        {
            memory = null;
        }

        return memory;
    }

    public void CheckAllSubtreeMemory(BehaviorTree subtree, BehaviorTree tree)
    {
        CheckSubtreeMemory(subtree.MemoryManager.LocalMemory, tree.MemoryManager.LocalMemory, LocalMemoryType);
        CheckSubtreeMemory(subtree.MemoryManager.AgentMemory, tree.MemoryManager.AgentMemory, AgentMemoryType);
        CheckSubtreeMemory(subtree.MemoryManager.SharedMemory, tree.MemoryManager.SharedMemory, SharedMemoryType);
        CheckSubtreeMemory(subtree.MemoryManager.GlobalMemory, tree.MemoryManager.GlobalMemory, GlobalMemoryType);
    }

    void CheckSubtreeMemory(Memory subtreeMemory, Memory parentMemory, LinkMemoryType type)
    {
        if(type == LinkMemoryType.Instance)
        {
            DebugUtils.Assert(subtreeMemory != parentMemory, "subtreeMemory != parentMemory");
        }
        else if(type == LinkMemoryType.Linked)
        {
            DebugUtils.Assert((subtreeMemory == parentMemory) || (subtreeMemory == null), "(subtreeMemory == parentMemory) || (subtreeMemory == null)");
        }
        else
        {
            DebugUtils.Assert(subtreeMemory == null, "subtreeMemory == null");
        }
    }
}

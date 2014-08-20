using UnityEngine;

public class SharedMemory : MonoBehaviour
{
    Memory _memory = new Memory();
    public Memory Memory { get{ return _memory;} }
}

using UnityEngine;
using System.Collections;

public abstract class BaseMover : MonoBehaviour
{
    public abstract void MoveTo(Vector3 worldPosition);
    public abstract void FaceTo(Vector3 worldPosition);
    public abstract bool IsMoving();
}

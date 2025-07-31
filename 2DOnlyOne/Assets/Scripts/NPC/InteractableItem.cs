using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public bool IsInUse { get; private set; }

    public bool TryClaim()
    {
        if (IsInUse) return false;
        IsInUse = true;
        return true;
    }

    public void Release()
    {
        IsInUse = false;
    }
}

using UnityEngine;

public abstract class IInputReceiver : MonoBehaviour
{
    protected IInputHandler[] inputHandlers;

    public abstract void OnInputReceived();

    private void Awake()
    {
        inputHandlers = GetComponents<IInputHandler>();
    }
}

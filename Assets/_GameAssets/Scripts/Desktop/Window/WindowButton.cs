using System;
using UnityEngine;

public class WindowButton : MonoBehaviour
{
    public event Action ButtonPressed;

    private void OnPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ButtonPressed?.Invoke();
    }
}

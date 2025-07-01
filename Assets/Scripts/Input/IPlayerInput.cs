using System;

public interface IPlayerInput
{
    event Action<float> OnHorizontalChanged; // -1..1
    event Action OnJump;
    event Action OnLeftPressed;
    event Action OnRightPressed;

    void Enable();
    void Disable();
    void Update();

    int GetForkDirection(); // -1 ←, 1 →, 0 — не нажато

}

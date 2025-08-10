using System;

public interface IPlayerInput
{
    event Action<float> HorizontalInputChanged;

    void Enable();
    void Disable();
    void Update();
}

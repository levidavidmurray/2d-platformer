
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == null) return;

        UIManager.DebugUI.OnStateChange(newState);

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}

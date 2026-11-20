using Godot;

namespace FSMGodot;

/// <summary>
/// Universal state machine for any object in Godot.
/// </summary>
[GlobalClass]
public sealed partial class FiniteStateMachine : Node
{
	public State CurrentState { get; private set; }
	public State PreviousState { get; private set; }

	private Node _actor;

	public override void _Ready()
	{
		_actor = GetParent();
		if (CurrentState == null && GetChildCount() > 0)
		{
			CallDeferred(nameof(SetState), GetChild(0).Name);
		}
	}

	public override void _Process(double delta)
	{
		CurrentState?.Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState?.PhysicsProcess(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		CurrentState?.UnhandledInput(@event);
	}

	public void SetState(string stateName)
	{
		if (CurrentState != null)
		{
			PreviousState = CurrentState;
			CurrentState.Exit();
			CurrentState.ProcessMode = ProcessModeEnum.Disabled;
		}
		else
		{
			PreviousState = null;
		}

		State newState = null;
		foreach (Node child in GetChildren())
		{
			if (child is State stateChild && stateChild.Is(stateName))
			{
				newState = stateChild;
			}
		}

		if (newState == null)
		{
			GD.PushError($"[FSM | {_actor.Name}] state {stateName} doesn't exist.");
			return;
		}

		CurrentState = newState;
		CurrentState.ProcessMode = ProcessModeEnum.Inherit;
		CurrentState.Enter(_actor);
	}

	public bool IsCurrentState(string stateName)
	{
		return CurrentState?.Is(stateName) ?? false;
	}
}

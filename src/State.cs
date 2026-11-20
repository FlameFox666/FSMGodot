using Godot;

namespace FSMGodot;

/// <summary>
/// Abstract state parent for any state in Godot.
/// </summary>
[GlobalClass]
public abstract partial class State : Node
{
	protected FiniteStateMachine fsm;

	public override void _EnterTree()
	{
		fsm = GetParent<FiniteStateMachine>();
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public virtual void Enter(Node actor) { }
	public virtual void Process(double delta) { }
	public virtual void PhysicsProcess(double delta) { }
	public virtual void UnhandledInput(InputEvent @event) { }
	public virtual void Exit() { }

	public bool Is(string stateName)
	{
		if (string.IsNullOrEmpty(stateName)) return false;

		return string.Equals(
			Name.ToString(),
			stateName,
			System.StringComparison.OrdinalIgnoreCase
		);
	}

	protected T GetActor<T>() where T : class
	{
		Node current = GetParent();
		while (current != null)
		{
			if (current is T target) return target;
			current = current.GetParent();
		}
		return null;
	}
}

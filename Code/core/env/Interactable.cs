using Sandbox;

public sealed class Interactable : Component, IInteractable
{
	public void Interact()
	{
		if( Components.TryGet( out IInteractable component ) )
			component.Interact();
	}
}

using Sandbox;

public sealed class Interactable : Component, IInteractable
{
	public void Interact(PlayerGameController player)
	{
		if ( Components.TryGet( out IInteractable component ) )
			component.Interact( player );
	}
}

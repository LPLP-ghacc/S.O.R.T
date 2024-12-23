using Sandbox;

public sealed class InvHand : Component, IItemController
{
	public void OnEnable()
	{
		RaycastInteractions.Instance.EnableObjectMove = true;
		Log.Info( RaycastInteractions.Instance.EnableObjectMove );
	}

	public void OnDisable()
	{
		RaycastInteractions.Instance.EnableObjectMove = false;
	}
}


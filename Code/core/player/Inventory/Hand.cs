using Sandbox;

public sealed class Hand : Component
{
	public bool IsRight { get; set; } = true;

	public GameObject Item { get; set; } = null;

	public void Initialize(InventoryItem item)
	{
		var model = item.Model.Clone();
	}

	public void Use()
	{

	}
}

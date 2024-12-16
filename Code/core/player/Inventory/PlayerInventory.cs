using Sandbox;

public sealed class PlayerInventory : Component
{
	public Hand Left { get; set; }
	public Hand Right { get; set; }
	public List<InventoryItem> Inventories { get; private set; } = new List<InventoryItem>();
	public InventoryItem Selected { get; private set; }
	public int SelectedIndex { get; private set; } = 0;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if( Input.MouseWheel.y > 0 )
		{
			if( SelectedIndex != Inventories.Count )
				SelectedIndex++;
		}
		else if( Input.MouseWheel.y < 0 )
		{
			if ( SelectedIndex != -1 )
				SelectedIndex--;
		}

		if( Selected.IsUseable )
		{
			var model = Selected.Model.Clone();
			model.SetParent(GameObject);
		}
	}

	public void AddItem( InventoryItem item ) 
	{
		Inventories.Add(item);
	}
}

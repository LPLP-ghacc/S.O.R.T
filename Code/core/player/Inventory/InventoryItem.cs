public class InventoryItem
{
	public InventoryItem( GameObject model, string modelImg, bool isUseable )
	{
		Model = model;
		ModelImg = modelImg;
		IsUseable = isUseable;
	}

	public InventoryItem( string name, string description, GameObject model, string modelImg, bool isUseable )
	{
		Name = name;
		Description = description;
		Model = model;
		ModelImg = modelImg;
		IsUseable = isUseable;
	}

	public string Name { get; set; } = "DEFAULT NAME";
	public string Description { get; set; } = "DESC";
	public GameObject Model { get; set; }
	public string ModelImg { get; set; }
	public bool IsUseable { get; set; }
}


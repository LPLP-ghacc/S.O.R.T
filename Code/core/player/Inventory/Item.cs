using Sandbox;
using System;

public sealed class Item : Component, IInteractable
{
	[Property]
	public int ItemId;

	[Property]
	public string Name { get; set; }

	[Property]
	public string Description { get; set; }

	[Property]
	public GameObject HandPrefab { get; set; }

	[Property]
	public string ModelImg { get; set; }

	[Property]
	public bool IsUseable { get; set; }

	public void Interact( PlayerGameController player )
	{
		PlayerInventory.Instance.AddItem( this );
		GameObject.Destroy();
	}
}

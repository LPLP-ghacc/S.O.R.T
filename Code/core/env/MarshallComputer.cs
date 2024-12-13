using Sandbox;
using System.Numerics;

public sealed class MarshallComputer : Component, IInteractable
{
	[RequireComponent]
	private Interactable Interactable { get; set; }

	private PlayerGameController Player { get; set; }

	[Property]
	public GameObject Camera { get; set; }

	public bool IsUse { get; set; } = false;

	public void Interact(PlayerGameController player)
	{
		if( player  != null )
		{
			Player = player;
			Player.IsCanMove = false;
			Player.PlayerCamera.Enabled = false;

			if( Camera != null )
			{
				Camera.Enabled = true;
			}
		}

		IsUse = true;
	}

	protected override void OnUpdate()
	{
		if( IsUse && Input.EscapePressed )
		{
			Player.IsCanMove = true;
			Player.PlayerCamera.Enabled = true;
			Camera.Enabled = false;
			IsUse = false;
		}
	}
}

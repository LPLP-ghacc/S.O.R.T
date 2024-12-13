using Sandbox;

public sealed class Chair : Component, IInteractable
{
	[RequireComponent]
	private Interactable Interactable { get; set; }

	private PlayerGameController Player { get; set; }

	[Property]
	public GameObject CameraPrefab { get; set; }

	[Property]
	public Vector3 CameraOffset { get; set; }

	public bool IsUse { get; set; } = false;

	public void Interact( PlayerGameController player )
	{
		if ( player != null )
		{
			Player = player;
			Player.IsCanMove = false;
			Player.PlayerCamera.Enabled = false;

			if ( CameraPrefab == null )
			{
				if ( CameraPrefab != null )
				{
					CameraPrefab = CameraPrefab.Clone( WorldPosition );
				}
				else
				{
					CameraPrefab = new GameObject( true, "ChairCamera" );

					CameraComponent cameraComponent = CameraPrefab.Components.Create<CameraComponent>();
					cameraComponent.ZFar = 30000;
					cameraComponent.FieldOfView = 90;
				}

				CameraPrefab.Name = "ChairCamera";
				CameraPrefab.SetParent( this.GameObject );

				CameraPrefab.WorldPosition = WorldPosition;
				CameraPrefab.LocalRotation = Rotation.Identity;
				CameraPrefab.LocalPosition = CameraOffset;
			}
		}

		IsUse = true;
	}

	protected override void OnUpdate()
	{
		if ( IsUse && Input.EscapePressed )
		{
			Player.IsCanMove = true;
			Player.PlayerCamera.Enabled = true;
			CameraPrefab.DestroyImmediate();
			CameraPrefab = null;
			IsUse = false;
		}
	}
}

using Sandbox;
using System;

[Group( "Player" )]
public sealed class RaycastInteractions : Component
{
	public static RaycastInteractions Instance { get; private set; }

	[Property, ReadOnly]
	public bool IsMove { get; set; } = false;

	public bool EnableObjectMove { get; set; } = true;

	[Property]
	public bool EnableInteract { get; set; } = true;

	[Property]
	public string MoveKey { get; set; } = "ObjectMove";

	[Property]
	public string InteractKey { get; set; } = "use";

	[Property]
	public string OnMovingKey { get; set; } = "attack2";

	[Property]
	public string ThrowKey { get; set; } = "attack1";

	[Property]
	public List<string> WithoutTags { get; set; }

	[Property]
	public string PropTag { get; set; } = "prop";

	private GameObject TargetCamera { get; set; }

	public GameObject CurrentGameObject { get; private set; }

	private Moveable MoveController { get; set; } = null;

	[Property]
	public float ThrowForce { get; set; } = 100000;

	[Property]
	private float MaxMovingDistance { get; set; } = 60;

	[Property]
	private float MinMovingDistance { get; set; } = 40;

	[Property]
	[ReadOnly]
	private float CurrentMovingDistance { get; set; }

	[Property]
	public float MouseMoveAcceleration { get; set; } = 5;

	[Property]
	public float TimeToArrive { get; set; } = 0.5f;

	[Property]
	public float InteractTimeCycle { get; set; } = 1f;

	[Property]
	public float CurrentInteractTimeCycle { get; set; } = 0f;

	private PlayerGameController gameController;

	protected override void OnStart()
	{
		base.OnStart();

		Instance = this;
	}

	protected override void OnEnabled()
	{
		gameController = GameObject.GetComponent<PlayerGameController>();

		TargetCamera = gameController.PlayerCamera;

		if ( WithoutTags.Count == 0 )
			WithoutTags = new List<string>() { "player" };

		CurrentMovingDistance = MinMovingDistance;
	}

	protected override void OnFixedUpdate()
	{
		if( CurrentInteractTimeCycle > 0 )
		{
			CurrentInteractTimeCycle -= Time.Delta;

			if(CurrentInteractTimeCycle <= 0 )
			{
				EnableInteract = true;
			}
		}

		CurrentGameObject = GetObj();

		if( EnableObjectMove )
			ObjectMove();
	}

	private void ObjectMoveMouseWheelHandler()
	{
		if ( CurrentMovingDistance <= MaxMovingDistance )
		{
			CurrentMovingDistance += Input.MouseWheel.y * MouseMoveAcceleration;
		}
		else
		{
			CurrentMovingDistance = MaxMovingDistance - MouseMoveAcceleration;
		}

		if ( CurrentMovingDistance <= MinMovingDistance )
		{
			CurrentMovingDistance = MinMovingDistance + MouseMoveAcceleration;
		}
	}

	private void ObjectMove()
	{
		if ( Input.Down( OnMovingKey ) )
		{
			IsMove = true;

			if ( MoveController != null )
			{
				MoveController.GameObject.Tags.Add( PropTag );
				WithoutTags.Add( PropTag );

				var tr = Scene.Trace
					.Ray( new Ray( TargetCamera.WorldPosition, TargetCamera.Transform.World.Forward ), CurrentMovingDistance )
					.WithoutTags( WithoutTags.ToArray() )
					.Run();

				ObjectMoveMouseWheelHandler();

				MoveController.MoveTo( tr.EndPosition, TimeToArrive );
				MoveController.RotateTo( WorldRotation, TimeToArrive );

				if ( Input.Down( ThrowKey ) )
				{
					Vector3 direction = Input.AnalogMove.Normal * Rotation.FromYaw( gameController.MouseAngles.yaw );

					MoveController.Throw( direction * ThrowForce );
					MoveController = null;

					WithoutTags.Remove( PropTag );
				}
			}
		}
		else
		{
			IsMove = false;
		}

		if ( Input.Released( OnMovingKey ) )
		{
			MoveController = null;

			WithoutTags.Remove( PropTag );
		}
	}

	private GameObject GetObj()
	{
		if ( Input.Down( MoveKey ) )
		{
			var tr = TraceIt();

			if ( tr.Hit )
			{
				foreach ( var item in tr.GameObject.Components.GetAll() )
				{
					if ( item is IMoveable )
					{
						if ( MoveController == null )
							MoveController = (Moveable)item;
					}
				}

				return tr.GameObject;
			}
		}

		if ( Input.Released( InteractKey ) && CurrentInteractTimeCycle <= 0 )
		{
			var tr = TraceIt();

			if ( tr.Hit )
			{
				Interact( tr );
			}
		}

		return null;
	}

	private void Interact(SceneTraceResult traceResult)
	{
		foreach ( var item in traceResult.GameObject.Components.GetAll() )
		{
			if ( item is IInteractable && !IsMove )
			{
				var interactable = (IInteractable)item;

				interactable?.Interact( GetComponent<PlayerGameController>() );

				CurrentInteractTimeCycle = InteractTimeCycle;

				EnableInteract = false;
			}
		}
	}

	private SceneTraceResult TraceIt()
	{
		return Scene.Trace
				.Ray( new Ray( TargetCamera.WorldPosition, TargetCamera.Transform.World.Forward ), MinMovingDistance )
				.Run();

	}
}

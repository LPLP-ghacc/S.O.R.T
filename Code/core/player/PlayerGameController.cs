using Sandbox;
using System;

public enum MovementState
{
	Running,
	Crouch,
	Creep,
	Jump,
	Default
}

public class CameraClamp
{
	public CameraClamp( float pY, float nY )
	{
		PY = pY;
		NY = nY;
	}

	/// <summary>
	/// The default value is 80, -20
	/// </summary>
	public CameraClamp()
	{
		PY = 80f; NY = -20f;
	}

	public float PY { get; set; }
	public float NY { get; set; }
}

[Group( "Player" )]
public sealed class PlayerGameController : Component
{
	[RequireComponent]
	public PlayerInput InputController { get => inputController; private set => inputController = value; }

	[Property]
	public GameObject PlayerCamera { get; private set; }
	private string DefaultCameraName = "Player Camera";

	public CameraClamp CameraClamp { get; private set; } = new CameraClamp();

	[Property]
	[Group( "Camera" )]
	public bool UseMouseMoveSmoothing { get; set; } = true;

	[Property]
	[Group( "Camera" )]
	[Range( 1f, 100f, 5f )]
	public float MouseAcceleration { get; private set; } = 10f;

	[Property]
	[Group( "Camera" )]
	public bool EnableRotate { get; set; } = true;

	[Property]
	[Group( "Camera" )]
	public Vector3 CameraOffset { get; private set; } = new Vector3( 0, 0, 70 );

	public Angles MouseAngles { get; private set; }

	/// <summary>
	/// It is used to draw the range of visibility, where the value 1 is full visibility. 
	/// The changes don't work in Runtime.
	/// </summary>
	[Property]
	[Group( "Camera" )]
	public int ZFarDivider { get; set; } = 1;

	/// <summary>
	/// keyboard on Start
	/// </summary>
	//private bool isUsingController = false;

	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }

	private MovementState MovementState { get; set; } = MovementState.Default;

	[Property]
	[Group( "Movement" )]
	public bool UseSimpleMovement { get; set; } = true;

	[Property]
	[Range( 0, 1000, 0.1f )]
	[Group( "Movement" )]
	private float MovementSpeed { get; set; } = 5f;

	[Property]
	[Group( "Movement" )]
	[Range( 1, 5, 0.1f )]
	public float RunSpeed { get; set; } = 2f;

	[Property]
	[Group( "Movement" )]
	[Range( 0.5f, 1, 0.1f )]
	public float CrouchSpeed { get; set; } = 0.5f;

	[Property]
	[Group( "Movement" )]
	[Range( 0.05f, 0.5f, 0.01f )]
	public float CreepSpeed { get; set; } = 0.05f;

	/// <summary>
	/// The basic value for the speed of movement
	/// </summary>
	[Property]
	[ReadOnly]
	[Group( "Movement" )]
	private float CurrentMovementMultiplier { get; set; }

	[Property]
	[ReadOnly]
	[Group( "Movement" )]
	public bool IsGrounded { get; set; } = false;

	[Property]
	[Group( "Movement" )]
	public Vector3 GroundBoxSize { get; set; } = new Vector3( 5, 5, 9 );

	[Property]
	[Group( "Movement" )]
	public float JumpForceMultiplier { get; private set; } = 35;

	[Property]
	[Group( "Movement" )]
	public float JumpCooldown { get; private set; } = 0.3f;

	private float jumpTimer;
	private PlayerInput inputController;

	protected override void OnAwake()
	{
		base.OnAwake();

		InitCamera();

		InputController.isUsingLerp = false;

		GameObject.Tags.Add("player");
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		Jump();
		PhysicsMovement();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		// Movement State Control
		if ( !Input.UsingController )
			KeyboardMovementStateControl();
		else
			ControllerMovementStateControl();

		if ( EnableRotate )
			MouseRotation();
	}

	private void MouseRotation()
	{
		MouseAngles += Input.AnalogLook;
		MouseAngles = MouseAngles.WithPitch( MathX.Clamp( MouseAngles.pitch, CameraClamp.NY, CameraClamp.PY ) );

		ModelRotate();
		CameraRotate();
	}

	private void ModelRotate()
	{
		float delta = UseMouseMoveSmoothing ? Time.Delta : 1;

		WorldRotation = Rotation.Slerp( WorldRotation, Rotation.FromYaw( MouseAngles.yaw ), delta * MouseAcceleration );
	}

	private void CameraRotate()
	{
		float delta = UseMouseMoveSmoothing ? Time.Delta : 1;

		PlayerCamera.LocalRotation = Rotation.Slerp( PlayerCamera.LocalRotation, Rotation.FromPitch( MouseAngles.pitch ), delta * MouseAcceleration );
	}

	private void SuperSimpleMovementSystem()
	{
		var speed = 100f;

		if ( Input.Down( "Forward" ) )
		{
			WorldPosition += new Vector3( speed * Time.Delta, 0, 0 );
		}

		if ( Input.Down( "Backward" ) )
		{
			WorldPosition += new Vector3( (speed * Time.Delta) * -1, 0, 0 );
		}

		if ( Input.Down( "Right" ) )
		{
			WorldPosition += new Vector3( 0, speed * Time.Delta, 0 );
		}

		if ( Input.Down( "Left" ) )
		{
			WorldPosition += new Vector3( 0, (speed * Time.Delta) * -1, 0 );
		}
	}

	/// <summary>
	/// Simple movement model
	/// </summary>
	private void WorldPosModelMovement()
	{
		Vector3 fromInputVelocity = new Vector3( InputController.InputVector.y, InputController.InputVector.x, 0 );
		Vector3 direction = Input.AnalogMove.Normal * Rotation.FromYaw( MouseAngles.yaw );

		CalculateMovementMultiplierByState();

		var velocity = (direction * fromInputVelocity.Length) * (MovementSpeed * CurrentMovementMultiplier);

		// inertia down
		if ( velocity.Length >= 1f && (MovementState == MovementState.Default || MovementState == MovementState.Running) )
		{
			this.WorldPosition += velocity;
		}
	}

	private void PhysicsMovement()
	{
		Vector3 direction = Input.AnalogMove.Normal * Rotation.FromYaw( MouseAngles.yaw );

		CalculateMovementMultiplierByState();

		var velocity = direction.Normal * MovementSpeed * CurrentMovementMultiplier;

		WorldPosition += new Vector3( 0, 0, 0.001f );

		if( velocity.Length != 0)
		{
			Rigidbody.ApplyForce( velocity * 1000 );
		}
		else
		{
			Rigidbody.ApplyForce( Rigidbody.Velocity * -1 * 10000 );
		}

		if( Rigidbody.Velocity.Length > 100 )
		{
			Rigidbody.ApplyForce( Rigidbody.Velocity * -1 * 10000 );
		}
	}

	private void Jump()
	{
		bool canJump = false;

		if ( jumpTimer <= 0 )
		{
			canJump = true;
		}
		else
		{
			canJump = false;
			jumpTimer -= Time.Delta;
		}

		IsGrounded = GetGround();

		if ( !(Input.Pressed( "Jump" ) && IsGrounded && canJump && Rigidbody.Gravity) )
			return;

		jumpTimer = JumpCooldown;

		Rigidbody.ApplyImpulse( Vector3.Up * 10000 * JumpForceMultiplier );
	}

	private bool GetGround()
	{
		var ignorer = new string[] { "player" };

		// Default GroundBoxSize is 5, 5, 9
		var traceResult = BuildRaytrace( WorldPosition, GroundBoxSize, ignorer );

		return traceResult.Hit;
	}

	public SceneTraceResult BuildRaytrace( Vector3 from, Vector3 to, string[] withoutTags )
	{
		//var bbox = BBox.FromPositionAndSize( Vector3.Zero, 1 );

		return Scene.Trace
		.Ray( new Ray( from, to ), GroundBoxSize.z )
		//.Size( bbox )
		.WithoutTags( withoutTags )
		.Run();
	}

	private void CalculateMovementMultiplierByState()
	{
		switch ( MovementState )
		{
			case MovementState.Running:
				CurrentMovementMultiplier = RunSpeed;
				break;
			case MovementState.Crouch:
				CurrentMovementMultiplier = CrouchSpeed;
				break;
			case MovementState.Creep:
				CurrentMovementMultiplier = CreepSpeed;
				break;
			default:
				CurrentMovementMultiplier = 1;
				break;
		}
	}

	/// <summary>
	/// performs <seealso cref="MovementState"/> enum switches by Input
	/// </summary>
	private void KeyboardMovementStateControl()
	{
		if ( Input.Down( "Run" ) )
		{
			MovementState = MovementState.Running;
		}
		else if ( Input.Down( "Jump" ) )
		{
			MovementState = MovementState.Jump;
		}
		else if ( Input.Down( "Duck" ) )
		{
			CrouchControl( true );


			MovementState = MovementState.Crouch;
		}
		else if ( Input.Down( "Creep" ) )
		{
			MovementState = MovementState.Creep;
		}
		else
		{
			MovementState = MovementState.Default;
		}

		if( Input.Released( "Duck" ) )
		{
			CrouchControl( false );
		}
	}

	private void CrouchControl(bool value)
	{
		if( value )
		{
			var coll = GetComponent<CapsuleCollider>();

			coll.Start = new Vector3( 0, 0, 0 );
		}
		else
		{
			var coll = GetComponent<CapsuleCollider>();

			coll.Start = new Vector3( 0, 0, -20.4f );
		}
	}

	private void ControllerMovementStateControl()
	{
		// Lazy developers....
	}

	private void InitCamera()
	{
		if ( PlayerCamera != null )
		{
			PlayerCamera = PlayerCamera.Clone( WorldPosition );
		}
		else
		{
			PlayerCamera = new GameObject( true, DefaultCameraName );

			CameraComponent cameraComponent = PlayerCamera.Components.Create<CameraComponent>();
			cameraComponent.ZFar = 30000 / ZFarDivider;
			cameraComponent.FieldOfView = 90;
		}

		PlayerCamera.Name = DefaultCameraName;
		PlayerCamera.SetParent( this.GameObject );

		PlayerCamera.WorldPosition = WorldPosition;
		PlayerCamera.LocalPosition = CameraOffset;
	}
}

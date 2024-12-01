using Sandbox;

public enum DoorState
{
	Closed,
	Opening,
	Opened,
	Closing
}

public sealed class DoorOpener : Component, IInteractable
{
	[RequireComponent]
	private Interactable Interactable { get; set; }

	[RequireComponent]
	public SoundPointComponent Sound { get; set; }

	private DoorState _state = DoorState.Closed;

	[Property]
	public float OpeningSpeed { get; set; } = 5f;

	private float StartRotation { get; set; }

	protected override void OnStart()
	{
		Sound = GetComponent<SoundPointComponent>();

		StartRotation = LocalRotation.y;
	}

	protected override void OnUpdate()
	{
		if ( _state == DoorState.Opening )
		{
			LocalRotation = Rotation.Slerp( LocalRotation, Rotation.FromYaw( StartRotation + 90 ), OpeningSpeed * Time.Delta );
		
			if ( IsApproximateFor( LocalRotation.Yaw(), StartRotation + 90, 0.5f ) )
			{
				_state = DoorState.Opened;
			}
		}
		
		if ( _state == DoorState.Closing )
		{
			LocalRotation = Rotation.Slerp( LocalRotation, Rotation.FromYaw( StartRotation ), OpeningSpeed * Time.Delta );
		
			if ( IsApproximateFor( LocalRotation.Yaw(), StartRotation, 0.5f ) )
			{
				_state = DoorState.Closed;
			}
		}
	}

	public static bool IsApproximateFor( float value, float comparedValue, float approximateValue )
	{
		return value > comparedValue - approximateValue && value < comparedValue + approximateValue + approximateValue;
	}

	public void Interact()
	{
		if ( _state == DoorState.Closed )
		{
			_state = DoorState.Opening;
			Sound.StartSound();
		}
		else if ( _state == DoorState.Opened )
		{
			_state = DoorState.Closing;
			Sound.StartSound();
		}
	}
}

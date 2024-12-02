using Sandbox;

public sealed class SwitchController : Component, IInteractable
{
	public bool State { get; set; } = true;

	[RequireComponent]
	public SoundPointComponent Sound { get; set; }

	[RequireComponent, Property]
	public SoundEvent SoundEvent { get; set; }

	[RequireComponent]
	public Interactable Interactable { get; set; }

	[Property]
	public Light ToggleLight { get; set; }

	/// <summary>
	/// Might be NULL
	/// </summary>
	[Property]
	public ModelRenderer Model { get; set; } = null;

	private float TimerCooldown { get; set; } = 0.3f;

	private float jumpTimer;
	private bool enableInteraction = true;

	protected override void OnStart()
	{
		base.OnStart();

		Sound.SoundEvent = SoundEvent;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( jumpTimer <= 0 )
		{
			enableInteraction = true;
		}
		else
		{
			jumpTimer -= Time.Delta;
		}
	}

	public void Interact()
	{
		if ( !enableInteraction )
			return;

		if( Model != null )
		{
			if ( State )
			{
				Model.MaterialGroup = "off";
			}
			else
			{
				Model.MaterialGroup = "default";
			}
		}

		State = !State;

		Sound.StartSound();
		ToggleLight.Enabled = State;

		jumpTimer = TimerCooldown;
		enableInteraction = false;
	}
}

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
	public List<Light> ToggleLight { get; set; } = new List<Light>();

	/// <summary>
	/// Might be NULL
	/// </summary>
	[Property]
	public List<ModelRenderer> Model { get; set; } = new List<ModelRenderer>();

	[Property]
	public string MaterialGroupStingOff { get; set; } = "";

	[Property]
	public string MaterialGroupStingOn { get; set; } = "";

	[Property]
	public EnvmapProbe Probe { get; set; }

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

	public void Interact(PlayerGameController player)
	{
		if ( !enableInteraction )
			return;

		State = !State;

		Sound.StartSound();

		foreach ( Light light in ToggleLight )
		{
			light.Enabled = State;
		}

		if( Probe != null )
		{
			Probe.Enabled = State;
		}

		foreach (var model in Model )
		{
			if ( State )
			{
				model.MaterialGroup = MaterialGroupStingOn;

			}
			else
			{
				model.MaterialGroup = MaterialGroupStingOff;
			}
		}

		jumpTimer = TimerCooldown;
		enableInteraction = false;
	}
}

using Sandbox;

public enum LifePain
{
	Good,
	Normal,
	Bad,
	Dying
}

public sealed class LifeExperience : Component
{
	public static LifeExperience Instance { get; private set; }

	[Property, ReadOnly] public float Health { get; set; } = 100;
	[Property, ReadOnly] public float Starvation { get; set; } = 100;
	[Property, ReadOnly] public float Sleepiness { get; set; } = 100;
	[Property, ReadOnly] public float Fear { get; set; } = 100;
	[Property, ReadOnly] public float Happiness { get; set; } = 100;

	private UiController _ui;

	protected override void OnStart()
	{
		base.OnStart();

		_ui = UiController.Instance;

		Instance = this;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		Starvation -= Time.Delta * 0.1f;
		Sleepiness -= Time.Delta * 0.05f;

		if ( Starvation <= 0 )
		{
			Health -= Time.Delta * 1f; 
		}
		if ( Sleepiness <= 0 )
		{
			Health -= Time.Delta * 1f;
		}

		_ui.GetHSSLEPValues(this);
	}

	public void AddHealth( float value )
	{

	}

	public void AddStarvation( float value )
	{
		if( Starvation + value > 100 )
		{
			Starvation = 100;
		}
		else
		{
			Starvation += value;
		}
	}

	public void AddSleepiness( float value )
	{

	}

	public void AddFear( float value )
	{

	}

	public void AddHappiness( float value )
	{

	}
}


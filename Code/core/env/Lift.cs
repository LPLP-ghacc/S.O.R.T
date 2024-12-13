using Sandbox;

public sealed class Lift : Component
{
	private Collider Trigger { get; set; }

	[Property]
	public GameResource LoadScene { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		Trigger = GetComponent<Collider>();

		Trigger.OnObjectTriggerEnter += ( coll ) =>
		{
			var player = (GameObject) coll;

			if ( player.Tags.Contains( "player" ) )
			{
				Scene.Load( LoadScene );
			}
		};
	}

	protected override void OnUpdate()
	{

	}
}

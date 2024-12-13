using Sandbox;
using System;

public sealed class CoreCrate : Component, IInteractable
{
	[Property]
	public string HatTagName { get; set; } = "hat";

	[Property]
	public GameObject Point { get; set; }

	[Property]
	public GameObject Hat { get; set; }

	private bool IsHat { get; set; } = true;

	protected override void OnStart()
	{
		base.OnStart();

		var coll = AddComponent<BoxCollider>();

		coll.IsTrigger = true;

		Action<GameObject> onTrigerEnter = ( obj ) =>
		{
			if ( obj.Tags.Contains( HatTagName ) )
			{
				if ( Point == null )
					return;
				Hat.SetParent( GameObject );
				var rig = Hat.GetComponent<Rigidbody>();
				rig = null;
				var moveable = Hat.AddComponent<Moveable>();
				moveable = null;

				Hat.WorldPosition = Point.WorldPosition;
			}
		};

		coll.OnObjectTriggerEnter += onTrigerEnter;

		coll.OnTriggerEnter += ( s ) =>
		{
			Log.Info( (s as Collider).GameObject.Name );
		};
	}

	public void Interact( PlayerGameController player )
	{
		Hat.SetParent( GameObject.Parent );
		Hat.AddComponent<Rigidbody>();
		Hat.AddComponent<Moveable>();
	}
}

using Sandbox;

public enum Weight
{
	Refrigerator,
	Microwave,
	OneLiterOfWater
}

public sealed class Moveable : Component, IMoveable
{
	[Property]
	public Weight Weight { get; set; } = Weight.Microwave;

	[RequireComponent]
	private Rigidbody Body { get; set; }

	protected override void OnStart()
	{
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if ( rigidbody != null )
		{
			Body = rigidbody;
		}
	}

	public void MoveTo( Vector3 to, float speed )
	{
		Body.SmoothMove( to, speed / GetWeightMultiplierBy( Weight ), Time.Delta );
	}

	public void RotateTo( Rotation rotation, float speed )
	{
		Body.SmoothRotate( rotation, speed / GetWeightMultiplierBy( Weight ), Time.Delta );
	}

	public void Throw( Vector3 f )
	{
		Body.ApplyImpulse( f );
	}

	private float GetWeightMultiplierBy( Weight weight )
	{
		switch ( weight )
		{
			case Weight.Microwave:
				return 0.9f;
			case Weight.OneLiterOfWater:
				return 5f;
			case Weight.Refrigerator:
				return 0.3f;
			default : return 1;
		}
	}
}

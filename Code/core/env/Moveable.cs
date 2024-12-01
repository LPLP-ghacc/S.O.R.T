using Sandbox;

public sealed class Moveable : Component, IMoveable
{
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
		if ( Body != null )
		{
			Body.SmoothMove( to, speed, Time.Delta );
		}
		else
		{
			Log.Info( $"NO RIGIDBODY!!! {this.GameObject.Name}" );
		}
	}

	public void RotateTo( Rotation rotation, float speed )
	{
		if ( Body != null )
		{
			Body.SmoothRotate( rotation, speed, Time.Delta );
		}
		else
		{
			Log.Info( $"NO RIGIDBODY!!! {this.GameObject.Name}" );
		}
	}

	public void Throw( Vector3 f )
	{
		if ( Body != null )
		{
			Body.ApplyImpulse( f );
		}
		else
		{
			Log.Info( $"NO RIGIDBODY!!! {this.GameObject.Name}" );
		}
	}
}

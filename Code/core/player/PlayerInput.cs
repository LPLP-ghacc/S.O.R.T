using System;

[Hide]
public sealed class PlayerInput : Component
{
	[Property]
	public bool isUsingLerp { get; set; } = true;

	[Property]
	public Vector3 InputVector { get; private set; } = Vector3.Zero;

	public bool isInput = false;

	/// <summary>
	/// 0.4 DEFAULT VALUE
	/// </summary>
	[Property]
	[Range( 0.1f, 1f, 0.1f )]
	public float KeyFactor { get; private set; } = 0.4f;

	private float x;
	private float y;

	// Optimize it later
	protected override void OnFixedUpdate()
	{
		isInput = x == 0 && y == 0 ? false : true;

		if ( isUsingLerp )
		{
			if ( Input.Down( "Forward" ) )
			{
				y = Input.Down( "Forward" ) ? MathX.Lerp( y, 1, KeyFactor ) : MathX.Lerp( y, 0, KeyFactor );
			}
			else
			{
				y = Input.Down( "Backward" ) ? MathX.Lerp( y, -1, KeyFactor ) : MathX.Lerp( y, 0, KeyFactor );
			}

			if ( Input.Down( "Right" ) )
			{
				x = Input.Down( "Right" ) ? MathX.Lerp( x, 1, KeyFactor ) : MathX.Lerp( x, 0, KeyFactor );
			}
			else
			{
				x = Input.Down( "Left" ) ? MathX.Lerp( x, -1, KeyFactor ) : MathX.Lerp( x, 0, KeyFactor );
			}

			InputVector = new Vector2( x, y );
		}
		else
		{
			if ( Input.Down( "Forward" ) )
			{
				y = Input.Down( "Forward" ) ? 1 : 0;
			}
			else
			{
				y = Input.Down( "Backward" ) ? -1 : 0;
			}

			if ( Input.Down( "Right" ) )
			{
				x = Input.Down( "Right" ) ? 1 : 0;
			}
			else
			{
				x = Input.Down( "Left" ) ? -1 : 0;
			}

			InputVector = new Vector2( x, y );
		}
	}

	// Why not? 
	public Vector2 GetInputVector()
	{
		return InputVector;
	}

	public void Print()
	{
		Log.Info( $"InputVector: {InputVector}" );
		Log.Info( $"isInput: {isInput}" );
	}
}

public interface IMoveable
{
	public void MoveTo( Vector3 to, float speed );

	public void RotateTo( Rotation rotation, float speed );

	public void Throw( Vector3 force );
}

using Sandbox;

public sealed class Wire : Component
{
	private GameObject From { get; set; }

	[Property]
	public GameObject To { get; set; }

	[Property]
	public int PointNum { get; set; } 

	List<GameObject> Points = new List<GameObject> { };

	protected override void OnStart()
	{
		base.OnStart();

		From = GameObject;

		if( To == null )
		{
			Log.Error("To - GameObject is null");
			return;
		}

		LineRenderer lineRenderer = AddComponent<LineRenderer>();

		var frame = new Gradient.ColorFrame();
		frame.Value = Color.Black;

		lineRenderer.Color = new Gradient( new Gradient.ColorFrame[] { frame } );
		lineRenderer.Width = new Curve( new List<Curve.Frame>() { new Curve.Frame(0.5f, 0.5f)} );
		lineRenderer.SplineInterpolation = 16;

		Vector3 fromPosition = From.WorldPosition;
		Vector3 toPosition = To.WorldPosition;
		float distance = Vector3.DistanceBetween( fromPosition, toPosition );

		float step = 1f / (PointNum - 1);

		for ( int i = 0; i < PointNum; i++ )
		{
			Vector3 pointPosition = Vector3.Lerp( fromPosition, toPosition, step * i );

			GameObject obj = new GameObject();
			obj.Name = $"Wire part [{i}]";
			obj.SetParent( this.GameObject );

			var rigi = obj.AddComponent<Rigidbody>();

			var locking = new PhysicsLock
			{
				Pitch = true,
				Roll = true,
				Yaw = true
			};
			rigi.Locking = locking;

			var coll = obj.AddComponent<SphereCollider>();
			coll.Radius = 1f;

			obj.WorldPosition = pointPosition;

			Points.Add( obj );
		}

		for ( int i = 0; i < Points.Count; i++ )
		{
			if ( i < Points.Count - 1 )
			{
				var joint = Points[i].AddComponent<HingeJoint>();

				joint.Body = Points[i + 1];
			}
			else
			{

			}
		}

		lineRenderer.Points = Points;

		var rig1 = Points[0].GetComponent<Rigidbody>();
		rig1.MotionEnabled = false;

		var rig2 = Points[Points.Count - 1].GetComponent<Rigidbody>();
		rig2.MotionEnabled = false;
		Points[Points.Count - 1].SetParent(To);
	}
}

using Sandbox;
using System.IO;
using System.Xml.Linq;

public sealed class Hand : Component
{
	public bool IsRight { get; set; } = true;

	public GameObject Selected { get; set; } = null;

	public void SetUp(Item item)
	{
		Selected = item.HandPrefab.Clone();
		Selected.SetParent( this.GameObject );
		Selected.LocalPosition = this.GameObject.LocalPosition;
		Selected.LocalRotation = this.GameObject.LocalRotation;

		foreach ( Component comp in Selected.Components.GetAll() )
		{
			if ( comp is IItemController controller )
			{
				controller?.OnEnable();
				Log.Info( "SetUp : controller?.OnEnable()" );
			}
		}

		Log.Info(Selected.Transform.World);
	}

	public void TakeOff()
	{
		if ( Selected == null )
			return;

		foreach ( Component comp in Selected.Components.GetAll() )
		{
			if ( comp is IItemController controller )
			{
				controller?.OnDisable();
			}
		}

		Selected.Destroy();
		Selected = null;
	}

	public void Use()
	{

	}
}

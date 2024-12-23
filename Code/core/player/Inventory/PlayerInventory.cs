using Microsoft.VisualBasic;
using Sandbox;

public sealed class PlayerInventory : Component
{
	public static PlayerInventory Instance = new PlayerInventory();

	public Hand Left { get; set; }
	public Hand Right { get; set; }
	public List<Item> Slots { get; private set; } = new List<Item>();
	public Item Selected { get; private set; }
	public int SelectedIndex { get; private set; } = 0;
	private bool IsSwitched { get; set; } = false;
	private RaycastInteractions Interaction { get; set; }

	protected override void OnEnabled()
	{
		Instance = this;

		Left = GameObject.Scene.Directory.FindByName( "LHAND" ).First().GetComponent<Hand>();
		Right = GameObject.Scene.Directory.FindByName( "RHAND" ).First().GetComponent<Hand>();

		Interaction = this.GetComponent<RaycastInteractions>();

		Item hands = new()
		{
			ItemId = 0,
			Name = "Hands",
			Description = "manipulators built in from birth to control the energy of the surrounding world"
		};

		GameObject gameObject = new GameObject();
		gameObject.SetPrefabSource( "prefabs/inv/hands.prefab" );
		hands.HandPrefab = gameObject;
		hands.ModelImg = "textures/items/reach.png";
		hands.IsUseable = true;
		AddItem( hands );
		Selected = hands;
		UpdateHandObject();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Slots.Count == 1 )
			return;

		if( !Interaction.IsMove )
		{
			HandleMouseWheelInput();

			if ( Selected != null && Selected.IsUseable && IsSwitched == false )
			{
				UpdateHandObject();
				IsSwitched = true;
			}
		}
	}

	private void HandleMouseWheelInput()
	{
		if ( Input.MouseWheel.y > 0 )
		{
			SelectedIndex = (SelectedIndex + 1) % Slots.Count;
			IsSwitched = false;
		}
		else if ( Input.MouseWheel.y < 0 )
		{
			SelectedIndex = (SelectedIndex - 1 + Slots.Count) % Slots.Count;
			IsSwitched = false;
		}

		Selected = Slots[SelectedIndex];
	}

	private void UpdateHandObject()
	{
		Right.TakeOff();

		Right.SetUp( Selected );
		Log.Info( $"Switched to item: {Selected.Name}" );
	}

	public void AddItem( Item item )
	{
		Slots.Add( item );
		if ( Slots.Count == 1 )
		{
			Selected = item;
			SelectedIndex = 0;
		}
	}

	public void RemoveItem( Item item )
	{
		if ( Slots.Contains( item ) )
		{
			int indexToRemove = Slots.IndexOf( item );
			Slots.Remove( item );

			if ( SelectedIndex >= Slots.Count )
			{
				SelectedIndex = Slots.Count - 1;
			}

			Selected = Slots.Count > 0 ? Slots[SelectedIndex] : null;
		}
	}
}

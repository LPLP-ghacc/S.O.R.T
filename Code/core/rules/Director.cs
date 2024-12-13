using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using static Sandbox.Gizmo;
using static Sandbox.Services.Inventory;

public enum GameTime
{
	Morning,
	Day,
	Evening,
	Night
}

public enum EventLevel
{
	Easy,
	Medium,
	Hard,
	End
}

public record GameEventProperties( int ID, string Name, string Description );

public class GameEvent
{
	public GameEvent( GameEventProperties properties, EventLevel level, TimeSpan timeToSolve, TimeSpan appearanceTime, int cycles, bool solvable )
	{
		EventProperties = properties;

		Level = level;
		TimeToFail = timeToSolve;
		AppearanceTime = appearanceTime;
		UpdateCounter = cycles;
		Solvable = solvable;
	}

	public Action OnAppearing { get; set; }
	public Action OnUpdated { get; set; }
	public Action OnFailed { get; set; }
	public Action OnSolved { get; set; }

	public GameEventProperties EventProperties { get; set; } = new GameEventProperties(1, "DefaultEventName", "DefaultDescription"); 
	public EventLevel Level { get; set; } = EventLevel.Medium;

	/// <summary>
	/// the time at which the event will not be resolved
	/// </summary>
	public TimeSpan TimeToFail { get; set; }
	public bool IsFailed { get; set; } = false;
	public TimeSpan AppearanceTime { get; set; }
	public int UpdateCounter { get; set; } = 1;
	public bool Solvable { get; set; } = true;
	public bool IsSolved { get; set; } = false;

	public void Start()
	{
		OnAppearing?.Invoke();
	}

	public void Update()
	{
		if ( IsFailed )
			return;

		OnUpdated?.Invoke();

		if( UpdateCounter != 0 )
		{
			UpdateCounter--;
		}
		else
		{
			OnSolved?.Invoke();
		}
	}

	public void Fail()
	{
		OnFailed?.Invoke();
	}

	public override string ToString()
	{
		return $"[{EventProperties.ID}] - [{EventProperties.Name}]\n[{EventProperties.Description}]\nСomplexity: {Enum.GetName( typeof( EventLevel ) , Level)}";
	}
}

public sealed class Director : Component
{
	public static Director Instance { get; private set; }

	public TimeSpan CurrentTime { get; set; }

	[Property, ReadOnly]
	public GameTime GameTime { get; set; }

	[Property, ReadOnly]
	public string STRCurrentTime { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		Instance = this;

		GameObject.Name = "GAME DIRECTOR";

		var arr = GetNumberArrayByTime( CurrentTime );

		foreach ( var item in arr )
		{
			Log.Info( item );
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		CurrentTime = DateTime.Now.TimeOfDay;

		STRCurrentTime = DateTime.Now.ToString( "HH:mm:ss" );

		if ( CurrentTime >= TimeSpan.FromHours( 6 ) && CurrentTime < TimeSpan.FromHours( 12 ) )
		{
			GameTime = GameTime.Morning;
		}
		else if ( CurrentTime >= TimeSpan.FromHours( 12 ) && CurrentTime < TimeSpan.FromHours( 18 ) )
		{
			GameTime = GameTime.Day;
		}
		else if ( CurrentTime >= TimeSpan.FromHours( 18 ) && CurrentTime < TimeSpan.FromHours( 21 ) )
		{
			GameTime = GameTime.Evening;
		}
		else
		{
			GameTime = GameTime.Night;
		}
	}

	private List<int> GetNumberArrayByTime( TimeSpan currentTime )
	{
		List<int> result = new List<int>();

		byte[] bytes = Encoding.UTF8.GetBytes( (currentTime).ToString() );

		for(int i = 0; i < bytes.Length; i++ )
		{
			Random random = new Random( bytes[i] + i );

			result.Add( random.Next( 5, 14 ) );
		}

		return result;
	}
}



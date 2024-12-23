using Sandbox;
using static System.Net.Mime.MediaTypeNames;
using System;
using static Sandbox.Gizmo;

public class FPSCounter
{
	public enum DeltaTimeType
	{
		Smooth,
		Unscaled
	}

	public DeltaTimeType DeltaType = DeltaTimeType.Smooth;

	private Dictionary<int, string> CachedNumberStrings = new();

	private int[] _frameRateSamples;
	private int _cacheNumbersAmount = 300;
	private int _averageFromAmount = 30;
	private int _averageCounter;
	private int _currentAveraged;

	public void Awake()
	{
		// Cache strings and create array
		{
			for ( int i = 0; i < _cacheNumbersAmount; i++ )
			{
				CachedNumberStrings[i] = i.ToString();
			}

			_frameRateSamples = new int[_averageFromAmount];
		}
	}

	public string Update()
	{
		// Sample
		{
			var currentFrame = (int)Math.Round( 1f / DeltaType switch
			{
				DeltaTimeType.Smooth => Time.Delta,
				DeltaTimeType.Unscaled => Time.Delta,
				_ => Time.Delta
			} );
			_frameRateSamples[_averageCounter] = currentFrame;
		}

		// Average
		{
			var average = 0f;

			foreach ( var frameRate in _frameRateSamples )
			{
				average += frameRate;
			}

			_currentAveraged = (int)Math.Round( average / _averageFromAmount );
			_averageCounter = (_averageCounter + 1) % _averageFromAmount;
		}

		// Assign to UI
		{
			return _currentAveraged switch
			{
				var x when x >= 0 && x < _cacheNumbersAmount => CachedNumberStrings[x],
				var x when x >= _cacheNumbersAmount => $"> {_cacheNumbersAmount}",
				var x when x < 0 => "< 0",
				_ => "?"
			};
		}
	}
}

public sealed class UiController : Component
{
	public static UiController Instance { get; private set; }

	private FPSCounter _averageCounter = new FPSCounter();

	[Property]
	public IngameMenu Menu { get; set; }

	public SceneCamera GetPlayerCamera()
	{
		return GetComponent<SceneCamera>();
	}

	protected override void OnAwake()
	{
		Instance = this;

		GameObject.Tags.Add("UI");
		base.OnAwake();
		_averageCounter.Awake();
	}

	protected override void OnUpdate()
	{
		Menu.FPSCounter = _averageCounter.Update();
	}

	public void GetHSSLEPValues(LifeExperience experience) 
	{
		Menu.Health = MathF.Round( experience.Health ).ToString();
		Menu.Starvation = experience.Starvation.ToString();
		//Menu.Sleepiness = )athF.Round( experience.Sleepiness );
	}
}	

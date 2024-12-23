using Sandbox;
using System;

public enum PowerOutput
{
	None,
	Low,
	Stable
}

public sealed class ElectricityGenerator : Component
{
	[Property, ReadOnly]
	public PowerOutput Power { get; set; } = PowerOutput.Low;

	public Action OnChanged { get; set; }

	public void ChangeTo( PowerOutput power )
	{
		Power = power;

		OnChanged?.Invoke();
	}
}

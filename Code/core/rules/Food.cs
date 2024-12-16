using Sandbox;

public sealed class Food : Component, IInteractable
{
	[Property]
	public float FoodValue { get; set; } = 15;

	[Property]
	public int FoodCycle { get; set; } = 5;

	public void Interact( PlayerGameController player )
	{
		LifeExperience experience = LifeExperience.Instance;

		if ( FoodCycle == 0 )
		{
			experience.AddStarvation( FoodValue );

			GameObject.Destroy();
		}
		else
		{
			experience.AddStarvation( FoodValue );
			FoodCycle--;
		}

		Log.Info($"added starvation, actual: {experience.Starvation}");
	}
}

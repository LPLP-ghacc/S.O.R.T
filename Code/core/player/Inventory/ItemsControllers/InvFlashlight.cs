public sealed class InvFlashlight : Component, IItemController
{
	private string InteractKey { get; set; } = "attack1";
	private SpotLight Light { get; set; }
	private bool IsEnabled { get; set; } = true;

	public void OnEnable()
	{
		var camGo = UiController.Instance;
		Light = camGo.GameObject.AddComponent<SpotLight>();

		Light.Radius = 560f;
		Light.ConeOuter = 30f;
		Light.ConeInner = 10f;
		Light.Attenuation = 0.1f;
	}

	public void OnDisable()
	{
		Light.Destroy();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(Input.Released( InteractKey ) )
		{
			IsEnabled = !IsEnabled;

			Light.Enabled = IsEnabled;
		}
	}
}

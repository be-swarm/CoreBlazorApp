using BeSwarm.WebApiClient;

namespace TestMauiBlazor
{
	public class SecureConfig : ISecureConfig
	{
		public string ServiceEntryPoint { get; set; } = "https://dev.user.beswarm.net";
		public string UserSwarm { get; set; } = "testdev";
		public string ApplicationId { get; set; } = "fc824c2d-5ce9-4f81-8199-4b76186d474d.77c8cdea-480f-47b6-8d20-67c6a39c9a9c.testdev";
		public string ClientSecret { get; set; } = "MySecret";
		public string CallBackUri { get; set; } = "com.beswarm.testmauiblazor://";
	}
}

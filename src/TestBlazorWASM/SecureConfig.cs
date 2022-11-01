using BeSwarm.WebApiClient;

namespace TestBlazorWASM
{
	public class SecureConfig : ISecureConfig
	{
		public string ServiceEntryPoint { get; set; } = "https://dev.user.beswarm.net";
		public string UserSwarm { get; set; } = "testdev";
		public string ApplicationId { get; set; } = "fc824c2d-5ce9-4f81-8199-4b76186d474d.28ea0ff0-6bdf-4d73-ac8d-063ecbebf9a0.testdev";
		public string ClientSecret { get; set; } = "MySecret";
		public string CallBackUri { get; set; } = "";
	}
}

using BeSwarm.WebApi;

namespace TestBlazorWASM
{
	public class SecureConfig : ISecureConfig
	{
		public string ServiceEntryPoint { get; set; } = "https://dev.beswarm.net";
		public string UserSwarm { get; set; } = "testdev";
		public string ApplicationId { get; set; } = "fc824c2d-5ce9-4f81-8199-4b76186d474d.c6f4141b-5085-461e-b4d1-b7aa4e85bbfb.testdev";
		public string ClientSecret { get; set; } = "MySecret";
		public string CallBackUri { get; set; } = "http://localhost:7173/Login";
	}
}

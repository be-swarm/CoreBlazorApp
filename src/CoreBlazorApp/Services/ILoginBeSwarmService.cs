namespace BeSwarm.CoreBlazorApp.Services;

public enum LoginActions
{
	none = 1,
	gettokens = 2
}
public class LoginAction
{
	public string CallBackUrl { get; set; } = "";
	public LoginActions Action { get; set; } = LoginActions.none;
}
public interface ILoginBeSwarmService
{

	Task<LoginAction> Login(string url, string callbackurl);
}

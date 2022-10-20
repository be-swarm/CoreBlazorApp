namespace BeSwarm.CoreBlazorApp.Services;


public interface IThemeService
{
	public ITheme child { get; set; }
	void SetDarkMode(bool darkmode);
}
public interface ITheme
{
	void SetDarkMode(bool darkmode);
}

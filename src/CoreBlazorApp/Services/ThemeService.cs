namespace BeSwarm.CoreBlazorApp.Services;

public class ThemeService : IThemeService
{

	ITheme _child = default!;

	ITheme IThemeService.child { get => _child; set => _child = value; }
	public void SetDarkMode(bool darkmode)
	{
		_child.SetDarkMode(darkmode);
	}
}

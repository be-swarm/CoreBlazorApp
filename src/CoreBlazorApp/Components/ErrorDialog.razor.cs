

using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class ErrorDialog
{
	[Inject] public ErrorDialogService Service { get; set; } = default!;
	public MudMessageBox mbox = default!;
	public string Caption = "";
	public string Message = "";
	public string OKCaption = "";
	public string CancelCaption = "";
	protected override async Task OnInitializedAsync()
	{
		Service.obj = this;
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		await base.OnAfterRenderAsync(FirstTime);
	}
}

public class ErrorDialogService
{
	public ErrorDialog? obj;


	public ErrorDialogService()
	{
	}
	public async Task Show(string title, string message)
	{
		if (obj is { })
		{
			obj.Caption = title;
			obj.Message = message;
			await obj.mbox.Show();
		}
	}

}




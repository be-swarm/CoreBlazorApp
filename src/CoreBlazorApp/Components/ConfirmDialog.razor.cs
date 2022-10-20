using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class ConfirmDialog
{
	[Inject] public ConfirmDialogService Service { get; set; } = default!;
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

public class ConfirmDialogService
{
	public delegate Task OnConfirmAction();
	public delegate Task OnCancelAction();

	public ConfirmDialog obj = default!;
	public async Task Show(string caption, string message, string okcaption = "OK", string cancelcaption = "CANCEL", OnConfirmAction? OnConfirmAction = null, OnCancelAction? OnCancelAction = null)
	{
		obj.Caption = caption;
		obj.Message = message;
		obj.OKCaption = okcaption;
		obj.CancelCaption = cancelcaption;
		DialogOptions dialogOptions = new()
		{
			DisableBackdropClick = true
		};

		bool? result = await obj.mbox.Show(dialogOptions);
		if (result == true) OnConfirmAction?.Invoke();
		else OnCancelAction?.Invoke();

	}
}




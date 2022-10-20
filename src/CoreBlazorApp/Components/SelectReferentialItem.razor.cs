using BeSwarm.WebApiClient.Models;

using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components;


public partial class SelectReferentialItem : IDisposable
{
	private ReferentialItemLang? _value;
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	public SelectReferentialItem()
	{

	}
	[Parameter] public List<ReferentialItemLang> Items { get; set; } = new();
	[Parameter] public string Caption { get; set; } = "";
	[Parameter]
	public ReferentialItemLang? Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<ReferentialItemLang>.Default.Equals(value, _value))
			{
				_value = value;
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	[Parameter] public EventCallback<ReferentialItemLang> SelectChanged { get; set; }
	[Parameter] public EventCallback<ReferentialItemLang> ValueChanged { get; set; }


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		if (FirstTime)
		{
			Session.EnvironmentHasChanged += async (ChangeEvents e) => await OnRefresh(e);
			await OnRefresh(ChangeEvents.Lang);
		}
		await base.OnAfterRenderAsync(FirstTime);

	}
	public async Task OnRefresh(ChangeEvents e)
	{
		//if (e == ChangeEvents.Login || e == ChangeEvents.Lang)  
		{
			Value = Items.FirstOrDefault(x => x.Uid == Session.Lang);
			StateHasChanged();
		}
	}
	private async void OnSelectedValuesChanged(IEnumerable<ReferentialItemLang> values)
	{
		await SelectChanged.InvokeAsync(values.First());
	}

	public void Dispose()
	{
		Session.EnvironmentHasChanged -= async (ChangeEvents e) => await OnRefresh(e);
	}
}

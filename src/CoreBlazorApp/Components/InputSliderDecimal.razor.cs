using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class InputSliderDecimal : IDisposable
{
	private Decimal _value;
	[Parameter]
	public Decimal Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<Decimal>.Default.Equals(value, _value))
			{
				_value = value;
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Parameter]
	public EventCallback<Decimal> ValueChanged { get; set; }
	[Parameter] public Expression<Func<Decimal>> ValidateProperty { get; set; } = default!;
	[CascadingParameter] public EditContext? EditContext { get; set; } = default!;
	[Parameter] public string Label { get; set; } = "";
	[Parameter] public Decimal Step { get; set; } = 1;
	[Parameter] public Decimal Min { get; set; } = Decimal.MinValue;
	[Parameter] public Decimal Max { get; set; } = Decimal.MaxValue;
	[Parameter] public bool ValidateStrict { get; set; }
	[Parameter] public bool ReadOnly { get; set; }

	protected override void OnInitialized()
	{
		// adjust MaxLength

		if (EditContext != null && ValidateProperty != null && ValidateStrict)
		{
			var sl = Validate.GetAttributeIfExist<RangeAttribute>(EditContext.Model!, ((MemberExpression)ValidateProperty.Body).Member.Name);
			if (sl is { })
			{
				Min = Decimal.Parse(sl.Minimum.ToString() ?? string.Empty);
				Max = Decimal.Parse(sl.Maximum.ToString() ?? string.Empty);

			}

		}

	}
	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		if (FirstTime)
		{
			Session.EnvironmentHasChanged += async (ChangeEvents e) => await OnRefresh(e);
			await OnRefresh(ChangeEvents.Lang);
		}
	}
	private async Task OnRefresh(ChangeEvents e)
	{
		if (e == ChangeEvents.Login || e == ChangeEvents.Lang || e == ChangeEvents.Init)
		{
			StateHasChanged();
		}

	}
	void IDisposable.Dispose()
	{
		Session.EnvironmentHasChanged -= async (ChangeEvents e) => await OnRefresh(e);
	}
}


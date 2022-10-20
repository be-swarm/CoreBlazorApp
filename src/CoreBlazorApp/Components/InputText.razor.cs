using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BeSwarm.CoreBlazorApp.Components;



public partial class InputText : IDisposable
{
	private string _value = default!;
	[Parameter]
	public string Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<string>.Default.Equals(value, _value))
			{
				_value = value is { } ? value : "";
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	[Parameter] public Expression<Func<string>>? ValidateProperty { get; set; }
	[CascadingParameter] public EditContext? EditContext { get; set; } = default!;

	[Parameter] public EventCallback<string> ValueChanged { get; set; }


	[Parameter] public string Label { get; set; } = "";

	[Parameter] public int MaxLength { get; set; } = 65535;


	[Parameter] public bool ReadOnly { get; set; }

	[Parameter] public bool ValidateStrict { get; set; }

	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;

	protected override void OnInitialized()
	{
		// adjust MaxLength
		Session.EnvironmentHasChanged += async (ChangeEvents e) => await OnRefresh(e);

		if (EditContext != null && ValidateProperty != null && ValidateStrict)
		{
			var sl = Validate.GetAttributeIfExist<MaxLengthAttribute>(EditContext.Model!, ((MemberExpression)ValidateProperty.Body).Member.Name);
			if (sl is { }) MaxLength = sl.Length;

		}

	}
	private async Task OnRefresh(ChangeEvents e)
	{
		await ValueChanged.InvokeAsync(_value); // force to control to refresh
		StateHasChanged();
	}
	void IDisposable.Dispose()
	{
		Session.EnvironmentHasChanged -= async (ChangeEvents e) => await OnRefresh(e);
	}
}






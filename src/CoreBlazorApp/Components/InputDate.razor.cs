using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class InputDate
{
	private DateTimeOffset _value;
	private DateTime? _date;
	[Parameter]
	public DateTimeOffset Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<DateTimeOffset>.Default.Equals(value, _value))
			{
				_value = value;
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Parameter]
	public EventCallback<DateTimeOffset> ValueChanged { get; set; }
	[Parameter] public Expression<Func<DateTimeOffset>> ValidateProperty { get; set; } = default!;
	[CascadingParameter] public EditContext? EditContext { get; set; } = default!;
	[Parameter] public string Label { get; set; } = "";

	[Parameter] public bool ValidateStrict { get; set; }
	[Parameter] public bool ReadOnly { get; set; }
	protected override async Task OnParametersSetAsync()
	{
		_date = Value.DateTime;
		await base.OnParametersSetAsync();
	}
	protected override void OnInitialized()
	{
		// adjust MaxLength

		if (EditContext != null && ValidateProperty != null && ValidateStrict)
		{
			var sl = Validate.GetAttributeIfExist<RangeAttribute>(EditContext.Model!, ((MemberExpression)ValidateProperty.Body).Member.Name);
			if (sl is { })
			{
			}
		}

	}

	void OnDateChange(DateTime? newDate)
	{
		_date= newDate;
		DateTime d = new DateTime(_date.Value.Year, _date.Value.Month, _date.Value.Day, Value.Hour, Value.Minute, 0);
		Value = new DateTimeOffset((DateTime)d);
	}

}
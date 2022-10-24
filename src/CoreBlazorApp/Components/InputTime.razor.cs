using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class InputTime
{
	private DateTimeOffset _value;
	private TimeSpan? _time;
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
		_time= Value.TimeOfDay;
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
	void OnTimeChange(TimeSpan? newTime)
	{
		_time = newTime;
		DateTime d = new DateTime(Value.Date.Year, Value.Date.Month, Value.Date.Day,_time.Value.Hours,_time.Value.Minutes,0);
		Value = new DateTimeOffset(d);
	}

}


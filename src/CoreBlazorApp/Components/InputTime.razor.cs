using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class InputTime:IDisposable
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
	[Parameter] public bool AmPm { get; set; }
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
		AmPm = Session.AmPm;
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
    protected override async Task OnAfterRenderAsync(bool FirstTime)
    {
        if (FirstTime)
        {
            Session.EnvironmentHasChanged += async (ChangeEvents e) => await OnRefresh(e);
            await OnRefresh(ChangeEvents.AmPm);
        }
    }
    private async Task OnRefresh(ChangeEvents e)
    {
        if (e == ChangeEvents.Login || e == ChangeEvents.AmPm || e == ChangeEvents.Lang || e == ChangeEvents.Init)
        {
            AmPm = Session.AmPm;
            StateHasChanged();
        }
    }

    void OnTimeChange(TimeSpan? newTime)
	{
		_time = newTime;
		DateTime d = new DateTime(Value.Date.Year, Value.Date.Month, Value.Date.Day,_time.Value.Hours,_time.Value.Minutes,0);
		Value = new DateTimeOffset(d);
	}
    void IDisposable.Dispose()
    {
        Session.EnvironmentHasChanged -= async (ChangeEvents e) => await OnRefresh(e);
    }
}


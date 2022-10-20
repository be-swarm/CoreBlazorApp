using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.Linq.Expressions;

namespace BeSwarm.CoreBlazorApp.Components;



public partial class Switch
{
	private bool _value;
	[Parameter]
	public bool Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(value, _value))
			{
				_value = value;
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	[Parameter] public Expression<Func<bool>>? ValidateProperty { get; set; }
	[CascadingParameter] public EditContext EditContext { get; set; } = default!;

	[Parameter]
	public EventCallback<bool> ValueChanged { get; set; }

	protected override void OnInitialized()
	{

	}
}






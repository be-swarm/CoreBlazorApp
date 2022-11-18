using BeSwarm.Validator;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BeSwarm.CoreBlazorApp.Components;

public partial class InputInteger
{
	private int _value;
	[Parameter]
	public int Value
	{
		get => _value;
		set
		{
			if (!EqualityComparer<int>.Default.Equals(value, _value))
			{
				_value = value;
				ValueChanged.InvokeAsync(value);
			}
		}
	}
	
	[Parameter] public EventCallback<int> ValueChanged { get; set; }
	[Parameter] public Expression<Func<int>> ValidateProperty { get; set; } = default!;
	[CascadingParameter] public EditContext? EditContext { get; set; } = default!;
	[Parameter] public string Label { get; set; } = "";
	[Parameter] public string Format { get; set; } = "D";

	[Parameter] public int Min { get; set; } = int.MinValue;
	[Parameter] public int Max { get; set; } = int.MaxValue;
	[Parameter] public bool ValidateStrict { get; set; }
	[Parameter] public bool ReadOnly { get; set; }
	[Parameter] public int Step { get; set; } = 1;
	protected override void OnInitialized()
	{
		// adjust MaxLength

		if (EditContext != null && ValidateProperty != null && ValidateStrict)
		{
			var sl = Validate.GetAttributeIfExist<RangeAttribute>(EditContext.Model!, ((MemberExpression)ValidateProperty.Body).Member.Name);
			if (sl is { })
			{
				Min = Int32.Parse(sl.Minimum.ToString() ?? string.Empty);
				Max = Int32.Parse(sl.Maximum.ToString() ?? string.Empty);

			}

		}

	}

}


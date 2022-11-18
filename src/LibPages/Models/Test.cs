using System.ComponentModel.DataAnnotations;

namespace LibPages.Models
{
	public class Test
	{
		[Required]
		[MaxLength(10), MinLength(4)]
		public string Name { get; set; } = default!;
		[Required]
		[MaxLength(10), MinLength(4)]
		public string FirstName { get; set; } = default!;

		[Range(1, 99)]
		public int Age { get; set; } =default!;

		[Range(50, 200)]
		public Decimal Weight { get; set; } = default!;

		public DateTimeOffset Date { get; set; } = default!;

		public Test()
		{
			Date = DateTimeOffset.Now;
		}

	}
}

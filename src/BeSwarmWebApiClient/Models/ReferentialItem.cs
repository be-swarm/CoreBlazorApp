using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BeSwarm.WebApi.Models;

public class DescriptionItem
{
	public DescriptionItem()
	{
		CodeLang = Description = "";
	}

	[JsonProperty("codelang", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[MinLength(1)]
	[MaxLength(2)]
	public string CodeLang { get; set; }

	[JsonProperty("description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(100)]
	public string Description { get; set; }
}

public class ReferentialItem
{
	[JsonProperty("uidreferential", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(50)]
	public string Uidreferential { get; set; }

	[JsonProperty("uid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(50)]
	public string Uid { get; set; }

	[JsonProperty("description", Required = Required.Always)]
	[Required]
	[MinLength(1)]
	[MaxLength(10)]
	public ICollection<DescriptionItem> Description { get; set; } = new Collection<DescriptionItem>();

	[JsonProperty("thumbnail", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(1024)]
	public string Thumbnail { get; set; }

	public string GetDescriptionInLang(string codelang)
	{
		foreach (var item in Description)
			if (item.CodeLang.ToUpper() == codelang.ToUpper())
				return item.Description;
		// not found but one lang at min
		if (Description.Count >= 1) return Description.First().Description;
		return "????";
	}
}

public class ReferentialItemLang
{
	[JsonProperty("uidreferential", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(50)]
	public string Uidreferential { get; set; }

	[JsonProperty("uid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(50)]
	public string Uid { get; set; }

	[JsonProperty("codelang", Required = Required.Always)]
	[Required]
	[StringLength(2, MinimumLength = 1)]
	public string Codelang { get; set; }

	[JsonProperty("description", Required = Required.Always)]
	[Required]
	[StringLength(100, MinimumLength = 1)]
	public string Description { get; set; }

	[JsonProperty("thumbnail", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(1024)]
	public string Thumbnail { get; set; }
}
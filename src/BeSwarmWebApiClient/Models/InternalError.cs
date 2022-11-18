using Newtonsoft.Json;

namespace BeSwarm.WebApi.Models;

public class InternalError
{
	[JsonProperty("errorCode", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public int ErrorCode { get; set; }

	[JsonProperty("description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	public string Description { get; set; }

	[JsonProperty("descriptionLang", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	public IDictionary<string, string> DescriptionLang { get; set; }

	[JsonProperty("function", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
	public string Function { get; set; }
}
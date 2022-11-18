using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.WebApi.Services
{

    public record Trace(DateTime Date, HttpStatusCode Status, Uri Uri, HttpMethod Method,string AdditionalInfo="");

    public interface  ITraceHttp
    {
        Task AddTrace(Trace t);
        Task<List<Trace>> GetTraces();
        Task DeleteTraces();
    }
}

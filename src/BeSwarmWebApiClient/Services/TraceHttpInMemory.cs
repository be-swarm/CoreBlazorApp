using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.WebApi.Services
{
    public class TraceHttpInMemory : ITraceHttp
    {
        private readonly List<Trace> _traces = new List<Trace>();
        public async Task AddTrace(Trace t)
        {
            _traces.Add(t);
            if (_traces.Count >= 500) _traces.RemoveAt(0);    // max last 500
        }

        public async Task DeleteTraces()
        {
           _traces.Clear();
        }

        public async Task<List<Trace>> GetTraces()
        {
			return _traces.OrderByDescending(x => x.Date).ToList(); ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Tact.Practices.LifetimeManagers.Attributes;

namespace Demo.Tact.Console.Services.Implementation
{
    [RegisterPerScope(typeof(IDemoService))]
    public class DemoService : IDemoService, IDisposable
    {
        private readonly IList<IThing> _things;

        public DemoService(IEnumerable<IThing> things)
        {
            _things = things.ToList();
        }

        public Guid Id { get; } = Guid.NewGuid();

        public bool IsDisposed { get; private set; }

        public IList<int> DemoAllOfTheThings()
        {
            return _things.Select(t => t.Number).ToList();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
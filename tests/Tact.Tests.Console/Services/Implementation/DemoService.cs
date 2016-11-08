using System;
using System.Collections.Generic;
using System.Linq;
using Tact.Configuration;
using Tact.Practices;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Tests.Console.Configuration;

namespace Tact.Tests.Console.Services.Implementation
{
    [RegisterPerScope(typeof(IDemoService)), Initialize]
    public class DemoService : IDemoService, IDisposable
    {
        public static string StaticString { get; private set; }

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
        
        public class InitializeAttribute : Attribute, IInitializeAttribute
        {
            public void Initialize(IContainer container)
            {
                var config = container.Resolve<DemoConfig>();
                StaticString = config.SomeString;
            }
        }
    }
}
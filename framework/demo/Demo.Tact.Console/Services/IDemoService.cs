using System;
using System.Collections.Generic;

namespace Tact.Tests.Console.Services
{
    public interface IDemoService
    {
        Guid Id { get; }

        bool IsDisposed { get; }

        IList<int> DemoAllOfTheThings();
    }
}
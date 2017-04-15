using System;
using System.Collections.Generic;

namespace Demo.Tact.Console.Services
{
    public interface IDemoService
    {
        Guid Id { get; }

        bool IsDisposed { get; }

        IList<int> DemoAllOfTheThings();
    }
}
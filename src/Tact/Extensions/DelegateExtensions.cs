using System;
using Tact.Reflection;

namespace Tact
{
    public static class DelegateExtensions
    {
        public static EfficientInvoker GetInvoker(this Delegate del)
        {
            return EfficientInvoker.ForDelegate(del);
        }
    }
}
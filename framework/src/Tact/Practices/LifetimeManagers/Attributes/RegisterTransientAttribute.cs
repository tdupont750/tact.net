﻿using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterTransientAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterTransientAttribute(Type fromType, string key = null)
        {
            _fromType = fromType;
            _key = key;
        }
        
        public void Register(IContainer container, Type toType)
        {
            container.RegisterTransient(_fromType, toType, _key);
        }
    }
}
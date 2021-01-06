using System;
using Microsoft.AspNetCore.Mvc;

namespace Tpk.DataServices.Server.Classes.Impl
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRequiredAttribute : TypeFilterAttribute
    {
        public AuthorizeRequiredAttribute(params string[] roles) : base(typeof(AuthorizeRequiredFilter))
        {
            Arguments = new object[] {roles};
        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ClaimRequiredAttribute : TypeFilterAttribute
    {
        public ClaimRequiredAttribute(string restrictItemName, string permissionName)
            : base(typeof(ClaimRequiredFilter))
        {
            var claim = TgClaimTypes.ToClaim(restrictItemName, permissionName);
            Arguments = new object[] {claim};
        }
    }
}
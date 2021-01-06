using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ContactTypeService : ServiceBase<ContactType>, IContactTypeService
    {
        public ContactTypeService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ContactTypeService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            ExceptColumns = new List<string> {"IsPermanent"};
        }
    }
}
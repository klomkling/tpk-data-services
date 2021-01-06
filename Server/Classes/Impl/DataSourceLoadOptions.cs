using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;

namespace Tpk.DataServices.Server.Classes.Impl
{
    [ModelBinder(BinderType = typeof(DataSourceLoadOptionsBinder))]
    public class DataSourceLoadOptions : DataSourceLoadOptionsBase
    {
        
    }
}
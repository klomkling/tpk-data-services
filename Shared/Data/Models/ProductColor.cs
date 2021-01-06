using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductColor : TgModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductUnit : TgModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsPermanent { get; set; }
    }
}
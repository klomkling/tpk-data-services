using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class MaterialType : TgModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
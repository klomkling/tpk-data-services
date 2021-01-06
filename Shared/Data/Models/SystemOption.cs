using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SystemOption : TgModelBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Stockroom : TgModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPermanent { get; set; }
    }
}
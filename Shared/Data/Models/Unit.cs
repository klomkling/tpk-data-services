using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Unit : TgModelBase
    {
        public string Name { get; set; }
        public int SectionId { get; set; }
    }
}
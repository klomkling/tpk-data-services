using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Section : TgModelBase
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
    }
}
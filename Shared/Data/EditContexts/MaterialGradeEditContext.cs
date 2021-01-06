using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class MaterialGradeEditContext : EditContextBase<MaterialSubType>
    {
        public MaterialGradeEditContext() : base(null)
        {
        }

        public MaterialGradeEditContext(MaterialSubType dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
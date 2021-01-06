using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class UserPermissionEditContext : EditContextBase<UserPermission>
    {
        public UserPermissionEditContext() : base(null)
        {
        }

        public UserPermissionEditContext(UserPermission dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            UserId = DataItem.UserId;
            ClaimTypeId = DataItem.ClaimTypeId;
            Permission = DataItem.Permission;
        }

        public int UserId { get; set; }
        public int ClaimTypeId { get; set; }
        public int Permission { get; set; }
    }
}
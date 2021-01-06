using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class UserEditContext : EditContextBase<User>
    {
        public UserEditContext() : base(null)
        {
        }

        public UserEditContext(User dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Username = DataItem.Username;
            Email = DataItem.Email;
            PasswordHash = DataItem.PasswordHash;
            RoleId = DataItem.RoleId;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
    }
}
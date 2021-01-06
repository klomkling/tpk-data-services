using Tpk.DataServices.Shared.Data.Constants;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgRoles : Enumeration
    {
        public static readonly TgRoles Anonymous = new TgRoles(0, RestrictRoles.Anonymous);
        public static readonly TgRoles User = new TgRoles(10, RestrictRoles.User);
        public static readonly TgRoles Supervisor = new TgRoles(30, RestrictRoles.Supervisor);
        public static readonly TgRoles Manager = new TgRoles(50, RestrictRoles.Manager);
        public static readonly TgRoles Director = new TgRoles(70, RestrictRoles.Director);
        public static readonly TgRoles Administrator = new TgRoles(90, RestrictRoles.Administrator);

        public TgRoles()
        {
        }

        private TgRoles(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
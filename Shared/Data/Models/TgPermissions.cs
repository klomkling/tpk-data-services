namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgPermissions : Enumeration
    {
        public static readonly TgPermissions None = new TgPermissions(0, "None");
        public static readonly TgPermissions View = new TgPermissions(1, "View");
        public static readonly TgPermissions Update = new TgPermissions(2, "Update");
        public static readonly TgPermissions Create = new TgPermissions(3, "Create");
        public static readonly TgPermissions Delete = new TgPermissions(4, "Delete");
        public static readonly TgPermissions FullAccess = new TgPermissions(5, "FullAccess");

        public TgPermissions()
        {
        }

        private TgPermissions(int id, string name) : base(id, name)
        {
        }
    }
}
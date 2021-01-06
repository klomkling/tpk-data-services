namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgOrderStatuses : StringEnumeration
    {
        public static readonly TgOrderStatuses New = new TgOrderStatuses("N", "New");
        public static readonly TgOrderStatuses Wait = new TgOrderStatuses("W", "Wait");
        public static readonly TgOrderStatuses Start = new TgOrderStatuses("S", "Start");
        public static readonly TgOrderStatuses ReadyToPickup = new TgOrderStatuses("R", "Ready to pickup");
        public static readonly TgOrderStatuses OnDelivery = new TgOrderStatuses("D", "On Delivery");
        public static readonly TgOrderStatuses Completed = new TgOrderStatuses("C", "Completed");
        public static readonly TgOrderStatuses Cancelled = new TgOrderStatuses("X", "Cancelled");

        public TgOrderStatuses()
        {
        }

        public TgOrderStatuses(string value, string name) : base(value, name)
        {
        }
    }
}
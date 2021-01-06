using Microsoft.AspNetCore.Components.Web;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgWarningModalEventArgs
    {
        public bool IsConfirm { get; set; }
        public MouseEventArgs MouseEventArgs { get; set; }
    }
}
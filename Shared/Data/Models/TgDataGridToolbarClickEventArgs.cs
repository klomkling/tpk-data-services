using Microsoft.AspNetCore.Components.Web;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgDataGridToolbarClickEventArgs
    {
        public string Name { get; set; }
        public MouseEventArgs MouseEventArgs { get; set; }
    }
}
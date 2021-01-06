using System;

namespace Tpk.DataServices.Shared.Interfaces
{
    public interface IEditContextBase<T>
    {
        int Id { get; set; }
        T DataItem { get; set; }
        
        Action StateHasChanged { get; set; } 
    }
}
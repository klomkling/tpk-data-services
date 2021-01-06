using System;
using Tpk.DataServices.Shared.Interfaces;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class EditContextBase<T> : IEditContextBase<T>
    {
        public EditContextBase(T dataItem)
        {
            dataItem ??= (T) Activator.CreateInstance(typeof(T));
            DataItem = dataItem;
        }

        public int Id { get; set; }
        public T DataItem { get; set; }
        public Action StateHasChanged { get; set; }
    }
}
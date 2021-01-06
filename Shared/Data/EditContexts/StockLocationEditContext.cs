using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class StockLocationEditContext : EditContextBase<StockLocation>
    {
        public StockLocationEditContext() : base(null)
        {
        }

        public StockLocationEditContext(StockLocation dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Building = DataItem.Building;
            Shelf = DataItem.Shelf;
            Position = DataItem.Position;
            IsTemporaryLocation = DataItem.IsTemporaryLocation;
            IsPermanent = DataItem.IsPermanent;
        }

        public string Building { get; set; }
        public string Shelf { get; set; }
        public string Position { get; set; }
        public bool IsTemporaryLocation { get; set; }
        public bool IsPermanent { get; set; }

        [NotMapped]
        public string IsTemp
        {
            get => IsTemporaryLocation ? "Temporary" : "Normal";
            set => IsTemporaryLocation =
                value.Equals("Temporary", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
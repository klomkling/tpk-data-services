using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class StockLocation : TgModelBase
    {
        public string Building { get; set; }
        public string Shelf { get; set; }
        public string Position { get; set; }
        public bool IsTemporaryLocation { get; set; }
        public bool IsPermanent { get; set; }

        [NotMapped] public string Location { get; set; }
    }
}
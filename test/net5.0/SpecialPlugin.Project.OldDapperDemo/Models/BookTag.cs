using System;
using System.ComponentModel.DataAnnotations;

namespace SpecialPlugin.Project.OldDapperDemo.Models
{
    public class BookTag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(36)]
        public string LibraryCode { get; set; }

        [Required, MaxLength(36)]
        public string Uid { get; set; }

        [Required, MaxLength(36)]
        public string Barcode { get; set; }

        [MaxLength(200)]
        public string BarcodeRaw { get; set; }

        [MaxLength(200)]
        public string UserRaw { get; set; }

        [ConcurrencyCheck]
        public DateTime? Updated { get; set; }

        public DateTime Created { get; set; }
    }
}

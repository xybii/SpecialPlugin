using System;
using System.ComponentModel.DataAnnotations;

namespace SpecialPlugin.Project.OldDapperDemo.Dtos
{
    public class BookTagDto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(36)]
        public string LibraryCode { get; set; }

        [Required, MaxLength(36)]
        public string Barcode { get; set; }

        public DateTime Created { get; set; }
    }
}

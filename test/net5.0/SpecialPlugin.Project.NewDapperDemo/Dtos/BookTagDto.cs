using System;
using System.ComponentModel.DataAnnotations;

namespace SpecialPlugin.Project.NewDapperDemo.Dtos
{
    public class BookTagDto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(36)]
        public string LibraryCode { get; set; }

        [Required, MaxLength(36)]
        public string Uid { get; set; }

        public DateTime Created { get; set; }
    }
}

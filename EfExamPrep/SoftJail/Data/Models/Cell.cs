using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Cell
    {
        //In case of a failed test check foreign keys and check if collections are required

        public Cell()
        {
            this.Prisoners = new HashSet<Prisoner>();
        }

        [Key]
        public int Id { get; set; }

        [Range(1, 1000)]
        [Required]
        public int CellNumber { get; set; }

        [Required]
        public bool HasWindow { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        [Required]
        public Department Department { get; set; }
        //DepartmentId - integer, foreign key
        //Department – the cell's department (required)
        public ICollection<Prisoner> Prisoners { get; set; }
        //Prisoners - collection of type Prisoner
    }
}

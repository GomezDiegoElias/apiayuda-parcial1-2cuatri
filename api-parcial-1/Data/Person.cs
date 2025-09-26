using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_parcial_1.Data
{
    [Table("tbl_person")]
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("dni")]
        public long Dni { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; }

        public Person() { }

        public Person(long dni, string firstName, string lastName)
        {
            Dni = dni;
            FirstName = firstName;
            LastName = lastName;
        }

        // ToString
        public override string ToString()
        {
            return $"Persona [ID: {Id}, DNI: {Dni}, Nombre: {FirstName}, Apellido: {LastName}]";
        }

    }
}

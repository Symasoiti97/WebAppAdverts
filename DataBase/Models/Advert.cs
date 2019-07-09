using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    public class Advert : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
    }
}

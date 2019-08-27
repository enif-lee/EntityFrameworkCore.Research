using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Study.Domain.Common
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
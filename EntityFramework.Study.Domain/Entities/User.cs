using System.ComponentModel.DataAnnotations;
using EntityFramework.Study.Domain.Common;

namespace EntityFramework.Study.Domain.Entities
{
    public class User : EntityBase, IStatus
    {
        [MaxLength(255)]
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public Status Status { get; set; }
    }
}
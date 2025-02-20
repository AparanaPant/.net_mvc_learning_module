using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models
{
    public class School
    {
        public int SchoolID { get; set; }

        [Required]
        [StringLength(100)]
        public string SchoolName { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public ICollection<SchoolAddress> SchoolAddresses { get; set; }
        public ICollection<UserSchool> UserSchools { get; set; }
    }

    public class SchoolAddress
    {
        [Key]
        public int AddressID { get; set; }
        public int SchoolID { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string AddressLine2 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(20)]
        public string ZIPCode { get; set; }

        public School School { get; set; }
    }

    public class UserSchool
    {
        public int UserSchoolID { get; set; }
        public string UserID { get; set; } // Assuming UserID is a string (from IdentityUser)
        public int SchoolID { get; set; }

        public ApplicationUser User { get; set; }
        public School School { get; set; }
    }

}

using LeadWomb.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeadWomb.Models
{
    public class ApplicationUser : ITokeneable
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        
        public long CompanyId { get; set; }


        

        // Summary:
        //     Used to record failures for the purposes of lockout
        public virtual int AccessFailedCount { get; set; }
        //
        // Summary:
        //     Navigation property for user claims
        //public virtual ICollection<TClaim> Claims { get; }
        //
        // Summary:
        //     Email
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name="Confirm Email")]
      
        // Summary:
        //     True if the email is confirmed, default is false
        public virtual bool EmailConfirmed { get; set; }
        
        // Summary:
        //     User ID (Primary Key)
        public virtual string Id { get; set; }
        //
        // Summary:
        //     Is lockout enabled for this user
        public virtual bool LockoutEnabled { get; set; }
        //
        // Summary:
        //     DateTime in UTC when lockout ends, any time in the past is considered not
        //     locked out.
       public virtual DateTime? LockoutEndDateUtc { get; set; }
        //
        // Summary:
        //     Navigation property for user logins
        //public virtual ICollection<TLogin> Logins { get; }
        //
        // Summary:
        //     The salted/hashed form of the user password
        //public virtual string Password { get; set; }
        //
        // Summary:
        //     PhoneNumber for the user
        public virtual string PhoneNumber { get; set; }
        //
        // Summary:
        //     True if the phone number is confirmed, default is false
        public virtual bool PhoneNumberConfirmed { get; set; }
        //
        // Summary:
        //     Navigation property for user roles
        //public virtual ICollection<TRole> Roles { get; }
        //
        // Summary:
        //     A random value that should change whenever a users credentials have changed
        //     (password changed, login removed)
        public virtual string SecurityStamp { get; set; }
        //
        // Summary:
        //     Is two factor enabled for the user
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// assigned users
        /// </summary>
        public List<AssignedUser> AssignedUsers { get; set; }
        //
        // Summary:
        //     User name
        
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName{get;set;}
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName{get;set;}
        /// <summary>
        /// Created date time
        /// </summary>
        public DateTime CreatedDateTime{get;set;}
        /// <summary>
        /// Role of user
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Project of user
        /// </summary>
        public Project Project { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
    }

   
}
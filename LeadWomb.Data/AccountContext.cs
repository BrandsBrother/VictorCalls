using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Model;
using LeadWomb.Data.AccountAdapterTableAdapters;
using LeadWomb.Models;
namespace LeadWomb.Data
{
    public class AccountContext
    {
        QueriesTableAdapter tableAdapter = new QueriesTableAdapter();
        public string CreateUser(ApplicationUser user)
        {
            
           object id = tableAdapter.Sp_CreateAspNetUser(user.UserName,user.Password,user.SecurityStamp,user.Email,user.EmailConfirmed,
               user.PhoneNumber,user.PhoneNumberConfirmed,user.TwoFactorEnabled,user.LockoutEndDateUtc==DateTime.MinValue?null:user.LockoutEndDateUtc,user.LockoutEnabled,
                user.AccessFailedCount,user.FirstName,user.LastName,DateTime.Now,user.CompanyId,user.RoleId);
           return id.ToString();
        }
        public void UpdateUser(ApplicationUser user)
        {

           tableAdapter.Sp_UpdateAspNetUser(user.Id ,user.UserName, user.Password, user.SecurityStamp, user.Email, user.EmailConfirmed,
                 user.PhoneNumber, user.PhoneNumberConfirmed, user.TwoFactorEnabled, user.LockoutEndDateUtc, user.LockoutEnabled,
                 user.AccessFailedCount, user.FirstName, user.LastName, user.CreatedDateTime, user.CompanyId, user.RoleId);
           
        }

        public ApplicationUser Login(string userName, string password)
        {
            ApplicationUser user = null;
            sp_loginTableAdapter tableAdapter = new sp_loginTableAdapter();
            AccountAdapter.sp_loginDataTable table = tableAdapter.GetUser(userName, password);
            if (table != null && table.Rows.Count > 0)
            {
                user = new ApplicationUser();
                foreach (AccountAdapter.sp_loginRow row in table.Rows)
                {
                    user.AccessFailedCount = row.AccessFailedCount;
                    user.CompanyId = row.CompanyId;
                    user.CreatedDateTime = row.CreatedDateTime;
                    user.Email = row.IsEmailNull()?null: row.Email;
                    user.FirstName = row.FirstName;
                    user.LastName = row.LastName;
                    user.LockoutEnabled =  row.LockoutEnabled;
                    user.LockoutEndDateUtc = row.IsLockoutEndDateUtcNull()?(DateTime?)null: row.LockoutEndDateUtc;
                    user.Password = row.PasswordHash;
                    user.PhoneNumber = row.IsPhoneNumberNull()?null: row.PhoneNumber;
                    user.PhoneNumberConfirmed =  row.PhoneNumberConfirmed;
                    user.RoleId = row.RoleId;
                    user.SecurityStamp = row.IsSecurityStampNull()?null: row.SecurityStamp;
                    user.TwoFactorEnabled = row.TwoFactorEnabled;
                    user.UserName = row.UserName;
                    user.Id = row.Id;

                }
            }
            return user;
        }

        public List<ApplicationUser> GetUsersByCompanyId(long companyId)
        {
            List<ApplicationUser> users = null;
            Sp_searchAspNetUserBy_Company_IdTableAdapter adapter = new Sp_searchAspNetUserBy_Company_IdTableAdapter();
            AccountAdapter.Sp_searchAspNetUserBy_Company_IdDataTable  table = adapter.GetUsersByCompanyId(companyId);
            if (table != null && table.Rows.Count > 0)
            {
                users = new List<ApplicationUser>();
               foreach(AccountAdapter.Sp_searchAspNetUserBy_Company_IdRow row in table.Rows)
               {
                   ApplicationUser user = new ApplicationUser();
                   user.Id =  row.Id;
                   user.FirstName = row.FirstName;
                   user.AccessFailedCount = row.AccessFailedCount;
                   user.CompanyId = row.CompanyId;
                   user.CreatedDateTime = row.CreatedDateTime;
                   user.Email = row.IsEmailNull()? string.Empty:user.Email;
                   user.LastName = row.LastName;
                   user.LockoutEnabled = row.LockoutEnabled;
                   user.LockoutEndDateUtc = row.IsLockoutEndDateUtcNull() ? (DateTime?)null : row.LockoutEndDateUtc;
                   user.PhoneNumber = row.PhoneNumber;
                   user.RoleId = row.RoleId;
                   user.SecurityStamp = row.SecurityStamp;
                   user.TwoFactorEnabled = row.TwoFactorEnabled;
                   user.UserName = row.UserName;
                   users.Add(user);
               }
            }
            return users;
        }

        
    }
}

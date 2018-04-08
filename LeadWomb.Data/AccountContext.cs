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
        public List<ApplicationUser> GetUsersOfCompany(string userName, string roleName)
        {
            List<ApplicationUser> users = null;
            sp_GetUsersOfCompanyandRoleIDTableAdapter adapter = new sp_GetUsersOfCompanyandRoleIDTableAdapter();
            AccountAdapter.sp_GetUsersOfCompanyandRoleIDDataTable table = adapter.GetUsersOfCompany(userName, roleName);
            if (table != null && table.Rows.Count > 0)
            {
                users = new List<ApplicationUser>();
                foreach (AccountAdapter.sp_GetUsersOfCompanyandRoleIDRow row in table.Rows)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Id = row.Id;
                    user.FirstName = row.FirstName;
                    user.AccessFailedCount = row.AccessFailedCount;
                    user.CompanyId = row.CompanyId;
                    user.CreatedDateTime = row.CreatedDateTime;
                    user.Email = row.IsEmailNull() ? string.Empty : user.Email;
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

        //public LeadStatusCounts GetStatusCountsByUserName(string userName)
        //{
        //    LeadStatusCounts statusCounts = null;
        //    Get tableAdapter = new sp_loginTableAdapter();
        //    AccountAdapter.sp_loginDataTable table = tableAdapter.GetStatusCountsByUserName(userName);
        //    if (table != null && table.Rows.Count > 0)
        //    { 
        //       foreach(AccountAdapter.)
        //    }
        //    return statusCounts;
        //}

        public LeadStatusCounts GetStatusCountsByUserName(string userName)
        {
            LeadStatusCounts user = new LeadStatusCounts();
            sp_GetStatusCountByUserNameTableAdapter tableAdapter = new sp_GetStatusCountByUserNameTableAdapter();
            AccountAdapter.sp_GetStatusCountByUserNameDataTable table = tableAdapter.GetStatusCountByUserName(userName);
            int rowCount = 0;
            foreach(AccountAdapter.sp_GetStatusCountByUserNameRow row in table.Rows)
            {
                switch (rowCount)
                { 
                    case 0:
                        user.CurrentLeadsCount = row.PhoneNumberCount;
                        break;
                    case 1:
                        user.NoWorkCount = row.PhoneNumberCount;
                        break;
                    case 2:
                        user.NoWorkCount = row.PhoneNumberCount;
                        break;
                    case 3:
                        user.FollowUpsCount = row.PhoneNumberCount;
                        break;
                    case 4:
                        user.VisitOnCounts = row.PhoneNumberCount;
                        break;
                    case 5:
                        user.VisitDoneCount = row.PhoneNumberCount;
                        break;
                    case 6:
                        user.VisitDeadCount = row.PhoneNumberCount;
                        break;
                    case 7:
                        user.OtherProjectsCount = row.PhoneNumberCount;
                        break;
                    case 8:
                        user.ResaleCount = row.PhoneNumberCount;
                        break;
                    case 9:
                        user.AlreadyBookedCount = row.PhoneNumberCount;
                        break;
                    case 10:
                        user.BookedDone = row.PhoneNumberCount;
                        break;
                    case 11:
                        user.DeadCount = row.PhoneNumberCount;
                        break;
                    case 12:
                        user.RentCount = row.PhoneNumberCount;
                        break;
                    case 13:
                        user.PlotCount = row.PhoneNumberCount;
                        break;
                    case 14:
                        user.DuplicateCount = row.PhoneNumberCount;
                        break;
                }
                    
                rowCount++;
                
                     
            }
            return user;
        }

        
    }
}

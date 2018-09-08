using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Model;
using LeadWomb.Models;

namespace LeadWomb.Data
{
    public class AccountMONGOContext : IAccountContext
    {
        public bool AddToken(string userName, string token)
        {
            throw new NotImplementedException();
        }

        public bool CreateCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public void CreateDocument(Document document)
        {
            throw new NotImplementedException();

        }

        public void CreateIntegration(Integration integration)
        {
            throw new NotImplementedException();
        }

        public bool CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public string CreateUser(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public string DecodeFrom64(string encodedData)
        {
            throw new NotImplementedException();
        }

        public object DeleteUserByUserID(string userID)
        {
            throw new NotImplementedException();
        }

        public string EncodePasswordToBase64(string password)
        {
            throw new NotImplementedException();
        }

        public List<Company> GetCompanies()
        {
            throw new NotImplementedException();
        }

        public Company GetCompanyByCompanyId(long companyId)
        {
            throw new NotImplementedException();
        }

        public List<Integration> GetCompanyIntegrations()
        {
            throw new NotImplementedException();
        }

        public Document GetDocument(int projectID, int documentID)
        {
            throw new NotImplementedException();
        }

        public List<Document> GetDocuments(int projectID)
        {
            throw new NotImplementedException();
        }

        public List<Document> GetDocuments(string userName)
        {
            throw new NotImplementedException();
        }

        public Integration GetIntegrationByID(int id)
        {
            throw new NotImplementedException();
        }

        public List<Integration> GetIntegrations(long CompanyID)
        {
            throw new NotImplementedException();
        }

        public List<Project> GetProjects(int companyID)
        {
            throw new NotImplementedException();
        }

        public List<Project> GetProjects(string userName)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationUser> GetProjectUsers(int projectId)
        {
            throw new NotImplementedException();
        }

        public List<Role> GetRoles()
        {
            throw new NotImplementedException();
        }

        public LeadStatusCounts GetStatusCountsByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationUser> GetUsersbyCompany(int CompanyId, string roleID)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationUser> GetUsersByCompanyId(long companyId)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationUser> GetUsersOfCompany(string userName, string roleName)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public void UpdateIntegration(Integration integration)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}

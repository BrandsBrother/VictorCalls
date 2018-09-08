using System.Collections.Generic;
using LeadWomb.Model;
using LeadWomb.Models;

namespace LeadWomb.Data
{
    public interface IAccountContext
    {
        bool AddToken(string userName, string token);
        bool CreateCompany(Company company);
        void CreateDocument(Document document);
        void CreateIntegration(Integration integration);
        bool CreateProject(Project project);
        string CreateUser(ApplicationUser user);
        string DecodeFrom64(string encodedData);
        object DeleteUserByUserID(string userID);
        string EncodePasswordToBase64(string password);
        List<Company> GetCompanies();
        Company GetCompanyByCompanyId(long companyId);
        List<Integration> GetCompanyIntegrations();
        Document GetDocument(int projectID, int documentID);
        List<Document> GetDocuments(int projectID);
        List<Document> GetDocuments(string userName);
        Integration GetIntegrationByID(int id);
        List<Integration> GetIntegrations(long CompanyID);
        List<Project> GetProjects(int companyID);
        List<Project> GetProjects(string userName);
        List<ApplicationUser> GetProjectUsers(int projectId);
        List<Role> GetRoles();
        LeadStatusCounts GetStatusCountsByUserName(string userName);
        ApplicationUser GetUser(string username);
        List<ApplicationUser> GetUsersbyCompany(int CompanyId, string roleID);
        List<ApplicationUser> GetUsersByCompanyId(long companyId);
        List<ApplicationUser> GetUsersOfCompany(string userName, string roleName);
        ApplicationUser Login(string userName, string password);
        bool UpdateCompany(Company company);
        void UpdateIntegration(Integration integration);
        void UpdateUser(ApplicationUser user);
    }
}
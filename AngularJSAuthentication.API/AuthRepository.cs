using AngularJSAuthentication.API.Entities;
using AngularJSAuthentication.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web; 
using LeadWomb.Data;
using LeadWomb.Models;
using LeadWomb.Model;
namespace AngularJSAuthentication.API
{

    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        private AccountContext context = null;
        public AuthRepository()
        {
            context = new AccountContext();
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));

        }
        public void CreateIntegration(Integration integration)
        {
            context.CreateIntegration(integration);
        }
        public void UpdateIntegration(Integration integration)
        {
            context.UpdateIntegration(integration);
        }
        public List<Integration> GetCompanyIntegrations()
        {
            return context.GetCompanyIntegrations();
        }
        public bool AddToken(string userName, string token)
        {
            return context.AddToken(userName, token);
        }
        public List<Integration> GetIntegrations(long companyID)
        {
            return context.GetIntegrations(companyID);
        }

        public Integration GetIntegrationByID(int integrationID)
        {
            return context.GetIntegrationByID(integrationID);

        }

        public void CreateDocument(Document document)
        {
            context.CreateDocument(document);
        }
        public bool CreateProject(Project project)
        {
            return context.CreateProject(project);
        }
        public bool AddCompany(Company company)
        {
            return context.CreateCompany(company);
        }
        public bool UpdateCompany(long companyId, Company company)
        {
            company.CompanyId = companyId;
            return context.UpdateCompany(company);
        }
        public List<Company> GetCompanies()
        {
            return context.GetCompanies();
        }
        public Company GetCompanybyCompanyId(long companyId)
        {
            return context.GetCompanyByCompanyId(companyId);
        }
        public Document GetDocument(int projectID, int documentID)
        {
          return context.GetDocument(projectID, documentID);
        }
        public List<Document> GetDocuments(string userName)
        {
           return context.GetDocuments(userName);
        }
        public List<Document> GetProjectDocuments(int projectID)
        {
            return context.GetDocuments(projectID);
        }
        public List<ApplicationUser> GetProjectUsers(int projectId)
        {
            return context.GetProjectUsers(projectId);
        }

        public string RegisterUser(ApplicationUser userModel)
        {
           return context.CreateUser(userModel);

       
        }

        public void UpdateUser(ApplicationUser userModel)
        {
            context.UpdateUser(userModel);
        }
        public List<Role> GetRoles()
        {
            List<Role> roles = context.GetRoles();
            roles.RemoveAll(x=>x.Name.ToLower().Equals("superadmin"));
            return roles;
        }
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }
        public ApplicationUser ValidateUser(string userName, string password)
        {
            ApplicationUser user = context.Login(userName, password);

            return user;
        }
        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }
        public void DeleteUser(string userID)
        {
            context.DeleteUserByUserID(userID);
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

           var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

           if (existingToken != null)
           {
             var result = await RemoveRefreshToken(existingToken);
           }
          
            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
           var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

           if (refreshToken != null) {
               _ctx.RefreshTokens.Remove(refreshToken);
               return await _ctx.SaveChangesAsync() > 0;
           }

           return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
             return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
             return  _ctx.RefreshTokens.ToList();
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
        public List<ApplicationUser> GetUsersbyCompany(int CompanyId, string roleID)
        {
            return context.GetUsersbyCompany(CompanyId, null);
        }
        public List<ApplicationUser> GetUsersOfCompany(string userName, string roleID)
        {
           return context.GetUsersOfCompany(userName, null);
        }
        public ApplicationUser GetUser(string username)
        {
            return context.GetUser(username);
        }
        public List<Project> GetProjects(string userName)
        {
           return context.GetProjects(userName);
        }
        public List<Project> GetProjects(int companyID)
        {
            return context.GetProjects(companyID);
        }
        public List<ApplicationUser> GetCompanyUsers(long companyID)
        {
            return context.GetUsersByCompanyId(companyID);
        }
    }
}
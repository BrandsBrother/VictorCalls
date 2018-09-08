using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Model;
using LeadWomb.Data.AccountAdapterTableAdapters;
using LeadWomb.Models;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace LeadWomb.Data
{
    /// <summary>
    /// This class is for SQL connection
    /// </summary>
    public class AccountSQLContext : IAccountContext
    {
        private const string APILINK = "http://api.victorcalls.com/api/Project/{0}/Document/{1}";
        QueriesTableAdapter tableAdapter = new QueriesTableAdapter();
        private SqlConnection connection = null;
        public string CreateUser(ApplicationUser user)
        {
            
           object id = tableAdapter.Sp_CreateAspNetUser(user.UserName,user.Password,user.SecurityStamp,user.Email,user.EmailConfirmed,
               user.PhoneNumber,user.PhoneNumberConfirmed,user.TwoFactorEnabled,user.LockoutEndDateUtc==DateTime.MinValue?null:user.LockoutEndDateUtc,user.LockoutEnabled,
                user.AccessFailedCount,user.FirstName,user.LastName,DateTime.Now,user.CompanyId,user.Role.RoleID,user.Project.ProjectId);
           return id.ToString();
        }
        public void UpdateUser(ApplicationUser user)
        {

           tableAdapter.Sp_UpdateAspNetUser(user.Id ,user.UserName, user.Password, user.SecurityStamp, user.Email, user.EmailConfirmed,
                 user.PhoneNumber, user.PhoneNumberConfirmed, user.TwoFactorEnabled, user.LockoutEndDateUtc, user.LockoutEnabled,
                 user.AccessFailedCount, user.FirstName, user.LastName, user.CreatedDateTime, user.CompanyId, user.Role.RoleID,user.Project.ProjectId);
           
        }
        public Document GetDocument(int projectID,int documentID)
        {
            Document document = null;
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetDocument";
            command.Parameters.Add(new SqlParameter("@projectID", projectID));
            command.Parameters.Add(new SqlParameter("@documentID", documentID));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    document = new Document();
                    document.DocumentID = Convert.ToInt32(row["Id"]);
                    document.ProjectID = Convert.ToInt32(row["ProjectId"]);
                    document.Name = row["Name"].ToString();
                    document.Link = string.Format(APILINK, document.ProjectID, document.DocumentID);
                }
            }
            return document;
        }
        public List<ApplicationUser> GetUsersbyCompany(int CompanyId, string roleID)
        {
            List<ApplicationUser> users = null;
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetUsersbyCompanyandRoleID";
            command.Parameters.Add(new SqlParameter("@CompanyId", CompanyId));
            command.Parameters.Add(new SqlParameter("@roleID", roleID));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    users = new List<ApplicationUser>();
                    foreach (DataRow row in table.Rows)
                    {
                        ApplicationUser user = new ApplicationUser();
                        user.Role = new Role();
                        user.Project = new Project();
                        user.Id = row["Id"].ToString();
                        user.FirstName = row["FirstName"].ToString();
                        user.AccessFailedCount = Convert.ToInt32(row["AccessFailedCount"]);
                        user.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        user.CreatedDateTime = Convert.ToDateTime(row["CreatedDateTime"]);
                        user.Email = row["Email"] == DBNull.Value ? string.Empty : row["Email"].ToString();
                        user.LastName = row["LastName"].ToString();
                        user.LockoutEnabled = Convert.ToBoolean(row["LockoutEnabled"]);
                        user.LockoutEndDateUtc = row["LockoutEndDateUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LockoutEndDateUtc"]);
                        user.PhoneNumber = row["PhoneNumber"].ToString();
                        user.Project.ProjectId = row["ProjectId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProjectId"]);
                        user.Role.RoleID = row["RoleId"].ToString();
                        //user.Role.Name = row["Name"].ToString();
                        user.SecurityStamp = row["SecurityStamp"] == DBNull.Value ? string.Empty : row["SecurityStamp"].ToString();
                        user.TwoFactorEnabled = Convert.ToBoolean(row["TwoFactorEnabled"]);
                        user.UserName = row["UserName"].ToString();
                        user.Token = row["Token"] != DBNull.Value ? row["Token"].ToString() : string.Empty;

                        users.Add(user);
                    }
                }
            }
            return users;


        }
        public ApplicationUser GetUser(string username)
        {
            ApplicationUser user = null;
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_GetUser";
            command.Parameters.AddWithValue("@username", username);
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)

            {
                user = new ApplicationUser();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                  user.CompanyId =  Convert.ToInt64(row["CompanyId"]);
                  user.Id =  row["Id"].ToString();
                }
        }
            return user;
        }
          

        public bool CreateCompany(Company company)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "AddCompany";
            command.Parameters.Add(new SqlParameter("@CompanyName",company.CompanyName));
            command.Parameters.Add(new SqlParameter("@Email", company.Email));
            command.Parameters.Add(new SqlParameter("@Phone", company.Email));
            command.Parameters.Add(new SqlParameter("@CompanyAddress",company.CompanyAddress));
            command.Parameters.Add(new SqlParameter("@City",company.City));
            command.Parameters.Add(new SqlParameter("@State", company.State));
            command.Parameters.Add(new SqlParameter("@Country", company.Country));
            command.Parameters.Add(new SqlParameter("@ContactPersonName", company.ContactPersonName));
            command.Parameters.Add(new SqlParameter("@ContactPhone", company.ContactPhone));
            command.Parameters.Add(new SqlParameter("@ContactEmail", company.ContactEmail));
            command.Parameters.Add(new SqlParameter("@ActivatedTill", company.ActivatedTill));
            command.Parameters.Add(new SqlParameter("@IsActivated", company.IsActivated));
            command.Parameters.Add(new SqlParameter("@LogoPath", company.Logopath));
            command.Parameters.Add(new SqlParameter("@CompanyType", company.CompanyType));
            //command.Parameters.Add(new SqlParameter(Project.DB_DISTRICT, project.District));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            return true;

        }
        public bool AddToken(string userName,string token)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_AddToken";
            command.Parameters.Add(new SqlParameter("@UserName", userName));
            command.Parameters.Add(new SqlParameter("@Token",token));          
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            return true;

        }
        public bool CreateProject(Project project)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_CreateProject";
            command.Parameters.Add(new SqlParameter(Project.DB_COMPANYID, project.CompanyId));
            command.Parameters.Add(new SqlParameter(Project.DB_LATTITUDE, project.Lattitude));
            command.Parameters.Add(new SqlParameter(Project.DB_LONGITUDE, project.Longitude));
            command.Parameters.Add(new SqlParameter(Project.DB_PROJECT_NAME, project.ProjectName));
            command.Parameters.Add(new SqlParameter(Project.DB_PRO_DESCRIPTION, project.Description));
            command.Parameters.Add(new SqlParameter(Project.DB_DISTRICT, project.District));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            return true;

        }
        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        } //this function Convert to Decord your Password
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        public void CreateIntegration(Integration integration)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_CreateIntegration";
            //command.Parameters.Add(new SqlParameter("@ID", integration.ID));
            command.Parameters.Add(new SqlParameter("@CompanyId", integration.CompanyId));
            command.Parameters.Add(new SqlParameter("@SourceID", integration.SourceType.ID));
            command.Parameters.Add(new SqlParameter("@Key", EncodePasswordToBase64(integration.Key)));
            command.Parameters.Add(new SqlParameter("@Value", EncodePasswordToBase64(integration.Value)));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void UpdateIntegration(Integration integration)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_UpdateIntegration";
            command.Parameters.Add(new SqlParameter("@ID", integration.ID));
            command.Parameters.Add(new SqlParameter("@CompanyId", integration.CompanyId));
            command.Parameters.Add(new SqlParameter("@SourceID", integration.SourceType.ID));
            command.Parameters.Add(new SqlParameter("@Key", EncodePasswordToBase64(integration.Key)));
            command.Parameters.Add(new SqlParameter("@Value", EncodePasswordToBase64(integration.Value)));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public List<Integration> GetIntegrations(long CompanyID)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_GetIntegrations";
            command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            List<Integration> integrations = null;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                integrations = new List<Integration>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Integration integration = new Integration();
                    integration.ID = Convert.ToInt32(row["ID"]);
                    integration.Key = DecodeFrom64(row["IntegrationKey"].ToString());
                    integration.Value = DecodeFrom64(row["IntegrationValue"].ToString());
                    integration.CompanyId = Convert.ToInt64(row["CompanyID"]);
                    integration.SourceType = new LeadSourceType();
                    integration.SourceType.ID = Convert.ToInt32(row["SourceID"]);
                    integrations.Add(integration);

                }
            }
            return integrations;
        }
        public Integration GetIntegrationByID(int id)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_GetIntegrationByID";
            command.Parameters.Add(new SqlParameter("@ID", id));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            Integration integration = null;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    integration = new Integration();
                    integration.ID = Convert.ToInt32(row["ID"]);
                    integration.Key = DecodeFrom64(row["IntegrationKey"].ToString());
                    integration.Value = DecodeFrom64(row["IntegrationValue"].ToString());
                    integration.CompanyId = Convert.ToInt64(row["CompanyID"]);
                    integration.SourceType = new LeadSourceType();
                    integration.SourceType.ID = Convert.ToInt32(row["SourceID"]);

                }
            }
            return integration;
        }
        public List<Integration> GetCompanyIntegrations()
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_GetCompanyIntegrations";
         
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            List<Integration> integrations = null;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                integrations = new List<Integration>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Integration integration = new Integration();
                    integration.ID = Convert.ToInt32(row["ID"]);
                    integration.Key = DecodeFrom64(row["IntegrationKey"].ToString());
                    integration.Value = DecodeFrom64(row["IntegrationValue"].ToString());
                    integration.CompanyId = Convert.ToInt64(row["CompanyID"]);
                    integration.SourceType = new LeadSourceType();
                    integration.SourceType.ID = Convert.ToInt32(row["SourceID"]);
                    integration.SourceType.Name = row["SOURCE"].ToString();
                    integrations.Add(integration);
                }
            }
            return integrations;
        }

        public Company GetCompanyByCompanyId(long companyId)
        {
            Company company = null;
            using (SqlConnection connection = new
               SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GetCompanyByCompanyId";
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.Parameters.Add(new SqlParameter(Project.DB_COMPANYID, companyId));
                command.Connection.Open();
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                   
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                       company = new Company();
                        company.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        company.CompanyName = row["CompanyName"].ToString();
                        company.Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : string.Empty;
                        company.Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : string.Empty;
                        company.CompanyAddress = row["CompanyAddress"] != DBNull.Value ? row["CompanyAddress"].ToString() : string.Empty;
                        company.City = row["City"] != DBNull.Value ? row["City"].ToString() : string.Empty;
                        company.ActivatedTill = row["ActivatedTill"] != DBNull.Value ? DateTimeOffset.Parse(row["ActivatedTill"].ToString()).DateTime : DateTime.MaxValue;
                        company.IsActivated = row["IsActivated"] != DBNull.Value ? Convert.ToBoolean(row["IsActivated"]) : true;
                        company.State = row["State"] != DBNull.Value ? Convert.ToString(row["State"]) : string.Empty;
                        company.Country = row["Country"] != DBNull.Value ? Convert.ToString(row["Country"]) : string.Empty;
                        company.CompanyType = row["CompanyType"] != DBNull.Value ? Convert.ToInt32(row["CompanyType"]) : 0;
                        company.ContactEmail = row["ContactEmail"] != DBNull.Value ?
                            Convert.ToString(row["ContactEmail"]) : string.Empty;
                        company.ContactPersonName = row["ContactPersonName"] != DBNull.Value ?
                            row["ContactPersonName"].ToString() : string.Empty;
                        company.ContactPhone = row["ContactPhone"] != DBNull.Value
                            ? row["ContactPhone"].ToString() : string.Empty;
                        
                    }
                }

            }
            return company;
        }
        public List<Company> GetCompanies()
        {
            List<Company> companies = null;
            using (SqlConnection connection = new
                SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GetCompanies";
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.Connection.Open();
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    companies = new List<Company>();
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        Company company = new Company();
                        company.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        company.CompanyName = row["CompanyName"].ToString();
                        company.Email = row["Email"]!= DBNull.Value ? row["Email"].ToString():string.Empty;
                        company.Phone = row["Phone"]!= DBNull.Value ? row["Phone"].ToString():string.Empty;
                        company.CompanyAddress = row["CompanyAddress"] != DBNull.Value ? row["CompanyAddress"].ToString() : string.Empty;
                        company.City = row["City"]!= DBNull.Value ? row["City"].ToString():string.Empty;
                        company.ActivatedTill = row["ActivatedTill"] != DBNull.Value ? DateTimeOffset.Parse(row["ActivatedTill"].ToString()).DateTime : DateTime.MaxValue;
                        company.IsActivated = row["IsActivated"] != DBNull.Value ? Convert.ToBoolean(row["IsActivated"]) : true;
                        company.State = row["State"] != DBNull.Value ? Convert.ToString(row["State"]) : string.Empty;
                        company.Country = row["Country"] != DBNull.Value ? Convert.ToString(row["Country"]) : string.Empty;
                        company.CompanyType = row["CompanyType"] != DBNull.Value ? Convert.ToInt32(row["CompanyType"]) : 0;
                        company.ContactEmail = row["ContactEmail"] != DBNull.Value ? 
                            Convert.ToString(row["ContactEmail"]) : string.Empty;
                        company.ContactPersonName = row["ContactPersonName"] != DBNull.Value ? 
                            row["ContactPersonName"].ToString() : string.Empty;
                        company.ContactPhone = row["ContactPhone"] != DBNull.Value
                            ? row["ContactPhone"].ToString() : string.Empty;
                        companies.Add(company);
                    }
                }

            }
            return companies; 
        }
        public bool UpdateCompany(Company company)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "UpdateCompany";
            command.Parameters.Add(new SqlParameter("@CompanyName", company.CompanyName));
            command.Parameters.Add(new SqlParameter("@Email", company.Email));
            command.Parameters.Add(new SqlParameter("@Phone", company.Email));
            command.Parameters.Add(new SqlParameter("@CompanyAddress", company.CompanyAddress));
            command.Parameters.Add(new SqlParameter("@City", company.City));
            command.Parameters.Add(new SqlParameter("@State", company.State));
            command.Parameters.Add(new SqlParameter("@Country", company.Country));
            command.Parameters.Add(new SqlParameter("@ContactPersonName", company.ContactPersonName));
            command.Parameters.Add(new SqlParameter("@ContactPhone", company.ContactPhone));
            command.Parameters.Add(new SqlParameter("@ContactEmail", company.ContactEmail));
            command.Parameters.Add(new SqlParameter("@ActivatedTill", company.ActivatedTill));
            command.Parameters.Add(new SqlParameter("@IsActivated", company.IsActivated));
            command.Parameters.Add(new SqlParameter("@LogoPath", company.Logopath));
            command.Parameters.Add(new SqlParameter("@CompanyType", company.CompanyType));
            //command.Parameters.Add(new SqlParameter(Project.DB_DISTRICT, project.District));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            return true;
        }
        public List<Document> GetDocuments(int projectID)
        {
            List<Document> documents = new List<Document>();
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetProjectDocuments";
            command.Parameters.Add(new SqlParameter("@ProjectID", projectID));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    Document document = new Document();
                    document.DocumentID = Convert.ToInt32(row["Id"]);
                    document.ProjectID = Convert.ToInt32(row["ProjectId"]);
                    document.Name = row["Name"].ToString();
                    document.Link = string.Format(APILINK, document.ProjectID, document.DocumentID);
                    documents.Add(document);
                }
            }
            return documents;
        }
        public List<Document> GetDocuments(string userName)
        {
            List<Document> documents = new List<Document>();
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetDocuments";
            command.Parameters.Add(new SqlParameter("@UserName", userName));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    Document document = new Document();
                    document.DocumentID = Convert.ToInt32(row["Id"]);
                    document.ProjectID =  Convert.ToInt32(row["ProjectId"]);
                    document.Name = row["Name"].ToString();
                    document.Link = string.Format(APILINK,document.ProjectID,document.DocumentID);
                    documents.Add(document);
                }
            }
            return documents;
        }


        public void CreateDocument(Document document)
        {
            SqlCommand command = null;
            //int recordCreated = 0;
            command = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_CreateDocument";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                command.Parameters.Add(new SqlParameter("@ProjectId", document.ProjectID));
                command.Parameters.Add(new SqlParameter("@Link", document.Link));
                command.Parameters.Add(new SqlParameter("@Name", document.Name));
                command.Connection = connection;
                command.Connection.Open();                
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public ApplicationUser Login(string userName, string password)
        {
            ApplicationUser user = null;
            sp_loginTableAdapter tableAdapter = new sp_loginTableAdapter();
            AccountAdapter.sp_loginDataTable table = tableAdapter.GetData(userName, password);
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
                    user.Project = new Project();
                    user.Project.ProjectId = row.IsProjectIdNull()?0: row.ProjectId;
                    user.SecurityStamp = row.IsSecurityStampNull()?string.Empty: row.SecurityStamp;
                    user.TwoFactorEnabled = row.TwoFactorEnabled;
                    user.UserName = row.UserName;
                    user.Id = row.Id;
                    user.Role = new Role();
                    user.Role.Name = row.RoleName;

                }
            }
            return user;
        }

        public List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            sp_GetRolesTableAdapter adapter = new sp_GetRolesTableAdapter();
            AccountAdapter.sp_GetRolesDataTable table = adapter.GetRoles();
            if (table != null && table.Rows.Count > 0)
            {
                foreach (AccountAdapter.sp_GetRolesRow row in table.Rows)
                {
                    Role role = new Role();
                    role.RoleID = row.Id;
                    role.Name = row.Name;
                    roles.Add(role);
                }
            }
            return roles;
        }
        public List<Project> GetProjects(string userName)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetProject";
            command.Parameters.Add(new SqlParameter("@UserName",userName));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            List<Project> projects = new List<Project>(); 
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Project project = new Project();
                    project.ProjectId = Convert.ToInt32(row["projectId"]);
                    project.ProjectName = row["Name"]!=DBNull.Value?Convert.ToString(row["Name"]):string.Empty;
                    project.Description = row["Description"]!=DBNull.Value? Convert.ToString(row["Description"]):string.Empty;
                    project.District = row["District"]!=DBNull.Value? Convert.ToString(row["District"]):string.Empty;
                    project.Longitude = row["Longitude"]!=DBNull.Value? Convert.ToString(row["Longitude"]):string.Empty;
                    project.Lattitude = row["Lattitude"]!=DBNull.Value? Convert.ToString(row["Lattitude"]):string.Empty;
                    project.CompanyId = Convert.ToInt64(row["companyId"]);
                    projects.Add(project);
                }
            }
            return projects;
        }
        public List<Project> GetProjects(int companyID)
        {
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetProjects";
            command.Parameters.Add(new SqlParameter("@CompanyId", companyID));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            List<Project> projects = new List<Project>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Project project = new Project();
                    project.ProjectId = Convert.ToInt32(row["projectId"]);
                    project.ProjectName = row["Name"] != DBNull.Value ? Convert.ToString(row["Name"]) : string.Empty;
                    project.Description = row["Description"] != DBNull.Value ? Convert.ToString(row["Description"]) : string.Empty;
                    project.District = row["District"] != DBNull.Value ? Convert.ToString(row["District"]) : string.Empty;
                    project.Longitude = row["Longitude"] != DBNull.Value ? Convert.ToString(row["Longitude"]) : string.Empty;
                    project.Lattitude = row["Lattitude"] != DBNull.Value ? Convert.ToString(row["Lattitude"]) : string.Empty;
                    project.CompanyId = Convert.ToInt64(row["companyId"]);
                    projects.Add(project);
                }
            }
            return projects;
        }
        public List<ApplicationUser> GetProjectUsers(int projectId)
        {
            List<ApplicationUser> users = null;
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetProjectUsers";
            command.Parameters.Add(new SqlParameter("@ProjectId", projectId));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    users = new List<ApplicationUser>();
                    foreach (DataRow row in table.Rows)
                    {
                        ApplicationUser user = new ApplicationUser();
                        user.Role = new Role();
                        user.Project = new Project();
                        user.Id = row["Id"].ToString();
                        user.FirstName = row["FirstName"].ToString();
                        user.AccessFailedCount = Convert.ToInt32(row["AccessFailedCount"]);
                        user.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        user.CreatedDateTime = Convert.ToDateTime(row["CreatedDateTime"]);
                        user.Email = row["Email"]==DBNull.Value ? string.Empty : row["Email"].ToString();
                        user.LastName = row["LastName"].ToString();
                        user.LockoutEnabled = Convert.ToBoolean(row["LockoutEnabled"]);
                        user.LockoutEndDateUtc = row["LockoutEndDateUtc"]==DBNull.Value   ? (DateTime?)null : Convert.ToDateTime(row["LockoutEndDateUtc"]);
                        user.PhoneNumber = row["PhoneNumber"].ToString();
                        user.Project.ProjectId = row["ProjectId"]==DBNull.Value ? 0 : Convert.ToInt32(row["ProjectId"]);
                        user.Role.RoleID = row["RoleId"].ToString();
                        //user.Role.Name = row["Name"].ToString();
                        user.SecurityStamp = row["SecurityStamp"]==DBNull.Value ? string.Empty : row["SecurityStamp"].ToString();
                        user.TwoFactorEnabled = Convert.ToBoolean(row["TwoFactorEnabled"]);
                        user.UserName = row["UserName"].ToString();
                        user.Token = row["Token"] != DBNull.Value ? row["Token"].ToString() : string.Empty;

                        users.Add(user);
                    }
                }
            }
            return users;
        

    }
    public List<ApplicationUser> GetUsersOfCompany(string userName, string roleName)
        {
            List<ApplicationUser> users = null;
            sp_GetUsersOfCompanyandRoleIDTableAdapter adapter = new sp_GetUsersOfCompanyandRoleIDTableAdapter();
            AccountAdapter.sp_GetUsersOfCompanyandRoleIDDataTable table = adapter.GetUsers(userName, (int?)null);
            if (table != null && table.Rows.Count > 0)
            {
                users = new List<ApplicationUser>();
                foreach (AccountAdapter.sp_GetUsersOfCompanyandRoleIDRow row in table.Rows)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Role = new Role();
                    user.Project = new Project();
                    user.Id = row.Id;
                    user.FirstName = row.FirstName;
                    user.AccessFailedCount = row.AccessFailedCount;
                    user.CompanyId = row.CompanyId;
                    user.CreatedDateTime = row.CreatedDateTime;
                    user.Email = row.IsEmailNull() ? string.Empty : row.Email;
                    user.LastName = row.LastName;
                    user.LockoutEnabled = row.LockoutEnabled;
                    user.LockoutEndDateUtc = row.IsLockoutEndDateUtcNull() ? (DateTime?)null : row.LockoutEndDateUtc;
                    user.PhoneNumber = row.PhoneNumber;
                    user.Project.ProjectId = row.IsProjectIdNull()?0:row.ProjectId;
                    user.Role.RoleID = row.RoleId.ToString();
                    user.Role.Name = row.Name;
                    user.SecurityStamp = row.IsSecurityStampNull() ? string.Empty:row.SecurityStamp;
                    user.TwoFactorEnabled = row.TwoFactorEnabled;
                    user.UserName = row.UserName;
                    users.Add(user);
                }
            }
            return users;
        }

        public object DeleteUserByUserID(string userID)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            return adapter.DeleteUserByUserID(userID);
        
        }
        public List<ApplicationUser> GetUsersByCompanyId(long companyId)
        {
            List<ApplicationUser> users = null;
            Sp_GetCompanyUsersTableAdapter adapter = new Sp_GetCompanyUsersTableAdapter();
            AccountAdapter.Sp_GetCompanyUsersDataTable  table = adapter.GetData(companyId);
            if (table != null && table.Rows.Count > 0)
            {
                users = new List<ApplicationUser>();
               foreach(AccountAdapter.Sp_GetCompanyUsersRow row in table.Rows)
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
                   user.SecurityStamp = row.IsSecurityStampNull()?string.Empty:row.SecurityStamp;
                   user.TwoFactorEnabled = row.TwoFactorEnabled;
                   user.UserName = row.UserName;
                    user.Token = row.IsTokenNull()?null: row.Token;
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
                rowCount = row.StatusID;
                switch (rowCount)
                { 
                    case 1:
                        user.CurrentLeadsCount = row.PhoneNumberCount;
                        break;
                    case 2:
                        user.NoWorkCount = row.PhoneNumberCount;
                        break;
                    case 3:
                        user.NotConnectedCount = row.PhoneNumberCount;
                        break;
                    case 4:
                        user.FollowUpsCount = row.PhoneNumberCount;
                        break;
                    case 5:
                        user.VisitOnCounts = row.PhoneNumberCount;
                        break;
                    case 6:
                        user.VisitDoneCount = row.PhoneNumberCount;
                        break;
                    case 7:
                        user.VisitDeadCount = row.PhoneNumberCount;
                        break;
                    case 8:
                        user.OtherProjectsCount = row.PhoneNumberCount;
                        break;
                    case 9:
                        user.ResaleCount = row.PhoneNumberCount;
                        break;
                    case 10:
                        user.AlreadyBookedCount = row.PhoneNumberCount;
                        break;
                    case 11:
                        user.BookedDone = row.PhoneNumberCount;
                        break;
                    case 12:
                        user.DeadCount = row.PhoneNumberCount;
                        break;
                    case 13:
                        user.RentCount = row.PhoneNumberCount;
                        break;
                    case 14:
                        user.PlotCount = row.PhoneNumberCount;
                        break;
                    case 15:
                        user.DuplicateCount = row.PhoneNumberCount;
                        break;
                    case 16:
                        user.RawLeadsCount = row.PhoneNumberCount;
                        break;
                }
                    
                rowCount++;
                
                     
            }
            return user;
        }

        
    }
}

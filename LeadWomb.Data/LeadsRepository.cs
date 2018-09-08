using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Data.LeadWombDatasetTableAdapters;
using LeadWomb.Model;
using LeadWomb.Data;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.OleDb;


namespace LeadWomb.Data
{
    public class LeadsRepository
    {
        private sp_GetLeadsByUserNameTableAdapter adapter = null;
        // private sp_GetLeadsByCompanyTableAdapter companyAdapter = null;
        private DataColumn leadIDColumn = new DataColumn("Lead_ID", typeof(int));
        private DataColumn queryRemarksColumn = new DataColumn("QueryRemarks", typeof(string));
        private DataColumn typeOfPropertyColumn = new DataColumn("TypeOfProperty", typeof(int));
        private DataColumn statusColumn = new DataColumn("Status", typeof(int));
        private DataColumn rangeFromColumn = new DataColumn("RangeFrom", typeof(int));
        private DataColumn rangeToColumn = new DataColumn("RangeTo", typeof(int));
        private DataColumn CmpctLabelColumn = new DataColumn("CmpctLabel", typeof(string));
        private DataColumn recivedOnColumn = new DataColumn("RecievedOn", typeof(DateTime));
        private DataColumn projNameColumn = new DataColumn("ProjName", typeof(string));
        private DataColumn assignedToColumn = new DataColumn("AssignedTo", typeof(string));
        private DataColumn builderInterestColumn = new DataColumn("BuilderInterest", typeof(bool));
        private DataColumn statusIDColumn = new DataColumn("StatusId", typeof(int));
        private DataColumn statusDateColumn = new DataColumn("StatusDate", typeof(DateTime));
        private DataColumn companyIDColumn = new DataColumn("CompanyId", typeof(long));
        private DataColumn leadItemIDColumn = new DataColumn("LeadItemID", typeof(int));
        private SqlConnection connection = null;
        private SqlBulkCopy sqlBulkCopy = null;
        public LeadsRepository()
        {
            adapter = new sp_GetLeadsByUserNameTableAdapter();
            //companyAdapter = new sp_GetLeadsByCompanyTableAdapter();
        }
        public LeadsRepository(string databaseType)
        {

        }
        private Leads CopyTo(Leads lead, List<LeadItems> existingItems)
        {
            lead.Items = new List<LeadItems>();
            foreach (LeadItems item in existingItems)
            {
                LeadItems liItem = new LeadItems();
                liItem.AssignedTo = item.AssignedTo;
                liItem.UserName = item.UserName;
                liItem.IsAssigned = item.IsAssigned;
                lead.Items.Add(liItem);
            }
            return lead;
        }
        public int CreateRecordings(int leadID, string userName, string fileName)
        {
            SqlCommand command = null;
            int recordCreated = 0;
            command = new SqlCommand();
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_CreateRecordings";
            command.Parameters.Add(new SqlParameter("@Lead_ID", leadID));
            command.Parameters.Add(new SqlParameter("@Name", fileName));
            command.Parameters.Add(new SqlParameter("@CreatedBy", userName));
            command.Connection = connection;
            using (connection)
            {
                connection.Open();
                recordCreated = command.ExecuteNonQuery();
            }
            return recordCreated;
        }
        public List<Recordings> GetRecordings(int leadID)
        {
            SqlCommand command = null;
            int recordCreated = 0;
            List<Recordings> recordings = null;
            command = new SqlCommand();
            SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetRecordings";
            command.Parameters.Add(new SqlParameter("@Lead_ID", leadID));
            command.Connection = connection;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet data = new DataSet();
            using (connection)
            {
                connection.Open();

                adapter.Fill(data);
                if (data.Tables != null && data.Tables.Count > 0)
                {
                    recordings = new List<Recordings>();
                    foreach (DataRow row in data.Tables[0].Rows)
                    {
                        Recordings recording = new Recordings();
                        recording.ID = Convert.ToInt32(row["ID"]);
                        recording.Lead_ID = Convert.ToInt32(row["Lead_ID"]);
                        recording.UserName = row["CreatedBy"].ToString();
                        recording.CreatedDateTime = Convert.ToDateTime(row["CreateDateTime"]);
                        recording.FileName = Convert.ToString(row["Name"]);
                        recordings.Add(recording);
                    }

                }
            }
            return recordings;
        }
        public IList<Leads> GetLeadsByCompanyWithPaging(long companyID, int pageSize, int pageNumber,
            int? statusID, int? projectID, string assignedTo, string leadName, int? leadNumber,
            DateTime? dateFrom, DateTime? dateTo)
        {

            SqlCommand command = null;
            List<Leads> leads = new List<Leads>();
            List<LeadItems> existingLeads = new List<LeadItems>();
            try
            {

                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetLeadsbyCompanyWithPaging";

                command.Parameters.Add(new SqlParameter("@CompanyID", companyID));
                command.Parameters.Add(new SqlParameter("@statusID", statusID));

                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
                command.Parameters.Add(new SqlParameter("@LeadName", leadName));
                command.Parameters.Add(new SqlParameter("@Lead_Id", leadNumber));
                command.Parameters.Add(new SqlParameter("@AssignedTo", assignedTo));
                command.Parameters.Add(new SqlParameter("@ProjectId", projectID));
                if (dateFrom != null && dateFrom != DateTime.MinValue)
                {
                    command.Parameters.Add(new SqlParameter("@DateFrom", dateFrom));
                }
                if (dateTo != null && dateTo != DateTime.MinValue)
                {
                    command.Parameters.Add(new SqlParameter("@DateTo", dateTo));
                }
                command.Connection = connection;
                SqlDataAdapter companyAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                command.Connection.Open();
                companyAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable leadData = dataSet.Tables[0];
                    DataTable userData = dataSet.Tables[1];
                    List<AssignedUser> existingUsers = new List<AssignedUser>();
                    foreach (DataRow row in userData.Rows)
                    {
                        LeadItems item = new LeadItems();
                        // AssignedUser existingUser = new AssignedUser();
                        item.AssignedTo = row["Id"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.IsAssigned = false;
                        existingLeads.Add(item);


                    }
                    foreach (DataRow row in leadData.Rows)
                    {


                        Leads lead = new Leads();
                        //lead.Items = new List<LeadItems>();

                        lead = CopyTo(lead, existingLeads);

                        LeadItems item = new LeadItems();

                        AssignedUser assignedUser = new AssignedUser();
                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        assignedUser.ID = row["Id"].ToString();
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        item.AssignedTo = row["AssignedTo"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.TypeOfProperty = row["TypeOfProperty"] != DBNull.Value ? Convert.ToInt32(row["TypeOfProperty"]) : 0;
                        item.ProjectName = row["ProjName"] != DBNull.Value ? row["ProjName"].ToString() : string.Empty;
                        item.QueryRemarks = row["QueryRemarks"] != DBNull.Value ? row["QueryRemarks"].ToString() : string.Empty;
                        item.RangeFrom = row["RangeFrom"] != DBNull.Value ? Convert.ToInt32(row["RangeFrom"]) : 0;
                        item.RangeTo = row["RangeTo"] != DBNull.Value ? Convert.ToInt32(row["RangeTo"]) : 0;
                        item.RecivedOn = row["ReceivedOn"] != DBNull.Value ? Convert.ToDateTime(row["ReceivedOn"]) : DateTime.MinValue;
                        item.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        item.StatusDate = row["StatusDate"] != DBNull.Value ? Convert.ToDateTime(row["StatusDate"]) : DateTime.MinValue;
                        item.StatusId = row["StatusId"] != DBNull.Value ? Convert.ToInt32(row["StatusId"]) : 0;
                        item.LeadItemID = Convert.ToInt32(row["LeadItemId"]);
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        if (row["BuilderInterest"] != DBNull.Value && row["BuilderInterest"].ToString() != string.Empty)
                        {
                            item.BuilderInterest = Convert.ToBoolean(row["BuilderInterest"]);
                        }
                        if (row["CmpctLabel"] != DBNull.Value)
                        {
                            item.CompactLabel = row["CmpctLabel"].ToString();
                        }
                        item.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        DateTime createDateTimeOffset = SqlDateTime.MinValue.Value;
                        if (DateTime.TryParse(row["CreateDateTimeOffset"].ToString(), out createDateTimeOffset))
                        {
                            lead.CreateDateTime = createDateTimeOffset;
                        }
                        lead.CreateUserID = row["CreateUser_ID"].ToString();
                        lead.EditUserId = row["EditUser_ID"] != null ? row["EditUser_ID"].ToString() : string.Empty;

                        lead.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();
                        lead.CmpctLabel = row["CmpctLabel"].ToString();
                        lead.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        leads = CreateOrUpdate(lead, leads, item, assignedUser);
                        //    }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }

            return AssignLeadIds(leads);

        }
        public IList<Leads> GetLeadsByCompany(long companyID, int? statusID)
        {
            SqlCommand command = null;
            List<Leads> leads = new List<Leads>();
            List<LeadItems> existingLeads = new List<LeadItems>();
            try
            {

                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetLeadsbyCompany";

                command.Parameters.Add(new SqlParameter("@CompanyID", companyID));
                command.Parameters.Add(new SqlParameter("@statusID", statusID));
                command.Connection = connection;
                SqlDataAdapter companyAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                command.Connection.Open();
                companyAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable leadData = dataSet.Tables[0];
                    DataTable userData = dataSet.Tables[1];
                    List<AssignedUser> existingUsers = new List<AssignedUser>();
                    foreach (DataRow row in userData.Rows)
                    {
                        LeadItems item = new LeadItems();
                        // AssignedUser existingUser = new AssignedUser();
                        item.AssignedTo = row["Id"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.IsAssigned = false;
                        existingLeads.Add(item);


                    }
                    int oldID = 0;
                    Leads lead = null;
                    foreach (DataRow row in leadData.Rows)
                    {

                        if (lead == null)
                        {
                            lead = new Leads();
                            lead.Items = new List<LeadItems>();
                        }

                        //lead.Items = new List<LeadItems>();

                        // lead = CopyTo(lead, existingLeads);

                        LeadItems item = new LeadItems();

                        AssignedUser assignedUser = new AssignedUser();
                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        assignedUser.ID = row["Id"].ToString();
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        item.AssignedTo = row["AssignedTo"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.TypeOfProperty = row["TypeOfProperty"] != DBNull.Value ? Convert.ToInt32(row["TypeOfProperty"]) : 0;
                        item.ProjectName = row["ProjName"] != DBNull.Value ? row["ProjName"].ToString() : string.Empty;
                        item.QueryRemarks = row["QueryRemarks"] != DBNull.Value ? row["QueryRemarks"].ToString() : string.Empty;
                        item.RangeFrom = row["RangeFrom"] != DBNull.Value ? Convert.ToInt32(row["RangeFrom"]) : 0;
                        item.RangeTo = row["RangeTo"] != DBNull.Value ? Convert.ToInt32(row["RangeTo"]) : 0;
                        item.RecivedOn = row["ReceivedOn"] != DBNull.Value ? Convert.ToDateTime(row["ReceivedOn"]) : DateTime.MinValue;
                        item.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        item.StatusDate = row["StatusDate"] != DBNull.Value ? Convert.ToDateTime(row["StatusDate"]) : DateTime.MinValue;
                        item.StatusId = row["StatusId"] != DBNull.Value ? Convert.ToInt32(row["StatusId"]) : 0;
                        item.LeadItemID = Convert.ToInt32(row["LeadItemId"]);
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        if (row["BuilderInterest"] != DBNull.Value && row["BuilderInterest"].ToString() != string.Empty)
                        {
                            item.BuilderInterest = Convert.ToBoolean(row["BuilderInterest"]);
                        }
                        if (row["CmpctLabel"] != DBNull.Value)
                        {
                            item.CompactLabel = row["CmpctLabel"].ToString();
                        }
                        item.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        DateTime createDateTimeOffset = SqlDateTime.MinValue.Value;
                        if (DateTime.TryParse(row["CreateDateTimeOffset"].ToString(), out createDateTimeOffset))
                        {
                            lead.CreateDateTime = createDateTimeOffset;
                        }
                        lead.CreateUserID = row["CreateUser_ID"].ToString();
                        lead.EditUserId = row["EditUser_ID"] != null ? row["EditUser_ID"].ToString() : string.Empty;

                        lead.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();
                        lead.CmpctLabel = row["CmpctLabel"].ToString();
                        lead.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        //new lead
                        if (oldID > 0 && oldID != lead.LeadId)
                        {
                            leads.Add(lead);
                            lead = new Leads();
                            lead.Items = new List<LeadItems>();
                        }
                        oldID = lead.LeadId;
                        lead.Items.Add(item);
                        //leads = CreateOrUpdateForUser(lead, leads, item);
                    }
                    if (lead != null)
                    {
                        leads.Add(lead);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }

            return AssignLeadIds(leads);

        }
        public IList<Leads> GetRawLeadsByCompanyWithPaging(long CompanyID, int pageSize, int pageNumber)
        {
            SqlCommand command = null;
            List<Leads> leads = new List<Leads>();
            List<LeadItems> existingLeads = new List<LeadItems>();
            try
            {

                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetRawLeadsbyCompanyWithPaging";

                command.Parameters.Add(new SqlParameter("@CompanyId", CompanyID));
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));

                command.Connection = connection;
                SqlDataAdapter companyAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                command.Connection.Open();
                companyAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable leadData = dataSet.Tables[0];
                    DataTable userData = dataSet.Tables[1];
                    List<AssignedUser> existingUsers = new List<AssignedUser>();
                    foreach (DataRow row in userData.Rows)
                    {
                        LeadItems item = new LeadItems();
                        // AssignedUser existingUser = new AssignedUser();
                        item.AssignedTo = row["Id"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.IsAssigned = false;
                        existingLeads.Add(item);


                    }
                    foreach (DataRow row in leadData.Rows)
                    {


                        Leads lead = new Leads();

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        if (row["CmpctLabel"] != DBNull.Value)
                        {
                            lead.CmpctLabel = row["CmpctLabel"].ToString();
                        }
                        lead.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        DateTime createDateTimeOffset = SqlDateTime.MinValue.Value;
                        if (DateTime.TryParse(row["CreateDateTimeOffset"].ToString(), out createDateTimeOffset))
                        {
                            lead.CreateDateTime = createDateTimeOffset;
                        }
                        lead.CreateUserID = row["CreateUser_ID"].ToString();
                        lead.EditUserId = row["EditUser_ID"] != null ? row["EditUser_ID"].ToString() : string.Empty;


                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        //item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();
                        lead.CmpctLabel = row["CmpctLabel"].ToString();
                        lead.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        lead.Items = existingLeads;
                        leads.Add(lead);
                        //leads = CreateOrUpdate(lead, leads, item, assignedUser);
                        //    }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }

            return leads;

        }

        public IList<Leads> GetRawLeadsByCompany(long companyID)
        {
            SqlCommand command = null;
            List<Leads> leads = new List<Leads>();
            List<LeadItems> existingLeads = new List<LeadItems>();
            try
            {

                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetRawLeadsbyCompany";

                command.Parameters.Add(new SqlParameter("@CompanyId", companyID));

                command.Connection = connection;
                SqlDataAdapter companyAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                command.Connection.Open();
                companyAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable leadData = dataSet.Tables[0];
                    DataTable userData = dataSet.Tables[1];
                    List<AssignedUser> existingUsers = new List<AssignedUser>();
                    foreach (DataRow row in userData.Rows)
                    {
                        LeadItems item = new LeadItems();
                        // AssignedUser existingUser = new AssignedUser();
                        item.AssignedTo = row["Id"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.IsAssigned = false;
                        existingLeads.Add(item);


                    }
                    foreach (DataRow row in leadData.Rows)
                    {


                        Leads lead = new Leads();

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        if (row["CmpctLabel"] != DBNull.Value)
                        {
                            lead.CmpctLabel = row["CmpctLabel"].ToString();
                        }
                        lead.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        DateTime createDateTimeOffset = SqlDateTime.MinValue.Value;
                        if (DateTime.TryParse(row["CreateDateTimeOffset"].ToString(), out createDateTimeOffset))
                        {
                            lead.CreateDateTime = createDateTimeOffset;
                        }
                        lead.CreateUserID = row["CreateUser_ID"].ToString();
                        lead.EditUserId = row["EditUser_ID"] != null ? row["EditUser_ID"].ToString() : string.Empty;


                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        //item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();
                        lead.CmpctLabel = row["CmpctLabel"].ToString();
                        lead.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        lead.Items = existingLeads;
                        leads.Add(lead);
                        //leads = CreateOrUpdate(lead, leads, item, assignedUser);
                        //    }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }

            return leads;

        }
        private List<Leads> AssignLeadIds(List<Leads> leads)
        {
            foreach (Leads item in leads)
            {
                foreach (LeadItems liItem in item.Items)
                {
                    liItem.LeadID = item.LeadId;
                }
            }
            return leads;
        }
        //LeadWombDataset.sp_GetLeadsByCompanyDataTable dataTable = companyAdapter.GetLeadsByCompany(userName, statusID);

        //if (dataTable != null && dataTable.Rows.Count > 0)
        //{
        //    leads = new List<Leads>();
        //    DateTime createDateTimeOffset;
        //    DateTime editDateTime = DateTime.MinValue;
        //   foreach (DataRow row in dataTable.Rows)

        ////    {
        //        Leads lead = new Leads();
        //        LeadItems item = new LeadItems();
        //        AssignedUser assignedUser = new AssignedUser();
        //        lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
        //        assignedUser.ID = row.Id;
        //        assignedUser.AssignedTo = row.Name;
        //        item.ProjectName = row[dataTable.ProjNameColumn] != DBNull.Value ? row[dataTable.ProjNameColumn].ToString() : string.Empty;
        //        item.QueryRemarks = row[dataTable.QueryRemarksColumn] != DBNull.Value ? row[dataTable.QueryRemarksColumn].ToString() : string.Empty;
        //        item.RangeFrom = row[dataTable.RangeFromColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeFromColumn]) : 0;
        //        item.RangeTo = row[dataTable.RangeToColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeToColumn]) : 0;
        //        item.RecivedOn = row[dataTable.ReceivedOnColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.ReceivedOnColumn]) : DateTime.MinValue;
        //        item.Status = row[dataTable.StatusColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.StatusColumn]) : 0;
        //        item.StatusDate = row[dataTable.StatusDateColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.StatusDateColumn]) : DateTime.MinValue;
        //        item.StatusId = !row.IsStatusIdNull() ? Convert.ToInt32(row[dataTable.StatusIdColumn]) : 0;
        //        //item.AssignedTo = row[dataTable.AssignedToColumn] != null ? row[dataTable.AssignedToColumn].ToString() : string.Empty;
        //        if (row[dataTable.BuilderInterestColumn] != DBNull.Value && row[dataTable.BuilderInterestColumn].ToString() != string.Empty)
        //        {
        //            item.BuilderInterest = Convert.ToBoolean(row[dataTable.BuilderInterestColumn]);
        //        }
        //        if (row[dataTable.CmpctLabelColumn] != DBNull.Value)
        //        {
        //            item.CompactLabel = row[dataTable.CmpctLabelColumn].ToString();
        //        }
        //        item.CompanyId = Convert.ToInt64(row[dataTable.CompanyIdColumn]);

        //        if (DateTime.TryParse(row[dataTable.CreateDateTimeOffsetColumn].ToString(), out createDateTimeOffset))
        //        {
        //            lead.CreateDateTime = createDateTimeOffset;
        //        }
        //        lead.CreateUserID = row[dataTable.CreateUser_IDColumn].ToString();
        //        lead.EditUserId = row[dataTable.EditUser_IDColumn] != null ? row[dataTable.EditUser_IDColumn].ToString() : string.Empty;


        //        if (row[dataTable.EmailColumn] != DBNull.Value)
        //        {
        //            lead.Email = row[dataTable.EmailColumn].ToString();
        //        }
        //        item.LeadID = Convert.ToInt32(row[dataTable.Lead_IDColumn]);

        //        lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
        //        lead.Name = row[dataTable.NameColumn].ToString();
        //        lead.PhoneNumber = row[dataTable.PhoneNumberColumn].ToString();

        //        leads = CreateOrUpdate(lead, leads, item,assignedUser);
        //    }
        //}
        //return leads;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedTo"></param>
        /// <returns></returns>
        public IList<Leads> GetLeads(string userName,
               int? statusID, int? projectID, string LeadName, int? Lead_Id,
               DateTime? dateFrom, DateTime? dateTo)
        {

            SqlCommand command = null;
            List<Leads> leads = new List<Leads>();
            List<LeadItems> existingLeads = new List<LeadItems>();
            try
            {

                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetLeadsByUserName";

                command.Parameters.Add(new SqlParameter("@UserName", userName));
                command.Parameters.Add(new SqlParameter("@statusID", statusID));

                command.Parameters.Add(new SqlParameter("@LeadName", LeadName));
                command.Parameters.Add(new SqlParameter("@Lead_Id", Lead_Id));
                command.Parameters.Add(new SqlParameter("@ProjectId", projectID));
                if (dateFrom != null && dateFrom != DateTime.MinValue)
                {
                    command.Parameters.Add(new SqlParameter("@DateFrom", Convert.ToDateTime(dateFrom)));
                }
                if (dateTo != null && dateTo != DateTime.MinValue)
                {
                    command.Parameters.Add(new SqlParameter("@DateTo", Convert.ToDateTime(dateTo)));
                }
                command.Connection = connection;
                SqlDataAdapter companyAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                command.Connection.Open();
                companyAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable leadData = dataSet.Tables[0];
                    DataTable userData = new DataTable();
                    List<AssignedUser> existingUsers = new List<AssignedUser>();
                    foreach (DataRow row in userData.Rows)
                    {
                        LeadItems item = new LeadItems();
                        // AssignedUser existingUser = new AssignedUser();
                        item.AssignedTo = row["Id"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.IsAssigned = false;
                        existingLeads.Add(item);


                    }
                    int oldID = 0;
                    Leads lead = null;
                    foreach (DataRow row in leadData.Rows)
                    {

                        if (lead == null)
                        {
                            lead = new Leads();
                            lead.Items = new List<LeadItems>();
                        }

                        //lead.Items = new List<LeadItems>();

                        // lead = CopyTo(lead, existingLeads);

                        LeadItems item = new LeadItems();

                        AssignedUser assignedUser = new AssignedUser();
                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        assignedUser.ID = row["Id"].ToString();
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        item.AssignedTo = row["AssignedTo"].ToString();
                        item.UserName = row["userName"].ToString();
                        item.TypeOfProperty = row["TypeOfProperty"] != DBNull.Value ? Convert.ToInt32(row["TypeOfProperty"]) : 0;
                        item.ProjectName = row["ProjName"] != DBNull.Value ? row["ProjName"].ToString() : string.Empty;
                        item.QueryRemarks = row["QueryRemarks"] != DBNull.Value ? row["QueryRemarks"].ToString() : string.Empty;
                        item.RangeFrom = row["RangeFrom"] != DBNull.Value ? Convert.ToInt32(row["RangeFrom"]) : 0;
                        item.RangeTo = row["RangeTo"] != DBNull.Value ? Convert.ToInt32(row["RangeTo"]) : 0;
                        item.RecivedOn = row["ReceivedOn"] != DBNull.Value ? Convert.ToDateTime(row["ReceivedOn"]) : DateTime.MinValue;
                        item.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        item.StatusDate = row["StatusDate"] != DBNull.Value ? Convert.ToDateTime(row["StatusDate"]) : DateTime.MinValue;
                        item.StatusId = row["StatusId"] != DBNull.Value ? Convert.ToInt32(row["StatusId"]) : 0;
                        item.LeadItemID = Convert.ToInt32(row["LeadItemId"]);
                        //item.AssignedTo = row["AssignedTo"] != null ? row["AssignedTo"].ToString() : string.Empty;
                        if (row["BuilderInterest"] != DBNull.Value && row["BuilderInterest"].ToString() != string.Empty)
                        {
                            item.BuilderInterest = Convert.ToBoolean(row["BuilderInterest"]);
                        }
                        if (row["CmpctLabel"] != DBNull.Value)
                        {
                            item.CompactLabel = row["CmpctLabel"].ToString();
                        }
                        item.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        DateTime createDateTimeOffset = SqlDateTime.MinValue.Value;
                        if (DateTime.TryParse(row["CreateDateTimeOffset"].ToString(), out createDateTimeOffset))
                        {
                            lead.CreateDateTime = createDateTimeOffset;
                        }
                        lead.CreateUserID = row["CreateUser_ID"].ToString();
                        lead.EditUserId = row["EditUser_ID"] != null ? row["EditUser_ID"].ToString() : string.Empty;

                        lead.CompanyId = Convert.ToInt64(row["CompanyId"]);
                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();
                        lead.CmpctLabel = row["CmpctLabel"].ToString();
                        lead.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                        //new lead
                        if (oldID > 0 && oldID != lead.LeadId)
                        {
                            leads.Add(lead);
                            lead = new Leads();
                            lead.Items = new List<LeadItems>();
                        }
                        oldID = lead.LeadId;
                        lead.Items.Add(item);
                        //leads = CreateOrUpdateForUser(lead, leads, item);
                    }
                    if (lead != null)
                    {
                        leads.Add(lead);
                    }    //    }

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }

            return AssignLeadIds(leads);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="listItems"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        private List<Leads> CreateOrUpdate(Leads item, List<Leads> listItems, LeadItems listItem, AssignedUser assignedUser)
        {
            Leads lead = listItems.Find(x => x.PhoneNumber == item.PhoneNumber && x.LeadId == item.LeadId);


            if (lead != null)
            {
                if (lead.Items == null)
                {
                    lead.Items = new List<LeadItems>();
                }
                int count = lead.Items.RemoveAll(x => x.AssignedTo == assignedUser.ID);
                if (count > 0)
                {
                    //listItem.LeadID = item.LeadId;
                    listItem.IsAssigned = true;
                    lead.Items.Add(listItem);
                }



            }
            else
            {
                //fresh lead..
                if (item.Items == null)
                {
                    item.Items = new List<LeadItems>();
                }
                int count = item.Items.RemoveAll(x => x.AssignedTo == assignedUser.ID);
                if (count > 0)
                {
                    //listItem.LeadID = item.LeadId;
                    listItem.IsAssigned = true;
                    item.Items.Add(listItem);
                }
                listItems.Add(item);
            }
            return listItems;
        }
        private List<Leads> CreateOrUpdateForUser(Leads item, List<Leads> listItems, LeadItems listItem)
        {
            Leads lead = listItems.Find(x => x.PhoneNumber == item.PhoneNumber && x.LeadId == item.LeadId);


            if (lead != null)
            {
                if (lead.Items == null)
                {
                    lead.Items = new List<LeadItems>();
                }
                //if (lead.AssignedUsers == null)

                lead.Items.Add(listItem);
            }
            else
            {
                //fresh lead..
                if (item.Items == null)
                {
                    item.Items = new List<LeadItems>();
                }
                //if (item.AssignedUsers == null)
                //{
                //    item.AssignedUsers = new List<AssignedUser>();
                //}
                //item.AssignedUsers.Find(x => x.ID == assignedUser.ID).Enabled = true;
                item.Items.Add(listItem);
                listItems.Add(item);
            }
            return listItems;
        }

        private DataTable CreateLeadItemsTable()
        {
            DataTable table = new DataTable("LeadItems");
            table.Columns.Add(leadIDColumn);
            table.Columns.Add(queryRemarksColumn);
            table.Columns.Add(typeOfPropertyColumn);
            table.Columns.Add(statusColumn);
            table.Columns.Add(rangeFromColumn);
            table.Columns.Add(rangeToColumn);
            table.Columns.Add(CmpctLabelColumn);
            table.Columns.Add(recivedOnColumn);
            table.Columns.Add(projNameColumn);
            table.Columns.Add(assignedToColumn);
            table.Columns.Add(builderInterestColumn);
            table.Columns.Add(statusIDColumn);
            table.Columns.Add(statusDateColumn);
            table.Columns.Add(companyIDColumn);
            table.Columns.Add(leadItemIDColumn);
            return table;
        }
        ////<summary>

        ////</summary>
        public bool UpdateLeads(Leads lead)
        {
            DataTable table = CreateLeadItemsTable();
            if (lead.Items != null)
            {

                foreach (LeadItems item in lead.Items)
                {
                    DataRow row = table.NewRow();
                    row[leadIDColumn] = lead.LeadId;
                    row[queryRemarksColumn] = item.QueryRemarks;
                    row[typeOfPropertyColumn] = item.TypeOfProperty;
                    row[statusColumn] = item.Status;
                    row[rangeFromColumn] = item.RangeFrom;
                    row[rangeToColumn] = item.RangeTo;
                    row[CmpctLabelColumn] = item.CompactLabel;
                    row[recivedOnColumn] = DateTime.Now;
                    row[projNameColumn] = item.ProjectName;
                    row[assignedToColumn] = item.AssignedTo;
                    row[builderInterestColumn] = item.BuilderInterest.HasValue ? item.BuilderInterest.Value : false;
                    row[statusIDColumn] = item.StatusId;
                    row[statusDateColumn] = item.StatusDate == DateTime.MinValue ? SqlDateTime.MinValue.Value : item.StatusDate;
                    row[companyIDColumn] = item.CompanyId;
                    row[leadItemIDColumn] = item.LeadItemID;
                    table.Rows.Add(row);
                }

            }
            new GetLeadsByCompanyIdandStatusIdTableAdapter().UpdateLeads(table,
                lead.LeadId, lead.EditUserId, DateTime.Now, lead.Name, lead.Email, lead.PhoneNumber, true, lead.CompanyId, null);

            return true;
        }

        public int AddLead(Leads lead)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            object r = adapter.CreateLeads(lead.CreateUserID, DateTime.Now, lead.EditUserId, lead.EditDateTime.HasValue ? lead.EditDateTime.Value : (DateTime?)null,
                lead.Name, lead.Email, lead.PhoneNumber, lead.Items[0].QueryRemarks, lead.Items[0].TypeOfProperty, lead.Items[0].Status, lead.Items[0].RangeFrom
                , lead.Items[0].RangeTo, lead.Items[0].CompactLabel, lead.Items[0].RecivedOn.HasValue ? lead.Items[0].RecivedOn : (DateTime?)null,
                lead.Items[0].ProjectName, lead.Items[0].AssignedTo, lead.Items[0].BuilderInterest, lead.Items[0].StatusId,
                lead.Items[0].StatusDate == DateTime.MinValue ? (DateTime?)null : lead.Items[0].StatusDate, lead.Items[0].CompanyId);
            return 0;

        }
        public int AddLeadForSMS(Leads lead, string userName)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            object r = adapter.CreateLeadsForSMS(string.Empty, DateTime.Now, lead.Name, lead.PhoneNumber, lead.CmpctLabel, userName, lead.Status);
            return 0;

        }


        public List<Location> GetEmployeesLocation(string userName)
        {
            List<Location> locations = null;
            sp_GetEmployeesLocationTableAdapter adapter = new sp_GetEmployeesLocationTableAdapter();
            LeadWombDataset.sp_GetEmployeesLocationDataTable table = adapter.GetEmployeesLocation(userName);
            if (table != null && table.Rows.Count > 0)
            {
                locations = new List<Location>();
                foreach (LeadWombDataset.sp_GetEmployeesLocationRow row in table.Rows)
                {
                    Location location = new Location();
                    location.LocationID = row.LocationId;
                    location.lng = row.Longitude;
                    location.lat = row.Lattitude;
                    location.description = row.UserName;
                    location.title = row.UserName;
                    location.CompanyID = row.CompanyId;
                    locations.Add(location);
                }
            }
            return locations;
        }

        public void CreateLocation(string userName, string longitude, string lattitude)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            object o = adapter.sp_CreateLocation(userName, longitude, lattitude);

        }
        public LeadStatusCounts GetStatusCountsByMobileUserName(string userName)
        {
            LeadStatusCounts user = new LeadStatusCounts();
            sp_GetStatusCountByMobileUserNameTableAdapter tableAdapter = new sp_GetStatusCountByMobileUserNameTableAdapter();
            LeadWombDataset.sp_GetStatusCountByMobileUserNameDataTable table = tableAdapter.GetStatusCountByMobileUserName(userName);
            int rowCount = 0;
            foreach (LeadWombDataset.sp_GetStatusCountByMobileUserNameRow row in table.Rows)
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
        public int UpdateLeadItem(LeadItems leadItem)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            UpdateLeadItemTableAdapter updateLeadItem = new UpdateLeadItemTableAdapter();
            updateLeadItem.UpdateLeadItem(leadItem.QueryRemarks, leadItem.TypeOfProperty, leadItem.Status, leadItem.RangeFrom, leadItem.RangeTo,
                leadItem.CompactLabel, leadItem.RecivedOn.HasValue || leadItem.RecivedOn == DateTime.MinValue ? leadItem.RecivedOn : (DateTime?)null, leadItem.ProjectName, leadItem.AssignedTo, leadItem.BuilderInterest, leadItem.StatusId,
                leadItem.StatusDate == DateTime.MinValue ? (DateTime?)null : leadItem.StatusDate, leadItem.CompanyId, leadItem.LeadItemID);
            return 0;
        }
        public void PostLeadData(DataTable table, Dictionary<string, string> columnList)
        {
            table.Rows[0].Delete();
            table.AcceptChanges();
            foreach (DataRow row in table.Rows)
            {
                string columnValue = string.Empty;
                columnList.TryGetValue("PhoneNumber", out columnValue);
                if (string.IsNullOrEmpty(columnValue))
                {
                    columnList.TryGetValue("Name", out columnValue);
                }
                if (row[columnValue] == null)
                {
                    row.Delete();
                }
                else
                {
                    if (row[columnValue].ToString() == string.Empty)
                    {
                        row.Delete();
                    }
                }
            }
            table.AcceptChanges();
            using (SqlConnection connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString))
            {
                using (sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (KeyValuePair<string, string> key in columnList)
                    {
                        sqlBulkCopy.ColumnMappings.Add(key.Value, key.Key);
                    }
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 600;
                    sqlBulkCopy.DestinationTableName = "Leads";
                    connection.Open();
                    sqlBulkCopy.WriteToServer(table);
                }
            }
        }
        public DataTable ReadExcel(string fileName, string fileExt)
        {
            SqlConnection sqlConnnection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                 conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  



                }

                catch { }
            }
            return dtexcel;
        }
        public List<Project> GetProjects(long CompanyId)
        {
            List<Project> projects = null;
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_GetProjects";
            command.Parameters.Add(new SqlParameter(Project.DB_COMPANYID, CompanyId));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                projects = new List<Project>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Project project = new Project();
                    project.ProjectId = Convert.ToInt32(row["projectId"]);
                    project.ProjectName = Convert.ToString(row["Name"]);
                    project.Longitude = Convert.ToString(row["Longitude"]);
                    project.Lattitude = Convert.ToString(row["Lattitude"]);
                    project.CompanyId = Convert.ToInt32(row["companyId"]);
                    project.Description = Convert.ToString(row["Description"]);
                    project.District = Convert.ToString(row["District"]);
                    projects.Add(project);
                }
            }
            return projects;

        }
        public List<Attendence> GetAttendance(string userName, DateTime attendanceDate)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetAttendances";
            command.Parameters.Add(new SqlParameter(Attendence.DB_UserName, userName));
            command.Parameters.Add(new SqlParameter(Attendence.DB_Date, attendanceDate));
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
            List<Attendence> attendences = null;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                attendences = new List<Attendence>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Attendence attendence = new Attendence();
                    attendence.UserId = row["Name"].ToString();
                    attendence.Attendance = row["Attendence"] != DBNull.Value ? Convert.ToBoolean(row["Attendence"]) : false;
                    attendence.AttendenceId = Convert.ToInt32(row["AttendanceId"]);
                    attendence.Date = Convert.ToDateTime(row["Date"]).Date;
                    attendence.DateIn = row["DateIn"] != DBNull.Value ? Convert.ToDateTime(row["DateIn"]) : DateTime.MinValue;
                    attendence.DateOut = row["DateOut"] != DBNull.Value ? Convert.ToDateTime(row["DateOut"]) : DateTime.MinValue;
                    attendences.Add(attendence);
                }
            }
            return attendences;
        }
        public bool CreateOrUpdateAttendence(Attendence attendence)
        {
            connection = new SqlConnection(global::LeadWomb.Data.Properties.Settings.Default.LeadPoliceConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_CreateAttendance";
            command.Parameters.Add(new SqlParameter(Attendence.DB_Attendence, attendence.Attendance));
            if (attendence.DateIn != DateTime.MinValue)
            {
                command.Parameters.Add(new SqlParameter(Attendence.DB_DateIn, attendence.DateIn));
            }
            if (attendence.DateOut != DateTime.MinValue)
            {
                command.Parameters.Add(new SqlParameter(Attendence.DB_DateOut, attendence.DateOut));
            }
            command.Parameters.Add(new SqlParameter(Attendence.DB_UserName, attendence.UserName));
            command.Parameters.Add(new SqlParameter(Attendence.DB_Date, attendence.Date));
            command.Parameters.Add(new SqlParameter(Attendence.DB_DistanceIn, attendence.DistanceIn));
            command.Parameters.Add(new SqlParameter(Attendence.DB_DistanceOut, attendence.DistanceOut));
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            return true;
        }




    }
}

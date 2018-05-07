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
        public LeadsRepository()
        {
            adapter = new sp_GetLeadsByUserNameTableAdapter();
            //companyAdapter = new sp_GetLeadsByCompanyTableAdapter();
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
        public IList<Leads> GetLeadsByCompany(string userName, int? statusID)
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

                command.Parameters.Add(new SqlParameter("@userName", userName));
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
                        item.TypeOfProperty = row["TypeOfProperty"] != null ? Convert.ToInt32(row["TypeOfProperty"]) : 0;
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


                        if (row["Email"] != DBNull.Value)
                        {
                            lead.Email = row["Email"].ToString();
                        }
                        item.LeadID = Convert.ToInt32(row["Lead_ID"]);

                        lead.LeadId = Convert.ToInt32(row["Lead_ID"]);
                        lead.Name = row["Name"].ToString();
                        lead.PhoneNumber = row["PhoneNumber"].ToString();

                        leads = CreateOrUpdate(lead, leads, item, assignedUser);
                        //    }
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
        public IList<Leads> GetLeads(string userName, int? statusID)
        {
            List<Leads> leads = null;

            LeadWombDataset.sp_GetLeadsByUserNameDataTable dataTable = adapter.GetLeadsByUserName(userName, statusID);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                leads = new List<Leads>();
                DateTime createDateTimeOffset;
                DateTime editDateTime = DateTime.MinValue;
                foreach (LeadWombDataset.sp_GetLeadsByUserNameRow row in dataTable.Rows)
                {
                    Leads lead = new Leads();
                    LeadItems item = new LeadItems();
                    AssignedUser assignedUser = new AssignedUser();
                    assignedUser.ID = row.Id;
                    assignedUser.AssignedTo = row.AssignedTo;
                    lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
                    item.ProjectName = row[dataTable.ProjNameColumn] != DBNull.Value ? row[dataTable.ProjNameColumn].ToString() : string.Empty;
                    item.QueryRemarks = row[dataTable.QueryRemarksColumn] != DBNull.Value ? row[dataTable.QueryRemarksColumn].ToString() : string.Empty;
                    item.RangeFrom = row[dataTable.RangeFromColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeFromColumn]) : 0;
                    item.RangeTo = row[dataTable.RangeToColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeToColumn]) : 0;
                    item.RecivedOn = row[dataTable.ReceivedOnColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.ReceivedOnColumn]) : DateTime.MinValue;
                    item.Status = row[dataTable.StatusColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.StatusColumn]) : 0;
                    item.StatusDate = row[dataTable.StatusDateColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.StatusDateColumn]) : DateTime.MinValue;
                    item.StatusId = Convert.ToInt32(row[dataTable.StatusIdColumn]);
                    item.AssignedTo = row[dataTable.AssignedToColumn] != null ? row[dataTable.AssignedToColumn].ToString() : string.Empty;
                    item.TypeOfProperty = row[dataTable.TypeOfPropertyColumn] != null ? Convert.ToInt32(row[dataTable.TypeOfPropertyColumn]) : 0;
                    item.LeadItemID = Convert.ToInt32(row["LeadItemId"]);
                    if (row[dataTable.BuilderInterestColumn] != DBNull.Value && row[dataTable.BuilderInterestColumn].ToString() != string.Empty)
                    {
                        item.BuilderInterest = Convert.ToBoolean(row[dataTable.BuilderInterestColumn]);
                    }
                    if (row[dataTable.CmpctLabelColumn] != DBNull.Value)
                    {
                        item.CompactLabel = row[dataTable.CmpctLabelColumn].ToString();
                    }
                    item.CompanyId = Convert.ToInt64(row[dataTable.CompanyIdColumn]);
                    if (DateTime.TryParse(row[dataTable.CreateDateTimeOffsetColumn].ToString(), out createDateTimeOffset))
                    {
                        lead.CreateDateTime = createDateTimeOffset;
                    }
                    lead.CreateUserID = row[dataTable.CreateUser_IDColumn].ToString();
                    lead.EditUserId = row[dataTable.EditUser_IDColumn] != null ? row[dataTable.EditUser_IDColumn].ToString() : string.Empty;


                    if (row[dataTable.EmailColumn] != DBNull.Value)
                    {
                        lead.Email = row[dataTable.EmailColumn].ToString();
                    }
                    item.LeadID = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
                    lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
                    lead.Name = row[dataTable.NameColumn].ToString();
                    lead.PhoneNumber = row[dataTable.PhoneNumberColumn].ToString();
                    leads = CreateOrUpdateForUser(lead, leads, item);
                }
            }
            return leads;
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
                    row[leadIDColumn] = item.LeadID;
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
                lead.LeadId, lead.EditUserId, DateTime.Now, lead.Name, lead.Email, lead.PhoneNumber);
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
                    location.lat= row.Lattitude;
                    location.description = row.UserId;
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
        public int UpdateLeadItem(LeadItems leadItem)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            UpdateLeadItemTableAdapter updateLeadItem = new UpdateLeadItemTableAdapter();
            updateLeadItem.UpdateLeadItem(leadItem.QueryRemarks, leadItem.TypeOfProperty, leadItem.Status, leadItem.RangeFrom, leadItem.RangeTo,
                leadItem.CompactLabel, leadItem.RecivedOn.HasValue || leadItem.RecivedOn == DateTime.MinValue ? leadItem.RecivedOn : (DateTime?)null, leadItem.ProjectName, leadItem.AssignedTo, leadItem.BuilderInterest, leadItem.StatusId,
                leadItem.StatusDate == DateTime.MinValue ? (DateTime?)null : leadItem.StatusDate, leadItem.CompanyId, leadItem.LeadItemID);
            return 0;
        }

    }
}

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
        public LeadsRepository()
        {
            adapter = new sp_GetLeadsByUserNameTableAdapter();
        }
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
                foreach (DataRow row in dataTable.Rows)
                {
                    Leads lead = new Leads();
                    LeadItems item = new LeadItems();
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
                    leads = CreateOrUpdate(lead, leads, item);
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
        private List<Leads> CreateOrUpdate(Leads item, List<Leads> listItems, LeadItems listItem)
        {
            Leads lead = listItems.Find(x => x.PhoneNumber == item.PhoneNumber && x.LeadId == item.LeadId);
           
           
            if (lead != null)
            {
                if (lead.Items == null)
                {
                    lead.Items = new List<LeadItems>();
                }
                lead.Items.Add(listItem);
            }
            else
            {
                //fresh lead..
                if (item.Items == null)
                {
                    item.Items = new List<LeadItems>();
                }
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
                    row[statusDateColumn] = item.StatusDate==DateTime.MinValue ? SqlDateTime.MinValue.Value :  item.StatusDate;
                    row[companyIDColumn] = item.CompanyId;
                    table.Rows.Add(row);
                }

            }
            new GetLeadsByCompanyIdandStatusIdTableAdapter().UpdateLeads(table,
                lead.LeadId, lead.EditUserId,DateTime.Now, lead.Name, lead.Email, lead.PhoneNumber);
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

    }
}

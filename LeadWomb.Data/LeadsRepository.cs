using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Data.LeadWombDatasetTableAdapters;
using LeadWomb.Model;
using LeadWomb.Data;
using System.Data;
namespace LeadWomb.Data
{
    public class LeadsRepository
    {
        private sp_GetLeadsByUserNameTableAdapter adapter = null;
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
          IList<Leads> leads = null;
          LeadWombDataset.sp_GetLeadsByUserNameDataTable dataTable = adapter.GetLeadsByUserName(userName, statusID);
          if (dataTable != null && dataTable.Rows.Count > 0)
          {
              leads = new List<Leads>();
              DateTimeOffset createDateTimeOffset;
              DateTime editDateTime = DateTime.MinValue;
              foreach (DataRow row in dataTable.Rows)
              {
                 Leads lead = new Leads();
                 lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
                 if (row[dataTable.BuilderInterestColumn] != DBNull.Value && row[dataTable.BuilderInterestColumn].ToString() != string.Empty)
                 {
                     lead.BuilderInterest = Convert.ToBoolean(row[dataTable.BuilderInterestColumn]);
                 }
                 if (row[dataTable.CmpctLabelColumn] != DBNull.Value)
                 {
                     lead.CompactLabel = row[dataTable.CmpctLabelColumn].ToString();
                 }
                 lead.CompanyId = Convert.ToInt64(row[dataTable.CompanyIdColumn]);
                 if (DateTimeOffset.TryParse(row[dataTable.CreateDateTimeOffsetColumn].ToString(), out createDateTimeOffset))
                 {
                     lead.CreateDateTime = createDateTimeOffset.DateTime;
                 }
                 lead.CreateUserID = Convert.ToInt32(row[dataTable.CreateUser_IDColumn]);

                 if (row[dataTable.EmailColumn] != DBNull.Value)
                  {
                    lead.Email = row[dataTable.EmailColumn].ToString();
                  }
                 lead.LeadId = Convert.ToInt32(row[dataTable.Lead_IDColumn]);
                 lead.Name = row[dataTable.NameColumn].ToString();
                 lead.PhoneNumber = row[dataTable.PhoneNumberColumn].ToString();
                 lead.ProjectName = row[dataTable.ProjNameColumn] != DBNull.Value ? row[dataTable.ProjNameColumn].ToString() : string.Empty;
                 lead.QueryRemarks = row[dataTable.QueryRemarksColumn] != DBNull.Value ? row[dataTable.QueryRemarksColumn].ToString() : string.Empty;
                 lead.RangeFrom = row[dataTable.RangeFromColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeFromColumn]) : 0;
                 lead.RangeTo = row[dataTable.RangeToColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.RangeToColumn]) : 0;
                 lead.RecivedOn = row[dataTable.ReceivedOnColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.ReceivedOnColumn]) : DateTime.MinValue;
                 lead.Status = row[dataTable.StatusColumn] != DBNull.Value ? Convert.ToInt32(row[dataTable.StatusColumn]) : 0;
                 lead.StatusDate = row[dataTable.StatusDateColumn] != DBNull.Value ? Convert.ToDateTime(row[dataTable.StatusDateColumn]) : DateTime.MinValue;
                 lead.StatusId = Convert.ToInt32(row[dataTable.StatusIdColumn]);
                 leads.Add(lead);
              }
          }
          return leads;
        }
         ////<summary>
         
         ////</summary>
        public bool UpdateLeads(Leads lead)
        {
            new GetLeadsByCompanyIdandStatusIdTableAdapter().UpdateLeads(
                lead.LeadId, lead.EditUserId, DateTimeOffset.Now, lead.Name, lead.Email, lead.PhoneNumber, lead.QueryRemarks, lead.TypeOfProperty, lead.StatusId,
                lead.RangeFrom, lead.RangeTo, lead.CompactLabel, Utilities.CheckDateTimeIfNullOrEmpty(lead.RecivedOn), lead.ProjectName, lead.AssignedTo, lead.BuilderInterest
                , lead.StatusId, Utilities.CheckDateTimeOffsetIfNullOrEmpty(lead.StatusDate), lead.CompanyId);
            return true;
        }

    }
}

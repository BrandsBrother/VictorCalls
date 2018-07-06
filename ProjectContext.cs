using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeadWomb.Data.AccountAdapterTableAdapters;
using LeadWomb.Model;
using LeadWomb.Models;

namespace LeadWomb.Data
{
   public class ProjectContext
    {
       private QueriesTableAdapter tableAdapter = new QueriesTableAdapter();

       public void CreateProject(Project project)
       {
           tableAdapter.CreateProject(project.ProjectName,project.Description,project.Distict,project.Longtitute,
               project.Latitute,project.CompanyId);
         
       }

       public void UpdateProjectDatabyID( Project user)
       {
           tableAdapter.updateProject(user.ProjectId, user.ProjectName, user.Description, user.Distict,
               user.Longtitute, user.Latitute, user.CompanyId);
       }

       public void DeleteProject(int projectId)
       {
           tableAdapter.deleteProjectbyID(projectId);
       }
     public List<Project> GetProjectDataById(int projectId)
       {
           List<Project> users = null;
           Sp_GetProjectTableAdapter adapter = new Sp_GetProjectTableAdapter();
           AccountAdapter.Sp_GetProjectDataTable dataTable = adapter.GetProjectDatabyId(projectId);
         if(dataTable!=null && dataTable.Rows.Count>0)
         {
             users=new List<Project>();
             foreach(AccountAdapter.Sp_GetProjectRow row in dataTable.Rows)
             {
                 Project user = new Project();
                 user.ProjectId = row.projectId;
                 user.ProjectName = row.projectName;
                 user.Description = row.proDescription;
                 user.Longtitute = row.longtitute;
                 user.Latitute = row.latitute;
                 user.Distict = row.Distict;
                 user.CompanyId =Convert.ToInt32(row.companyId);
                 users.Add(user);
             }
         }
         return users;
       }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeadWomb.Data;
using LeadWomb.Model;
using LeadWomb.Models;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/project")]
    public class ProjectController : ApiController
    {
        private ProjectContext _repo = null;
        public ProjectController()
        {
            _repo = new ProjectContext();
        }
        [Route("Id")]
        public IHttpActionResult Get(int projectId)
        {
            return Ok(_repo.GetProjectDataById(projectId));
        }

        [Route("create")]
        public IHttpActionResult Post([FromBody]Project project)
        {
            _repo.CreateProject(project);
            return Ok();
        }

        [Route("Id")]
        public IHttpActionResult Put([FromBody] Project users)
        {
            _repo.UpdateProjectDatabyID(users);
            return Ok();

        }

        [Route("delete")]
        public IHttpActionResult Delete(int projectId)
        {
            _repo.DeleteProject(projectId);
            return Ok();
        }
    }
}

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NISA.DSSelfAPI.WebApi.Controllers
{
    public class VersionController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try

            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;

                return Request.CreateResponse(HttpStatusCode.OK, version);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}

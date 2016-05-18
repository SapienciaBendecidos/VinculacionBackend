using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using System.Web.OData;
using VinculacionBackend.Data.Entities;
using VinculacionBackend.Exceptions;
using VinculacionBackend.Interfaces;

namespace VinculacionBackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MajorsController : ApiController
    {
        private readonly IMajorsServices _majorsServices;

        public MajorsController(IMajorsServices majorsServices)
        {
            _majorsServices = majorsServices;
        }

        // GET: api/Majors  
        [EnableQuery]        
         
        [Route("api/Majors")]
        public IQueryable<Major> GetMajors()
        {
           return _majorsServices.All();
        }

        // GET: api/Majors/5
        [ResponseType(typeof(Major))]
        [Route("api/Majors/{majorId}")]
        public IHttpActionResult GetMajor(string majorId)
        {
            Major major = _majorsServices.Find(majorId);
            return Ok(major);
        }

     
       
    }
}
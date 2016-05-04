﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using VinculacionBackend.Models;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.OData;
using VinculacionBackend.Data.Entities;
using VinculacionBackend.ActionFilters;
using VinculacionBackend.CustomDataNotations;
using VinculacionBackend.Interfaces;
using VinculacionBackend.Security.BasicAuthentication;
using VinculacionBackend.Security.Interfaces;

namespace VinculacionBackend.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [NotFoundExceptionFilter]
    public class StudentsController : ApiController
    {            
        private readonly IStudentsServices _studentsServices;
        private readonly IEmail _email;
        private readonly IEncryption _encryption;

        public StudentsController(IStudentsServices studentServices,IEmail email,IEncryption encryption)
        {
            _studentsServices = studentServices;
            _email = email;
            _encryption = encryption;
        }

        // GET: api/Students
        [System.Web.Http.Route("api/Students")]
        [CustomAuthorize(Roles = "Admin,Professor")]
        [EnableQuery]
       
        public IQueryable<User> GetStudents()
        {
            return _studentsServices.AllUsers();
        }

        // GET: api/Students/5

        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students/{accountId}")]
        [CustomAuthorize(Roles = "Admin,Professor,Student")]
        public IHttpActionResult GetStudent(string accountId)
        {
            var student = _studentsServices.Find(accountId);
            return Ok(student);
        }


        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students/{accountId}/Hour")]
        [CustomAuthorize(Roles = "Admin,Professor,Student")]
        public IHttpActionResult GetStudentHour(string accountId)
        {
            var total = _studentsServices.GetStudentHours(accountId);
            return Ok(total);
        }

        [System.Web.Http.Route("api/Students/Filter/{status}")]
        [CustomAuthorize(Roles = "Admin,Professor")]
        public IQueryable<User> GetStudents(string status)
        {
            return _studentsServices.ListbyStatus(status);

        }

        //Put: api/Students/Verified
        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students/Verified")]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateModel]
        public IHttpActionResult PutAcceptVerified(VerifiedModel model) 
        {
            var student = _studentsServices.VerifyUser(model.AccountId);
            _email.Send(student.Email, "Fue Aceptado para participar en Projectos de Vinculación", "Vinculación");
            return Ok(student);
        }

        //Get: api/Students/Avtive
        [ResponseType(typeof(User))] 
        [System.Web.Http.Route("api/Students/{guid}/Active")]
        public IHttpActionResult GetActiveStudent(string guid)
        {
            var accountId = _encryption.Decrypt(HttpContext.Current.Server.UrlDecode(guid));
            var student = _studentsServices.ActivateUser(accountId);
            return Ok(student);
        }

        //Post: api/Students/Rejected
        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students/Rejected")]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateModel]
        public IHttpActionResult PostRejectStudent(RejectedModel model)
        {
            var student = _studentsServices.RejectUser(model.AccountId);
            _email.Send(student.Email, model.Message, "Vinculación");
             return Ok(student);
        }
        // POST: api/Students
        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students")]
        [CustomAuthorize(Roles = "Anonymous")]
        [ValidateModel]
        public IHttpActionResult PostStudent(UserEntryModel userModel)
        {
            var newUser = _studentsServices.Map(userModel);
            _studentsServices.Add(newUser);
            var stringparameter = _encryption.Encrypt(newUser.AccountId);
            _email.Send(newUser.Email, "Hacer click en el siguiente link para Activar: " + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/api/Students/" + HttpContext.Current.Server.UrlEncode(stringparameter) + "/Active", "Vinculación");
            return Ok(newUser);
        }
        // DELETE: api/Students/5
        [ResponseType(typeof(User))]
        [System.Web.Http.Route("api/Students/{accountId}")]
        [CustomAuthorize(Roles = "Admin,Professor")]
        public IHttpActionResult DeleteStudent(string accountId)
        {
            User user = _studentsServices.DeleteUser(accountId);
            return Ok(user);
        }
    }
}
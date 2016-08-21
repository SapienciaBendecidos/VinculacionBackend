﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using OfficeOpenXml;
using VinculacionBackend.Extensions;
using VinculacionBackend.Interfaces;

namespace VinculacionBackend.Controllers
{
    public class MyStreamProvider : MultipartFormDataStreamProvider
    {
        public MyStreamProvider(string uploadPath)
            : base(uploadPath)
        {

        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = headers.ContentDisposition.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".data";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ReportsController : ApiController
    {
        private readonly IFacultiesServices _facultiesServices;
        private readonly ISheetsReportsServices _reportsServices;
        private readonly IStudentsServices _studentServices;
        private readonly IClassesServices _classesServices;
        private readonly IProjectServices _projectServices;


        public ReportsController(IFacultiesServices facultiesServices, ISheetsReportsServices reportsServices, IStudentsServices studentsServices, IProjectServices projectServices, IClassesServices classesServices)
        {
            _facultiesServices = facultiesServices;
            _reportsServices = reportsServices;
            _projectServices = projectServices;
            _classesServices = classesServices;
            _studentServices = studentsServices;
        }


        [Route("api/Reports/CostsReport/{year}")]
        public IHttpActionResult GetCostsReport(int year)
        {
            var context = _reportsServices.GenerateReport(_facultiesServices.CreateFacultiesCostReport(year).ToDataTable(),
                "Reporte de Costos por Facultad");
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        [Route("api/Reports/ProjectsByMajorReport")]
        public IHttpActionResult GetProjectCountByMajorReport()
        {
            var context = _reportsServices.GenerateReport(_projectServices.CreateProjectsByMajor(),
                "Proyectos por Carrera");
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        [Route("api/Reports/HoursReport/{year}")]
        public IHttpActionResult GetHoursReport(int year)
        {
            var context = _reportsServices.GenerateReport(_facultiesServices.CreateFacultiesHourReport(year),
                "Reporte de Horas por Facultad");
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        [Route("api/Reports/StudentsReport/{year}")]
        public IHttpActionResult GetStudentsReport(int year)
        {
            var context = _reportsServices.GenerateReport(_studentServices.CreateStudentReport(year),
                "Reporte de Alumnos");
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        [Route("api/Reports/ProjectsByClassReport/{classId}")]
        public IHttpActionResult GetProjectsByClassReport(long classId)
        {
            var context = _reportsServices.GenerateReport(_projectServices.ProjectsByClass(classId), "Projectos Por "+_classesServices.Find(classId).Name);
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        [Route("api/Reports/PeriodReport/{year}")]
        public IHttpActionResult GetPeriodReport(int year)
        {
            var context = _reportsServices.GenerateReport(_projectServices.CreatePeriodReport(year, 1),
                1 + " " + year);
            context.Response.Flush();
            context.Response.End();
            return Ok();
        }

        public async Task<List<string>> UploadStudents()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/Files");

                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);

                await Request.Content.ReadAsMultipartAsync(streamProvider);

                foreach (var file in streamProvider.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    if (Path.GetExtension(fi.Name) == ".xlsx")
                    {
                        ExcelPackage package = new ExcelPackage(fi);
                        var enumerable = package.ToDataTable();
                    }
                }

                return new List<string>();
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

    }
}
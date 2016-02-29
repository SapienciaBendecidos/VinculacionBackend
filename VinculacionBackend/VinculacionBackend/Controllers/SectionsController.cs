﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VinculacionBackend.Database;
using VinculacionBackend.Entities;

namespace VinculacionBackend.Controllers
{
    public class SectionsController : ApiController
    {
        private VinculacionContext db = new VinculacionContext();

        // GET: api/Sections
        [Route("api/Sections")]
        public IQueryable<Section> GetSections()
        {
            var sections = db.Sections.Include(a => a.Class).Include(b => b.User).Include(c => c.Period);
            return sections;
        }

        // GET: api/Sections/5
        [Route("api/Sections/{sectionId}")]
        [ResponseType(typeof(Section))]
        public IHttpActionResult GetSection(long sectionId)
        {
            var section = db.Sections.Include(a => a.Class).Include(b => b.User).Include(c => c.Period).FirstOrDefault(d => d.Id == sectionId);
            if (section == null)
            {
                return NotFound();
            }

            return Ok(section);
        }

        // PUT: api/Sections/5
        [ResponseType(typeof(void))]
        [Route("api/Sections/{sectionId}")]
        public IHttpActionResult PutSection(long sectionId, Section section)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (sectionId != section.Id)
            {
                return BadRequest();
            }

            var tmpSection = db.Sections.FirstOrDefault(x => x.Id == sectionId);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectionExists(sectionId))
                {
                    return NotFound();
                }
                else
                {
                    return InternalServerError(new DbUpdateConcurrencyException());
                }
            }

            return Ok(tmpSection);
        }

        // POST: api/Sections
        [Route("api/Sections")]
        [ResponseType(typeof(Section))]
        public IHttpActionResult PostSection(Section section)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sections.Add(section);
            db.SaveChanges();

            return Ok(section);
        }

        // DELETE: api/Sections/5
        [ResponseType(typeof(Section))]
        [Route("api/Sections/{sectionId}")]
        public IHttpActionResult DeleteSection(long sectionId)
        {
            Section section = db.Sections.Find(sectionId);
            if (section == null)
            {
                return NotFound();
            }

            //db.Sections.Remove(section);
            //db.SaveChanges();

            return Ok(section);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SectionExists(long sectionId)
        {
            return db.Sections.Count(e => e.Id == sectionId) > 0;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using VinculacionBackend.Data.Entities;

namespace VinculacionBackend.Data.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        void AssignStudent(long sectionId, List<string> studenstIds);
        void RemoveStudent(long sectionId, List<string> studentsIds);
        IQueryable<User> GetSectionStudents(long sectionId);
        IQueryable<Project> GetSectionProjects(long sectionId);
    }
}

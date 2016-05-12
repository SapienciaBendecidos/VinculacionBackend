using System.CodeDom;
using System.Linq;
using VinculacionBackend.Data.Entities;
using VinculacionBackend.Data.Interfaces;
using VinculacionBackend.Exceptions;
using VinculacionBackend.Interfaces;
using VinculacionBackend.Models;

namespace VinculacionBackend.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ISectionsServices _sectionServices;

        public ProjectServices(IProjectRepository projectRepository, ISectionRepository sectionRepository, ISectionsServices sectionServices)
        {
            _projectRepository = projectRepository;
            _sectionServices = sectionServices;
          
        }

        public Project Find(long id)
        {
            var project = _projectRepository.Get(id);
            if (project==null)
                throw new NotFoundException("No se encontro el proyecto");
            return project;
        }

        public IQueryable<Project> All()
        {
            return _projectRepository.GetAll();
        }


        private Project Map(ProjectModel model)
        {
            var newProject = new Project();
            newProject.ProjectId = model.ProjectId;
            newProject.Name = model.Name;
            newProject.Description = model.Description;
            newProject.Cost = model.Cost;
            newProject.MajorIds = model.MajorIds;
            newProject.SectionId = model.SectionId;
            newProject.BeneficiariesAlias = model.BenificiariesAlias;
            newProject.BeneficiariesQuantity = model.BenificiariesQuantity;
            return newProject;
        }

        public Project Add(ProjectModel model)
        {
            var project = Map(model);
            _projectRepository.Insert(project, model.MajorIds,model.SectionId);
            _projectRepository.Save();
            return project;
        }

        public Project Delete(long projectId)
        {
            var project = _projectRepository.Delete(projectId);
            if (project == null)
                throw new NotFoundException("No se encontro el proyecto");
            _projectRepository.Save();
            return project;
        }

        public IQueryable<User> GetProjectStudents(long projectId)
        {
            return _projectRepository.GetProjectStudents(projectId);
        }

        public Project UpdateProject(long projectId, ProjectModel model)
        {
            var tmpProject = _projectRepository.Get(projectId);
            if (tmpProject == null)
                throw new NotFoundException("No se encontro el proyecto");
            tmpProject.ProjectId = model.ProjectId;
            tmpProject.Name = model.Name;
            tmpProject.Description = model.Description;
            tmpProject.Cost = model.Cost;
            tmpProject.MajorIds = model.MajorIds;
            tmpProject.SectionId = model.SectionId;
            tmpProject.BeneficiariesAlias = model.BenificiariesAlias;
            tmpProject.BeneficiariesQuantity = model.BenificiariesQuantity;
            _projectRepository.Update(tmpProject);
            _projectRepository.Save();
            return tmpProject;
        }

        public bool AssignSection(ProjectSectionModel model)
        {
            var project = Find(model.SectionId);
            var section = _sectionServices.Find(model.SectionId);
            _projectRepository.AssignToSection(model.ProjectId, model.SectionId);
            _projectRepository.Save();
            return true;
        }
    }

}
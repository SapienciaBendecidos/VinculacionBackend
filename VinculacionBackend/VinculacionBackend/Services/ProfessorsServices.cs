﻿using System;
using System.Linq;
using VinculacionBackend.Data.Entities;
using VinculacionBackend.Data.Enums;
using VinculacionBackend.Data.Interfaces;
using VinculacionBackend.Exceptions;
using VinculacionBackend.Interfaces;
using VinculacionBackend.Models;

namespace VinculacionBackend.Services
{
    public class ProfessorsServices:IProfessorsServices
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly IEncryption _encryption;

        public ProfessorsServices(IProfessorRepository professorRepository, IEncryption encryption)
        {
            _professorRepository = professorRepository;
            _encryption = encryption;
        }

        public void Map( User professor,ProfessorEntryModel professorModel)
        {
            professor.AccountId = professorModel.AccountId;
            professor.Name = professorModel.Name;
            professor.Password = _encryption.Encrypt(professorModel.Password);
            professor.Major = null;
            professor.Campus = professorModel.Campus;
            professor.Email = professorModel.Email;
            professor.Status = Status.Verified;
            professor.CreationDate = DateTime.Now;
            professor.ModificationDate = DateTime.Now;
        }


        public void PutMap(User professor, ProfessorEntryModel professorModel)
        {
            professor.AccountId = professorModel.AccountId;
            professor.Name = professorModel.Name;
            professor.Password = _encryption.Encrypt(professorModel.Password);
            professor.Major = null;
            professor.Campus = professorModel.Campus;
            professor.Email = professorModel.Email;
            professor.ModificationDate = DateTime.Now;
        }


        public void AddProfessor(User professor)
        {
            _professorRepository.Insert(professor);
            _professorRepository.Save();
        }

        public User Find(string accountId)
        {
            var professor = _professorRepository.GetByAccountId(accountId);
            if(professor==null)
                throw new NotFoundException("No se encontro el profesor");
            return professor;
        }

        public User DeleteProfessor(string accountId)
        {
            var professor = _professorRepository.DeleteByAccountNumber(accountId);
            if(professor==null)
                throw new NotFoundException("No se encontro el profesor");
            _professorRepository.Save();
            return professor;
        }

        public IQueryable<User> GetProfessors()
        {
            return _professorRepository.GetAll();
        }


        public User UpdateProfessor(string accountId,ProfessorEntryModel model)
        {
            var professor = _professorRepository.GetByAccountId(accountId);
            if (professor == null)
                throw new NotFoundException("No se encontro el professor");
            PutMap(professor, model);
            _professorRepository.Update(professor);
            _professorRepository.Save();
            return professor;
        }
    }
}
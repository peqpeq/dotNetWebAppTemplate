using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.DAL.App.Repositories;
using DAL.App.DTO;
using DAL.App.DTO.Identity;
using DAL.App.EF.Mappers;
using DAL.Base.EF.Repositories;
using Domain.App.Entity;

namespace DAL.App.EF.Repositories
{
    public class TestEntityRepository: BaseRepository<AppDbContext, AppUserDAL, TestEntity, TestEntityDAL>, ITestEntityRepository
    {
        public TestEntityRepository(AppDbContext repoDbContext) : base(repoDbContext, new DALMapper<TestEntity, TestEntityDAL>())
        {
            
        }
        


    }

}
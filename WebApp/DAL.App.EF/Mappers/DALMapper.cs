using AutoMapper;
using DAL.App.DTO;
using DAL.App.DTO.Identity;
using DAL.Base.Mappers;
using Domain.App.Entity;
using Domain.App.Identity;

namespace DAL.App.EF.Mappers
{
public class DALMapper<TLeftObject, TRightObject> : BaseDALMapper<TLeftObject, TRightObject>
        where TRightObject : class?, new()
        where TLeftObject : class?, new()
    {
        public DALMapper() : base()
        {
            
            // Identity AppUser
            MapperConfigurationExpression.CreateMap<AppUser, AppUserDAL>();
            MapperConfigurationExpression.CreateMap<AppUserDAL, AppUser>();

            // Identity AppRole
            MapperConfigurationExpression.CreateMap<AppRole, AppRoleDAL>();
            MapperConfigurationExpression.CreateMap<AppRoleDAL, AppRole>();
            
            // Identity TestEntity
            MapperConfigurationExpression.CreateMap<TestEntity, TestEntityDAL>();
            MapperConfigurationExpression.CreateMap<TestEntityDAL, TestEntity>();
            
            Mapper = new Mapper(new MapperConfiguration(MapperConfigurationExpression));
        }
    }

}
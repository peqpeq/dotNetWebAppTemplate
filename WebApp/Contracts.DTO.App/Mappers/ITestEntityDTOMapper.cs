using Contract.DTO.Base;
using Domain.App.Entity;
using PublicApi.DTO.v1.DTO.Items;

namespace Contracts.DTO.App.Mappers
{
    public interface ITestEntityDTOMapper: IDTOMapper<TestEntity, TestEntityDTO>
    {
        public TestEntity Map(TestEntityDTO inObject);
        public TestEntityDTO Map(TestEntity inObject);
    }
}
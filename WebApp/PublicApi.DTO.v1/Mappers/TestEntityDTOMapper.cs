using Contract.DTO.Base;
using DAL.App.DTO;
using Domain.App.Entity;
using PublicApi.DTO.v1.DTO.Items;

namespace PublicApi.DTO.v1.Mappers
{
    public class TestEntityDTOMapper: IDTOMapper<TestEntityDAL, TestEntityDTO>
    {
        public TestEntityDAL Map(TestEntityDTO inObject)
        {
            return new TestEntityDAL()
            {
                Id = inObject.Id,
                CreatedAt = inObject.CreatedAt,
                CreatedBy = inObject.CreatedBy
            };
        }

        public new TestEntityDTO Map(TestEntityDAL inObject)
        {
            return new TestEntityDTO()
            {
                Id = inObject.Id,
                CreatedAt = inObject.CreatedAt,
                CreatedBy = inObject.CreatedBy
            };
        }

    }
}
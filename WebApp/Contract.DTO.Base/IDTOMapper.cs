using System;

namespace Contract.DTO.Base
{
    public interface IDTOMapper<TLeftObject, TRightObject>
    {
        public TLeftObject Map(TRightObject inObject);
        public TRightObject Map(TLeftObject inObject);

    }

}
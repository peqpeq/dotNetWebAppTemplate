using Contracts.DAL.Base.Mappers;

namespace DAL.Base.Mappers
{
    /// <summary>
    /// Maps using Automapper. No mapper configuration. Property types and names have to match.
    /// </summary>
    /// <typeparam name="TLeftObject"></typeparam>
    /// <typeparam name="TRightObject"></typeparam>
    public class IdentityDALMapper<TLeftObject, TRightObject> : IBaseDALMapper<TLeftObject, TRightObject> 
        where TRightObject : class?, new() 
        where TLeftObject : class?, new()
    {
        public TRightObject? Map(TLeftObject inObject)
        {
            return inObject as TRightObject ?? default!;
        }

        public TLeftObject? Map(TRightObject inObject)
        {
            return inObject as TLeftObject ?? default!;
        }
    }

}
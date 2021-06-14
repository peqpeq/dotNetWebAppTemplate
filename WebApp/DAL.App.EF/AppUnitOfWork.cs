using Contracts.DAL.App;
using Contracts.DAL.App.Repositories;
using DAL.App.EF.Repositories;
using DAL.Base.EF;

namespace DAL.App.EF
{
public class AppUnitOfWork : EFBaseUnitOfWork<AppDbContext>, IAppUnitOfWork
    {

        public AppUnitOfWork(AppDbContext uowDbContext) : base(uowDbContext)
        {
        }
       
        public ITestEntityRepository TestEntityRepository =>
            GetRepository<ITestEntityRepository>(() => new TestEntityRepository(UOWDbContext));
        
    }

}
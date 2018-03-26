using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class LoggingRepository : EntityBaseRepository<Error>, ILoggingRepository, ISqlRepository
    {
        public LoggingRepository(GalleryContext context)
            : base(context)
        { }

        public override async Task CommitAsync()
        {
            try
            {
                await base.CommitAsync();
            }
            catch
            {
                //todo
            }
        }
    }
}

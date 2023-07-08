using Microsoft.EntityFrameworkCore;

namespace MedicineCoreApi.Context
{
    public class MedicineCoreApiContext : DbContext
    {
        public MedicineCoreApiContext(DbContextOptions<MedicineCoreApiContext> options) : base(options) { }
        public DbSet<PostMedicine> Medicines { get; set; }

    }

}

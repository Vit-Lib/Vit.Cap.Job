using Microsoft.EntityFrameworkCore;

namespace Vit.Cap.Job.Entity
{
    public partial class VitCapJobDBContext : Vit.Orm.EntityFramework.Index.DbContext
    {
        public VitCapJobDBContext(DbContextOptions<VitCapJobDBContext> options)
            : base(options)
        {
   
        } 
    }
}
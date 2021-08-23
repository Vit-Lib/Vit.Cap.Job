using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vit.Core.Util.ComponentModel.Model;


namespace Vit.Cap.Job.Entity
{
    public partial class VitCapJobDBContext
    {
        public DbSet<JobLock> JobLock { get; set; }
    }
}



namespace Vit.Cap.Job.Entity
{
    [Table(JobLock.TableName)]
    public class JobLock
    {
        public const string TableName = "vit_cap_job.job_lock";

        [Key]
        public int jobId { get; set; }

    }
}

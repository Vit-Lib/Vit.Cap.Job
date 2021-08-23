using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vit.Core.Util.ComponentModel.Model;

namespace Vit.Cap.Job.Entity
{
    public partial class VitCapJobDBContext
    {
        public DbSet<JobStatus> JobStatus { get; set; }
    }
}



namespace Vit.Cap.Job.Entity
{
    [Table("vit_cap_job.job_status")]
    public class JobStatus
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }



        [Vit.Orm.EntityFramework.Index.Index]
        public int jobId { get; set; }
        //[SsExample("job_000001")]
        //[StringLength(100)]
        //[Vit.Orm.EntityFramework.Index.Index]
        //public string jobGuid { get; set; }

        /// <summary>
        /// 任务完成度  [0,100]
        /// </summary>
        [SsExample("20.15")]
        public float? percent { get; set; }


        [SsExample("解压中,预计剩余时间2分钟")]
        [StringLength(1000)]
        public string remarks { get; set; }


        /// <summary>
        /// 任务状态参数
        /// </summary>
        [SsExample("{\"type\":\"zip\",\"resGuid\":\"res_001\"}")]
        [StringLength(2000)]
        public string statusArgs { get; set; }

        /// <summary>
        /// 更新状态时间
        /// </summary>
        public DateTime?time { get; set; }
    }
}

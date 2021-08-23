using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Extensions;

namespace Vit.Cap.Job.Entity
{
    public partial class VitCapJobDBContext
    {
        public DbSet<JobInfo> JobInfo { get; set; }
    }
}



namespace Vit.Cap.Job.Entity
{
    [Table("vit_cap_job.job_info")]
    public class JobInfo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        #region ref      
        
        /// <summary>
        /// 外部对象的id
        /// </summary>
        [SsExample("job_000001")]
        [StringLength(500)]
        public string ref_id { get; set; }

        /// <summary>
        /// 外部对象的类型
        /// </summary>
        [SsExample("demo")]
        [StringLength(100)]
        public string ref_type { get; set; }

        /// <summary>
        /// 外部对象的名称
        /// </summary>
        [SsExample("job_000001")]
        [StringLength(1000)]
        public string ref_name { get; set; }

        /// <summary>
        /// 外部对象的备注
        /// </summary>
        [StringLength(2000)]
        public string ref_remark { get; set; }
        #endregion


        /// <summary>
        /// 任务工作组
        /// </summary>
        [SsExample("pool1")]
        [StringLength(500)]
        public string jobGroup { get; set; }

        /// <summary>
        /// 任务名称（任务类型）
        /// </summary>
        [SsExample("解压文件")]
        [StringLength(100)]
        public string jobName { get; set; }


        /// <summary>
        /// 任务完成度  [0,100]
        /// </summary>
        [SsExample("20.15")]
        public float? percent { get; set; }


        #region jobArg        

        /// <summary>
        /// 任务参数
        /// </summary>
        [SsExample("{\"type\":\"zip\",\"resGuid\":\"res_001\"}")]
        [StringLength(2000)]
        public string jobArg { get; set; }


        public T GetArg<T>()
        {
            return jobArg.Deserialize<T>();
        }

        public void SetArg<T>(T args)
        {
            jobArg = args.Serialize();
        }
        #endregion







        /// <summary>
        /// 任务状态： 等待处理，处理中，已处理，处理出现错误
        /// </summary>
        [SsExample("等待处理")]
        [StringLength(100)]
        public string jobStatus { get; set; }


        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? publicTime { get; set; }

        /// <summary>
        /// 开始处理时间
        /// </summary>
        public DateTime? beginTime { get; set; }


        /// <summary>
        /// 处理结束时间
        /// </summary>
        public DateTime? endTime { get; set; }





        public JobStatus NewStatus(float? percent)
        {
            return new JobStatus { jobId = this.id, percent = percent };
        }


    }
}

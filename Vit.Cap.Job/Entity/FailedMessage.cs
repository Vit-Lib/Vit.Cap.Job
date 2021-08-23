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
        public DbSet<FailedMessage> FailedMessage { get; set; }
    }
}



namespace Vit.Cap.Job.Entity
{
    [Table("vit_cap_job.failed_message")]
    public class FailedMessage
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        [StringLength(1000)]
        public string MessageId { get; set; }


        [StringLength(1000)]
        public string MessageType { get; set; }


        [StringLength(1000)]
        public string Name { get; set; }



 
        public string Headers { get; set; }


        public string Value { get; set; }



        public DateTime? time { get; set; }



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



        #region ext

        [StringLength(1000)]
        public string ext_dealed { get; set; }


        [StringLength(1000)]
        public string ext_status { get; set; }



        [StringLength(1000)]
        public string ext_remarks { get; set; }


        public DateTime? ext_time { get; set; }

        #endregion
    }
}

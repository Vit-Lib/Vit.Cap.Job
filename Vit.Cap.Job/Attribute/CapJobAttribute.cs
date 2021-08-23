using DotNetCore.CAP;
using Vit.Cap.Job.Entity;

namespace Vit.Cap.Job.Attribute
{
    /// <summary>
    ///  若不指定jobGroup，则使用配置文件 appsettings.json::Cap.jobGroup 的值
    /// </summary>
    public class CapJobAttribute : CapSubscribeAttribute
    {
   

        /// <summary>
        /// 若不指定jobGroup，则使用配置文件 appsettings.json::Cap.jobGroup 的值
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobEvent">默认为 JobWaitForStart </param>
        /// <param name="jobGroup"></param>
        public CapJobAttribute(string jobName, string jobEvent = JobEvent.JobWaitForStart, string jobGroup = null)
            : base(JobHelp.GetCapEventName(jobName, jobEvent, jobGroup), false)
        {
            //Group = JobHelp.DefaultCapGroup;
        }
    }
}

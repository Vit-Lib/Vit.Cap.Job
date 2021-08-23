


# jobEvent
	JobWaitForStart JobAfterBegin  JobAfterEnd


# CapEventName
	Vit.Cap.Job.{jobGroup}.{jobName}.{jobEvent}


# 消息失败
  消息重试指定次数后仍然失败，则把消息保存至表 failed_message：
    {
        MessageId = failInfo.Message.GetId(),
        MessageType = failInfo.MessageType.ToString(),
        Name = failInfo.Message.GetName(),
        Headers = failInfo.Message.Headers.Serialize(),
        Value = failInfo.Message.Value.Serialize(),
        time = DateTime.Now
    };


# 消息去重
消息去重通过对mysql表job_lock进行行锁定实现，job在开始前尝试锁定job_lock表中对应行，若锁定失败则终止job，job执行结束（无论成功与否）后解除数据行锁。

 参考
 mysql锁：  
   https://www.cnblogs.com/maybreath/p/12254454.html
   https://blog.csdn.net/qq_44766883/article/details/105879308
   https://www.cnblogs.com/stdpain/p/11016062.html
   

  alter table `vit_cap_job.job_lock` engine=InnoDB;








  # ----------------------------------
   Message (Name:xxx) can not be found subscriber.
   解决办法见 https://github.com/dotnetcore/CAP/issues/63
   > 在 v2.1.1+ 版本中，当应用程序启动时CAP会将默认Group设置为当前程序集名称，这样可以一定程度解决队列冲突问题。


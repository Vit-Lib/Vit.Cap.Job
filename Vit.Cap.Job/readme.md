


# jobEvent
	JobWaitForStart JobAfterBegin  JobAfterEnd


# CapEventName
	Vit.Cap.Job.{jobGroup}.{jobName}.{jobEvent}


# ��Ϣʧ��
  ��Ϣ����ָ����������Ȼʧ�ܣ������Ϣ�������� failed_message��
    {
        MessageId = failInfo.Message.GetId(),
        MessageType = failInfo.MessageType.ToString(),
        Name = failInfo.Message.GetName(),
        Headers = failInfo.Message.Headers.Serialize(),
        Value = failInfo.Message.Value.Serialize(),
        time = DateTime.Now
    };


# ��Ϣȥ��
��Ϣȥ��ͨ����mysql��job_lock����������ʵ�֣�job�ڿ�ʼǰ��������job_lock���ж�Ӧ�У�������ʧ������ֹjob��jobִ�н��������۳ɹ���񣩺�������������

 �ο�
 mysql����  
   https://www.cnblogs.com/maybreath/p/12254454.html
   https://blog.csdn.net/qq_44766883/article/details/105879308
   https://www.cnblogs.com/stdpain/p/11016062.html
   

  alter table `vit_cap_job.job_lock` engine=InnoDB;








  # ----------------------------------
   Message (Name:xxx) can not be found subscriber.
   ����취�� https://github.com/dotnetcore/CAP/issues/63
   > �� v2.1.1+ �汾�У���Ӧ�ó�������ʱCAP�ὫĬ��Group����Ϊ��ǰ�������ƣ���������һ���̶Ƚ�����г�ͻ���⡣


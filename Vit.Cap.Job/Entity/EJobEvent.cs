using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Cap.Job.Entity
{
    public enum EJobEvent
    {
        JobWaitForStart,
        JobAfterBegin,
        JobAfterEnd
    }


    public class JobEvent
    {
        public const string JobWaitForStart = "JobWaitForStart";
        public const string JobAfterBegin = "JobAfterBegin";
        public const string JobAfterEnd = "JobAfterEnd";       
    }
}

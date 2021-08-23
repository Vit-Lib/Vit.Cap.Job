using Microsoft.AspNetCore.Mvc;
using System;
using Vit.Core.Util.ComponentModel.Data;
using DotNetCore.CAP;
using Vit.Core.Util.ComponentModel.Model;
using System.Threading;

namespace App.Controllers
{

    [Route("Cap")]
    [ApiController]
    public class CapController : ControllerBase
    {

        #region (x.1)简单订阅

        [HttpGet("publish1")]
        public ApiReturn Publish([FromQuery, SsExample("xxx.services.publish1")] string name, [FromQuery, SsExample("hello")] string content, [FromServices] DotNetCore.CAP.ICapPublisher _capBus)
        {
            _capBus.Publish(name, content);
            return true;
        }


        [NonAction]
        [CapSubscribe("xxx.services.publish1")]
        public void Subscribe1(string arg)
        {
            Console.WriteLine("[Subscribe1]get msg from Cap2, arg: " + arg);
        }
        #endregion



        #region (x.2)with arg


        public class CapArg
        {
            public string name { get; set; }
        }

        [HttpGet("publish2")]
        public ApiReturn Publish2([FromQuery,SsExample("xxx.services.publish2")]string name, [FromQuery, SsExample("hello")] string content, [FromServices] DotNetCore.CAP.ICapPublisher _capBus)
        {
            _capBus.Publish(name, new { name = content });
            return true;
        }


        [NonAction]
        [CapSubscribe("xxx.services.publish2")]
        public void Subscribe1(CapArg arg)
        {
            Console.WriteLine("[Subscribe1]get msg from Cap2, arg: " + arg);
        }
        #endregion



        #region (x.3) Attribute       

        [NonAction]
        [CapSubscribe2("xxx.services.show.time{time}")]
        public void Subscribe2(string arg)
        {
            Console.WriteLine("[Subscribe2]get msg from Cap2, arg: " + arg);
        }

        [NonAction]
        [CapSubscribe2("xxx.services.show.time{time}")]
        public void Subscribe3(string arg)
        {            
            Console.WriteLine("[Subscribe3]get msg from Cap2, arg: " + arg);
        }      

        public class CapSubscribe2Attribute : CapSubscribeAttribute
        {
            public CapSubscribe2Attribute(string name, bool isPartial = false)
                : base(name.Replace("{time}", Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string>("Cap.time")??"2"), isPartial)
            {
                //Group = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string>("Cap.Group") ?? "Vit.Cap.Job";
            } 
        }
        #endregion



    }


}

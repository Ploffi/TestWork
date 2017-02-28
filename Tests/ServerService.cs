using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using Newtonsoft.Json;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;

namespace Tests
{
    
    public class ServerService
    {
        private ServerController _controller;
        private static Dictionary<string, ServerModel> models;

        [SetUp]
        public void SetUp()
        {
           _controller = new ServerController();
            models = new Dictionary<string, ServerModel>()
            {
                {"correct", new ServerModel() {Name = "end",GameModes = new [] {"DM","TDM"}} },
                {"uncorrect", new ServerModel() {Name = "name",GameModes = new [] {"DM","TDM"}} }
            };
        }


        [Test]
        [TestCase("{}","server1")]
        public void AdvertiseCorrectServer(string data,string endPoint)
        {
            var serverModel = JsonConvert.DeserializeObject<ServerModel>(data);
            var server = new Server()
            {
                EndPoint = endPoint,Name = serverModel.Name,
                GameModes = serverModel.GameModes.Select(name => new GameMode() {Name = name}).ToList()
            };

            var advertiseResult  = _controller.AdvertiseServer(data, endPoint);
            Assert.IsInstanceOf(typeof(OkResult), advertiseResult);

            var serverResult =  _controller.GetInfoByEndPoint(endPoint) as OkNegotiatedContentResult<Server>;
            Assert.AreEqual(server, serverResult);

            
        }
    }
}

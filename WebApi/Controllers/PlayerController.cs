﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Web.Http;

using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/players")]
    public class PlayerController: ApiController
    {
        private PlayerService _playerService;
        public PlayerController()
        {
            _playerService = new PlayerService();
        }

        [Route("{name}/stats")]
        public IHttpActionResult GetPlayerStats(string name)
        {
            name = HttpUtility.UrlDecode(name);
            return Ok();
        }
    }
}
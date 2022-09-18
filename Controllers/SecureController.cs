using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APPExpert_WebAPI.Services;
using APPExpert_WebAPI.Models;

namespace APPExpert_WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SecureController : ControllerBase
    {
        private ISecureService _secureService;

        public SecureController(ISecureService secureservice)
        {
            _secureService = secureservice;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _secureService.GetAll();
            return Ok(users);
        }
    }
}

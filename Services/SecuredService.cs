using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using APPExpert_WebAPI.Models;
using APPExpert_WebAPI.Entities;
using APPExpert_WebAPI.Helpers;

namespace APPExpert_WebAPI.Services
{
    public interface ISecuredService
    {
        IEnumerable<UserMaster> GetALL();
    }


    public class SecuredService : ISecuredService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;

        public SecuredService(
            DataContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<UserMaster> GetALL()
        {
            return _context.UserMaster;
        }
    }
}

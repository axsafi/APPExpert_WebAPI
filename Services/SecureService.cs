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
    public interface ISecureService
    {
        IEnumerable<User> GetAll();
    }

    public class SecureService : ISecureService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;

        public SecureService(
            DataContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }
    }
}

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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace APPExpert_WebAPI.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private DBContext _dbcontext;
        private readonly AppSettings _appSettings;

        public UserService(DataContext context, DBContext DBcontext, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _dbcontext = DBcontext;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            try
            {
                var Username = new SqlParameter("@Username", model.Username);
                var Password = new SqlParameter("@Password", model.Password);

                var Sec = _dbcontext
                            .Security_UserMaster
                            .FromSqlRaw("exec SpAPP_GetAPPUser @Username, @Password", Username, Password)
                            .ToList().SingleOrDefault();

                if (Sec == null)
                {
                    return null;
                }

                var Defaultuser = _context.Users.SingleOrDefault(x => x.Username == "Admin" && x.Password == "Admin");
                // return null if user not found
                if (Defaultuser == null)
                {
                    return null;
                }

                // authentication successful so generate jwt and refresh tokens
                var jwtToken = generateJwtToken(Sec.UserName);
                var refreshToken = generateRefreshToken(ipAddress);

                // save refresh token
                Defaultuser.RefreshTokens.Add(refreshToken);
                _context.Update(Defaultuser);
                _context.SaveChanges();

                return new AuthenticateResponse(Defaultuser, jwtToken, refreshToken.Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

                // return null if no user found with token
                if (user == null) return null;

                var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

                // return null if token is no longer active
                if (!refreshToken.IsActive) return null;

                // replace old refresh token with a new one and save
                var newRefreshToken = generateRefreshToken(ipAddress);
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                refreshToken.ReplacedByToken = newRefreshToken.Token;
                user.RefreshTokens.Add(newRefreshToken);
                _context.Update(user);
                _context.SaveChanges();

                // generate new jwt
                var jwtToken = generateJwtToken(user.Username);

                return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

                // return false if no user found with token
                if (user == null) return false;

                var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

                // return false if token is not active
                if (!refreshToken.IsActive) return false;

                // revoke token and save
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                _context.Update(user);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        // helper methods

        private string generateJwtToken(string Username)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, Username.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            try
            {
                using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
                {
                    var randomBytes = new byte[64];
                    rngCryptoServiceProvider.GetBytes(randomBytes);
                    return new RefreshToken
                    {
                        Token = Convert.ToBase64String(randomBytes),
                        Expires = DateTime.UtcNow.AddDays(7),
                        Created = DateTime.UtcNow,
                        CreatedByIp = ipAddress
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
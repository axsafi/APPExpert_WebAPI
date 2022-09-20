using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APPExpert_WebAPI.Services;
using APPExpert_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using System;
using APPExpert_WebAPI.Entities;
using System.Net;
using System.Net.Http;
//using System.Web.Http;

namespace APPExpert_WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        Failure sFail = new Failure();
        string sMessage;
        public string strContentType = "application/json";
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            AuthenticateResponse response = null;
            HttpResponseMessage objres = new HttpResponseMessage();
            try
            {
                if (String.IsNullOrEmpty(model.Username))
                {
                    sFail.status = "Fail";
                    sMessage = "Username should not be blank.";
                    sFail.statusCode = HttpStatusCode.InternalServerError;
                    sFail.error = FailureStatus(HttpStatusCode.InternalServerError, sMessage, "");

                    //To return json format
                    return BadRequest(new { sFail });
                    //return this.Request.CreateResponse(HttpStatusCode.OK, sFail, objStatus.strContentType);
                }
                response = _userService.Authenticate(model, ipAddress());

                if (response == null)
                {
                    return BadRequest(new { message = "Username or password is incorrect" });
                }
                setTokenCookie(response.RefreshToken);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            AuthenticateResponse response = null;
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                response = _userService.RefreshToken(refreshToken, ipAddress());

                if (response == null)
                    return Unauthorized(new { message = "Invalid token" });

                setTokenCookie(response.RefreshToken);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            bool response = false;
            try
            {
                // accept token from request body or cookie
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Token is required" });

                response = _userService.RevokeToken(token, ipAddress());

                if (!response)
                {
                    return NotFound(new { message = "Token not found" });
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            User user = null;
            try
            {
                user = _userService.GetById(id);
                if (user == null) return NotFound();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Ok(user.RefreshTokens);
        }

        // helper methods

        private void setTokenCookie(string token)
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", token, cookieOptions);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private string ipAddress()
        {
            string IPAddress = string.Empty;
            try
            {
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    IPAddress = Request.Headers["X-Forwarded-For"];
                else
                    IPAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return IPAddress;
        }

        public class USuccess
        {
            public String Status { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }
        public class Failure
        {
            public String status { get; set; }
            public HttpStatusCode statusCode { get; set; }
            public iException error { get; set; }
        }
        public class iException
        {
            public HttpStatusCode code { get; set; }
            public String message { get; set; }
            public innerException innerError { get; set; }
        }
        public class innerException
        {
            public String requestid { get; set; }
            public DateTime date { get; set; }
        }
        private iException FailureStatus(HttpStatusCode code, String message, String FunctionShortCode)
        {
            Guid requestid;
            requestid = Guid.NewGuid();
            DateTime dttmInTime = DateTime.Now;
            iException istatus = new iException();
            try
            {
                innerException innerstatus = new innerException();
                innerstatus.requestid = requestid.ToString();
                innerstatus.date = DateTime.Now;

                istatus = new iException();
                istatus.code = code;
                istatus.message = message;
                istatus.innerError = innerstatus;

                return istatus;

            }
            catch (Exception ex)
            {
                #region Print Log
                try
                {
                    //ErrorTrace.BLLTrace(ex, requestid.ToString(), "", FunctionShortCode, dttmInTime, DateTime.Now, "");
                }
                catch (Exception)
                {
                }

                var resp = new HttpResponseMessage()
                {
                    Content = new StringContent(ex.Message),
                    StatusCode = HttpStatusCode.PartialContent,
                    ReasonPhrase = ex.Message.Replace("\r", "").Replace("\n", " ")
                };
                //throw new HttpResponseException(resp);
                #endregion
            }
            finally
            {

            }
            return istatus;
        }
    }



}

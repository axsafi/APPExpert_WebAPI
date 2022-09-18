using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace APPExpert_WebAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }

    public class UserMaster
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string FullName { get; set; }

        //[JsonIgnore]
        //public List<RefreshToken> RefreshTokens { get; set; }
    }
}
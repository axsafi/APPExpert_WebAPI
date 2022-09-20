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
        public string FullName { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }


        //[JsonIgnore]
        //public List<RefreshToken> RefreshTokens { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Security_UserMaster
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string BranchCode { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }
        public string UserRoleCode { get; set; }
        //public string AddressLine1 { get; set; }
        //public string AddressLine2 { get; set; }
        //public string AddressLine3 { get; set; }
        //public string Country { get; set; }
        //public string PostalCode { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }

        [JsonIgnore]
        public string Password { get; set; }


        //[JsonIgnore]
        //public List<RefreshToken> RefreshTokens { get; set; }
    }


}
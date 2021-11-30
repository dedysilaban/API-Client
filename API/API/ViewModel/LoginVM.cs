using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class LoginVM
    {
        public string Email { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}

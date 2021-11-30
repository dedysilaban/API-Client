using API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class RegisterVM
    {
        public string NIK { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public DateTime Birthdate { get; set; }
        public int Salary { get; set; }
        public string Email { get; set; }

/*        [Required]*/
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }

        public string Password { get; set; }
        public string Degree { get; set; }
        public string GPA { get; set; }
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string NameRole { get; set; }

    }
    public enum Gender
    {
        Male,
        Female
    }
}

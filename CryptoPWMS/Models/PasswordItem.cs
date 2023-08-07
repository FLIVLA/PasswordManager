using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPWMS.Models
{
    public class PasswordItem
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Grp_Id { get; set; }
        public string Platform { get; set; }
        public string URL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastUpdated { get; set; }
    }
}

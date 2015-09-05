using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public enum GenderOption
    {
        Male,
        Female
    }
    public interface IUser
    {
        string UserName { get; set; }
        string NickName { get; set; }
        GenderOption Gender { get; set; }
        int Age { get; set; }
        string Email { get; set; }
        int Head { get; set; }
    }
}

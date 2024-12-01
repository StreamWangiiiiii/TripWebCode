using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos;

namespace TripWebService
{
    public interface ITokenService
    {
        public string GenerateToken(UserDto userDto);
    }
}

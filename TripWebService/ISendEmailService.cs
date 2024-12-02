using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData;
using TripWebData.Dtos;

namespace TripWebService
{
    public interface ISendEmailService
    {
        Task<Results<string>> SendCertifictionCodeAsync(string email);
    }
}

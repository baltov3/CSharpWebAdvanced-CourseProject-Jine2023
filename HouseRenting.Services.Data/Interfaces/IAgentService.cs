using HouseRentingSystem.Web.ViewModels.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRenting.Services.Data.Interfaces
{
    public interface IAgentService
    {
        Task<bool> AgentexistByUserId(string userId);

        Task<bool> UserWithPhoneNumberExists(string phonenumber);
        Task<bool> UserHasRents(string userId);
        Task Create(string userId, BecomeAgentFormModel model);
        Task<string?> GetAgentIdByuserIdAsync(string userId);

    }
}

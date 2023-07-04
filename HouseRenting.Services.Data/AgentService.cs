using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Web.Data;
using HouseRentingSystem.Web.ViewModels.Agent;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants;

namespace HouseRenting.Services.Data
{
    public class AgentService : IAgentService
    {
        private readonly HouseRentingDbContext dbContext;

        public AgentService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AgentexistByUserId(string userId)
        {
            bool result = await this.dbContext.Agents.AnyAsync(a=>a.UserId.ToString()==userId);
            return result;
        }

        public async Task Create(string userId, BecomeAgentFormModel model)
        {
            Agent agent = new Agent()
            {
               
                PhoneNumber= model.PhoneNumber,
                UserId=Guid.Parse(userId)
            };
          await  this.dbContext.Agents.AddAsync(agent);
           await this.dbContext.SaveChangesAsync();
        }

        public async Task<string?> GetAgentIdByuserIdAsync(string userId)
        {
            Agent? agent = await this.dbContext.Agents.FirstOrDefaultAsync(a=>a.UserId.ToString()==userId);
            if (agent==null)
            {
                return null;

            }
            return agent.Id.ToString();
        }

        public async Task<bool> UserHasRents(string userId)
        {
           ApplicationUser? user = await this.dbContext.Users.FirstOrDefaultAsync(u=>u.Id.ToString()==userId);
            if (user==null)
            {
                return false;
            }
            return user.RentedHouses.Any();
        }

        public async Task<bool> UserWithPhoneNumberExists(string phonenumber)
        {
            bool result = await this.dbContext.Agents.AnyAsync(a => a.PhoneNumber == phonenumber);
            return result;
        }
    }
}

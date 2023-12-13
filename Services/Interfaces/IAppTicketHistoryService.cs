﻿using BugOut.Models;

namespace BugOut.Services.Interfaces
{
    public interface IAppTicketHistoryService
    {
        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
        Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int projectId, int companyId);
        Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int companyId);     
    }
}
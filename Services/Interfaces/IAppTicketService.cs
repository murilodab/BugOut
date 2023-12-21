﻿using BugOut.Models;

namespace BugOut.Services.Interfaces
{
    public interface IAppTicketService
    {
        public Task AddNewTicketAsync(Ticket ticket);
        public Task UpdateTicketAsync(Ticket ticket);
        public Task<Ticket> GetTicketByIdAsync(int ticketId);
        public Task ArchiveTicketAsync(Ticket ticket);
        public Task AssignTicketAsync(int ticketId, string userId);
        public Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
        public Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
        public Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);
        public Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);
        public Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);
        public Task<AppUser> GetTicketDeveloperAsync(int ticketId, int companyId);
        public Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int projectId);
        public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId,int companyId);
        public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);
        public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);
        public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);

        public Task<int?> LookupTicketPriorityIdAsync(string priorityName);
        public Task<int?> LookupTicketStatusIdAsync(string statusName);
        public Task<int?> LookupTicketTypeIdAsync(string typeName);

        public Task RestoreTicketAsync(Ticket ticket);

    }
}

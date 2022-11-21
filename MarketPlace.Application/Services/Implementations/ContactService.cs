using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.Entities.Contacts;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Implementations;

public class ContactService : IContactService
{
    #region constructor

    private readonly IGenericRepository<ContactUs> _contactUsRepository;
    private readonly IGenericRepository<Ticket> _ticketRepository;
    private readonly IGenericRepository<TicketMessage> _ticketMessageRepository;

    public ContactService(IGenericRepository<ContactUs> contactUsRepository,
        IGenericRepository<Ticket> ticketRepository,
        IGenericRepository<TicketMessage> ticketMessageRepository)
    {
        _contactUsRepository = contactUsRepository;
        _ticketRepository = ticketRepository;
        _ticketMessageRepository = ticketMessageRepository;
    }

    #endregion

    #region contact us

    public async Task CreateContactUs(CreateContactUsDTO contact, string userIp, long? userId)
    {
        var newContact = new ContactUs
        {
            UserId = userId != null && userId != 0 ? userId : (long?) null,
            UserIp = userIp,
            FullName = contact.FullName,
            Subject = contact.Subject,
            Email = contact.Email,
            Text = contact.Text
        };

        await _contactUsRepository.AddEntity(newContact);
        await _contactUsRepository.SaveChanges();
    }

    #endregion

    #region ticket

    public async Task<AddTicketResult> AddUserTicket(AddTicketDTO ticket, long userId)
    {
        if (string.IsNullOrEmpty(ticket.Text)) return AddTicketResult.Error;

        // add ticket
        var newTicket = new Ticket
        {
            OwnerId = userId,
            IsReadByOwner = true,
            TicketPriority = ticket.TicketPriority,
            Title = ticket.Title,
            TicketSection = ticket.TicketSection,
            TicketState = TicketState.UnderProgress
        };

        await _ticketRepository.AddEntity(newTicket);
        await _ticketRepository.SaveChanges();

        // add ticket message
        var newMessage = new TicketMessage
        {
            TicketId = newTicket.Id,
            Text = ticket.Text,
            SenderId = userId
        };

        await _ticketMessageRepository.AddEntity(newMessage);
        await _ticketMessageRepository.SaveChanges();

        return AddTicketResult.Success;
    }

    public async Task<FilterTicketDTO> FilterTickets(FilterTicketDTO filter)
    {
        var query = _ticketRepository.GetQuery().AsQueryable();

        #region state

        switch (filter.FilterTicketState)
        {
            case FilterTicketState.All:
                break;
            case FilterTicketState.NotDeleted:
                query = query.Where(x => !x.IsDeleted);
                break;
            case FilterTicketState.Deleted:
                query = query.Where(x => x.IsDeleted);
                break;
        }

        switch (filter.OrderBy)
        {
            case FilterTicketOrder.CreateDate_DES:
                query = query.OrderByDescending(x => x.CreateDate);
                break;
            case FilterTicketOrder.CreateDate_ASC:
                query = query.OrderBy(x => x.CreateDate);
                break;
        }

        #endregion

        #region filter

        if (filter.TicketSection != null)
            query = query.Where(s => s.TicketSection == filter.TicketSection.Value);

        if (filter.TicketPriority != null)
            query = query.Where(s => s.TicketPriority == filter.TicketPriority.Value);

        if (filter.UserId != null && filter.UserId != 0)
            query = query.Where(s => s.OwnerId == filter.UserId.Value);

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(s => EF.Functions.Like(s.Title, $"%{filter.Title}%"));

        #endregion

        #region paging

        var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
        var allEntities = await query.Paging(pager).ToListAsync();

        #endregion

        return filter.SetPaging(pager).SetTickets(allEntities);
    }

    public async Task<TicketDetailDTO> GetTicketForShow(long ticketId, long userId)
    {
        var ticket = await _ticketRepository.GetQuery().AsQueryable()
            .Include(s => s.Owner).SingleOrDefaultAsync(s => s.Id == ticketId );

        if (ticket == null && ticket.OwnerId != userId) return null;

        return new TicketDetailDTO
        {
            Ticket = ticket,
            TicketMessages = await _ticketMessageRepository.GetQuery().AsQueryable()
            .OrderByDescending(s => s.CreateDate)
                .Where(s => s.TicketId == ticketId && !s.IsDeleted).ToListAsync()
        };
    }

    public async Task<AnswerTicketResult> AnswerTicket(AnswerTicketDTO answer, long userId)
    {
        var ticket = await _ticketRepository.GetEntityById(answer.Id);
        if (ticket == null) return AnswerTicketResult.NotFound;
        if (ticket.OwnerId != userId) return AnswerTicketResult.NotForUser;

        var ticketMessage = new TicketMessage
        {
            TicketId = ticket.Id,
            SenderId = userId,
            Text = answer.Text
        };

        await _ticketMessageRepository.AddEntity(ticketMessage);
        await _ticketMessageRepository.SaveChanges();

        ticket.IsReadByAdmin = false;
        ticket.IsReadByOwner = true;
        await _ticketRepository.SaveChanges();
        return AnswerTicketResult.Success;
    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        await _contactUsRepository.DisposeAsync();
    }

    #endregion
}

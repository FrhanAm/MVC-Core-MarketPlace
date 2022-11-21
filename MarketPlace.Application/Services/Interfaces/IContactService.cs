using MarketPlace.DataLayer.DTOs.Contacts;

namespace MarketPlace.Application.Services.Interfaces;

public interface IContactService : IAsyncDisposable
{
    #region contact us

    Task CreateContactUs(CreateContactUsDTO contact, string userIp, long? userId);

    #endregion

    #region ticket

    Task<AddTicketResult> AddUserTicket(AddTicketDTO ticket, long userId);
    Task<FilterTicketDTO> FilterTickets(FilterTicketDTO filter);
    Task<TicketDetailDTO> GetTicketForShow(long ticketId, long userId);
    Task<AnswerTicketResult> AnswerTicket(AnswerTicketDTO answer, long userId);

    #endregion
}

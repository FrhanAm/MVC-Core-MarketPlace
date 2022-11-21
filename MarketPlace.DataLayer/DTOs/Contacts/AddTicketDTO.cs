using MarketPlace.DataLayer.Entities.Contacts;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Contacts;

public class AddTicketDTO
{

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Title { get; set; }

    [Display(Name = "بخش مورد نظر")]
    public TicketSection TicketSection { get; set; }

    [Display(Name = "اولویت")]
    public TicketPriority TicketPriority { get; set; }

    [Display(Name = "متن پیام")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Text { get; set; }
}

public enum AddTicketResult
{
    Error,
    Success,
}
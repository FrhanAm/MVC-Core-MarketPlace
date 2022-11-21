using MarketPlace.DataLayer.Entities.Common;
using MarketPlace.DataLayer.Entities.Contacts;
using MarketPlace.DataLayer.Entities.Store;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.Entities.Account
{
    public class User : BaseEntity
    {
        #region properties

        [Display(Name = "ایمیل")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمیباشد")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string EmailActiveCode { get; set; }

        [Display(Name = "ایمیل فعال / غیر فعال")]
        public bool IsEmailActive { get; set; }

        [Display(Name = "تلفن همراه")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string MobileActiceCode { get; set; }

        [Display(Name = "موبایل فعال / غیر فعال")]
        public bool IsMobileActive { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string LastName { get; set; }

        [Display(Name = "تصویر آواتار")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
        public string? Avatar { get; set; }

        [Display(Name = "بلاک شده / نشده")]
        public bool IsBlocked { get; set; }

        #endregion

        #region relations

        public ICollection<ContactUs> ContactUses { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<TicketMessage> TicketMessages { get; set; }
        public ICollection<Seller> Sellers { get; set; }

        #endregion
    }
}

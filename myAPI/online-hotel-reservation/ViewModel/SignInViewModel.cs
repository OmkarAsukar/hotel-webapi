using System.ComponentModel.DataAnnotations;

namespace online_hotel_reservation.ViewModel
{
    public class SignInViewModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Password { get; set; }
        }
}
using System.ComponentModel.DataAnnotations;

namespace online_hotel_reservation.Model
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string role { get; set; }

    }
}
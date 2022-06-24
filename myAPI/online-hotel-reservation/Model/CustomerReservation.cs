using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_hotel_reservation.Model
{
    public class CustomerReservation
    {
        [Key]
        public int ReservationId { get; set; }  
        [ForeignKey("AppUser")]
        public int CustomerId { get; set; }  
        public string HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime Checkout { get; set; }
        public int NumberofGuests { get; set; }
        public string ReservationStatus { get; set; }
        public string ReservationComments { get; set; }
    }
}

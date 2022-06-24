using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using online_hotel_reservation.Model;
using online_hotel_reservation.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;

namespace online_hotel_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
         public ApplicationDbContext _context;
         public IConfiguration _configuration;
        // public UserManager<IdentityUser> userManager;
        // public SignInManager<IdentityUser> signInManager;
        // public RoleManager<IdentityRole> roleManager;

        public ReservationController(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context; 
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> PostUsers(AppUser model)
        {
            if(ModelState.IsValid)
            {
                _context.AppUsers.Add(model);
                await _context.SaveChangesAsync();
                
                return Ok(model);
            }
            return BadRequest(ModelState.Values);
        }
        
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if(ModelState.IsValid)
            {
                AppUser user =  await _context.AppUsers.Where(x => x.UserName == model.UserName && x.Password==model.Password).FirstOrDefaultAsync();
                if(user!=null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler(); 
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key"));
                    var signingcredentials= new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Role, user.role)
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        SigningCredentials = signingcredentials
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    // var obj = new
                    // {
                    //     jwtToken = tokenHandler.WriteToken(token),
                    //     UserId=user.UserId,
                    //     UserName=user.UserName,
                    //     Role=user.role
                    // };
                    
                    // List<Claim> claims = new List<Claim>
                    // {
                    //     new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    //     new Claim(ClaimTypes.Name, user.UserName),
                    //     new Claim(ClaimTypes.Role, user.role)
                    // };

                    // SymmetricSecurityKey key = new SymmetricSecurityKey(
                    //     Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
                    // );

                    // SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                    // SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                    // {
                    //     Subject = new ClaimsIdentity(claims),
                    //     Expires = DateTime.Now.AddDays(1),
                    //     SigningCredentials = creds
                    // };

                    // JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    // SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(new { Token = tokenHandler.WriteToken(token) });
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="TravelAgent")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> Get()
        {
            var users = await _context.AppUsers.ToListAsync();
            return Ok(users);
        }

        [HttpGet("getreservation/{reservationId}")]
        public async Task<ActionResult<CustomerReservation>> GetReservationsById(int reservationId)
        {
            var reservation =  await _context.CustomerReservations.FindAsync(reservationId);
            if(reservation == null)
            {
                return NotFound();
            }
            return reservation;
        }

        [HttpGet("getAllReservations")]
        public async Task<ActionResult<IEnumerable<CustomerReservation>>> GetAllReservations()
        {
            var reservations = await _context.CustomerReservations
                                             .Select(x => new CustomerReservation
                                             {
                                                 ReservationId = x.ReservationId,
                                                 CustomerId = x.CustomerId,
                                                 HotelId = x.HotelId,
                                                 CheckIn = x.CheckIn,
                                                 Checkout = x.Checkout,
                                                 NumberofGuests= x.NumberofGuests,
                                                 ReservationStatus = x.ReservationStatus,
                                                 ReservationComments = x.ReservationComments
                                             })
                                             .ToListAsync();
            return Ok(reservations);
        }

        [HttpPost("InsertBooking")]
        public async Task<ActionResult<CustomerReservation>> PostReservation(CustomerReservation customerReservation)
        {
            if(ModelState.IsValid)
            {
                if(customerReservation.CheckIn < customerReservation.Checkout 
                    && customerReservation.CheckIn > DateTime.Now
                    && customerReservation.NumberofGuests >= 1)
                {
                    _context.CustomerReservations.Add(customerReservation);
                    var result = await _context.SaveChangesAsync();
                    //return CreatedAtAction("GetReservationsById", new { reservationid = customerReservation.ReservationId });
                    return Ok( new { 
                        status = 200,
                        ReservationId = customerReservation.ReservationId
                    });
                }
                else
                {
                    ModelState.AddModelError("","The validations have failed");
                    return BadRequest(ModelState.Values);
                }
            }
            return BadRequest(ModelState.Values);
        }

        [HttpPut("UpdateBooking/{ReservationId}")]
        public async Task<IActionResult> PutReservation(int ReservationId,CustomerReservation customerReservation)
        {
            if( ReservationId != customerReservation.ReservationId)
            {
                return BadRequest();
            }
            _context.Entry(customerReservation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!CustomerReservationExists(ReservationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }          
            }
            //return NoContent();
            return Ok( new {status = 200} );
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<CustomerReservation>> DeleteReservation(int id)
        {
            var reservation = await _context.CustomerReservations.FindAsync(id);
            if(reservation==null)
            {
                return NotFound();
            }
            _context.CustomerReservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return Ok( new {status = 200} );

        }

        private bool CustomerReservationExists(int id)
        {
            return _context.CustomerReservations.Any(e => e.ReservationId == id);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoneyTransfer.Business.Abstract;
using MoneyTransfer.Business.Concrete;
using MoneyTransfer.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace MoneyTransfer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerService;
        private readonly IConfiguration _configuration;

        public CustomerController(ICustomerService customerService, IConfiguration configuration)
        {
            _customerService = customerService;
            _configuration = configuration;
        }

        [HttpGet]
        public List<Customer> Get()
        {
            return _customerService.GetAllCustomers();
        }

        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _customerService.GetCustomerById(id);
        }

        //[HttpPost]
        //public Customer Post(Customer customer) 
        //{
        //    return _customerService.CreateCustomer(customer);
        //}

        [HttpPut]
        public Customer Put(Customer customer)
        {
            return _customerService.UpdateCustomer(customer);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
             _customerService.DeleteCustomer(id);
        }

        [AllowAnonymous]
        [HttpPost("/api/login")]
        public IActionResult Login([FromBody] LoginRequestModel model)
        {
            var customer = _customerService.Login(model.Username, model.Password);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    StatusCode = "404",
                    Message = "Giriş bilgileri hatalıdır.",
                    IsSucceed = false
                });
            }

            var token = GenerateJwtToken(customer);
            customer.RefreshToken = token.RefreshToken;
            customer.RefreshTokenExpiration = token.RefreshTokenExpiration;
            _customerService.UpdateCustomer(customer);
            
            return Ok(new ApiResponse
            {
                StatusCode = "200",
                Message = "Giriş Başarılı.",
                Data = token,
                IsSucceed = true
            });
        }

        private Token GenerateJwtToken(Customer customer)
        {
            Token token = new();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!); //appsettings.json dosyasındaki Jwt:Key
            token.Expiration = DateTime.Now.AddHours(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] //kullanıcıya ait bilgiler
                {
                    new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                    new Claim(ClaimTypes.Name, customer.Name!),
                    new Claim(ClaimTypes.Surname, customer.Surname!),
                    new Claim(ClaimTypes.Email, customer.Mail!)
                }),
                Expires = token.Expiration,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            token.AccessToken = tokenHandler.WriteToken(accessToken);
            token.RefreshToken = GenerateRefreshToken();
            token.RefreshTokenExpiration = DateTime.Now.AddDays(10);

            return token;
        }

        private string GenerateRefreshToken()
        {
            byte[] numbers = new byte[32];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(numbers);
                return Convert.ToBase64String(numbers);
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequestModel model)
        {
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            if (principal == null)
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = "Geçersiz access token veya refresh token.",
                    IsSucceed = false
                });
            }

            var customerId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = "Geçersiz access token veya refresh token.",
                    IsSucceed = false
                });
            }

            var customer = _customerService.GetCustomerById(int.Parse(customerId));
            if (customer == null || customer.RefreshToken != model.RefreshToken || customer.RefreshTokenExpiration <= DateTime.Now)
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = "Geçersiz access token veya refresh token.",
                    IsSucceed = false
                });
            }

            var newToken = GenerateJwtToken(customer);
            customer.RefreshToken = newToken.RefreshToken;
            customer.RefreshTokenExpiration = newToken.RefreshTokenExpiration;
            _customerService.UpdateCustomer(customer);

            return Ok(new ApiResponse
            {
                StatusCode = "200",
                Message = "Yeni Token Oluşturuldu.",
                Data = newToken,
                IsSucceed = true
            });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!); // appsettings.json dosyasındaki Jwt:Key
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false, // Ignore token expiration
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"]
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token geçerli değildir.");
            }

            return principal;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = "Geçersiz müşteri verileri",
                    IsSucceed = false
                });
            }

            try
            {
                var createdCustomer = _customerService.CreateCustomer(customer);

                return Ok(new ApiResponse
                {
                    StatusCode = "200",
                    Message = "Yeni Müşteri Oluşturuldu.",
                    Data = createdCustomer,
                    IsSucceed = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = ex.Message,
                    IsSucceed = false
                });

            }
        }

        public class LoginRequestModel
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public class RefreshTokenRequestModel
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
        }
    }
}

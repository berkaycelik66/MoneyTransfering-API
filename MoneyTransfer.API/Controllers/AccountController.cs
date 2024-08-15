using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Business.Abstract;
using MoneyTransfer.Business.Concrete;
using MoneyTransfer.Entities;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace MoneyTransfer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public List<Account> Get()
        {
            return _accountService.GetAllAccount();
        }

        [HttpGet("{id}")]
        public Account Get(int id)
        {
            return _accountService.GetAccountById(id);
        }

        [HttpPost]
        public Account Post(Account account)
        {
            return _accountService.CreateAccount(account);
        }

        [HttpPut]
        public Account Put(Account account)
        {
            return _accountService.UpdateAccount(account);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _accountService.DeleteAccount(id);
        }

        [HttpGet("/api/Account/CustomerAccounts/")]
        public IActionResult GetAccountsByCustomerId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var customerIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (customerIdClaim != null)
                {
                    var customerId = int.Parse(customerIdClaim.Value);
                    var accounts = _accountService.GetAccountsByCustomerId(customerId);
                    if (accounts == null || accounts.Count == 0)
                    {
                        return NotFound(new ApiResponse
                        {
                            StatusCode = "404",
                            Message = "Bu müşteriye ait bir hesap bulunmamaktadır.",
                            IsSucceed = false
                        });
                    }

                    return Ok(new ApiResponse
                    {
                        StatusCode = "200",
                        Message = "Hesap listesi getirilmiştir.",
                        Data = accounts,
                        IsSucceed = true
                    });
                }
            }

            return Unauthorized(new ApiResponse
            {
                StatusCode = "401",
                Message = "Müşteri kimliği bulunamadı.",
                IsSucceed = false
            });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MoneyTransfer.Business.Abstract;
using MoneyTransfer.Business.Concrete;
using MoneyTransfer.Entities;
using System.Security.Claims;

namespace MoneyTransfer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private ITransferService _transferService;
        private IAccountService _accountService;

        public TransferController(ITransferService transferService, IAccountService accountService)
        {
            _transferService = transferService;
            _accountService = accountService;
        }

        [HttpGet]
        public List<Transfer> Get()
        {
            return _transferService.GetAllTransfer();
        }

        [HttpGet("{id}")]
        public Transfer Get(int id)
        {
            return _transferService.GetTransferById(id);
        }

        [HttpPut]
        public Transfer Put(Transfer transfer)
        {
            return _transferService.UpdateTransfer(transfer);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _transferService.DeleteTransfer(id);
        }

        [HttpGet("/api/Transfer/AccountTransfers/{accountId}")] //http://localhost:5056/api/AccountTransfers/3?startDate=18-07-2024&endDate=19-07-2024
        public IActionResult GetAllTransferByAccountId(int accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var customerIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (customerIdClaim != null)
                {
                    var customerId = int.Parse(customerIdClaim.Value);
                    var account = _accountService.GetAccountById(accountId);

                    if (account == null)
                    {
                        return NotFound(new ApiResponse
                        {
                            StatusCode = "404",
                            Message = "Hesap bulunamadı.",
                            IsSucceed = false
                        });
                    }

                    if (account.CustomerId != customerId)
                    {
                        return NotFound(new ApiResponse
                        {
                            StatusCode = "404",
                            Message = "Hesap size ait değildir.",
                            IsSucceed = false
                        });
                    }
                }
            }
            var transfers = _transferService.GetAllTransfersByAccountIdWithCustomer(accountId);

            if (startDate.HasValue && endDate.HasValue) //İki kısım da dolu ise
            {
                transfers = transfers.Where(t => t.Date.Date >= startDate && t.Date.Date <= endDate).ToList();
            }
            else if (startDate.HasValue) //sadece başlangıç tarihi girilmişse
            {
                transfers = transfers.Where(t => t.Date.Date >= startDate).ToList();
            }
            else if (endDate.HasValue) //sadece bitiş tarihi girilmişse
            {
                transfers = transfers.Where(t => t.Date.Date <= endDate).ToList();
            }

            if ((transfers == null || transfers.Count == 0) && !startDate.HasValue && !endDate.HasValue)
            {
                return NotFound(new ApiResponse
                {
                    StatusCode = "404",
                    Message = "Bu müşteriye ait hesap hareketi bulunmamaktadır.",
                    IsSucceed = false
                });
            }

            if ((transfers == null || transfers.Count == 0) && (startDate.HasValue || endDate.HasValue))
            {
                return NotFound(new ApiResponse
                {
                    StatusCode = "404",
                    Message = "Belirtilen tarih aralığında bir hesap hareketi bulunmamaktadır.",
                    IsSucceed = false
                });
            }

            var transferWithCustomerInfo = transfers.Select(t => new
            {
                t.Id,
                t.AccountFromId,
                t.AccountToId,
                t.Amount,
                t.Fee,
                t.TransferCurrency,
                t.Date,
                t.Description,
                FromCustomerName = t.AccountFrom!.Customer!.Name + " " + t.AccountFrom.Customer.Surname,
                ToCustomerName = t.AccountTo!.Customer!.Name + " " + t.AccountTo.Customer.Surname
            });

            return Ok(new ApiResponse
            {
                StatusCode = "200",
                Message = "Hesap hareketleri getirilmiştir.",
                Data = transferWithCustomerInfo,
                IsSucceed = true
            });
        }

        [HttpPost("/api/Transfer/moneyTransfer")]
        public IActionResult Transfer([FromBody] TransferRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transferResult = _transferService.TransferMoney(model.AccountFromId, model.ToAccountNumber.Replace(" ", ""), model.Amount, model.Description!);

            if (transferResult)
            {
                return Ok(new ApiResponse
                {
                    StatusCode = "200",
                    Message = "Para transferi gerçekleştirildi.",
                    IsSucceed = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse
                {
                    StatusCode = "400",
                    Message = "Para transferi işlemi başarısız.",
                    IsSucceed = false
                });
            }
        }
    }

    public class TransferRequestModel
    {
        public int AccountFromId { get; set; }
        public string? ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}

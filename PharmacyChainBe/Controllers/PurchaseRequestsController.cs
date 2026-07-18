using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Operations Manager")]
    public class PurchaseRequestsController : ControllerBase
    {
        private readonly IPurchaseRequestService _purchaseRequestService;

        public PurchaseRequestsController(IPurchaseRequestService purchaseRequestService)
        {
            _purchaseRequestService = purchaseRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseRequests([FromQuery] PurchaseRequestFilter filter)
        {
            try
            {
                var pagedResponse = await _purchaseRequestService.GetPurchaseRequestsAsync(filter);
                return Ok(new BaseApiResponse<PagedResponse<IEnumerable<PurchaseRequestDto>>>
                {
                    Success = true,
                    Message = "Purchase requests retrieved successfully.",
                    Data = pagedResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while fetching purchase requests."
                });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseApiResponse<PurchaseRequestDto>>> GetPurchaseRequest(int id)
        {
            try
            {
                var request = await _purchaseRequestService.GetPurchaseRequestByIdAsync(id);
                return Ok(new BaseApiResponse<PurchaseRequestDto> { Success = true, Message = "Fetched purchase request.", Data = request });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new BaseApiResponse<PurchaseRequestDto> { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("{id}/review")]
        public async Task<IActionResult> ReviewPurchaseRequest(int id, [FromBody] ReviewPurchaseRequestDto dto)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new BaseApiResponse<object> { Success = false, Message = "Invalid user token." });
                }

                await _purchaseRequestService.ReviewPurchaseRequestAsync(id, dto, userId);
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Purchase request reviewed successfully."
                });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new BaseApiResponse<object> { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseApiResponse<object> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _purchaseRequestService.GetBranchesAsync();
            return Ok(new BaseApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Lấy danh sách chi nhánh thành công.",
                Data = branches
            });
        }
        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _purchaseRequestService.GetSuppliersAsync();
            return Ok(new BaseApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Lấy danh sách nhà cung cấp thành công.",
                Data = suppliers
            });
        }
    }
}

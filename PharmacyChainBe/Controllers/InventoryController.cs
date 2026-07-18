using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Branch Manager,Administrator")] // Apply BR-02 Role Access
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private int GetUserBranchId()
        {
            var branchIdClaim = User.FindFirst("BranchID")?.Value;
            if (string.IsNullOrEmpty(branchIdClaim) || !int.TryParse(branchIdClaim, out int branchId))
            {
                throw new ApiException("Người dùng không thuộc về chi nhánh nào.", 403);
            }
            return branchId;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventories([FromQuery] InventoryRequestDto request)
        {
            // Apply BR-01 Data Isolation
            int branchId = GetUserBranchId();

            var response = await _inventoryService.GetInventoriesAsync(branchId, request);
            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<InventoryResponseDto>>> 
            { 
                Success = true, 
                Data = response 
            });
        }

    }
}

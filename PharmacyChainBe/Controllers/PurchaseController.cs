using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Branch Manager,Administrator")] // BR-01
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        private (int BranchId, int UserId) GetUserClaims()
        {
            var branchIdClaim = User.FindFirst("BranchID")?.Value;
            if (string.IsNullOrEmpty(branchIdClaim) || !int.TryParse(branchIdClaim, out int branchId))
            {
                throw new ApiException("Người dùng không thuộc về chi nhánh nào.", 403);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new ApiException("Không xác định được ID người dùng.", 401);
            }

            return (branchId, userId);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetPurchaseRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var claims = GetUserClaims();
            var requests = await _purchaseService.GetPurchaseRequestsAsync(claims.BranchId, pageNumber, pageSize);

            return Ok(new BaseApiResponse<DTOs.PagedResponse<IEnumerable<DTOs.Response.PurchaseRequestResponseDto>>>
            {
                Success = true,
                Message = "Lấy danh sách yêu cầu nhập hàng thành công.",
                Data = requests
            });
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreatePurchaseRequest([FromBody] CreatePurchaseRequestDto request)
        {
            var claims = GetUserClaims();
            await _purchaseService.CreatePurchaseRequestAsync(claims.BranchId, claims.UserId, request);

            return Ok(new BaseApiResponse<object> 
            { 
                Success = true, 
                Message = "Yêu cầu nhập hàng đã được tạo thành công." 
            });
        }
        [HttpPost("order/{id}/receive")]
        public async Task<IActionResult> ReceivePurchaseOrder(int id, [FromBody] ReceiveMedicinesDto request)
        {
            var claims = GetUserClaims();
            await _purchaseService.ReceivePurchaseOrderAsync(claims.BranchId, id, request);

            return Ok(new BaseApiResponse<object> 
            { 
                Success = true, 
                Message = "Nhận hàng và cập nhật tồn kho thành công." 
            });
        }
    }
}

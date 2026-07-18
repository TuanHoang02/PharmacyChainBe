using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]
        [Authorize(Roles = "BranchManager,Pharmacist")]
        public async Task<ActionResult<PagedResponse<List<MedicineDto>>>> GetPaged([FromQuery] MedicineQuery query, CancellationToken cancellationToken)
        {
            var response = await _medicineService.GetPagedAsync(query, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "BranchManager,Pharmacist")]
        public async Task<ActionResult<BaseApiResponse<MedicineDetailDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _medicineService.GetByIdAsync(id, cancellationToken);
            return Ok(new BaseApiResponse<MedicineDetailDto>
            {
                Success = true,
                Message = "Lấy thông tin thuốc thành công.",
                Data = data
            });
        }

        [HttpPost]
        [Authorize(Roles = "BranchManager")]
        public async Task<ActionResult<BaseApiResponse<MedicineDetailDto>>> Create([FromBody] CreateMedicineRequest request, CancellationToken cancellationToken)
        {
            var data = await _medicineService.CreateAsync(request, cancellationToken);
            return Ok(new BaseApiResponse<MedicineDetailDto>
            {
                Success = true,
                Message = "Tạo thuốc thành công.",
                Data = data
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "BranchManager")]
        public async Task<ActionResult<BaseApiResponse<MedicineDetailDto>>> Update(int id, [FromBody] UpdateMedicineRequest request, CancellationToken cancellationToken)
        {
            var data = await _medicineService.UpdateAsync(id, request, cancellationToken);
            return Ok(new BaseApiResponse<MedicineDetailDto>
            {
                Success = true,
                Message = "Cập nhật thuốc thành công.",
                Data = data
            });
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "BranchManager")]
        public async Task<ActionResult<BaseApiResponse<object>>> Deactivate(int id, CancellationToken cancellationToken)
        {
            await _medicineService.DeactivateAsync(id, cancellationToken);
            return Ok(new BaseApiResponse<object>
            {
                Success = true,
                Message = "Ngưng hoạt động thuốc thành công."
            });
        }
    }
}

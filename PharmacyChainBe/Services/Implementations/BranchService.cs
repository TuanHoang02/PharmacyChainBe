using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<PagedResponse<IEnumerable<BranchDto>>> GetBranchesAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) = await _branchRepository.GetBranchesAsync(searchTerm, isActive, pageNumber, pageSize);

            var dtos = items.Select(b => MapToDto(b));

            return new PagedResponse<IEnumerable<BranchDto>>
            {
                Data = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount
            };
        }

        public async Task<BranchDto> GetBranchByIdAsync(int id)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);
            if (branch == null)
            {
                throw new ApiException("Chi nhánh không tồn tại.", 404);
            }

            return MapToDto(branch);
        }

        public async Task<BranchDto> CreateBranchAsync(CreateBranchDto request)
        {
            var isDuplicate = await _branchRepository.ExistsByNameAsync(request.BranchName);
            if (isDuplicate)
            {
                throw new ApiException("Tên chi nhánh đã tồn tại.", 400);
            }

            var branch = new Branch
            {
                BranchName = request.BranchName.Trim(),
                Address = request.Address.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                Email = request.Email?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdBranch = await _branchRepository.AddBranchAsync(branch);
            return MapToDto(createdBranch);
        }

        public async Task<BranchDto> UpdateBranchAsync(int id, UpdateBranchDto request)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);
            if (branch == null)
            {
                throw new ApiException("Chi nhánh không tồn tại.", 404);
            }

            var isDuplicate = await _branchRepository.ExistsByNameAsync(request.BranchName, id);
            if (isDuplicate)
            {
                throw new ApiException("Tên chi nhánh đã tồn tại.", 400);
            }

            branch.BranchName = request.BranchName.Trim();
            branch.Address = request.Address.Trim();
            branch.PhoneNumber = request.PhoneNumber?.Trim();
            branch.Email = request.Email?.Trim();
            branch.IsActive = request.IsActive;

            await _branchRepository.UpdateBranchAsync(branch);
            return MapToDto(branch);
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);
            if (branch == null)
            {
                throw new ApiException("Chi nhánh không tồn tại.", 404);
            }

            // Thực hiện Soft Delete
            branch.IsActive = false;
            return await _branchRepository.UpdateBranchAsync(branch);
        }

        private static BranchDto MapToDto(Branch branch)
        {
            return new BranchDto
            {
                BranchID = branch.BranchID,
                BranchName = branch.BranchName,
                Address = branch.Address,
                PhoneNumber = branch.PhoneNumber,
                Email = branch.Email,
                IsActive = branch.IsActive,
                CreatedAt = branch.CreatedAt,
                UpdatedAt = branch.UpdatedAt
            };
        }
    }
}

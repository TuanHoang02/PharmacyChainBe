using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _repository;

        public MedicineService(IMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<List<MedicineDto>>> GetPagedAsync(MedicineQuery query, CancellationToken cancellationToken = default)
        {
            var paged = await _repository.GetPagedAsync(query, cancellationToken);
            var mappedList = paged.Data.Select(MapToDto).ToList();

            return new PagedResponse<List<MedicineDto>>
            {
                Data = mappedList,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalRecords = paged.TotalRecords
            };
        }

        public async Task<MedicineDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var medicine = await _repository.GetByIdAsync(id, cancellationToken);
            if (medicine == null)
            {
                throw new ApiException("Không tìm thấy thuốc.", 404);
            }
            return MapToDetailDto(medicine);
        }

        public async Task<MedicineDetailDto> CreateAsync(CreateMedicineRequest request, CancellationToken cancellationToken = default)
        {
            // Trim inputs
            request.MedicineName = request.MedicineName?.Trim() ?? string.Empty;
            request.GenericName = request.GenericName?.Trim();
            request.Unit = request.Unit?.Trim() ?? string.Empty;
            request.DosageInstructions = request.DosageInstructions?.Trim();

            // Validations
            if (string.IsNullOrWhiteSpace(request.MedicineName))
            {
                throw new ApiException("Tên thuốc không được để trống.", 400);
            }

            if (request.MedicineName.Length > 200)
            {
                throw new ApiException("Tên thuốc không được vượt quá 200 ký tự.", 400);
            }

            if (request.GenericName != null && request.GenericName.Length > 200)
            {
                throw new ApiException("Tên gốc không được vượt quá 200 ký tự.", 400);
            }

            if (request.SellingPrice <= 0 || request.SellingPrice > 10000000)
            {
                throw new ApiException("Giá bán phải lớn hơn 0 và nhỏ hơn hoặc bằng 10.000.000.", 400);
            }

            if (string.IsNullOrWhiteSpace(request.Unit))
            {
                throw new ApiException("Đơn vị tính không được để trống.", 400);
            }

            // Check if category exists and is active
            var categoryExists = await _repository.CategoryExistsAsync(request.CategoryID, cancellationToken);
            if (!categoryExists)
            {
                throw new ApiException("Danh mục không tồn tại hoặc đã bị khóa.", 400);
            }

            // Check duplicate name
            var existingByName = await _repository.GetByNameAsync(request.MedicineName, cancellationToken);
            if (existingByName != null)
            {
                throw new ApiException("Tên thuốc đã tồn tại.", 400);
            }

            // Map and Save
            var medicine = new Medicine
            {
                MedicineName = request.MedicineName,
                GenericName = request.GenericName,
                CategoryID = request.CategoryID,
                SellingPrice = request.SellingPrice,
                ImportPrice = 0,
                Unit = request.Unit,
                DosageInstructions = request.DosageInstructions,
                RequiresPrescription = request.RequiresPrescription,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(medicine, cancellationToken);

            // Re-fetch to get loaded Category navigation property
            var result = await _repository.GetByIdAsync(created.MedicineID, cancellationToken);
            if (result == null)
            {
                throw new ApiException("Có lỗi xảy ra khi tạo thuốc.", 500);
            }

            return MapToDetailDto(result);
        }

        public async Task<MedicineDetailDto> UpdateAsync(int id, UpdateMedicineRequest request, CancellationToken cancellationToken = default)
        {
            // Trim inputs
            request.MedicineName = request.MedicineName?.Trim() ?? string.Empty;
            request.GenericName = request.GenericName?.Trim();
            request.Unit = request.Unit?.Trim() ?? string.Empty;
            request.DosageInstructions = request.DosageInstructions?.Trim();

            // Validate existence
            var medicine = await _repository.GetByIdAsync(id, cancellationToken);
            if (medicine == null)
            {
                throw new ApiException("Không tìm thấy thuốc.", 404);
            }

            // Validations
            if (string.IsNullOrWhiteSpace(request.MedicineName))
            {
                throw new ApiException("Tên thuốc không được để trống.", 400);
            }

            if (request.MedicineName.Length > 200)
            {
                throw new ApiException("Tên thuốc không được vượt quá 200 ký tự.", 400);
            }

            if (request.GenericName != null && request.GenericName.Length > 200)
            {
                throw new ApiException("Tên gốc không được vượt quá 200 ký tự.", 400);
            }

            if (request.SellingPrice <= 0 || request.SellingPrice > 10000000)
            {
                throw new ApiException("Giá bán phải lớn hơn 0 và nhỏ hơn hoặc bằng 10.000.000.", 400);
            }

            if (string.IsNullOrWhiteSpace(request.Unit))
            {
                throw new ApiException("Đơn vị tính không được để trống.", 400);
            }

            // Check if category exists and is active
            var categoryExists = await _repository.CategoryExistsAsync(request.CategoryID, cancellationToken);
            if (!categoryExists)
            {
                throw new ApiException("Danh mục không tồn tại hoặc đã bị khóa.", 400);
            }

            // Check duplicate name excluding current medicine
            var existingByName = await _repository.GetByNameAsync(request.MedicineName, cancellationToken);
            if (existingByName != null && existingByName.MedicineID != id)
            {
                throw new ApiException("Tên thuốc đã tồn tại.", 400);
            }

            // Map updates
            medicine.MedicineName = request.MedicineName;
            medicine.GenericName = request.GenericName;
            medicine.CategoryID = request.CategoryID;
            medicine.SellingPrice = request.SellingPrice;
            medicine.Unit = request.Unit;
            medicine.DosageInstructions = request.DosageInstructions;
            medicine.RequiresPrescription = request.RequiresPrescription;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(medicine, cancellationToken);

            // Re-fetch to get loaded Category navigation property
            var result = await _repository.GetByIdAsync(medicine.MedicineID, cancellationToken);
            if (result == null)
            {
                throw new ApiException("Có lỗi xảy ra khi cập nhật thông tin thuốc.", 500);
            }

            return MapToDetailDto(result);
        }

        public async Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            var medicine = await _repository.GetByIdAsync(id, cancellationToken);
            if (medicine == null)
            {
                throw new ApiException("Không tìm thấy thuốc.", 404);
            }

            if (!medicine.IsActive)
            {
                throw new ApiException("Thuốc này đã ở trạng thái ngưng hoạt động.", 400);
            }

            medicine.IsActive = false;
            medicine.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(medicine, cancellationToken);
        }

        #region Helper Mappings

        private MedicineDto MapToDto(Medicine medicine)
        {
            return new MedicineDto
            {
                MedicineID = medicine.MedicineID,
                MedicineName = medicine.MedicineName,
                GenericName = medicine.GenericName,
                CategoryID = medicine.CategoryID,
                CategoryName = medicine.Category?.CategoryName ?? string.Empty,
                SellingPrice = medicine.SellingPrice,
                Unit = medicine.Unit,
                RequiresPrescription = medicine.RequiresPrescription,
                IsActive = medicine.IsActive
            };
        }

        private MedicineDetailDto MapToDetailDto(Medicine medicine)
        {
            return new MedicineDetailDto
            {
                MedicineID = medicine.MedicineID,
                MedicineName = medicine.MedicineName,
                GenericName = medicine.GenericName,
                CategoryID = medicine.CategoryID,
                CategoryName = medicine.Category?.CategoryName ?? string.Empty,
                SellingPrice = medicine.SellingPrice,
                Unit = medicine.Unit,
                DosageInstructions = medicine.DosageInstructions,
                RequiresPrescription = medicine.RequiresPrescription,
                IsActive = medicine.IsActive,
                CreatedAt = medicine.CreatedAt,
                UpdatedAt = medicine.UpdatedAt
            };
        }

        #endregion
    }
}

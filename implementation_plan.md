# Kế hoạch triển khai Frontend (Flutter) & Điều chỉnh Backend

Bản kế hoạch này mô tả việc:
1. **Thiết kế lại phân quyền hợp lý**:
   - **Admin**: Chỉ tập trung quản trị hệ thống kỹ thuật (không trực tiếp CRUD Branch, Category, Supplier, Staff).
   - **Operations Manager**: Toàn quyền quản trị nghiệp vụ chuỗi (Branch, Category, Supplier, Staff toàn chuỗi).
   - **Branch Manager**: Quản lý trong phạm vi chi nhánh (Staff của chi nhánh, Kho hàng, Báo cáo).
2. **Triển khai giao diện FE (Flutter)** cho các UC03, UC04, UC05, UC09.
3. **Cải tiến thiết kế màu sắc**: Chuyển từ Dark Mode sang **Light Mode chuyên nghiệp** với tông màu xanh y tế/dược phẩm nhã nhặn (Teal/Mint Green) giúp dễ đọc số liệu, tránh mỏi mắt.

---

## 🎨 Thiết kế Theme Giao diện (Medical Light Theme)

Thay vì sử dụng Dark Mode gây khó nhìn khi đọc bảng số liệu thuốc và nhân sự, hệ thống sẽ sử dụng giao diện sáng hiện đại (Light Mode) chuyên dụng cho y tế:

* **Primary Color (Màu chủ đạo)**: `0xFF00796B` (Teal - Xanh mòng két đậm) - mang lại cảm giác an tâm, chuyên nghiệp, đặc trưng y tế.
* **Secondary Color (Màu phụ)**: `0xFF00B0FF` (Xanh dương nhẹ) hoặc `0xFF80CBC4` (Xanh ngọc nhạt).
* **Background (Màu nền)**: `0xFFF8FAFC` (Màu xám/xanh nhạt dịu mắt, tránh chói lóa so với nền trắng tinh).
* **Surface (Màu thẻ/bảng)**: `0xFFFFFFFF` (Trắng tinh tế với đổ bóng nhẹ).
* **Text Color (Màu chữ)**: `0xFF1E293B` (Màu Slate đậm, tương phản cao, cực kỳ sắc nét dễ đọc).

---

## 🔐 Cập nhật phân quyền Backend (BE)

Tôi sẽ điều chỉnh phân quyền ở các Controller trên Backend để phù hợp với định hướng mới của bạn:
- **`BranchController`**: Sửa quyền ghi `POST/PUT/DELETE` từ `Admin,OperationsManager` thành chỉ `OperationsManager`.
- **`CategoryController`**: Sửa quyền ghi `POST/PUT/DELETE` thành chỉ `OperationsManager`.
- **`SupplierController`**: Sửa quyền ghi `POST/PUT/DELETE` thành chỉ `OperationsManager`.
- **`StaffController`**: Loại bỏ quyền của `Admin` ra khỏi quản lý Staff chi nhánh. Quyền ghi và đọc chỉ còn dành cho `OperationsManager` (toàn chuỗi) và `BranchManager` (nội bộ chi nhánh).

---

## 🗺️ Thiết kế Luồng & Route trên Frontend (GoRouter)

### 1. Dành cho Operations Manager (`/operations`)
Giao diện chính sẽ dùng BottomNavigationBar hoặc NavigationDrawer để truy cập 5 phân hệ chính:
- **Dashboard** (`/operations`): Thống kê doanh thu toàn chuỗi, cảnh báo hết hạn, tồn kho.
- **Chi nhánh** (`/operations/branches` - UC03): Danh sách, thêm/sửa, bật/tắt hoạt động chi nhánh.
- **Danh mục** (`/operations/categories` - UC04): Quản lý các nhóm thuốc bán chung.
- **Nhà cung cấp** (`/operations/suppliers` - UC05): Quản lý thông tin đối tác nhập hàng.
- **Nhân sự chuỗi** (`/operations/staff` - UC09): Xem và phân công nhân sự, quản lý tài khoản cho tất cả chi nhánh.

### 2. Dành cho Branch Manager (`/manager`)
Tập trung vào chi nhánh cụ thể của họ:
- **Dashboard** (`/manager`): Chỉ hiển thị số liệu của chi nhánh đó.
- **Tồn kho** (`/manager/inventory`): Quản lý số lượng thuốc tại kho của mình.
- **Nhân sự** (`/manager/staff` - UC09): Chỉ quản lý (xem, thêm, sửa, xóa mềm) các dược sĩ thuộc chi nhánh mình.
- **Báo cáo** (`/manager/reports`): Báo cáo bán hàng của chi nhánh.

---

## 📂 Cấu trúc Code FE sẽ được xây dựng

Tôi sẽ xây dựng các View, Controller (Services) tương ứng theo từng module trong thư mục `lib/features`:

### 1. UC03 - Manage Branch
- **[NEW]** `lib/features/operations_manager/services/branch_service.dart`: Gọi API `/api/branch` (GET/POST/PUT/DELETE).
- **[NEW]** `lib/features/operations_manager/views/branch_list_screen.dart`: Màn hình danh sách chi nhánh, tìm kiếm, lọc trạng thái.
- **[NEW]** `lib/features/operations_manager/views/branch_form_screen.dart`: Màn hình thêm mới/chỉnh sửa chi nhánh.

### 2. UC04 - Manage Category
- **[NEW]** `lib/features/operations_manager/services/category_service.dart`: Gọi API `/api/category`.
- **[NEW]** `lib/features/operations_manager/views/category_list_screen.dart`: Quản lý danh mục thuốc.

### 3. UC05 - Manage Supplier
- **[NEW]** `lib/features/operations_manager/services/supplier_service.dart`: Gọi API `/api/supplier`.
- **[NEW]** `lib/features/operations_manager/views/supplier_list_screen.dart`: Quản lý nhà cung cấp.

### 4. UC09 - Manage Staff
- **[NEW]** `lib/features/shared/services/staff_service.dart`: Gọi API chung `/api/staff` cho cả 2 role.
- **[NEW]** `lib/features/operations_manager/views/staff_management_screen.dart`: Màn hình quản lý nhân sự toàn chuỗi của Operations Manager.
- **[NEW]** `lib/features/branch_manager/views/branch_staff_screen.dart`: Màn hình quản lý nội bộ nhân sự của Branch Manager.
- **[NEW]** `lib/features/shared/views/staff_form_screen.dart`: Form chung thêm/sửa nhân viên (truyền tham số tự động gán BranchID).

---

## 🔬 Kế hoạch Kiểm thử & Xác minh (Verification Plan)

### 1. Build & Run
- Chạy `flutter pub get` để cập nhật gói.
- Chạy dự án bằng lệnh `flutter run -d chrome` hoặc thiết bị máy ảo Android/iOS để kiểm tra UI.

### 2. Manual Verification
- **Test theme màu**: Đảm bảo toàn bộ ứng dụng chuyển sang Light Mode tông xanh Teal/Mint dễ chịu, các bảng biểu hiển thị sắc nét.
- **Test phân quyền Admin**: Đăng nhập bằng tài khoản `admin` -> Xác minh không nhìn thấy các menu Branch, Category, Supplier, Staff.
- **Test phân quyền Operations Manager**: Đăng nhập bằng `opsmanager@gmail.com` -> Xác minh có đầy đủ 5 mục quản lý toàn chuỗi.
- **Test phân quyền Branch Manager**: Đăng nhập bằng `branchmanager@gmail.com` -> Vào mục Nhân sự -> Xác minh chỉ hiển thị nhân sự của chi nhánh mình, không thể chỉnh sửa nhân viên chi nhánh khác.

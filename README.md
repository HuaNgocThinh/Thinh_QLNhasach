# 📚 Thinh_QLNhasach — Phần Mềm Quản Lý Nhà Sách

> Ứng dụng Windows Forms (C# / .NET) quản lý toàn bộ hoạt động kinh doanh của một cửa hàng sách: từ quản lý kho, nhập hàng, bán hàng, khách hàng cho đến thống kê báo cáo.

---

## 📋 Mục lục

- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt & Khôi phục Database](#-cài-đặt--khôi-phục-database)
- [Cấu hình kết nối](#-cấu-hình-kết-nối)
- [Chức năng hệ thống](#-chức-năng-hệ-thống)
- [Sơ đồ cơ sở dữ liệu](#-sơ-đồ-cơ-sở-dữ-liệu)
- [Tác giả](#-tác-giả)

---

## 💻 Yêu cầu hệ thống

| Thành phần | Phiên bản |
|---|---|
| Hệ điều hành | Windows 10 / 11 |
| .NET Framework | 4.8 hoặc .NET 6+ |
| SQL Server | SQL Server 2019 / 2022 hoặc SQL Server Express |
| SQL Server Management Studio (SSMS) | 18.x trở lên |
| Visual Studio | 2019 / 2022 |

---

## 🗄️ Cài đặt & Khôi phục Database

### Bước 1 — Mở SQL Server Management Studio (SSMS)

1. Khởi động **SSMS**.
2. Tại hộp thoại **Connect to Server**, nhập thông tin kết nối:
   - **Server type:** Database Engine
   - **Server name:** `localhost\SQLEXPRESS` *(hoặc tên instance SQL Server của bạn)*
   - **Authentication:** Windows Authentication *(hoặc SQL Server Authentication nếu có tài khoản)*
3. Nhấn **Connect**.

### Bước 2 — Restore file `.bak`

1. Trong **Object Explorer**, chuột phải lên mục **Databases** → chọn **Restore Database...**.

   ![Restore menu](https://i.imgur.com/placeholder-restore.png)

2. Tại cửa sổ **Restore Database**:
   - Chọn **Device** → nhấn nút **...** (Browse).
   - Nhấn **Add** → điều hướng đến file `BookShop.bak` → nhấn **OK**.

3. Tại mục **Destination**:
   - **Database:** nhập tên `BookShop` *(hoặc giữ nguyên tên mặc định từ file backup)*.

4. Chuyển sang tab **Options**:
   - Tích chọn ✅ **Overwrite the existing database (WITH REPLACE)** *(nếu database đã tồn tại)*.
   - Kiểm tra đường dẫn lưu file `.mdf` và `.ldf` trong phần **Restore As** — đảm bảo thư mục tồn tại.

5. Nhấn **OK** để bắt đầu quá trình khôi phục.

6. Khi xuất hiện thông báo:
   ```
   Database 'BookShop' restored successfully.
   ```
   quá trình đã hoàn tất.

### Bước 3 — Kiểm tra database

Mở **New Query** và chạy lệnh sau để xác nhận:

```sql
USE BookShop;
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

---

## ⚙️ Cấu hình kết nối

Mở file cấu hình kết nối trong project (thường là `app.config` hoặc một class `DBConnection.cs`), chỉnh sửa **Connection String** cho phù hợp với môi trường của bạn:

```xml
<!-- app.config -->
<connectionStrings>
  <add name="BookShopDB"
       connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=BookShop;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **Lưu ý:** Nếu dùng SQL Server Authentication, thay `Integrated Security=True` bằng `User ID=sa;Password=yourpassword`.

---

## 🧩 Chức năng hệ thống

### 1. 🔐 Đăng nhập & Phân quyền

- Màn hình đăng nhập với tài khoản và mật khẩu.
- Hệ thống phân quyền theo vai trò:
  - **Admin:** toàn quyền truy cập tất cả chức năng.
  - **Nhân viên bán hàng:** chỉ truy cập bán hàng, tra cứu sách, xem hóa đơn.
  - **Thủ kho:** quản lý nhập hàng và tồn kho.
- Đổi mật khẩu tài khoản.

---

### 2. 📖 Quản lý Sách

| Chức năng | Mô tả |
|---|---|
| Thêm sách | Nhập thông tin sách mới vào hệ thống |
| Sửa thông tin sách | Cập nhật tên, giá, mô tả, ảnh bìa, v.v. |
| Xóa sách | Xóa sách khỏi danh mục (kiểm tra ràng buộc) |
| Tìm kiếm sách | Tìm theo tên, mã sách, tác giả, thể loại, NXB |
| Xem chi tiết sách | Hiển thị đầy đủ thông tin: tên, tác giả, NXB, thể loại, giá bán, số lượng tồn |
| Quản lý ảnh bìa | Upload và hiển thị ảnh bìa sách |

**Thông tin sách bao gồm:** Mã sách, Tên sách, Tác giả, Nhà xuất bản, Thể loại, Năm xuất bản, Giá nhập, Giá bán, Số lượng tồn kho, Mô tả, Ảnh bìa.

---

### 3. 🗂️ Quản lý Danh mục

#### Thể loại sách
- Thêm / sửa / xóa thể loại (Văn học, Khoa học, Kỹ thuật, Thiếu nhi, v.v.).

#### Nhà xuất bản
- Thêm / sửa / xóa thông tin nhà xuất bản: tên, địa chỉ, số điện thoại, email.

#### Tác giả
- Quản lý thông tin tác giả: tên, quốc tịch, tiểu sử ngắn.

---

### 4. 📦 Quản lý Nhập hàng (Kho)

- Tạo **phiếu nhập hàng** từ nhà cung cấp.
- Nhập nhiều đầu sách trong một phiếu nhập.
- Tự động **cập nhật số lượng tồn kho** sau khi nhập.
- Xem lịch sử các phiếu nhập theo ngày, theo nhà cung cấp.
- In phiếu nhập hàng.

---

### 5. 🏪 Quản lý Nhà cung cấp

- Thêm / sửa / xóa nhà cung cấp.
- Thông tin: tên công ty, địa chỉ, số điện thoại, email, người liên hệ.
- Xem lịch sử nhập hàng theo từng nhà cung cấp.

---

### 6. 👥 Quản lý Khách hàng

- Thêm / sửa / xóa thông tin khách hàng.
- Thông tin: họ tên, số điện thoại, địa chỉ, email, ngày sinh.
- Tra cứu lịch sử mua hàng của từng khách hàng.
- Tìm kiếm khách hàng theo tên hoặc số điện thoại.

---

### 7. 🛒 Bán hàng & Lập hóa đơn

- Giao diện **bán hàng nhanh** (Point of Sale):
  - Tìm kiếm và chọn sách bằng mã hoặc tên.
  - Thêm sách vào giỏ hàng, điều chỉnh số lượng.
  - Chọn khách hàng (có thể bán cho khách vãng lai).
  - Áp dụng **khuyến mãi / giảm giá**.
  - Tính tiền, nhận tiền, trả lại tiền thừa.
- Tự động **trừ số lượng tồn kho** sau khi bán.
- **In hóa đơn** bán hàng (hỗ trợ máy in hóa đơn nhiệt).
- Xem lại và tìm kiếm hóa đơn đã lập.

---

### 8. 👨‍💼 Quản lý Nhân viên

- Thêm / sửa / xóa thông tin nhân viên.
- Thông tin: họ tên, chức vụ, số điện thoại, địa chỉ, ngày vào làm.
- Phân công tài khoản đăng nhập cho nhân viên.
- Theo dõi hoạt động của từng nhân viên (hóa đơn đã lập, phiếu nhập đã tạo).

---

### 9. 📊 Thống kê & Báo cáo

| Báo cáo | Nội dung |
|---|---|
| Doanh thu theo ngày / tháng / năm | Tổng doanh thu, số hóa đơn, trung bình/hóa đơn |
| Top sách bán chạy | Xếp hạng sách theo số lượng đã bán |
| Tồn kho hiện tại | Danh sách sách còn tồn, cảnh báo sách sắp hết |
| Báo cáo nhập hàng | Tổng giá trị nhập theo kỳ |
| Doanh thu theo nhân viên | Hiệu suất bán hàng từng nhân viên |
| Khách hàng thân thiết | Xếp hạng khách theo tổng chi tiêu |

- Hiển thị biểu đồ cột / đường trực quan (Chart).
- Xuất báo cáo ra **Excel (.xlsx)** hoặc **PDF**.
- Lọc báo cáo theo khoảng thời gian tùy chọn.

---

### 10. 🔔 Cảnh báo hệ thống

- Cảnh báo sách **sắp hết hàng** (dưới ngưỡng tồn kho tối thiểu).
- Thông báo khi **số lượng nhập** vượt quá giới hạn cho phép.

---

## 🗃️ Sơ đồ cơ sở dữ liệu

```
NhanVien ────────────────────────────────────────────┐
   │                                                  │
   │ (tao boi)                                        │
   ▼                                                  ▼
PhieuNhap ──── ChiTietPhieuNhap ──── Sach ──── ChiTietHoaDon
                                      │               │
                              NhaXuatBan         HoaDon ── KhachHang
                              TheLoaiSach
                              TacGia
                              NhaCungCap
```

**Các bảng chính:**

| Bảng | Mô tả |
|---|---|
| `Sach` | Thông tin sách (mã, tên, giá, tồn kho, ...) |
| `TheLoai` | Thể loại sách |
| `TacGia` | Tác giả |
| `NhaXuatBan` | Nhà xuất bản |
| `NhaCungCap` | Nhà cung cấp |
| `KhachHang` | Khách hàng |
| `NhanVien` | Nhân viên |
| `TaiKhoan` | Tài khoản đăng nhập, vai trò, mật khẩu |
| `HoaDon` | Hóa đơn bán hàng |
| `ChiTietHoaDon` | Chi tiết từng dòng hóa đơn |
| `PhieuNhap` | Phiếu nhập kho |
| `ChiTietPhieuNhap` | Chi tiết từng dòng phiếu nhập |

---

## 📁 Cấu trúc Project

```
Thinh_QLNhasach/
├── Thinh_QLNhasach.sln
├── Thinh_QLNhasach/
│   ├── Forms/
│   │   ├── frmDangNhap.cs          # Màn hình đăng nhập
│   │   ├── frmMain.cs              # Form chính / menu
│   │   ├── frmQuanLySach.cs        # Quản lý sách
│   │   ├── frmQuanLyKhachHang.cs   # Quản lý khách hàng
│   │   ├── frmQuanLyNhanVien.cs    # Quản lý nhân viên
│   │   ├── frmBanHang.cs           # Giao diện bán hàng
│   │   ├── frmHoaDon.cs            # Xem / in hóa đơn
│   │   ├── frmNhapHang.cs          # Nhập kho
│   │   ├── frmThongKe.cs           # Thống kê báo cáo
│   │   └── frmDanhMuc.cs           # Quản lý danh mục
│   ├── DAL/                        # Data Access Layer
│   │   ├── DBConnection.cs
│   │   ├── SachDAL.cs
│   │   └── ...
│   ├── BLL/                        # Business Logic Layer
│   │   ├── SachBLL.cs
│   │   └── ...
│   ├── Models/                     # Entity classes
│   │   ├── Sach.cs
│   │   ├── KhachHang.cs
│   │   └── ...
│   ├── Resources/                  # Ảnh, icon
│   └── app.config
└── BookShop.bak                    # File backup SQL Server
```

---

## 🚀 Hướng dẫn chạy project

1. **Clone** repository về máy:
   ```bash
   git clone https://github.com/<your-username>/Thinh_QLNhasach.git
   ```

2. **Restore database** theo hướng dẫn ở phần [Cài đặt & Khôi phục Database](#-cài-đặt--khôi-phục-database).

3. **Mở solution** `Thinh_QLNhasach.slnx` bằng Visual Studio.

4. **Chỉnh sửa Connection String** trong `app.config` cho phù hợp với tên SQL Server instance của bạn.

5. **Build & Run** (F5).

---

## 👤 Tác giả

| | |
|---|---|
| **Họ tên** | Thịnh |
| **Tài khoản máy** | `THINHLALUOT` |
| **SQL Server Instance** | `THINHLALUOT\SQLEXPRESS01` |
| **Công nghệ** | C# · Windows Forms · SQL Server · ADO.NET |

---

## 📄 License

Dự án được phát triển cho mục đích học tập. Vui lòng ghi nguồn khi sử dụng lại.

# 📚 Hệ Thống Quản Lý Nhà Sách — BookShop

> **Nền tảng:** Windows Forms (.NET) · **CSDL:** SQL Server (SSMS) · **Tên DB:** `BookShop`

---

## 📋 Mục Lục

- [Giới thiệu](#-giới-thiệu)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt Database (SSMS)](#-hướng-dẫn-cài-đặt-database-ssms)
- [Cấu trúc Database](#-cấu-trúc-database)
- [Tài khoản mặc định](#-tài-khoản-mặc-định)
- [Chức năng hệ thống](#-chức-năng-hệ-thống)
- [Sơ đồ quan hệ (ERD)](#-sơ-đồ-quan-hệ-erd)

---

## 🧾 Giới thiệu

**BookShop** là ứng dụng quản lý cửa hàng sách xây dựng bằng **Windows Forms (C#)**, kết nối **SQL Server** thông qua ADO.NET. Hệ thống hỗ trợ đầy đủ nghiệp vụ bán sách: từ quản lý danh mục, nhập hàng, lập hóa đơn bán hàng, quản lý kho đến phân quyền người dùng và ghi nhật ký hoạt động.

---

## 💻 Yêu cầu hệ thống

| Thành phần | Phiên bản tối thiểu |
|---|---|
| .NET Framework / .NET | .NET 6+ hoặc .NET Framework 4.8 |
| SQL Server | SQL Server 2019+ hoặc SQL Server Express |
| SSMS | 18.x trở lên |
| Visual Studio | 2022 (có hỗ trợ WinForms) |
| Hệ điều hành | Windows 10/11 |

---

## 🗄️ Hướng dẫn cài đặt Database (SSMS)

### Bước 1 — Mở SSMS và kết nối Server

1. Khởi động **SQL Server Management Studio (SSMS)**.
2. Tại hộp thoại **Connect to Server**, nhập thông tin:
   - **Server type:** Database Engine
   - **Server name:** `.\SQLEXPRESS` hoặc tên instance SQL Server của bạn (ví dụ: `localhost`)
   - **Authentication:** Windows Authentication (hoặc SQL Server Authentication nếu có tài khoản riêng)
3. Nhấn **Connect**.

### Bước 2 — Mở file script SQL

1. Trên thanh công cụ, chọn **File → Open → File...** (hoặc nhấn `Ctrl + O`).
2. Duyệt đến file `Thinh_QLNhaSach.sql` và nhấn **Open**.
3. File script sẽ mở trong cửa sổ Query Editor.

### Bước 3 — Chạy script để tạo Database

1. Đảm bảo kết nối đang trỏ đến đúng server (kiểm tra thanh trạng thái phía dưới bên phải).
2. Nhấn **F5** hoặc nhấn nút **Execute** (▶) để chạy toàn bộ script.
3. Script sẽ tự động:
   - Tạo database `BookShop`
   - Tạo toàn bộ các bảng
   - Chèn dữ liệu mẫu vào tất cả các bảng

> ⚠️ **Lưu ý:**  
> - Nếu database `BookShop` đã tồn tại, hãy xóa trước: chuột phải vào `BookShop` → **Delete** → tích chọn **Close existing connections** → **OK**.  
> - Script được mã hóa **UTF-16 LE** — SSMS đọc được trực tiếp, không cần chuyển đổi.  
> - Đường dẫn lưu file `.mdf` mặc định trong script là `C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\`. Nếu instance của bạn khác, SQL Server sẽ tự điều chỉnh hoặc bạn có thể sửa đường dẫn trước khi chạy.

### Bước 4 — Kiểm tra kết quả

Sau khi chạy thành công, trong **Object Explorer** bạn sẽ thấy:

```
BookShop
 └── Tables
      ├── dbo.ChiTietHoaDon
      ├── dbo.ChiTietPhieuNhap
      ├── dbo.HoaDon
      ├── dbo.Kho
      ├── dbo.NguoiDung
      ├── dbo.NhaCungCap
      ├── dbo.NhatKyHoatDong
      ├── dbo.NhaXuatBan
      ├── dbo.PhieuNhap
      ├── dbo.Sach
      ├── dbo.TacGia
      └── dbo.TheLoai
```

### Bước 5 — Cấu hình connection string trong project

Mở file cấu hình kết nối trong project (thường là `App.config` hoặc một class `DBConnection`) và chỉnh sửa chuỗi kết nối cho phù hợp với môi trường của bạn:

```xml
<connectionStrings>
  <add name="BookShop"
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=BookShop;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> Thay `.\SQLEXPRESS` bằng tên instance SQL Server thực tế trên máy bạn.

---

## 🗂️ Cấu trúc Database

### Danh sách các bảng

| Tên bảng | Mô tả |
|---|---|
| `NguoiDung` | Tài khoản người dùng hệ thống (Admin, Staff, Nhân viên) |
| `Sach` | Danh mục sách |
| `TacGia` | Thông tin tác giả |
| `TheLoai` | Thể loại sách |
| `NhaXuatBan` | Thông tin nhà xuất bản |
| `NhaCungCap` | Thông tin nhà cung cấp |
| `HoaDon` | Hóa đơn bán hàng |
| `ChiTietHoaDon` | Chi tiết từng dòng sách trong hóa đơn |
| `PhieuNhap` | Phiếu nhập hàng từ nhà cung cấp |
| `ChiTietPhieuNhap` | Chi tiết từng dòng sách trong phiếu nhập |
| `Kho` | Số lượng tồn kho theo từng đầu sách |
| `NhatKyHoatDong` | Nhật ký ghi lại các thao tác của người dùng |

### Chi tiết cột chính

#### `NguoiDung` — Người dùng
| Cột | Kiểu | Mô tả |
|---|---|---|
| `MaND` | int IDENTITY | Khóa chính |
| `TenDangNhap` | nvarchar(50) | Tên đăng nhập (duy nhất) |
| `HoTen` | nvarchar(100) | Họ tên đầy đủ |
| `NgaySinh` | date | Ngày sinh |
| `SoDienThoai` | nvarchar(15) | Số điện thoại |
| `VaiTro` | nvarchar(20) | Vai trò: `Admin` / `Staff` / `Nhân viên` |
| `TrangThai` | nvarchar(20) | Trạng thái: `Còn làm` / nghỉ việc |
| `MatKhau` | varbinary(max) | Mật khẩu được mã hóa MD5 |

#### `Sach` — Sách
| Cột | Kiểu | Mô tả |
|---|---|---|
| `MaSach` | int IDENTITY | Khóa chính |
| `TenSach` | nvarchar(200) | Tên sách |
| `MaTG` | int | FK → TacGia |
| `MaTL` | int | FK → TheLoai |
| `MaNXB` | int | FK → NhaXuatBan |
| `NamXuatBan` | int | Năm xuất bản |
| `GiaNhap` | decimal(10,2) | Giá nhập |
| `GiaBan` | decimal(10,2) | Giá bán |
| `SoLuongTon` | int | Số lượng tồn kho |
| `MoTa` | nvarchar(max) | Mô tả nội dung sách |
| `HinhAnh` | nvarchar(255) | Tên file ảnh bìa |

#### `HoaDon` — Hóa đơn bán hàng
| Cột | Kiểu | Mô tả |
|---|---|---|
| `MaHD` | nvarchar(50) | Khóa chính (ví dụ: HD001) |
| `MaND` | int | FK → NguoiDung (nhân viên lập) |
| `TenKhachHang` | nvarchar(100) | Tên khách hàng |
| `NgayLap` | datetime | Ngày lập hóa đơn (mặc định: getdate()) |
| `TongTien` | decimal(10,2) | Tổng tiền trước giảm giá |
| `GiamGia` | decimal(10,2) | Số tiền giảm giá |
| `ThanhTien` | decimal(10,2) | Thành tiền sau giảm giá |

#### `PhieuNhap` — Phiếu nhập hàng
| Cột | Kiểu | Mô tả |
|---|---|---|
| `MaPN` | nvarchar(50) | Khóa chính (ví dụ: PN001) |
| `NgayNhap` | datetime | Ngày nhập hàng |
| `MaNCC` | int | FK → NhaCungCap |
| `MaND` | int | FK → NguoiDung (người lập phiếu) |
| `TongTien` | decimal(10,2) | Tổng giá trị phiếu nhập |
| `GhiChu` | nvarchar(255) | Ghi chú thêm |

---

## ⚙️ Chức năng hệ thống

Hệ thống phân quyền theo 3 vai trò: **Admin**, **Staff** và **Nhân viên**. Dưới đây là toàn bộ chức năng hiện có.

---

### 🔐 1. Đăng nhập & Phân quyền

- Màn hình đăng nhập bằng tên đăng nhập và mật khẩu.
- Mật khẩu được mã hóa MD5 trước khi so sánh với database.
- Sau khi đăng nhập thành công, hệ thống load giao diện phù hợp theo vai trò.
- Tài khoản có trạng thái nghỉ việc sẽ không được phép đăng nhập.

---

### 📖 2. Quản lý Sách

Cho phép xem, thêm, sửa, xóa thông tin sách trong hệ thống.

**Thông tin quản lý:**
- Tên sách, tác giả, thể loại, nhà xuất bản, năm xuất bản
- Giá nhập, giá bán
- Số lượng tồn kho
- Mô tả và hình ảnh bìa sách

**Tính năng:**
- Tìm kiếm sách theo tên (có index `IDX_Sach_Ten` hỗ trợ tìm kiếm nhanh)
- Upload và hiển thị ảnh bìa sách
- Lưu hàng loạt thay đổi (Thêm / Sửa / Xóa) vào database trong một lần xác nhận
- Ghi nhật ký hoạt động sau mỗi lần lưu

---

### ✍️ 3. Quản lý Tác giả

Quản lý danh sách tác giả của các đầu sách.

**Thông tin quản lý:**
- Tên tác giả, quê quán, năm sinh, năm mất (nếu có)

**Tính năng:**
- Thêm, sửa, xóa tác giả trực tiếp trên lưới dữ liệu (DataGridView)
- Chốt danh sách (lưu hàng loạt) với thông báo số lượng thêm / sửa / xóa
- Ghi nhật ký chi tiết: ví dụ *"Thêm: 1, Sửa: 0, Xóa: 0"*

---

### 🏷️ 4. Quản lý Thể loại

Quản lý các thể loại sách (Tiểu thuyết, Khoa học, Ngôn tình, Kinh dị, Kỹ năng sống, Trinh thám, Tâm lý,...).

**Thông tin quản lý:**
- Tên thể loại, mô tả

**Tính năng:**
- Thêm, sửa, xóa thể loại
- Lưu thay đổi hàng loạt và ghi nhật ký

---

### 🏢 5. Quản lý Nhà xuất bản

Quản lý danh sách các nhà xuất bản liên kết với cửa hàng.

**Thông tin quản lý:**
- Tên NXB, địa chỉ, số điện thoại, email

**Tính năng:**
- Thêm, sửa, xóa nhà xuất bản
- Lưu thay đổi và ghi nhật ký hoạt động

---

### 🚛 6. Quản lý Nhà cung cấp

Quản lý các đơn vị cung cấp sách cho cửa hàng.

**Thông tin quản lý:**
- Tên nhà cung cấp, địa chỉ, số điện thoại, email

**Tính năng:**
- Thêm, sửa, xóa nhà cung cấp

---

### 📦 7. Quản lý Nhập hàng (Phiếu nhập)

Nghiệp vụ nhập sách từ nhà cung cấp vào kho.

**Thông tin phiếu nhập:**
- Mã phiếu nhập (tự sinh, ví dụ: PN001)
- Ngày nhập, nhà cung cấp, người lập phiếu
- Danh sách sách nhập kèm số lượng và đơn giá nhập
- Tổng tiền, ghi chú

**Tính năng:**
- Tạo phiếu nhập mới, chọn nhà cung cấp
- Thêm nhiều dòng sách vào một phiếu nhập
- Tự động tính thành tiền theo từng dòng và tổng tiền toàn phiếu
- Sau khi lưu, số lượng tồn kho trong bảng `Sach` được cập nhật tự động
- Ghi nhật ký: *"Đã lập phiếu nhập mới mã PNxxx với tổng tiền x,xxx,xxx VNĐ"*
- Xem lịch sử các phiếu nhập đã lập

---

### 🧾 8. Quản lý Bán hàng (Hóa đơn)

Nghiệp vụ bán sách và lập hóa đơn cho khách hàng.

**Thông tin hóa đơn:**
- Mã hóa đơn (tự sinh, ví dụ: HD001)
- Tên khách hàng, ngày lập, nhân viên lập
- Danh sách sách bán kèm số lượng và đơn giá
- Tổng tiền, giảm giá, thành tiền

**Tính năng:**
- Tạo hóa đơn mới, nhập tên khách hàng
- Thêm sách vào hóa đơn, tự động lấy giá bán từ danh mục
- Hỗ trợ **giảm giá** theo số tiền cụ thể
- Tự động tính thành tiền sau giảm giá
- Sau khi lưu hóa đơn, số lượng tồn kho tự động giảm theo
- **Hủy hóa đơn:** hoàn lại số lượng sách vào kho
- Ghi nhật ký: *"Lập thành công hóa đơn HDxxx - Tổng tiền: x,xxx,xxx VNĐ"*
- Xem lịch sử hóa đơn đã lập (có index `IDX_HoaDon_Ngay` hỗ trợ lọc theo ngày)

---

### 🏪 9. Quản lý Kho

Theo dõi số lượng tồn kho của từng đầu sách.

**Thông tin:**
- Mã sách, tên sách, số lượng tồn

**Tính năng:**
- Xem danh sách tồn kho hiện tại
- Số lượng kho được cập nhật tự động khi có phiếu nhập hoặc hóa đơn bán ra

---

### 👥 10. Quản lý Người dùng (Admin)

Chức năng dành riêng cho **Admin** để quản lý tài khoản nhân viên.

**Thông tin quản lý:**
- Tên đăng nhập, họ tên, ngày sinh, số điện thoại
- Vai trò (Admin / Staff / Nhân viên)
- Trạng thái (Còn làm / Nghỉ việc)
- Mật khẩu (mã hóa MD5)

**Tính năng:**
- Thêm tài khoản nhân viên mới
- Chỉnh sửa thông tin, vai trò, trạng thái nhân viên
- Vô hiệu hóa tài khoản (đổi trạng thái) thay vì xóa vĩnh viễn
- Đặt lại mật khẩu

---

### 📋 11. Nhật ký hoạt động

Tự động ghi lại toàn bộ thao tác quan trọng của người dùng trong hệ thống.

**Thông tin ghi lại:**
- Tên đăng nhập thực hiện
- Hành động (Tạo Hóa Đơn, Lập Phiếu Nhập, Cập nhật Sách, Hủy Hóa Đơn,...)
- Chi tiết nội dung thao tác
- Thời gian thực hiện (tự động lấy `getdate()`)

**Các hành động được ghi nhật ký:**

| Hành động | Ví dụ chi tiết |
|---|---|
| Tạo Hóa Đơn | `Lập thành công hóa đơn HD015 - Tổng tiền: 3,135,000 VNĐ` |
| Hủy Hóa Đơn | `Đã hủy hóa đơn HD013 và hoàn lại sách vào kho` |
| Lập Phiếu Nhập | `Đã lập phiếu nhập mới mã PN007 với tổng tiền 10,000,000 VNĐ` |
| Cập nhật Sách | `Đã lưu thay đổi danh sách Sách vào hệ thống` |
| Cập nhật Tác giả | `Thêm: 1, Sửa: 0, Xóa: 0` |
| Cập nhật Thể Loại | `Đã lưu thay đổi danh sách Thể Loại vào hệ thống` |
| Cập nhật Nhà Xuất Bản | `Đã lưu thay đổi danh sách Nhà Xuất Bản vào hệ thống` |

---

## 🔗 Sơ đồ quan hệ (ERD)

```
TacGia ──────┐
             ├──→ Sach ←── TheLoai
NhaXuatBan ──┘      │
                     │
              ┌──────┴──────┐
              ↓             ↓
       ChiTietHoaDon   ChiTietPhieuNhap
              │             │
              ↓             ↓
           HoaDon       PhieuNhap ←── NhaCungCap
              │             │
              └──────┬──────┘
                     ↓
                 NguoiDung
                 
Sach ──→ Kho
NhatKyHoatDong (ghi log độc lập)
```

### Các ràng buộc khóa ngoại (Foreign Key)

| Bảng con | Cột | Bảng cha |
|---|---|---|
| `Sach` | `MaTG` | `TacGia` |
| `Sach` | `MaTL` | `TheLoai` |
| `Sach` | `MaNXB` | `NhaXuatBan` |
| `HoaDon` | `MaND` | `NguoiDung` |
| `ChiTietHoaDon` | `MaHD` | `HoaDon` |
| `ChiTietHoaDon` | `MaSach` | `Sach` |
| `PhieuNhap` | `MaNCC` | `NhaCungCap` |
| `PhieuNhap` | `MaND` | `NguoiDung` |
| `ChiTietPhieuNhap` | `MaPN` | `PhieuNhap` |
| `ChiTietPhieuNhap` | `MaSach` | `Sach` |
| `Kho` | `MaSach` | `Sach` |

---

## 👨‍💻 Tác giả

**Đào Ngọc Thịnh** — Dự án học phần Lập trình Windows Forms

---


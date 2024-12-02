CREATE DATABASE QLCoffee;
GO
USE QLCoffee;
GO

CREATE TABLE ChiNhanh (
    MaChiNhanh CHAR(5) PRIMARY KEY, -- Khóa chính
    TenChiNhanh NVARCHAR(255), -- Tên chi nhánh
    DiaChi NVARCHAR(255), -- Địa chỉ chi nhánh
    SoDienThoai NVARCHAR(20) -- Số điện thoại
);

-- 2. Bảng Khách hàng (Customer)
CREATE TABLE KhachHang (
    MaKhachHang CHAR(10) Not null, -- Mã khách hàng
    HoTen NVARCHAR(100), -- Họ và tên
    SoDienThoai NVARCHAR(15), -- Số điện thoại
    Email NVARCHAR(100), -- Email
    NgaySinh DATE, -- Ngày sinh
    DiaChi NVARCHAR(255), -- Địa chỉ
    GioiTinh NVARCHAR(10), -- Giới tính
    MaChiNhanh CHAR(5), -- Khóa ngoại
    LoaiKhachHang NVARCHAR(20) DEFAULT 'Thường', -- Loại khách hàng
    GhiChu NVARCHAR(255), -- Ghi chú
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh),
	primary key (MaKhachHang)
);
-- 3. Bảng Nhân viên (Employee)
CREATE TABLE NhanVien (
    MaNhanVien CHAR(10) not null, -- Mã nhân viên
    HoTen NVARCHAR(100), -- Họ và tên
    SoDienThoai NVARCHAR(15), -- Số điện thoại
    Email NVARCHAR(100), -- Email
    ChucVu NVARCHAR(50), -- Chức vụ
    LuongCoBan DECIMAL(10, 2), -- Lương cơ bản
    NgayVaoLam DATE, -- Ngày vào làm
    MaChiNhanh CHAR(5), -- Khóa ngoại
    GhiChu NVARCHAR(255), -- Ghi chú
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh),
	PRIMARY KEY (MaNhanVien)
);

-- 4. Bảng Phòng hát (Room)
CREATE TABLE BanAn (
    MaBan CHAR(10) not null, 
    TenBan CHAR(10), 
    TrangThai NVARCHAR(20), -- Trạng thái
    MaChiNhanh CHAR(5), -- Khóa ngoại
    NgayTao DATETIME DEFAULT GETDATE(),
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh),
	PRIMARY KEY (MaBan)
);

CREATE TABLE HoaDon (
    MaHoaDon CHAR(10) PRIMARY KEY, -- Mã hóa đơn
    MaKhachHang CHAR(10), -- Khóa ngoại
    MaBan CHAR(10), -- Khóa ngoại
    NgayLapHoaDon DATE, -- Ngày lập
    TongTien DECIMAL(10, 2), -- Tổng tiền
    ThanhToan NVARCHAR(10), -- Trạng thái thanh toán
    GiamGia DECIMAL(18, 2) DEFAULT 0, -- Giảm giá
    GhiChu NVARCHAR(255), -- Ghi chú
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaBan) REFERENCES BanAn(MaBan)
);


-- 8. Bảng Đặt phòng (Booking)
CREATE TABLE DatBan (
    MaDatBan CHAR(10) PRIMARY KEY, -- Mã đặt bàn
    MaKhachHang CHAR(10), -- Khóa ngoại
    MaBan CHAR(10), -- Khóa ngoại
    TinhTrang NVARCHAR(20), -- Tình trạng
    GhiChu NVARCHAR(255), -- Ghi chú
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaBan) REFERENCES BanAn(MaBan)
);


-- 9. Bảng Bảng lương (Payroll)
CREATE TABLE BangLuong (
    MaBangLuong CHAR(10) PRIMARY KEY, -- Mã bảng lương
    MaNhanVien CHAR(10), -- Khóa ngoại
    ThangTinhLuong Date, -- Tháng tính lương
    SoGioLam int,
    LuongThucLanh DECIMAL(10, 2), -- Lương thực lãnh
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

-- 10. Bảng Điểm danh (Attendance)
CREATE TABLE DiemDanh (
    MaDiemDanh Char(10) PRIMARY KEY, 
    MaNhanVien CHAR(10), -- Khóa ngoại
    NgayDiemDanh DATE, -- Ngày điểm danh
    ThoiGianDiLam DATETIME, -- Thời gian đến
    ThoiGianVeLam DATETIME, -- Thời gian về
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

CREATE TABLE LichSuNhanLuong (
    MaNhanVien Char(10),
    Thang INT,
    Nam INT,
    TongGioLam DECIMAL(18,2),
    TongLuong DECIMAL(18,2),
    TrangThaiNhanLuong BIT,
    PRIMARY KEY(MaNhanVien, Thang, Nam),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

-- 13. Bảng Doanh thu (Revenue)
CREATE TABLE DoanhThu (
    MaDoanhThu CHAR(10) PRIMARY KEY, -- Mã doanh thu
    Thang DATE, -- Tháng
    TongDoanhThu DECIMAL(18, 2), -- Tổng doanh thu
    TongChiPhi DECIMAL(18, 2), -- Tổng chi phí
    LoiNhuan DECIMAL(18, 2), -- Lợi nhuận
    GhiChu NVARCHAR(255), -- Ghi chú
    MaChiNhanh CHAR(5), -- Khóa ngoại
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

-- 14. Bảng nhà cung cấp
CREATE TABLE NhaCungCap (
	MaNhaCungCap CHAR(10) PRIMARY KEY,
	MaChiNhanh char(5),
	TenNhaCungCap NVARCHAR(100),
	DiaChi NVARCHAR(250),
	SoDienThoai Char(15),
	FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

-- 15. Bảng Sản phẩm (Product)
CREATE TABLE SanPham (
    MaSanPham CHAR(10) PRIMARY KEY, -- Mã sản phẩm
    TenSanPham NVARCHAR(100), -- Tên sản phẩm
    LoaiSanPham NVARCHAR(50), -- Loại sản phẩm
    DonViTinh NVARCHAR(50), -- Đơn vị tính
    GiaBan DECIMAL(18, 2), -- Giá bán
    MaChiNhanh CHAR(5), -- Khóa ngoại
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

-- 16. Bảng Nhập hàng (ImportOrder)
CREATE TABLE NhapHang (
    MaNhapHang CHAR(10) PRIMARY KEY,
	MaSanPham CHAR(10),
    NgayNhapHang DATE,
    SoLuong INT, -- Số lượng
    DonGia DECIMAL(18, 2), -- Đơn giá nhập
    ThanhTien DECIMAL(18, 2), -- Thành tiền
    MaNhaCungCap CHAR(10), 
    MaChiNhanh CHAR(5),
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaNhaCungCap) REFERENCES NhaCungCap(MaNhaCungCap)
);
-- 17. Bảng Chi phí khác (Expenses)
CREATE TABLE ChiPhiKhac (
    MaChiPhi CHAR(10) PRIMARY KEY, -- Mã chi phí
    TenChiPhi NVARCHAR(100), -- Tên chi phí
    SoTien DECIMAL(18, 2) CHECK (SoTien >= 0), -- Số tiền
    NgayChi DATE, -- Ngày phát sinh
    GhiChu NVARCHAR(255), -- Ghi chú
    MaChiNhanh CHAR(5), -- Khóa ngoại
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);
-- 18. Bảng Quản lý kho (Inventory)
CREATE TABLE QuanLyKho (
    MaSanPham CHAR(10), -- Khóa ngoại
    SoLuongTon INT,
	MaChiNhanh CHAR(5),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

CREATE TABLE GoiMon (
	MaGoiMon char(10),
	MaBan char(10),
	NgayTao date,
	TongTien decimal(18, 2),
	TrangThai bit,
	GhiChu nvarchar(255),
	primary key (MaGoiMon),
    FOREIGN KEY (MaBan) REFERENCES BanAn(MaBan)
);

CREATE TABLE ChiTietGoiMon (
	MaGoiMon char(10),
	MaSanPham char(10),
	SoLuong int,
	DonGia decimal(18, 2),
	ThanhTien decimal(18, 2),
	primary key (MaGoiMon, MaSanPham),
    FOREIGN KEY (MaGoiMon) REFERENCES GoiMon(MaGoiMon),
	FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

CREATE TABLE KhuyenMai (
    MaKhuyenMai CHAR(10) PRIMARY KEY, -- Mã khuyến mãi
    TenKhuyenMai NVARCHAR(100), -- Tên khuyến mãi
    PhanTramGiam DECIMAL(5, 2), -- Phần trăm giảm giá
    MaChiNhanh CHAR(5), -- Khóa ngoại
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

-- Thêm chỉ mục cho các trường thường xuyên được tìm kiếm
CREATE INDEX IDX_KhachHang ON KhachHang(MaKhachHang);
CREATE INDEX IDX_NhanVien ON NhanVien(MaNhanVien);
CREATE INDEX IDX_BanAn ON BanAn(MaBan);
CREATE INDEX IDX_HoaDon ON HoaDon(MaHoaDon);
CREATE INDEX IDX_DatBan ON DatBan(MaDatBan);
CREATE INDEX IDX_ChiPhiKhac ON ChiPhiKhac(MaChiPhi);
CREATE INDEX IDX_DoanhThu ON DoanhThu(MaDoanhThu);
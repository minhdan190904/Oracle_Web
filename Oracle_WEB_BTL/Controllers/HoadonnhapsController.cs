using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Oracle_WEB_BTL.Controllers
{
    [Authorize]
    public class HoadonnhapsController : Controller
    {
        private readonly OracleContext _context;

        public HoadonnhapsController(OracleContext context)
        {
            _context = context;
        }

        // GET: Hoadonnhaps
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Hoadonnhaps
                .Include(h => h.ManccNavigation)
                .Include(h => h.ManvNavigation)
                .Select(h => new
                {
                    h.Sohdn,
                    h.Ngaynhap,
                    NhaCungCap = h.ManccNavigation.Tenncc,
                    NhanVien = h.ManvNavigation.Tennv,
                    TongTien = _context.Chitiethoadonnhaps
                        .Where(ct => ct.Sohdn == h.Sohdn)
                        .Sum(ct => (ct.Soluong ?? 0) *
                                   (_context.Dmhanghoas
                                       .Where(mh => mh.Mahang == ct.Mahang)
                                       .Select(mh => mh.Dongianhap ?? 0).FirstOrDefault()) *
                                   (1 - (ct.Giamgia ?? 0) / 100))
                })
                .ToListAsync();

            var result = invoices.Select(i => new Hoadonnhap
            {
                Sohdn = i.Sohdn,
                Ngaynhap = i.Ngaynhap,
                ManccNavigation = new Nhacungcap { Tenncc = i.NhaCungCap },
                ManvNavigation = new Nhanvien { Tennv = i.NhanVien },
                TongTien = i.TongTien
            }).ToList();

            return View(result);
        }

        // GET: Hoadonnhap/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Hoadonnhaps
                .Include(i => i.ManccNavigation)
                .Include(i => i.ManvNavigation)
                .Include(i => i.Chitiethoadonnhaps)
                    .ThenInclude(d => d.MahangNavigation)
                .FirstOrDefaultAsync(i => i.Sohdn == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Tính tổng tiền từ Soluong, Dongianhap và Giamgia
            invoice.TongTien = invoice.Chitiethoadonnhaps.Sum(d =>
                d.Soluong.HasValue && d.MahangNavigation.Dongianhap.HasValue
                ? d.Soluong.Value * d.MahangNavigation.Dongianhap.Value * (1 - (d.Giamgia ?? 0) / 100)
                : 0);

            return View("Details", invoice);
        }

        // POST: Hoadonnhap/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var invoice = await _context.Hoadonnhaps
                .Include(i => i.Chitiethoadonnhaps)
                .FirstOrDefaultAsync(i => i.Sohdn == id);

            if (invoice == null)
            {
                return NotFound();
            }

            _context.Chitiethoadonnhaps.RemoveRange(invoice.Chitiethoadonnhaps);

            _context.Hoadonnhaps.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hoadonnhap hoadonnhap, List<Chitiethoadonnhap> Chitiethoadonnhaps)
        {
            // Xóa lỗi trước đó
            ModelState.Clear();

            // Kiểm tra dữ liệu hóa đơn nhập
            if (hoadonnhap.Mancc == null || hoadonnhap.Mancc <= 0)
            {
                ModelState.AddModelError(nameof(hoadonnhap.Mancc), "Nhà cung cấp không được để trống.");
            }

            // Kiểm tra danh sách chi tiết hóa đơn nhập
            if (Chitiethoadonnhaps == null || !Chitiethoadonnhaps.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm.");
            }
            else
            {
                // Loại bỏ các mục trùng lặp theo Mahang
                Chitiethoadonnhaps = Chitiethoadonnhaps
                    .GroupBy(d => new { d.Sohdn, d.Mahang })
                    .Select(g => g.First())
                    .ToList();

                foreach (var detail in Chitiethoadonnhaps)
                {
                    if (detail.Mahang <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Mã sản phẩm không hợp lệ.");
                        break;
                    }

                    if (detail.Soluong == null || detail.Soluong <= 0)
                    {
                        ModelState.AddModelError(string.Empty, $"Số lượng cho sản phẩm mã {detail.Mahang} không hợp lệ.");
                        break;
                    }

                    if (detail.Giamgia < 0 || detail.Giamgia > 100)
                    {
                        ModelState.AddModelError(string.Empty, $"Giảm giá cho sản phẩm mã {detail.Mahang} phải từ 0 đến 100.");
                        break;
                    }
                }
            }

            // Nếu có lỗi, trả về view
            if (!ModelState.IsValid)
            {
                PopulateViewBag();
                return View(hoadonnhap);
            }

            // Sử dụng giao dịch để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy MaNV từ claims người dùng
                var manvClaim = User.Claims.FirstOrDefault(c => c.Type == "Manv");
                if (manvClaim != null && decimal.TryParse(manvClaim.Value, out decimal manv))
                {
                    hoadonnhap.Manv = manv;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin nhân viên.");
                    PopulateViewBag();
                    return View(hoadonnhap);
                }

                hoadonnhap.Ngaynhap = DateTime.Now;

                // Thêm hóa đơn nhập vào DbContext
                _context.Hoadonnhaps.Add(hoadonnhap);
                await _context.SaveChangesAsync(); // Lưu để tạo Sohdn

                // Thêm chi tiết hóa đơn nhập
                foreach (var detail in Chitiethoadonnhaps)
                {
                    // Kiểm tra sản phẩm có tồn tại không
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Sản phẩm với mã {detail.Mahang} không tồn tại.");
                        await transaction.RollbackAsync();
                        PopulateViewBag();
                        return View(hoadonnhap);
                    }

                    // Kiểm tra nếu chi tiết hóa đơn nhập đã được theo dõi
                    var existingDetail = _context.Chitiethoadonnhaps.Local
                        .FirstOrDefault(d => d.Sohdn == hoadonnhap.Sohdn && d.Mahang == detail.Mahang);

                    if (existingDetail == null)
                    
                    {
                        // Gán Sohdn cho chi tiết hóa đơn nhập và thêm mới
                        detail.Sohdn = hoadonnhap.Sohdn;
                        _context.Chitiethoadonnhaps.Add(detail);
                    }
                }

                // Lưu tất cả thay đổi
                await _context.SaveChangesAsync();

                // Hoàn tất giao dịch
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Hủy bỏ giao dịch nếu có lỗi
                await transaction.RollbackAsync();

                // Ghi lỗi và hiển thị thông báo
                Console.WriteLine($"Lỗi: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                PopulateViewBag();
                return View(hoadonnhap);
            }
        }

        // GET: Hoadonnhaps/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadonnhap = await _context.Hoadonnhaps
                .Include(h => h.Chitiethoadonnhaps)
                    .ThenInclude(d => d.MahangNavigation)
                .Include(h => h.ManccNavigation)
                .FirstOrDefaultAsync(h => h.Sohdn == id);

            if (hoadonnhap == null)
            {
                return NotFound();
            }

            PopulateViewBag();

            // Truyền danh sách chi tiết hóa đơn nhập sang ViewBag để sử dụng trong View
            ViewBag.ExistingDetails = hoadonnhap.Chitiethoadonnhaps.Select(d => new
            {
                Mahang = d.Mahang,
                Tenhanghoa = d.MahangNavigation.Tenhanghoa,
                Soluong = d.Soluong,
                Dongianhap = d.MahangNavigation.Dongianhap,
                Giamgia = d.Giamgia
            }).ToList();

            return View(hoadonnhap);
        }


        // POST: Hoadonnhaps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, Hoadonnhap hoadonnhap, List<Chitiethoadonnhap> Chitiethoadonnhaps)
        {
            if (id != hoadonnhap.Sohdn)
            {
                return NotFound();
            }

            // Xóa lỗi trước đó
            ModelState.Clear();

            // Kiểm tra dữ liệu hóa đơn nhập
            if (hoadonnhap.Mancc == null || hoadonnhap.Mancc <= 0)
            {
                ModelState.AddModelError(nameof(hoadonnhap.Mancc), "Nhà cung cấp không được để trống.");
            }

            // Kiểm tra danh sách chi tiết hóa đơn nhập
            if (Chitiethoadonnhaps == null || !Chitiethoadonnhaps.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm.");
            }
            else
            {
                // Loại bỏ các mục trùng lặp theo Mahang
                Chitiethoadonnhaps = Chitiethoadonnhaps
                    .GroupBy(d => d.Mahang)
                    .Select(g => g.First())
                    .ToList();

                foreach (var detail in Chitiethoadonnhaps)
                {
                    if (detail.Mahang <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Mã sản phẩm không hợp lệ.");
                        break;
                    }

                    if (detail.Soluong == null || detail.Soluong <= 0)
                    {
                        ModelState.AddModelError(string.Empty, $"Số lượng cho sản phẩm mã {detail.Mahang} không hợp lệ.");
                        break;
                    }

                    if (detail.Giamgia < 0 || detail.Giamgia > 100)
                    {
                        ModelState.AddModelError(string.Empty, $"Giảm giá cho sản phẩm mã {detail.Mahang} phải từ 0 đến 100.");
                        break;
                    }
                }
            }

            // Nếu có lỗi, trả về view
            if (!ModelState.IsValid)
            {
                PopulateViewBag();
                return View(hoadonnhap);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy thông tin nhân viên từ claims
                var manvClaim = User.Claims.FirstOrDefault(c => c.Type == "Manv");
                if (manvClaim != null && decimal.TryParse(manvClaim.Value, out decimal manv))
                {
                    hoadonnhap.Manv = manv;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin nhân viên.");
                    PopulateViewBag();
                    return View(hoadonnhap);
                }

                // Cập nhật thông tin hóa đơn nhập
                _context.Entry(hoadonnhap).State = EntityState.Modified;

                // Lấy danh sách chi tiết hóa đơn nhập cũ
                var existingDetails = await _context.Chitiethoadonnhaps
                    .Where(d => d.Sohdn == hoadonnhap.Sohdn)
                    .ToListAsync();

                // Điều chỉnh số lượng sản phẩm trong kho dựa trên chi tiết cũ
                foreach (var detail in existingDetails)
                {
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product != null)
                    {
                        // Giảm số lượng tồn kho theo chi tiết cũ
                        product.Soluongton -= detail.Soluong ?? 0;
                        if (product.Soluongton < 0) product.Soluongton = 0;

                        _context.Update(product);
                    }
                }

                // Xóa các chi tiết hóa đơn nhập cũ
                _context.Chitiethoadonnhaps.RemoveRange(existingDetails);
                await _context.SaveChangesAsync();

                // Thêm các chi tiết hóa đơn nhập mới và cập nhật số lượng tồn kho
                foreach (var detail in Chitiethoadonnhaps)
                {
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Sản phẩm với mã {detail.Mahang} không tồn tại.");
                        await transaction.RollbackAsync();
                        PopulateViewBag();
                        return View(hoadonnhap);
                    }

                    // Tăng số lượng tồn kho theo chi tiết mới
                    product.Soluongton += detail.Soluong ?? 0;
                    _context.Update(product);

                    detail.Sohdn = hoadonnhap.Sohdn;
                    _context.Chitiethoadonnhaps.Add(detail);
                }

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                PopulateViewBag();
                return View(hoadonnhap);
            }
        }





        // GET: Hoadonnhaps/Create
        public IActionResult Create()
        {
            PopulateViewBag();
            return View();
        }

        // GET: Hoadonnhaps/GetSupplierInfo
        [HttpGet]
        public async Task<JsonResult> GetSupplierInfo(decimal supplierId)
        {
            var supplier = await _context.Nhacungcaps
                .Where(s => s.Mancc == supplierId)
                .Select(s => new
                {
                    mancc = s.Mancc,
                    tenncc = s.Tenncc,
                    diachi = s.Diachi,
                    dienthoai = s.Dienthoai
                })
                .FirstOrDefaultAsync();

            return Json(supplier);
        }

        // GET: Hoadonnhaps/GetProductInfo
        [HttpGet]
        public async Task<JsonResult> GetProductInfo(decimal productId)
        {
            var product = await _context.Dmhanghoas
                .Where(p => p.Mahang == productId)
                .Select(p => new
                {
                    mahang = p.Mahang,
                    tenhanghoa = p.Tenhanghoa,
                    dongianhap = p.Dongianhap
                })
                .FirstOrDefaultAsync();

            return Json(product);
        }

        // Helper method để đổ dữ liệu vào ViewBag
        private void PopulateViewBag()
        {
            ViewBag.Suppliers = new SelectList(_context.Nhacungcaps?.ToList() ?? new List<Nhacungcap>(), "Mancc", "Tenncc");
            ViewBag.Products = _context.Dmhanghoas?.Select(p => new SelectListItem
            {
                Value = p.Mahang.ToString(),
                Text = p.Tenhanghoa,
            }).ToList() ?? new List<SelectListItem>();
        }

        // Các action khác: Edit, Details, Delete...

        private bool HoadonnhapExists(decimal id)
        {
            return _context.Hoadonnhaps.Any(e => e.Sohdn == id);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Oracle_WEB_BTL.Controllers
{
    [Authorize]
    public class HoadonbansController : Controller
    {
        private readonly OracleContext _context;

        public HoadonbansController(OracleContext context)
        {
            _context = context;
        }

        // GET: Hoadonbans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadonban = await _context.Hoadonbans
                .Include(h => h.Chitiethoadonbans)
                    .ThenInclude(d => d.MahangNavigation)
                .Include(h => h.MakhachNavigation)
                .FirstOrDefaultAsync(h => h.Sohdb == id);

            if (hoadonban == null)
            {
                return NotFound();
            }

            PopulateViewBag();

            // Truyền danh sách chi tiết hóa đơn bán sang ViewBag để sử dụng trong View
            ViewBag.ExistingDetails = hoadonban.Chitiethoadonbans.Select(d => new
            {
                Mahang = d.Mahang,
                Tenhanghoa = d.MahangNavigation.Tenhanghoa,
                Soluong = d.Soluong,
                Dongiaban = d.MahangNavigation.Dongiaban,
                Giamgia = d.Giamgia
            }).ToList();

            return View(hoadonban);
        }

        // POST: Hoadonbans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, Hoadonban hoadonban, List<Chitiethoadonban> Chitiethoadonbans)
        {
            if (id != hoadonban.Sohdb)
            {
                return NotFound();
            }

            // Xóa lỗi trước đó
            ModelState.Clear();

            // Kiểm tra dữ liệu hóa đơn bán
            if (hoadonban.Makhach == null || hoadonban.Makhach <= 0)
            {
                ModelState.AddModelError(nameof(hoadonban.Makhach), "Khách hàng không được để trống.");
            }

            // Kiểm tra danh sách chi tiết hóa đơn bán
            if (Chitiethoadonbans == null || !Chitiethoadonbans.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm.");
            }
            else
            {
                // Loại bỏ các mục trùng lặp theo Mahang
                Chitiethoadonbans = Chitiethoadonbans
                    .GroupBy(d => d.Mahang)
                    .Select(g => g.First())
                    .ToList();

                foreach (var detail in Chitiethoadonbans)
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
                return View(hoadonban);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy thông tin nhân viên từ claims
                var manvClaim = User.Claims.FirstOrDefault(c => c.Type == "Manv");
                if (manvClaim != null && decimal.TryParse(manvClaim.Value, out decimal manv))
                {
                    hoadonban.Manv = manv;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin nhân viên.");
                    PopulateViewBag();
                    return View(hoadonban);
                }

                // Cập nhật thông tin hóa đơn bán
                _context.Entry(hoadonban).State = EntityState.Modified;

                // Lấy danh sách chi tiết hóa đơn bán cũ
                var existingDetails = await _context.Chitiethoadonbans
                    .Where(d => d.Sohdb == hoadonban.Sohdb)
                    .ToListAsync();

                // Điều chỉnh số lượng sản phẩm trong kho dựa trên chi tiết cũ
                foreach (var detail in existingDetails)
                {
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product != null)
                    {
                        // Tăng số lượng tồn kho theo chi tiết cũ (vì trước đó đã bán, giờ hoàn lại)
                        product.Soluongton += detail.Soluong ?? 0;
                        _context.Update(product);
                    }
                }

                // Xóa các chi tiết hóa đơn bán cũ
                _context.Chitiethoadonbans.RemoveRange(existingDetails);
                await _context.SaveChangesAsync();

                // Thêm các chi tiết hóa đơn bán mới và cập nhật số lượng tồn kho
                foreach (var detail in Chitiethoadonbans)
                {
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Sản phẩm với mã {detail.Mahang} không tồn tại.");
                        await transaction.RollbackAsync();
                        PopulateViewBag();
                        return View(hoadonban);
                    }

                    if (product.Soluongton < detail.Soluong)
                    {
                        ModelState.AddModelError(string.Empty, $"Số lượng tồn kho của sản phẩm mã {detail.Mahang} không đủ.");
                        await transaction.RollbackAsync();
                        PopulateViewBag();
                        return View(hoadonban);
                    }

                    // Giảm số lượng tồn kho theo chi tiết mới
                    product.Soluongton -= detail.Soluong ?? 0;
                    _context.Update(product);

                    detail.Sohdb = hoadonban.Sohdb;
                    _context.Chitiethoadonbans.Add(detail);
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
                return View(hoadonban);
            }
        }



        // GET: Hoadonban/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Hoadonbans
                .Include(i => i.MakhachNavigation)
                .Include(i => i.ManvNavigation)
                .Include(i => i.Chitiethoadonbans)
                    .ThenInclude(d => d.MahangNavigation)
                .FirstOrDefaultAsync(i => i.Sohdb == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Tính tổng tiền từ Soluong, Dongiaban và Giamgia
            invoice.TongTien = invoice.Chitiethoadonbans.Sum(d =>
                d.Soluong.HasValue && d.MahangNavigation.Dongiaban.HasValue
                ? d.Soluong.Value * d.MahangNavigation.Dongiaban.Value * (1 - (d.Giamgia ?? 0) / 100)
                : 0);

            return View("Details", invoice);
        }

        // POST: Hoadonban/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var invoice = await _context.Hoadonbans
                .Include(i => i.Chitiethoadonbans)
                .FirstOrDefaultAsync(i => i.Sohdb == id);

            if (invoice == null)
            {
                return NotFound();
            }

            _context.Chitiethoadonbans.RemoveRange(invoice.Chitiethoadonbans);
            _context.Hoadonbans.Remove(invoice);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Hoadonbans
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Hoadonbans
                .Include(h => h.MakhachNavigation)
                .Include(h => h.ManvNavigation)
                .Select(h => new
                {
                    h.Sohdb,
                    h.Ngayban,
                    KhachHang = h.MakhachNavigation.Tenkhach,
                    NhanVien = h.ManvNavigation.Tennv,
                    TongTien = _context.Chitiethoadonbans
                        .Where(ct => ct.Sohdb == h.Sohdb)
                        .Sum(ct => (ct.Soluong ?? 0) *
                                   (_context.Dmhanghoas
                                       .Where(mh => mh.Mahang == ct.Mahang)
                                       .Select(mh => mh.Dongiaban ?? 0).FirstOrDefault()) *
                                   (1 - (ct.Giamgia ?? 0) / 100))
                })
                .ToListAsync();

            var result = invoices.Select(i => new Hoadonban
            {
                Sohdb = i.Sohdb,
                Ngayban = i.Ngayban,
                MakhachNavigation = new Khachhang { Tenkhach = i.KhachHang },
                ManvNavigation = new Nhanvien { Tennv = i.NhanVien },
                TongTien = i.TongTien
            }).ToList();

            return View(result);
        }

        // GET: Hoadonbans/Create
        public IActionResult Create()
        {
            PopulateViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hoadonban hoadonban, List<Chitiethoadonban> Chitiethoadonbans)
        {
            // Kiểm tra và thêm lỗi vào ModelState (giữ nguyên như trước)

            if (!ModelState.IsValid)
            {
                PopulateViewBag();
                return View(hoadonban);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy Manv từ claims
                var manvClaim = User.Claims.FirstOrDefault(c => c.Type == "Manv");
                if (manvClaim != null && decimal.TryParse(manvClaim.Value, out decimal manv))
                {
                    hoadonban.Manv = manv;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin nhân viên.");
                    PopulateViewBag();
                    return View(hoadonban);
                }

                hoadonban.Ngayban = DateTime.Now;

                // Thêm hóa đơn bán vào DbContext
                _context.Hoadonbans.Add(hoadonban);
                await _context.SaveChangesAsync(); // Lưu để tạo Sohdb

                // Thêm chi tiết hóa đơn bán
                foreach (var detail in Chitiethoadonbans)
                {
                    // Kiểm tra sản phẩm có tồn tại không
                    var product = await _context.Dmhanghoas.FindAsync(detail.Mahang);
                    if (product == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Sản phẩm với mã {detail.Mahang} không tồn tại.");
                        await transaction.RollbackAsync();
                        PopulateViewBag();
                        return View(hoadonban);
                    }

                    // Kiểm tra nếu chi tiết hóa đơn bán đã được theo dõi
                    var existingDetail = _context.Chitiethoadonbans.Local
                        .FirstOrDefault(d => d.Sohdb == hoadonban.Sohdb && d.Mahang == detail.Mahang);

                    if (existingDetail == null)
                    {
                        // Gán Sohdb cho chi tiết hóa đơn bán và thêm mới
                        detail.Sohdb = hoadonban.Sohdb;
                        _context.Chitiethoadonbans.Add(detail);
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
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                PopulateViewBag();
                return View(hoadonban);
            }
        }


        // AJAX action để lấy thông tin sản phẩm
        [HttpGet]
        public async Task<JsonResult> GetProductInfo(decimal productId)
        {
            // Lấy thông tin sản phẩm
            var product = await _context.Dmhanghoas
                .Where(p => p.Mahang == productId)
                .Select(p => new
                {
                    productId = p.Mahang,
                    productName = p.Tenhanghoa,
                    sellingPrice = p.Dongiaban
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return Json(null);
            }

            // Tính số lượng tồn kho
            var totalPurchased = await _context.Chitiethoadonnhaps
                .Where(ct => ct.Mahang == productId)
                .SumAsync(ct => ct.Soluong ?? 0);

            var totalSold = await _context.Chitiethoadonbans
                .Where(ct => ct.Mahang == productId)
                .SumAsync(ct => ct.Soluong ?? 0);

            var availableQuantity = totalPurchased - totalSold;

            // Trả về thông tin sản phẩm cùng với số lượng tồn kho
            return Json(new
            {
                product.productId,
                product.productName,
                sellingPrice = product.sellingPrice,
                availableQuantity
            });
        }

        // AJAX action để lấy thông tin khách hàng
        [HttpGet]
        public async Task<JsonResult> GetCustomerInfo(decimal customerId)
        {
            var customer = await _context.Khachhangs
                .Where(c => c.Makhach == customerId)
                .Select(c => new
                {
                    customerId = c.Makhach,
                    customerName = c.Tenkhach,
                    address = c.Diachi,
                    phoneNumber = c.Dienthoai
                })
                .FirstOrDefaultAsync();

            return Json(customer);
        }

        // Helper method để đổ dữ liệu vào ViewBag
        private void PopulateViewBag()
        {
            ViewBag.Customers = new SelectList(_context.Khachhangs?.ToList() ?? new List<Khachhang>(), "Makhach", "Tenkhach");
            ViewBag.Products = _context.Dmhanghoas?.Select(p => new SelectListItem
            {
                Value = p.Mahang.ToString(),
                Text = p.Tenhanghoa,
            }).ToList() ?? new List<SelectListItem>();
        }

        // Các action khác: Index, Details, Delete...
    }
}

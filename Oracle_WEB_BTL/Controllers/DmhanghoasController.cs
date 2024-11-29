using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oracle_WEB_BTL.Controllers
{
    [Authorize]
    public class DmhanghoasController : Controller
    {
        private readonly OracleContext _context;
        private const int itemsPerPage = 1;

        public DmhanghoasController(OracleContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hanghoas = _context.Dmhanghoas.Include(h => h.MansxNavigation).AsQueryable();

            // Lấy dữ liệu phân trang
            var paginatedHanghoas = await hanghoas.Take(itemsPerPage).ToListAsync();

            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = (int)Math.Ceiling(hanghoas.Count() / (double)itemsPerPage);
            ViewBag.SearchTerm = string.Empty;

            return View("Index", paginatedHanghoas);
        }

        // GET: Dmhanghoas/GetProducts
        public async Task<IActionResult> GetProducts(int page = 1, string searchTerm = "")
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = string.Empty;
            }

            // Tách từ khóa tìm kiếm
            var keywords = Regex.Split(searchTerm, @"\s+").Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.ToLower()).ToList();

            // Truy vấn với EF Core
            var query = _context.Dmhanghoas
                .Include(h => h.MaloaiNavigation)
                .Include(h => h.MachatlieuNavigation)
                .Include(h => h.MansxNavigation)
                .Include(h => h.MamauNavigation)
                .AsQueryable();

            // Áp dụng điều kiện tìm kiếm
            foreach (var keyword in keywords)
            {
                query = query.Where(h =>
                    h.Tenhanghoa.ToLower().Contains(keyword) ||
                    h.Mahang.ToString().Contains(keyword) ||
                    (h.MaloaiNavigation != null && h.MaloaiNavigation.Tenloai.ToLower().Contains(keyword)) ||
                    (h.MansxNavigation != null && h.MansxNavigation.Tennsx.ToLower().Contains(keyword)) ||
                    (h.MamauNavigation != null && h.MamauNavigation.Tenmau.ToLower().Contains(keyword)) ||
                    h.Dongiaban.ToString().Contains(keyword));
            }

            // Lấy dữ liệu phân trang
            var paginatedHanghoas = await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)itemsPerPage);
            ViewBag.SearchTerm = searchTerm;

            return PartialView("_ProductListPartial", paginatedHanghoas);
        }

        public async Task<IActionResult> Details(decimal id)
        {
            var dmhanghoa = await _context.Dmhanghoas
                .Include(h => h.MaloaiNavigation)
                .Include(h => h.MakichthuocNavigation)
                .Include(h => h.MachatlieuNavigation)
                .Include(h => h.MansxNavigation)
                .Include(h => h.MamauNavigation)
                .Include(h => h.ManuocsxNavigation)
                .FirstOrDefaultAsync(h => h.Mahang == id);

            if (dmhanghoa == null)
            {
                return NotFound();
            }

            // Lấy dữ liệu nhập
            var tongSoLuongNhap = _context.Chitiethoadonnhaps
                .Where(n => n.Mahang == id)
                .AsEnumerable() // Chuyển sang phía client
                .Sum(n => (decimal?)n.Soluong) ?? 0;

            // Lấy dữ liệu bán
            var tongSoLuongBan = _context.Chitiethoadonbans
                .Where(b => b.Mahang == id)
                .AsEnumerable() // Chuyển sang phía client
                .Sum(b => (decimal?)b.Soluong) ?? 0;

            // Tính số lượng tồn kho
            var soLuongTonKho = tongSoLuongNhap - tongSoLuongBan;

            // Gán giá trị vào ViewBag
            ViewBag.TongSoLuongNhap = tongSoLuongNhap;
            ViewBag.TongSoLuongBan = tongSoLuongBan;
            ViewBag.SoLuongTonKho = soLuongTonKho;

            return View(dmhanghoa);
        }



    }
}

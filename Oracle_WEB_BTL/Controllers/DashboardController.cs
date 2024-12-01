using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;
using Oracle_WEB_BTL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Oracle_WEB_BTL.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly OracleContext _context;
        private readonly ICompositeViewEngine _viewEngine;

        public DashboardController(OracleContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy 8 nhân viên mới nhất
            var latestEmployees = await _context.Nhanviens
                .OrderByDescending(e => e.Manv)
                .Take(8)
                .ToListAsync();

            // Số lượng hóa đơn bán hôm nay
            ViewBag.TotalSalesInvoices = await _context.Hoadonbans
                .CountAsync(i => i.Ngayban.Value.Date == DateTime.Today);

            // Tổng doanh thu trong tháng hiện tại
            ViewBag.TotalMonthIncome = await _context.Hoadonbans
                .Where(i => i.Ngayban.Value.Month == DateTime.Today.Month && i.Ngayban.Value.Year == DateTime.Today.Year)
                .SelectMany(i => i.Chitiethoadonbans)
                .SumAsync(ct => (decimal?)(ct.Soluong ?? 0) * (ct.MahangNavigation.Dongiaban ?? 0) * (1 - (ct.Giamgia ?? 0) / 100)) ?? 0;

            // Dữ liệu cho biểu đồ Donut và Pie
            var totalProductsSold = await _context.Chitiethoadonbans
                .Include(ct => ct.MahangNavigation)
                    .ThenInclude(p => p.MaloaiNavigation)
                .GroupBy(ct => new { ct.MahangNavigation.MaloaiNavigation.Maloai, ct.MahangNavigation.MaloaiNavigation.Tenloai })
                .Select(g => new
                {
                    Maloai = g.Key.Maloai,
                    CategoryName = g.Key.Tenloai ?? "Không xác định",
                    TotalSold = g.Sum(ct => ct.Soluong ?? 0),
                    TotalRevenue = g.Sum(ct => (ct.Soluong ?? 0) * (ct.MahangNavigation.Dongiaban ?? 0) * (1 - (ct.Giamgia ?? 0) / 100))
                })
                .ToListAsync();

            // Truyền dữ liệu biểu đồ sang ViewBag
            ViewBag.DonutData = totalProductsSold.Select(x => new { x.Maloai, x.CategoryName, x.TotalSold }).ToList();
            ViewBag.PieData = totalProductsSold.Select(x => new { x.Maloai, x.CategoryName, x.TotalRevenue }).ToList();

            // Tổng số khách hàng và sản phẩm
            ViewBag.TotalCustomers = await _context.Khachhangs.CountAsync();
            ViewBag.TotalProducts = await _context.Dmhanghoas.CountAsync();

            return View(latestEmployees);
        }

        public async Task<IActionResult> Statistics(DateTime? startDate, DateTime? endDate)
        {
            // Đặt phạm vi thời gian mặc định nếu không được cung cấp
            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            // Lấy hóa đơn bán trong khoảng thời gian
            var salesInvoices = await _context.Hoadonbans
                .Where(i => i.Ngayban >= startDate && i.Ngayban <= endDate)
                .Include(i => i.MakhachNavigation)
                .Include(i => i.ManvNavigation)
                .Include(i => i.Chitiethoadonbans)
                    .ThenInclude(ct => ct.MahangNavigation)
                .ToListAsync();

            // Tính tổng doanh thu
            var totalRevenue = salesInvoices
                .SelectMany(i => i.Chitiethoadonbans)
                .Sum(ct => (decimal?)(ct.Soluong ?? 0) * (ct.MahangNavigation.Dongiaban ?? 0) * (1 - (ct.Giamgia ?? 0) / 100)) ?? 0;

            // Truyền dữ liệu sang View
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
            ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");

            return View(salesInvoices);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesInvoiceData(DateTime startDate, DateTime endDate)
        {
            // Lọc hóa đơn bán theo khoảng thời gian
            var salesInvoices = await _context.Hoadonbans
                .Where(i => i.Ngayban >= startDate && i.Ngayban <= endDate)
                .Include(i => i.MakhachNavigation)
                .Include(i => i.ManvNavigation)
                .Include(i => i.Chitiethoadonbans)
                    .ThenInclude(ct => ct.MahangNavigation)
                .ToListAsync();

            // Tính tổng doanh thu
            var totalRevenue = salesInvoices
                .SelectMany(i => i.Chitiethoadonbans)
                .Sum(ct => (decimal?)(ct.Soluong ?? 0) * (ct.MahangNavigation.Dongiaban ?? 0) * (1 - (ct.Giamgia ?? 0) / 100)) ?? 0;

            // Render partial view thành chuỗi
            var htmlTable = RenderRazorViewToString("_SalesInvoiceTable", salesInvoices);

            // Trả về JSON response
            return Json(new { htmlTable, totalRevenue });
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}

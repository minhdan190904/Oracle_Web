using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Models;
using Oracle_WEB_BTL.Helpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Oracle_WEB_BTL.Controllers
{
    public class NhanviensController : Controller
    {
        private readonly OracleContext _context;

        public NhanviensController(OracleContext context)
        {
            _context = context;
        }

        // GET: Nhanviens
        public async Task<IActionResult> Index()
        {
            var nhanviens = _context.Nhanviens.Include(n => n.MacvNavigation);
            return View(await nhanviens.ToListAsync());
        }

        // GET: Nhanviens/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MacvNavigation)
                .FirstOrDefaultAsync(n => n.Manv == id);

            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }

        // GET: Nhanviens/Create
        public IActionResult Create()
        {
            var nhanvien = new Nhanvien
            {
                Anh = AppDefaults.DefaultImageFile
            };

            SetViewBagData();
            return View(nhanvien);
        }

        // POST: Nhanviens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Nhanvien nhanvien, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image file
                if (imageFile != null && imageFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), AppDefaults.DefaultImageFolder, imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    nhanvien.Anh = imageFile.FileName;
                }

                // Save to database
                _context.Add(nhanvien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SetViewBagData();
            return View(nhanvien);
        }


        // GET: Nhanviens/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien == null)
            {
                return NotFound();
            }

            SetViewBagData();
            return View(nhanvien);
        }

        // POST: Nhanviens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, Nhanvien nhanvien, IFormFile? imageFile, string oldImage)
        {
            if (id != nhanvien.Manv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), AppDefaults.DefaultImageFolder, imageFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        nhanvien.Anh = imageFile.FileName;
                    }
                    else
                    {
                        nhanvien.Anh = oldImage;
                    }

                    _context.Update(nhanvien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanvienExists(nhanvien.Manv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            SetViewBagData();
            return View(nhanvien);
        }

        // GET: Nhanviens/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MacvNavigation)
                .FirstOrDefaultAsync(n => n.Manv == id);

            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }

        // POST: Nhanviens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien != null)
            {
                _context.Nhanviens.Remove(nhanvien);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper to check if a Nhanvien exists
        private bool NhanvienExists(decimal id)
        {
            return _context.Nhanviens.Any(n => n.Manv == id);
        }

        private void SetViewBagData()
        {
            ViewBag.JobList = new SelectList(_context.Congviecs.ToList(), "Macv", "Tencv");
            ViewBag.GenderList = new SelectList(new[]
            {
        new { Value = true, Text = "Nam" },
        new { Value = false, Text = "Nữ" }
    }, "Value", "Text");

            // Adding Quyenadmin options to ViewBag
            ViewBag.QuyenadminList = new SelectList(new[]
            {
        new { Value = true, Text = "Có" },
        new { Value = false, Text = "Không" }
    }, "Value", "Text");
        }

    }
}

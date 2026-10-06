namespace ResumeForge.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDb db;
    public AdminController(AppDb db) { this.db = db; }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsers = await db.Users.CountAsync();
        ViewBag.TotalResumes = await db.Resumes.CountAsync();
        ViewBag.Templates = await db.Resumes.GroupBy(r => r.Template)
            .Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
        var rows = await db.Users.OrderBy(u => u.Id).Select(u => new UserRow
        {
            Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role,
            Resumes = db.Resumes.Count(r => r.UserId == u.Id)
        }).ToListAsync();
        return View(rows);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            TempData["Msg"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }
        var u = await db.Users.FindAsync(id);
        if (u != null) { db.Users.Remove(u); await db.SaveChangesAsync(); TempData["Msg"] = "User and their resumes deleted."; }
        return RedirectToAction(nameof(Index));
    }
}

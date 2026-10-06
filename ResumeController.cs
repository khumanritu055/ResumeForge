namespace ResumeForge.Controllers;

[Authorize]
public class ResumeController : Controller
{
    private readonly AppDb db;
    public ResumeController(AppDb db) { this.db = db; }
    private int Uid => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    private const string Fields = "Id,Title,FullName,Email,Phone,Address,Summary,Skills,Education,Experience,Projects,Template";

    public async Task<IActionResult> Index()
    {
        var list = await db.Resumes.Where(r => r.UserId == Uid).OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(list);
    }

    [HttpGet] public IActionResult Create() => View(new Resume { Template = "Classic" });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Fields)] Resume model)
    {
        if (!ModelState.IsValid) return View(model);
        model.Id = 0;
        model.UserId = Uid;
        model.CreatedAt = DateTime.Now;
        db.Resumes.Add(model);
        await db.SaveChangesAsync();
        TempData["Msg"] = "Resume saved.";
        return RedirectToAction(nameof(Preview), new { id = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var r = await db.Resumes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid);
        return r == null ? NotFound() : View(r);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind(Fields)] Resume model)
    {
        if (!ModelState.IsValid) return View(model);
        var r = await db.Resumes.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == Uid);
        if (r == null) return NotFound();
        r.Title = model.Title; r.FullName = model.FullName; r.Email = model.Email; r.Phone = model.Phone;
        r.Address = model.Address; r.Summary = model.Summary; r.Skills = model.Skills; r.Education = model.Education;
        r.Experience = model.Experience; r.Projects = model.Projects; r.Template = model.Template;
        await db.SaveChangesAsync();
        TempData["Msg"] = "Resume updated.";
        return RedirectToAction(nameof(Preview), new { id = r.Id });
    }

    public async Task<IActionResult> Preview(int id)
    {
        var r = await db.Resumes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid);
        return r == null ? NotFound() : View(r);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await db.Resumes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid);
        if (r != null) { db.Resumes.Remove(r); await db.SaveChangesAsync(); TempData["Msg"] = "Resume deleted."; }
        return RedirectToAction(nameof(Index));
    }
}

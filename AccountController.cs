namespace ResumeForge.Controllers;

public class AccountController : Controller
{
    private readonly AppDb db;
    private readonly PasswordHasher<AppUser> hasher = new();
    public AccountController(AppDb db) { this.db = db; }

    [HttpGet] public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var email = vm.Email.Trim().ToLower();
        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View(vm);
        }
        // The very first person to register becomes the Admin
        var user = new AppUser { Name = vm.Name.Trim(), Email = email, Role = await db.Users.AnyAsync() ? "User" : "Admin" };
        user.PasswordHash = hasher.HashPassword(user, vm.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        await SignInUser(user);
        return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "Resume");
    }

    [HttpGet] public IActionResult Login() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var email = vm.Email.Trim().ToLower();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Wrong email or password.");
            return View(vm);
        }
        await SignInUser(user);
        return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "Resume");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(AppUser u)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new Claim(ClaimTypes.Name, u.Name),
            new Claim(ClaimTypes.Role, u.Role)
        };
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
    }
}

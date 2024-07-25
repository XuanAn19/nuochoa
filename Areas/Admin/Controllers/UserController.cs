using CK_ASP_NET_CORE.Models;
using CK_ASP_NET_CORE.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace CK_ASP_NET_CORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    //[Authorize(Roles = "Admin")]
   // [Authorize]
    public class UserController : Controller
    {


        private readonly UserManager<AppUseModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(UserManager<AppUseModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
           /* return View(await _userManager.Users.OrderByDescending(p => p.Id).ToListAsync());*/
            var userRole =await (from u in _dataContext.Users join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                                                join r in _dataContext.Roles on ur.RoleId equals  r.Id
                                                                select new {User =u, RoleName =r.Name}).ToListAsync();
            return View(userRole);
             
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUseModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUseModel user)
        {
            if (!ModelState.IsValid)
            {
                var createUsser = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUsser.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);
                    var userId = createUser.Id;
                    var role = _roleManager.FindByIdAsync(user.RoleId);
                    //gán role
                    var addRole = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if (!addRole.Succeeded)
                    {
                        foreach (var error in createUsser.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUsser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = "Model bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if(!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id, AppUseModel user)
        {
            var exitingUser = await _userManager.FindByIdAsync(id);
            if(exitingUser== null)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                exitingUser.UserName = user.UserName;
                exitingUser.Email = user.Email;
                exitingUser.PhoneNumber = user.PhoneNumber;
                exitingUser.RoleId = user.RoleId;

                var update= await _userManager.UpdateAsync(exitingUser);
                if(update.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var identityError in update.Errors)
                    {
                        ModelState.AddModelError(string.Empty, identityError.Description);
                    }
                    return View(user);
                }

            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            TempData["error"] = "Model có lỗi";
            var error = ModelState.Values.SelectMany(v=>v.Errors.Select(e=>e.ErrorMessage)).ToList();
            string errorMess = string.Join("\n", error);
            return View(exitingUser);
        }
    }
}
    
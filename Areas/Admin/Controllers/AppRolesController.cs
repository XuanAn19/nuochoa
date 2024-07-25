using CK_ASP_NET_CORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace CK_ASP_NET_CORE.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Route("Admin/AppRoles/")]
	//[Authorize(Roles = "Admin")]
	//[Authorize]

	public class AppRolesController : Controller
	{

		private readonly RoleManager<IdentityRole> _roleManager;
		public AppRolesController(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}
		//List all the roles created by Users
		//[Route("Index")]
		public IActionResult Index()
		{
			var roles = _roleManager.Roles;
			return View(roles);
		}
		//[Route("Create")]
		//[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		//[Route("Create")]
		[HttpPost]

		public async Task<IActionResult> Create(IdentityRole model)
		{
			//avoid duplicate role
			/* if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
             {
                 _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
                 //TempData["success"] = "Tạo quyền thành công";
             }
             return Redirect("Index");*/
			if (!await _roleManager.RoleExistsAsync(model.Name))
			{
				await _roleManager.CreateAsync(new IdentityRole(model.Name));
			}
			return RedirectToAction("Index");
		}
		[HttpGet]
		//[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();//Handle missing Id
			}
			var role = await _roleManager.FindByIdAsync(id);
			return View(role);
		}
		[HttpPost]
		//[Route("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, IdentityRole model)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();//Handle missing Id
			}
			if (ModelState.IsValid)//Validate model state before processing
			{
				var role = await _roleManager.FindByIdAsync(id);
				if (role == null)
				{
					return NotFound();
				}
				role.Name = model.Name;
				try
				{
					await _roleManager.UpdateAsync(role);
					TempData["success"] = "Cập nhật phân quyền thành công";
					return RedirectToAction("Index");//Redirect to the index action
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Lỗi cập nhật phân quyền");
				}

			}//nếu mô hình không hợp lệ hoặc không tìm thấy vai trò, hãy trả lại chế độ xem bằng mô hình
			return View(model ?? new IdentityRole { Id = id });
		}
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return NotFound();
			}

			try
			{
				var result = await _roleManager.DeleteAsync(role);
				if (result.Succeeded)
				{
					TempData["success"] = "Xóa vai trò thành công";
					return RedirectToAction("Index");
				}
				else
				{
					TempData["error"] = "Lỗi xóa vai trò";
				}
			}
			catch (Exception ex)
			{
				TempData["error"] = "Lỗi xóa vai trò: " + ex.Message;
			}

			return RedirectToAction("Index");
		}
	}
}

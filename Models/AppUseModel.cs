using Microsoft.AspNetCore.Identity;

namespace CK_ASP_NET_CORE.Models
{
	public class AppUseModel : IdentityUser
	{
		public string ngheNghiep { get; set; }
		public string RoleId {  get; set; }
	}
}

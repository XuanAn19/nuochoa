using System.ComponentModel.DataAnnotations.Schema;

namespace CK_ASP_NET_CORE.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public string tenNguoiDat { get; set; }
		public string OrderCode { get; set; }
		public long IdSanPham { get; set; }
		public decimal Gia { get; set; }
		public int soLuong { get; set; }
		[ForeignKey("ProductId")]
		public ProductModel product { get; set; }
	}
}

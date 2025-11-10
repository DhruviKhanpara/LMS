using LMS.Common.Helpers;
using LMS.Presentation.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace LMS.Presentation.HTMLHelper;

public static class WidgetHtmlHelpers
{
	public static HtmlString ProfileCard(this IHtmlHelper htmlHelper, IHttpContextAccessor httpContextAccessor)
	{
		var authUserName = httpContextAccessor.HttpContext!.GetUserName();
		var authUserPhoto = httpContextAccessor.HttpContext!.GetProfilePhoto();

		var model = new UserProfileViewModel()
		{
			UserName = !string.IsNullOrWhiteSpace(authUserName) ? authUserName : "Guest",
			ProfilePhoto = !string.IsNullOrWhiteSpace(authUserPhoto) ? authUserPhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"
		};

		StringBuilder result = new StringBuilder();
		result.Append("<a href='#' class='d-flex align-items-center text-white text-decoration-none dropdown-toggle' id='profile-menu' data-bs-toggle='dropdown' aria-expanded='false' data-bs-display='dynamic'>");
		result.AppendFormat("<img src='{0}' alt='User' width='30' height='30' class='rounded-circle me-2'>", model.ProfilePhoto);
		result.AppendFormat("<span class='sidebar-option-text'><strong class='sidebar-option'>{0}</strong></span>", model.UserName);
		result.Append("</a>");

		return new HtmlString(result.ToString());
	}

	public static HtmlString Heading(this IHtmlHelper htmlHelper, string heading)
	{
		StringBuilder result = new StringBuilder();
		result.Append("<div class='d-flex justify-content-center p-1 rounded-3 text-white bg-color-purple etxt-center'>");
		result.Append("<h2>");
		result.Append(heading);
		result.Append("</h2></div>");

		return new HtmlString(result.ToString());
	}

	public static HtmlString TransparentModal(this IHtmlHelper htmlHelper)
	{
		StringBuilder result = new StringBuilder();
        result.Append("<div class='modal fade' id='transparentModal' tabindex='-1' aria-hidden='true'>");
        result.Append("<div class='modal-dialog modal-dialog-centered modal-xl'>");
        result.Append("<div class='modal-content bg-transparent border-0'>");
        result.Append("<div class='modal-body d-flex justify-content-center p-0 w-100' style=' height: 80vh; min-height: 500px;'>");
        result.Append("</div></div></div></div>");

		return new HtmlString(result.ToString());
    }
}

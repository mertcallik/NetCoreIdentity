using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using NetIdentityApp.Models;

namespace NetIdentityApp.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")]
    public class RoleUserTagHelper : TagHelper
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public RoleUserTagHelper(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HtmlAttributeName("asp-role-users")] public string RoleId { get; set; } = null!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(RoleId);
            var users = await _userManager.Users.ToListAsync();
            if (role != null)
            {
                foreach (var user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name ?? ""))
                    {
                        userNames.Add(user.UserName ?? "");
                    }

                }

                output.Content.SetHtmlContent(userNames.Count==0?"Kullanıcının henüz bir rolü yok.":ToHtml(userNames));

            }


        }

        private string? ToHtml(List<string> userNames)
        {
            var html = "<ul>";
            foreach (var userName in userNames)
            {
                html+="<li>"+userName+"</li>";
            }
            html+="</ul>";
            return html;
        }
    }
}

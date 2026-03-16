using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheAgooProjectDataAccess;

namespace SKNSchoolsPortal.Pages.Student
{
    [Authorize(Roles = SD.IsStudent)]
    public class OtherPaymentsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

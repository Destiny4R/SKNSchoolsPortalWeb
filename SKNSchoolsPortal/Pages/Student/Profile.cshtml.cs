using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheAgooProjectDataAccess;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectModel;

namespace SKNSchoolsPortal.Pages.Student
{
    [Authorize(Roles = SD.IsStudent)]
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public StudentsData student { get; set; }
        public ParentStudent parent { get; set; }
        public ProfileModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
            var ClaimsId = (ClaimsIdentity)User.Identity;
            var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
            student = dbContext.StudentTable.Include(k=>k.SessionYear).FirstOrDefault(j => j.ApplicationUserId == claim.Value);
            if (student != null)
            {
                parent = dbContext.ParentStudents.Include(k => k.ParentTable).FirstOrDefault(k => k.StudentsdataId == student.Id);
            }
        }

    }
}

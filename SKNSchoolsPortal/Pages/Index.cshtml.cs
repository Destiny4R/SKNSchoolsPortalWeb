using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SKNSchoolsPortal.Model;
using System.Security.Claims;
using System.Text.Json;
using TheAgooProjectDataAccess;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectModel;
using TheAgooProjectModel.ViewModels;

namespace SKNSchoolsPortal.Pages
{
    [Authorize(Roles = SD.IsStudent)]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public StudentsData _Studentsdata { get; set; }
        public IEnumerable<TermRegistration> _TermReg { get; set; }
        public List<ResultTable> _Results { get; set; }
        public ViewSelectModel _selectionModel { get; set; }
        public terminalResultVM terminal { get; set; }
        public AnnualResultVM annual { get; set; }
        public string Name { get; set; }
        public string StudentReg { get; set; }
        public int StudentId { get; set; }
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public void OnGet()
        {
            var ClaimsId = (ClaimsIdentity)User.Identity;
            var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
            string _getvalue = Request?.Cookies["Doom-Faith-Love"];
            if (string.IsNullOrEmpty(_getvalue))
            {
                Guid value = Guid.NewGuid();
                _getvalue = value.ToString();
                Set("Doom-Faith-Love", _getvalue.ToString(), TimeSpan.FromMinutes(1440));
                var visit = dbContext.Visitors.FirstOrDefault();
                if (visit != null)
                {
                    visit.Portal += 1;
                    dbContext.Update(visit);
                }
                else
                {
                    var vist = new Visit
                    {
                        Website = 0,
                        Portal = 1
                    };
                    dbContext.Add(vist);
                }
                dbContext.SaveChanges();
            }
            _TermReg = dbContext.TermRegistrations.Include(j => j.StudentsData.ApplicationUser).Include(j => j.SessionYear).Include(j => j.Schoolclasses).Include(j => j.SubClasses).Where(k => k.StudentsData.ApplicationUserId == claim.Value).ToList();
            StudentReg = _TermReg.FirstOrDefault().StudentsData.StudentRegNo;
            StudentId = _TermReg.FirstOrDefault().StudentsData.Id;
            if (_TermReg.Any())
            {
                _Studentsdata = _TermReg.FirstOrDefault().StudentsData;
                _Results = dbContext.ResultTable.Include(j => j.Termregistration.StudentsData).Where(h => h.Termregistration.StudentId == _Studentsdata.Id).ToList();
                Name = _Studentsdata.FirstName + " " + _Studentsdata.SurName;
                //GET THE SESSION A STUDENT AS BEEN REGISTERED FOR
                _Session = new List<SelectListItem>();
                foreach (var item in _TermReg.GroupBy(K => K.SessionYearId))
                {
                    var selt = new SelectListItem();
                    selt.Text = item.FirstOrDefault().SessionYear.Name;
                    selt.Value = item.FirstOrDefault().SessionYear.Id.ToString();
                    _Session.Add(selt);
                }
                _Session = _Session.OrderByDescending(k => k.Text).ToList();
                //GET THE SESSION A STUDENT IS BEEN REGISTERED FOR
                _theSession = new string[_Session.Count];
                for (int i = 0; i < _Session.Count; i++)
                {
                    _theSession[i] = _Session[i].Text;
                }
                //GET THE CLASSES A STUDENT IS BEEN REGISTERED FOR
                _Class = new List<SelectListItem>();
                foreach (var item in _TermReg.GroupBy(K => K.ClassesInSchoolId))
                {
                    var selt = new SelectListItem();
                    selt.Text = item.FirstOrDefault().Schoolclasses.Name;
                    selt.Value = item.FirstOrDefault().Schoolclasses.Id.ToString();
                    _Class.Add(selt);
                }
                _Class = _Class.OrderByDescending(k => k.Text).ToList();

                // Build chart data (oldest ? newest for display)
                var chartSessions = _theSession.Reverse().Where(s => !string.IsNullOrEmpty(s) && s != "0").ToArray();
                SessionLabelsJson = JsonSerializer.Serialize(chartSessions);
                FirstTermScoresJson = JsonSerializer.Serialize(chartSessions.Select(s => _Terminal(s, SD.Terms[0], _Results)).ToArray());
                SecondTermScoresJson = JsonSerializer.Serialize(chartSessions.Select(s => _Terminal(s, SD.Terms[1], _Results)).ToArray());
                ThirdTermScoresJson = JsonSerializer.Serialize(chartSessions.Select(s => _Terminal(s, SD.Terms[2], _Results)).ToArray());
                AnnualScoresJson = JsonSerializer.Serialize(chartSessions.Select(s => _Annual(s, _Results)).ToArray());
            }
        }

        public async Task<IActionResult> OnPostTermResult(terminalResultVM terminal)
        {
            if (terminal.Session > 0 && !string.IsNullOrEmpty(terminal.Term))
            {
                var ClaimsId = (ClaimsIdentity)User.Identity;
                var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
                var result = dbContext.TermRegistrations.Include(j => j.StudentsData.ApplicationUser).Include(j => j.SessionYear).Include(j => j.Schoolclasses).Include(j => j.SubClasses).FirstOrDefault(k => k.StudentsData.ApplicationUserId == claim.Value && k.Term == terminal.Term && k.SessionYearId == terminal.Session && k.ClassesInSchoolId == terminal.Classes);
                if (result != null)
                {
                    //CHECK FEES PAYMENT STATUS
                    var admin = await userManager.GetUsersInRoleAsync(SD.Role_Admin);
                    if (admin.Any())
                    {
                        var newAdmin = admin.FirstOrDefault();
                        var settings = dbContext.Settings.FirstOrDefault(k => k.ApplicationUserId == newAdmin.Id);
                        if (settings == null)
                        {
                            TempData["error"] = "The results are still under computation. Please check back later.";
                            return RedirectToPage();
                        }
                        if (!settings.CanPrintResult)
                        {
                            var fees = dbContext.PaymentReports.Include(l => l.Termregistration.StudentsData).FirstOrDefault(k => k.Termregistration.StudentsData.ApplicationUserId == claim.Value && k.Termregistration.Term == terminal.Term && k.Termregistration.SessionYearId == terminal.Session);
                            if (fees != null)
                            {
                                if (fees.Status != SD.Fees_Status_Completed)
                                {
                                    TempData["error"] = "Our system has detected an incomplete fee, please visit your fees history for details";
                                    return RedirectToPage();
                                }
                            }
                            else
                            {
                                TempData["error"] = "Our system has detected an incomplete fee, please visit your fees history for details";
                                return RedirectToPage();
                            }
                        }
                    }
                    if (result.Schoolclasses.Elective)
                    {
                        return RedirectToPage("/vs/Senior-Sheet", new { session = terminal.Session, classes = result.ClassesInSchoolId, subclass = result.SubClassId, term = result.Term });
                    }
                    return RedirectToPage("/vs/junior-Sheet", new { session = terminal.Session, classes = result.ClassesInSchoolId, subclass = result.SubClassId, term = result.Term });
                }
                TempData["error"] = "No results are available for your selection.";
                return RedirectToPage();
            }
            TempData["error"] = "Please make a selection";
            return RedirectToPage();
        }
        public IActionResult OnPostAnnualResult(AnnualResultVM annual)
        {
            if (annual.Session > 0)
            {
                return RedirectToPage("/vs/Annual-Sheet", new { session = annual.Session });
            }
            TempData["error"] = "Please make a selection";
            return RedirectToPage();
        }
        void Set(string key, string value, TimeSpan? expireTime = null)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.Add(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddDays(10);
            }

            option.HttpOnly = true;
            option.Secure = Request.IsHttps;

            Response.Cookies.Append(key, value, option);
        }


        public IList<SelectListItem> _Session { get; set; }
        public IList<SelectListItem> _Class { get; set; }
        public string[] _theSession { get; set; }

        // Chart data (serialized as JSON for the view)
        public string SessionLabelsJson { get; set; } = "[]";
        public string FirstTermScoresJson { get; set; } = "[]";
        public string SecondTermScoresJson { get; set; } = "[]";
        public string ThirdTermScoresJson { get; set; } = "[]";
        public string AnnualScoresJson { get; set; } = "[]";

        public double _Annual(string session, List<ResultTable> result)
        {
            if (result?.Count() > 0)
            {
                double scores = 0;
                var results = result.Where(k => k.Termregistration.SessionYear.Name == session).ToList();
                if (results.Count() > 0)
                {
                    foreach (var item in results)
                    {
                        scores += (double)(item?.Total ?? 0);
                    }
                    return scores;
                }
            }
            return 0;
        }
        public double _Terminal(string session, string term, List<ResultTable> result)
        {
            if (result?.Count() > 0)
            {
                double scores = 0;
                var results = result.Where(k => k.Termregistration.SessionYear.Name == session && k.Termregistration.Term == term).ToList();
                if (results.Count() > 0)
                {
                    foreach (var item in results)
                    {
                        scores += (double)(item?.Total ?? 0);
                    }
                    return scores;
                }
            }
            return 0;
        }
        public string GetValueAtIndex(string[] array, int index)
        {
            try
            {
                if (array == null || array.Length == 0)
                {
                    return "0";
                }
                if (index >= 0 && index < array.Length)
                {
                    return array[index];
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}

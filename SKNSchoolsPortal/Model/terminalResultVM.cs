using System.ComponentModel.DataAnnotations;

namespace SKNSchoolsPortal.Model
{
    public class terminalResultVM
    {
        [Required(ErrorMessage = "Please select a session")]
        public int Session { get; set; }
        [Required(ErrorMessage = "Please select a class")]
        public int Classes { get; set; }
        [Required(ErrorMessage = "Please select a term")]
        public string Term { get; set; }
    }

    public class AnnualResultVM
    {
        [Required(ErrorMessage = "Please select a session")]
        public int Session { get; set; }
    }
}

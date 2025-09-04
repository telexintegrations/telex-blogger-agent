using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling.Types
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SaveOrganizationContextInput
    {
        [Required]
        [Description("Company name.")]
        public string Name { get; set; }

        [Required]
        [Description("Short description of what the company does.")]
        public string Overview { get; set; }

        [Required]
        [Description("The industry the company operates in.")]
        public string Industry { get; set; }

        [Description("Company website URL.")]
        public string Website { get; set; }

        [Description("Preferred writing tone for blogs.")]
        public string Tone { get; set; }

        [Description("Audience the blogs should speak to.")]
        public string TargetAudience { get; set; }
    }

}

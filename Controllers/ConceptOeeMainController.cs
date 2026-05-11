using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/oee-main")]
    [ApiController]
    public class ConceptOeeMainController : ControllerBase
    {
        /// <summary>
        /// GET /api/concept/oee-main/tabs
        /// Returns the tab definitions displayed in OeeMain.vue.
        /// </summary>
        [HttpGet("tabs")]
        public IActionResult GetTabs()
        {
            return Ok(new[]
            {
                new { id = "oee",  label = "OEE",                   icon = "fa-solid fa-industry"        },
                new { id = "shop", label = "Shop Floor Monitoring",  icon = "fa-solid fa-warehouse"       },
                new { id = "pm",   label = "Preventive Maintenance", icon = "fa-solid fa-wrench"          },
                new { id = "mes",  label = "MES / WES Overview",     icon = "fa-solid fa-diagram-project" },
            });
        }
    }
}

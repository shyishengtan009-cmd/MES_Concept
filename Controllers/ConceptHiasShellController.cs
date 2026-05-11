using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/shell")]
    [ApiController]
    public class ConceptHiasShellController : ControllerBase
    {
        /// <summary>
        /// GET /api/concept/shell/menus
        /// Returns the sidebar navigation menu structure used by HiasMockup.vue.
        /// </summary>
        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            return Ok(new[]
            {
                new { name = "Dashboard",                 icon = "fa-solid fa-square-poll-vertical", children = new string[] { } },
                new { name = "Document",                  icon = "fa-solid fa-folder",               children = new[] { "Document List", "Document Category" } },
                new { name = "Certification",             icon = "fa-solid fa-folder",               children = new[] { "Halal Certification", "Industry Certification" } },
                new { name = "Audit",                     icon = "fa-solid fa-clipboard-check",      children = new[] { "Internal Audit", "Supplier Audit", "Third-Party Audit" } },
                new { name = "Non-Conformance",           icon = "fa-solid fa-triangle-exclamation", children = new[] { "NC List", "NC Report" } },
                new { name = "AI Matching",               icon = "fa-solid fa-wand-magic-sparkles",  children = new[] { "Smart Matching", "Matching Settings" } },
                new { name = "Operation",                 icon = "fa-solid fa-briefcase",            children = new[] { "Product", "Raw Material", "Manufacturing" } },
                new { name = "Purchasing",                icon = "fa-solid fa-file-invoice-dollar",  children = new[] { "Supplier", "Purchase Order" } },
                new { name = "MES",                       icon = "fa-solid fa-diagram-project",      children = new[] { "Dashboard", "Inventory", "Cost Analysis" } },
                new { name = "Collaborator",              icon = "fa-solid fa-building",             children = new[] { "Collaborator List", "Collaboration Requests" } },
                new { name = "Report",                    icon = "fa-solid fa-list",                 children = new[] { "Audit Report", "Compliance Report", "Export Report" } },
                new { name = "Monitoring",                icon = "fa-solid fa-eye",                  children = new[] { "Typhoid Vaccination", "Cold Truck Real-Time Monitoring", "Alert" } },
                new { name = "Human Resource",            icon = "fa-solid fa-people-group",         children = new[] { "Employee", "Training", "Competency" } },
                new { name = "Customer Voice Management", icon = "fa-solid fa-comments",             children = new string[] { } },
                new { name = "User Management",           icon = "fa-solid fa-user",                 children = new string[] { } },
            });
        }

        /// <summary>
        /// GET /api/concept/shell/page-map
        /// Returns which sidebar child routes to which page key.
        /// </summary>
        [HttpGet("page-map")]
        public IActionResult GetPageMap()
        {
            return Ok(new Dictionary<string, string>
            {
                { "Dashboard",     "oee-main"     },
                { "Inventory",     "inventory"    },
                { "Cost Analysis", "cost-analysis"},
            });
        }
    }
}

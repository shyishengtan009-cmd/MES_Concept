using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/cost-analysis")]
    [ApiController]
    public class ConceptCostAnalysisController : ControllerBase
    {
        [HttpGet("overview")]
        public IActionResult GetOverview()
        {
            return Ok(new
            {
                totalCostMtd   = "RM 284,700",
                materialCost   = "RM 196,400",
                overheadCost   = "RM 88,300",
                vsBudget       = "+RM 12,700",
                vsBudgetPct    = "+4.7%",
                materialPct    = "69.0%",
                overheadPct    = "31.0%",
                vsPrevMonth    = "+4.2%"
            });
        }

        [HttpGet("breakdown")]
        public IActionResult GetBreakdown()
        {
            return Ok(new[]
            {
                new { name = "Raw Materials", pct = 52.4, barPct = 100, value = "RM 149,200", color = "#1565c0" },
                new { name = "Packaging",     pct = 16.6, barPct = 44,  value = "RM 47,200",  color = "#42a5f5" },
                new { name = "Utilities",     pct = 11.8, barPct = 30,  value = "RM 33,600",  color = "#7b1fa2" },
                new { name = "Labour",        pct = 9.8,  barPct = 26,  value = "RM 27,900",  color = "#ef9a9a" },
                new { name = "Maintenance",   pct = 6.3,  barPct = 17,  value = "RM 17,900",  color = "#f9a825" },
                new { name = "Other",         pct = 3.1,  barPct = 11,  value = "RM 8,900",   color = "#9e9e9e" },
            });
        }

        [HttpGet("budget-vs-actual")]
        public IActionResult GetBudgetVsActual()
        {
            return Ok(new[]
            {
                new { name = "Raw Materials",  budget = "RM 142,000", actual = "RM 149,200", variance = "+5.1%",  over = true,  budgetBarPct = 100, actualBarPct = 105 },
                new { name = "Packaging",      budget = "RM 48,000",  actual = "RM 47,200",  variance = "−1.7%",  over = false, budgetBarPct = 100, actualBarPct = 98  },
                new { name = "Utilities",      budget = "RM 30,000",  actual = "RM 33,600",  variance = "+12.0%", over = true,  budgetBarPct = 100, actualBarPct = 112 },
                new { name = "Labour",         budget = "RM 28,000",  actual = "RM 27,900",  variance = "−0.4%",  over = false, budgetBarPct = 100, actualBarPct = 99  },
                new { name = "Maintenance",    budget = "RM 15,000",  actual = "RM 17,900",  variance = "+19.3%", over = true,  budgetBarPct = 100, actualBarPct = 119 },
                new { name = "Other Overhead", budget = "RM 9,000",   actual = "RM 8,900",   variance = "−1.1%",  over = false, budgetBarPct = 100, actualBarPct = 99  },
            });
        }

        [HttpGet("material-cost")]
        public IActionResult GetMaterialCost([FromQuery] string category = "all")
        {
            var all = new[]
            {
                new { name = "Halal Seasoning Blend A", category = "food",      total = 38200, pct = "19.5%", vsLastMonth = "+1,400", vsPositive = true  },
                new { name = "FFS Film Roll 500mm",      category = "packaging", total = 28600, pct = "14.6%", vsLastMonth = "−320",   vsPositive = false },
                new { name = "Tomato Paste (Drum)",      category = "food",      total = 24800, pct = "12.6%", vsLastMonth = "+2,100", vsPositive = true  },
                new { name = "Palm Olein Oil (5L)",      category = "food",      total = 18900, pct = "9.6%",  vsLastMonth = "−600",   vsPositive = false },
                new { name = "Corrugated Box 400×300",   category = "packaging", total = 17000, pct = "8.7%",  vsLastMonth = "+800",   vsPositive = true  },
                new { name = "White Sugar (50kg bag)",   category = "food",      total = 6900,  pct = "3.5%",  vsLastMonth = "−300",   vsPositive = false },
                new { name = "Label Sticker 50×30mm",    category = "packaging", total = 3360,  pct = "1.7%",  vsLastMonth = "+160",   vsPositive = true  },
                new { name = "LDPE Shrink Film 20µm",    category = "packaging", total = 1980,  pct = "1.0%",  vsLastMonth = "−220",   vsPositive = false },
                new { name = "Sodium Benzoate E211",     category = "chemical",  total = 1296,  pct = "0.7%",  vsLastMonth = "+96",    vsPositive = true  },
                new { name = "Vitamin C Ascorbic Acid",  category = "chemical",  total = 912,   pct = "0.5%",  vsLastMonth = "−38",    vsPositive = false },
            };

            var filtered = category == "all" ? all : all.Where(r => r.category == category).ToArray();
            return Ok(filtered);
        }

        [HttpGet("overhead")]
        public IActionResult GetOverhead()
        {
            return Ok(new[]
            {
                new { category = "Utilities",    item = "Electricity — Production",   budget = 19100, actual = 21400, variance = "+2,300", over = true  },
                new { category = "Utilities",    item = "Electricity — Cooling",      budget = 6800,  actual = 7000,  variance = "+200",   over = true  },
                new { category = "Utilities",    item = "Natural Gas",                budget = 3600,  actual = 3400,  variance = "−200",   over = false },
                new { category = "Utilities",    item = "Water",                      budget = 1900,  actual = 1800,  variance = "−100",   over = false },
                new { category = "Labour",       item = "Direct Labour (Production)", budget = 22000, actual = 21600, variance = "−400",   over = false },
                new { category = "Labour",       item = "Overtime Allowance",         budget = 4000,  actual = 4800,  variance = "+800",   over = true  },
                new { category = "Labour",       item = "Contractor Labour",          budget = 2000,  actual = 1500,  variance = "−500",   over = false },
                new { category = "Maintenance",  item = "Preventive Maintenance",     budget = 8000,  actual = 9200,  variance = "+1,200", over = true  },
                new { category = "Maintenance",  item = "Corrective Repairs",         budget = 4000,  actual = 6400,  variance = "+2,400", over = true  },
                new { category = "Maintenance",  item = "Consumables / Lubrication",  budget = 3000,  actual = 2300,  variance = "−700",   over = false },
                new { category = "Other",        item = "Logistics / Inbound Freight",budget = 5000,  actual = 5200,  variance = "+200",   over = true  },
                new { category = "Other",        item = "Admin & General",            budget = 4000,  actual = 3700,  variance = "−300",   over = false },
            });
        }
    }
}

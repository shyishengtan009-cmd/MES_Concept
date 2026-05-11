using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/inventory")]
    [ApiController]
    public class ConceptInventoryController : ControllerBase
    {
        [HttpGet("kpis")]
        public IActionResult GetKpis()
        {
            return Ok(new { totalSkus = 248, stockValue = "RM 1.24M", lowCritical = 14, pendingReceipts = 7 });
        }

        [HttpGet("by-category")]
        public IActionResult GetByCategory()
        {
            return Ok(new[]
            {
                new { name = "Packaging Material",  qty = 41200, pct = 82, color = "#1565c0" },
                new { name = "Food Ingredients",    qty = 32800, pct = 65, color = "#388E3C" },
                new { name = "Finished Product",    qty = 27600, pct = 55, color = "#7b1fa2" },
                new { name = "Chemical / Additive", qty = 14200, pct = 28, color = "#f9a825" },
                new { name = "Spare Parts",         qty = 9100,  pct = 18, color = "#9e9e9e" },
            });
        }

        [HttpGet("recent-movements")]
        public IActionResult GetRecentMovements()
        {
            return Ok(new[]
            {
                new { time = "10:38", item = "Palm Olein Oil (5L)",       movement = "received",   qty = "+100",    @ref = "GRN-00214" },
                new { time = "09:55", item = "Chili Sauce Pouch 250g",   movement = "dispatch",   qty = "−1,200",  @ref = "ISO-00389" },
                new { time = "09:20", item = "FFS Film Roll 500mm",      movement = "production", qty = "−80 kg",  @ref = "ISO-00388" },
                new { time = "08:45", item = "Sodium Benzoate E211",     movement = "corrected",  qty = "−2 kg",   @ref = "ADJ-00047" },
                new { time = "08:10", item = "Corrugated Box 400×300",   movement = "received",   qty = "+500",    @ref = "GRN-00213" },
                new { time = "07:50", item = "Tomato Paste (Drum)",      movement = "received",   qty = "+20",     @ref = "GRN-00212" },
                new { time = "Yest 16:30", item = "Curry Paste Tub 500g",movement = "dispatch",   qty = "−300",    @ref = "ISO-00387" },
                new { time = "Yest 15:10", item = "Label Sticker 50×30mm",movement= "production", qty = "−800",    @ref = "ISO-00386" },
                new { time = "Yest 14:00", item = "Halal Seasoning Blend A",movement="received",  qty = "+200 kg", @ref = "GRN-00211" },
            });
        }

        [HttpGet("stock-register")]
        public IActionResult GetStockRegister()
        {
            return Ok(new[]
            {
                new { code = "RM-001", name = "Palm Olein Oil (5L)",      category = "Food Ingredient",    type = "RM", onHand = "12",    reorderPt = "50",    status = "critical" },
                new { code = "RM-002", name = "Tomato Paste (Drum)",      category = "Food Ingredient",    type = "RM", onHand = "220",   reorderPt = "50",    status = "normal"   },
                new { code = "RM-003", name = "Halal Seasoning Blend A",  category = "Food Ingredient",    type = "RM", onHand = "1,200", reorderPt = "300",   status = "normal"   },
                new { code = "RM-004", name = "Sodium Benzoate E211",     category = "Chemical / Additive",type = "RM", onHand = "28",    reorderPt = "40",    status = "low"      },
                new { code = "RM-005", name = "FFS Film Roll 500mm",      category = "Packaging",          type = "RM", onHand = "3,400", reorderPt = "500",   status = "normal"   },
                new { code = "RM-006", name = "LDPE Shrink Film 20µm",    category = "Packaging",          type = "RM", onHand = "80",    reorderPt = "200",   status = "critical" },
                new { code = "RM-007", name = "Corrugated Box 400×300",   category = "Packaging",          type = "RM", onHand = "140",   reorderPt = "200",   status = "low"      },
                new { code = "RM-008", name = "Vitamin C Ascorbic Acid",  category = "Chemical / Additive",type = "RM", onHand = "5.2",   reorderPt = "10",    status = "low"      },
                new { code = "RM-009", name = "Label Sticker 50×30mm",    category = "Packaging",          type = "RM", onHand = "1,200", reorderPt = "2,000", status = "low"      },
                new { code = "RM-010", name = "White Sugar (50kg bag)",   category = "Food Ingredient",    type = "RM", onHand = "85",    reorderPt = "30",    status = "normal"   },
                new { code = "FG-001", name = "Chili Sauce Pouch 250g",   category = "Finished Product",   type = "FG", onHand = "8,400", reorderPt = "1,000", status = "normal"   },
                new { code = "FG-002", name = "Curry Paste Tub 500g",     category = "Finished Product",   type = "FG", onHand = "5,200", reorderPt = "800",   status = "normal"   },
                new { code = "FG-003", name = "Sambal Belacan Jar 200g",  category = "Finished Product",   type = "FG", onHand = "2,100", reorderPt = "500",   status = "normal"   },
                new { code = "SP-001", name = "Conveyor Belt Segment",    category = "Spare Part",         type = "SP", onHand = "4",     reorderPt = "2",     status = "normal"   },
                new { code = "SP-002", name = "Sealing Element 12mm",     category = "Spare Part",         type = "SP", onHand = "6",     reorderPt = "5",     status = "normal"   },
            });
        }

        [HttpGet("movements")]
        public IActionResult GetMovements([FromQuery] string period = "today", [FromQuery] string type = "all")
        {
            var all = new[]
            {
                new { date = "15/04 10:38", refNo = "GRN-00214", item = "Palm Olein Oil (5L)",         movement = "received",   qty = "+100 unit",  by = "Amirul",    period = "today" },
                new { date = "15/04 09:55", refNo = "ISO-00389", item = "Chili Sauce Pouch 250g",      movement = "dispatch",   qty = "−1,200 pcs", by = "Farah",     period = "today" },
                new { date = "15/04 09:20", refNo = "ISO-00388", item = "FFS Film Roll 500mm",         movement = "production", qty = "−80 kg",     by = "Amirul",    period = "today" },
                new { date = "15/04 08:45", refNo = "ADJ-00047", item = "Sodium Benzoate E211",        movement = "corrected",  qty = "−2 kg",      by = "Supervisor",period = "today" },
                new { date = "15/04 08:10", refNo = "GRN-00213", item = "Corrugated Box 400×300",      movement = "received",   qty = "+500 unit",  by = "Saliza",    period = "today" },
                new { date = "15/04 07:50", refNo = "GRN-00212", item = "Tomato Paste (Drum)",         movement = "received",   qty = "+20 unit",   by = "Saliza",    period = "today" },
                new { date = "14/04 16:30", refNo = "ISO-00387", item = "Curry Paste Tub 500g",        movement = "dispatch",   qty = "−300 pcs",   by = "Farah",     period = "week"  },
                new { date = "14/04 15:10", refNo = "ISO-00386", item = "Label Sticker 50×30mm",       movement = "production", qty = "−800 pcs",   by = "Amirul",    period = "week"  },
                new { date = "14/04 14:00", refNo = "GRN-00211", item = "Halal Seasoning Blend A",     movement = "received",   qty = "+200 kg",    by = "Saliza",    period = "week"  },
                new { date = "14/04 11:20", refNo = "ADJ-00046", item = "Sambal Belacan Jar 200g",     movement = "corrected",  qty = "+50 pcs",    by = "Supervisor",period = "week"  },
                new { date = "14/04 10:05", refNo = "GRN-00210", item = "White Sugar (50kg bag)",      movement = "received",   qty = "+10 bag",    by = "Saliza",    period = "week"  },
                new { date = "14/04 09:30", refNo = "ISO-00385", item = "Halal Seasoning Blend A",     movement = "production", qty = "−150 kg",    by = "Amirul",    period = "week"  },
                new { date = "10/04 09:00", refNo = "GRN-00208", item = "LDPE Shrink Film 20µm",       movement = "received",   qty = "+50 roll",   by = "Saliza",    period = "month" },
                new { date = "09/04 14:20", refNo = "ISO-00382", item = "Sambal Belacan Jar 200g",     movement = "dispatch",   qty = "−600 pcs",   by = "Farah",     period = "month" },
                new { date = "08/04 11:00", refNo = "ISO-00380", item = "Palm Olein Oil (5L)",         movement = "production", qty = "−60 unit",   by = "Amirul",    period = "month" },
                new { date = "07/04 10:30", refNo = "ADJ-00044", item = "FFS Film Roll 500mm",         movement = "corrected",  qty = "+5 kg",      by = "Supervisor",period = "month" },
            };

            var filtered = all.Where(r =>
                (period == "month" || r.period == period || (period == "week" && (r.period == "today" || r.period == "week")))
                && (type == "all" || r.movement == type)
            ).ToArray();

            return Ok(filtered);
        }
    }
}

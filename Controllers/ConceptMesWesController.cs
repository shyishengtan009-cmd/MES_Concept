using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/mes-wes")]
    [ApiController]
    public class ConceptMesWesController : ControllerBase
    {
        [HttpGet("kpis")]
        public IActionResult GetKpis()
        {
            return Ok(new
            {
                activeWorkOrders = 12,
                ordersComplete   = 47,
                onTimeRate       = "91%",
                warehouseTasks   = 38,
                agvsActive       = "4/5",
                pickAccuracy     = "99.2%",
                exceptions       = 3
            });
        }

        [HttpGet("work-orders")]
        public IActionResult GetWorkOrders()
        {
            return Ok(new[]
            {
                new { order = "WO-2026-041", product = "Pack A 500g", line = "Line 1", qty = "2,400", progress = 82, status = "RUNNING" },
                new { order = "WO-2026-042", product = "Pack B 250g", line = "Line 1", qty = "1,800", progress = 55, status = "RUNNING" },
                new { order = "WO-2026-043", product = "Mix Batch C", line = "Line 2", qty = "80 bat", progress = 100, status = "DONE" },
                new { order = "WO-2026-044", product = "Pack A 1kg",  line = "Line 2", qty = "600",    progress = 20, status = "SETUP" },
                new { order = "WO-2026-045", product = "Mix Batch D", line = "Line 2", qty = "120 bat",progress = 0,  status = "QUEUED" },
            });
        }

        [HttpGet("quality-holds")]
        public IActionResult GetQualityHolds()
        {
            return Ok(new[]
            {
                new { lot = "LOT-2026-088", reason = "Fill weight deviation", qtyHeld = "240 units", raised = "09:07 am", status = "ON HOLD"   },
                new { lot = "LOT-2026-085", reason = "Seal integrity check",  qtyHeld = "80 units",  raised = "08:50 am", status = "REVIEWING" },
                new { lot = "LOT-2026-081", reason = "Label placement",       qtyHeld = "150 units", raised = "07:30 am", status = "RELEASED"  },
            });
        }

        [HttpGet("wes-flow")]
        public IActionResult GetWesFlow()
        {
            return Ok(new
            {
                receiving = 8,
                storage   = 2840,
                picking   = 14,
                packing   = 6,
                staging   = 3,
                dispatch  = 47
            });
        }

        [HttpGet("agv-status")]
        public IActionResult GetAgvStatus()
        {
            return Ok(new[]
            {
                new { unit = "AGV Unit 01", zone = "Zone D",   task = "Rack A → Staging",    battery = "88%", status = "ACTIVE"   },
                new { unit = "Forklift 01", zone = "Zone D",   task = "Receiving → Rack C",  battery = "—",   status = "RUNNING"  },
                new { unit = "AGV Unit 02", zone = "Zone B",   task = "Batch → Zone D",      battery = "61%", status = "ACTIVE"   },
                new { unit = "AGV Unit 03", zone = "—",        task = "Idle at dock",        battery = "100%",status = "IDLE"     },
                new { unit = "AGV Unit 04", zone = "Charging", task = "—",                   battery = "32%", status = "CHARGING" },
                new { unit = "AGV Unit 05", zone = "—",        task = "Maintenance hold",    battery = "—",   status = "OFFLINE"  },
            });
        }

        [HttpGet("mes-events")]
        public IActionResult GetMesEvents()
        {
            return Ok(new[]
            {
                new { color = "#e53935", time = "09:14 am", text = "WO-2026-044: setup delay — awaiting raw material from Zone B" },
                new { color = "#388E3C", time = "09:02 am", text = "WO-2026-043 completed — 80 batches dispatched to Zone D" },
                new { color = "#f9a825", time = "08:57 am", text = "LOT-2026-088: quality hold raised on Line 1" },
                new { color = "#1565c0", time = "08:44 am", text = "Shift 2 started — WO-2026-041 resumed on Line 1" },
                new { color = "#388E3C", time = "08:30 am", text = "LOT-2026-081 released — 150 units cleared by QC" },
            });
        }

        [HttpGet("wes-events")]
        public IActionResult GetWesEvents()
        {
            return Ok(new[]
            {
                new { color = "#e53935", time = "09:11 am", text = "AGV Unit 05 offline — maintenance hold flagged" },
                new { color = "#388E3C", time = "09:05 am", text = "Order #ORD-882 dispatched — 480 units shipped" },
                new { color = "#1565c0", time = "08:58 am", text = "AGV Unit 02 rerouted — Zone B congestion" },
                new { color = "#388E3C", time = "08:44 am", text = "Rack C replenished — 320 units putaway complete" },
                new { color = "#f9a825", time = "08:30 am", text = "AGV Unit 04 low battery — returning to dock" },
            });
        }
    }
}

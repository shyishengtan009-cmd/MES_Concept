using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/preventive-maintenance")]
    [ApiController]
    public class ConceptPreventiveMaintenanceController : ControllerBase
    {
        [HttpGet("schedule")]
        public IActionResult GetSchedule([FromQuery] string line = "ffs")
        {
            if (line == "bp")
            {
                return Ok(new[]
                {
                    new { asset = "Batching Plant 01", zone = "Zone B", task = "Mixer blade wear check",          frequency = "Monthly",   lastDone = "01 Mar 2026", nextDue = "01 Apr 2026", status = "OVERDUE",     assigned = "Kumar S." },
                    new { asset = "Batching Plant 01", zone = "Zone B", task = "Cement silo sensor calibration", frequency = "Monthly",   lastDone = "06 Mar 2026", nextDue = "06 Apr 2026", status = "IN PROGRESS", assigned = "Kumar S." },
                    new { asset = "Batching Plant 02", zone = "Zone B", task = "Belt conveyor inspection",       frequency = "Bi-weekly", lastDone = "28 Mar 2026", nextDue = "11 Apr 2026", status = "DUE SOON",    assigned = "Kumar S." },
                    new { asset = "Mixer Unit 01",     zone = "Zone B", task = "Drive belt tension check",       frequency = "Bi-weekly", lastDone = "24 Mar 2026", nextDue = "07 Apr 2026", status = "OK",          assigned = "Kumar S." },
                    new { asset = "Mixer Unit 02",     zone = "Zone B", task = "Drive belt tension check",       frequency = "Bi-weekly", lastDone = "24 Mar 2026", nextDue = "07 Apr 2026", status = "OK",          assigned = "Kumar S." },
                    new { asset = "Batching Plant 01", zone = "Zone B", task = "Weigh hopper calibration",       frequency = "Monthly",   lastDone = "05 Mar 2026", nextDue = "05 Apr 2026", status = "OK",          assigned = "Kumar S." },
                    new { asset = "Batching Plant 02", zone = "Zone B", task = "Water flow meter calibration",   frequency = "Monthly",   lastDone = "10 Mar 2026", nextDue = "10 Apr 2026", status = "OK",          assigned = "Kumar S." },
                    new { asset = "Batching Plant 03", zone = "Zone B", task = "Aggregate hopper cleaning",      frequency = "Weekly",    lastDone = "04 Apr 2026", nextDue = "11 Apr 2026", status = "OK",          assigned = "Kumar S." },
                    new { asset = "Batching Plant 01", zone = "Zone B", task = "Admixture pump inspection",      frequency = "Monthly",   lastDone = "08 Mar 2026", nextDue = "08 Apr 2026", status = "OK",          assigned = "Kumar S." },
                });
            }
            return Ok(new[]
            {
                new { asset = "FFS Machine 01",  zone = "Zone A", task = "Seal jaw inspection",          frequency = "Weekly",  lastDone = "29 Mar 2026", nextDue = "05 Apr 2026", status = "OVERDUE",     assigned = "Ali B." },
                new { asset = "FFS Machine 04",  zone = "Zone B", task = "Horizontal jaw calibration",   frequency = "Weekly",  lastDone = "25 Mar 2026", nextDue = "01 Apr 2026", status = "OVERDUE",     assigned = "Raj T." },
                new { asset = "FFS Machine 02",  zone = "Zone A", task = "Film roll guide lubrication",  frequency = "Weekly",  lastDone = "02 Apr 2026", nextDue = "09 Apr 2026", status = "DUE SOON",    assigned = "Ali B." },
                new { asset = "FFS Machine 01",  zone = "Zone A", task = "Drive belt inspection",        frequency = "Monthly", lastDone = "05 Mar 2026", nextDue = "05 Apr 2026", status = "IN PROGRESS", assigned = "Ali B." },
                new { asset = "FFS Machine 03",  zone = "Zone A", task = "Sealing wire replacement",     frequency = "Monthly", lastDone = "10 Mar 2026", nextDue = "10 Apr 2026", status = "OK",          assigned = "Ali B." },
                new { asset = "FFS Machine 02",  zone = "Zone A", task = "Dosing hopper cleaning",       frequency = "Weekly",  lastDone = "04 Apr 2026", nextDue = "11 Apr 2026", status = "OK",          assigned = "Ali B." },
                new { asset = "FFS Machine 03",  zone = "Zone A", task = "Motor current check",          frequency = "Monthly", lastDone = "06 Mar 2026", nextDue = "06 Apr 2026", status = "OK",          assigned = "Raj T." },
                new { asset = "Conveyor Belt A", zone = "Zone A", task = "Belt tension & alignment",     frequency = "Monthly", lastDone = "07 Mar 2026", nextDue = "07 Apr 2026", status = "OK",          assigned = "Ali B." },
                new { asset = "Inspection Unit", zone = "Zone A", task = "Camera lens clean",            frequency = "Weekly",  lastDone = "03 Apr 2026", nextDue = "10 Apr 2026", status = "OK",          assigned = "Raj T." },
            });
        }

        [HttpGet("asset-health")]
        public IActionResult GetAssetHealth([FromQuery] string line = "ffs")
        {
            if (line == "bp")
            {
                return Ok(new[]
                {
                    new { name = "Batching Plant 01", health = 48, color = "#f9a825" },
                    new { name = "Batching Plant 02", health = 72, color = "#388E3C" },
                    new { name = "Batching Plant 03", health = 65, color = "#f9a825" },
                    new { name = "Mixer Unit 01",     health = 80, color = "#388E3C" },
                    new { name = "Mixer Unit 02",     health = 76, color = "#388E3C" },
                });
            }
            return Ok(new[]
            {
                new { name = "FFS Machine 01",  health = 62, color = "#f9a825" },
                new { name = "FFS Machine 02",  health = 81, color = "#388E3C" },
                new { name = "FFS Machine 03",  health = 79, color = "#388E3C" },
                new { name = "FFS Machine 04",  health = 34, color = "#e53935" },
                new { name = "Conveyor Belt A", health = 71, color = "#388E3C" },
                new { name = "Inspection Unit", health = 85, color = "#388E3C" },
            });
        }

        [HttpGet("recent-activity")]
        public IActionResult GetRecentActivity([FromQuery] string line = "ffs")
        {
            if (line == "bp")
            {
                return Ok(new[]
                {
                    new { color = "#e53935", time = "09:11 am", text = "BP01 — aggregate hopper low level alarm (18.2%)" },
                    new { color = "#e53935", time = "09:00 am", text = "BP01 — mixer blade check overdue by 13 days" },
                    new { color = "#1565c0", time = "08:50 am", text = "BP01 — cement silo calibration started by Kumar S." },
                    new { color = "#388E3C", time = "08:02 am", text = "BP02 — batch #47 completed (2,450 kg dispatched)" },
                    new { color = "#388E3C", time = "07:55 am", text = "Mixer Unit 02 — belt tension within spec ✓" },
                    new { color = "#f9a825", time = "07:44 am", text = "BP03 set to IDLE — awaiting work order" },
                });
            }
            return Ok(new[]
            {
                new { color = "#e53935", time = "09:14 am", text = "FFS M04 stopped — E-42 jaw overheat fault" },
                new { color = "#e53935", time = "09:08 am", text = "FFS M01 — seal jaw inspection overdue by 2 days" },
                new { color = "#1565c0", time = "08:55 am", text = "FFS M01 — drive belt inspection started by Ali B." },
                new { color = "#f9a825", time = "08:10 am", text = "FFS M02 — film roll lubrication due in 3 days" },
                new { color = "#388E3C", time = "07:45 am", text = "Conveyor Belt A — tension inspection completed ✓" },
                new { color = "#388E3C", time = "07:20 am", text = "FFS M03 — sealing wire replacement completed ✓" },
            });
        }
    }
}

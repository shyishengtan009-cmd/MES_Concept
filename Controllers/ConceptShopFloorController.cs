using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/shop-floor")]
    [ApiController]
    public class ConceptShopFloorController : ControllerBase
    {
        [HttpGet("machines")]
        public IActionResult GetMachines([FromQuery] string tab = "indoor")
        {
            if (tab == "outdoor")
            {
                return Ok(new[]
                {
                    new { id = "bp01", name = "Batching Plant 01", zone = "Zone A", status = "ACTIVE",  batchWeight = "2,450 kg", mixerRpm = "18.4 rpm", aggFeed = "53.6 t/h" },
                    new { id = "bp02", name = "Batching Plant 02", zone = "Zone A", status = "ACTIVE",  batchWeight = "2,380 kg", mixerRpm = "17.8 rpm", aggFeed = "51.2 t/h" },
                    new { id = "bp03", name = "Batching Plant 03", zone = "Zone B", status = "IDLE",    batchWeight = "— —",      mixerRpm = "0.0 rpm",  aggFeed = "Standby"   },
                });
            }
            return Ok(new[]
            {
                new { id = "m01", name = "FFS Machine 01", zone = "Zone A", status = "ACTIVE", fillWeight = "250.4 g", sealTemp = "182.5 °C", feedRate = "48.2 kg/h" },
                new { id = "m02", name = "FFS Machine 02", zone = "Zone A", status = "ACTIVE", fillWeight = "249.8 g", sealTemp = "181.0 °C", feedRate = "47.9 kg/h" },
                new { id = "m03", name = "FFS Machine 03", zone = "Zone B", status = "ACTIVE", fillWeight = "251.2 g", sealTemp = "183.2 °C", feedRate = "49.0 kg/h" },
                new { id = "m04", name = "FFS Machine 04", zone = "Zone B", status = "FAULT",  fillWeight = "— —",     sealTemp = "FAULT",    feedRate = "0.0 A"    },
            });
        }

        [HttpGet("kpis")]
        public IActionResult GetKpis([FromQuery] string tab = "indoor")
        {
            if (tab == "outdoor")
            {
                return Ok(new
                {
                    total       = 3,
                    active      = 2,
                    idle        = 1,
                    fault       = 0,
                    batchesToday = 47,
                    activeAlarms = 1,
                    siloLevel   = "83%"
                });
            }
            return Ok(new
            {
                total        = 4,
                active       = 3,
                idle         = 0,
                fault        = 1,
                outputToday  = 18640,
                activeAlarms = 2,
                avgFillWt    = 250.0
            });
        }

        [HttpGet("events")]
        public IActionResult GetEvents()
        {
            return Ok(new[]
            {
                new { time = "09:14 am", color = "#e53935", text = "FFS Machine 04 stopped — E-42 jaw overheat fault" },
                new { time = "09:08 am", color = "#e53935", text = "Fill weight deviation alarm — M01 +3.8 g over tolerance" },
                new { time = "09:02 am", color = "#43a047", text = "FFS Machine 03 resumed after film roll change" },
                new { time = "08:44 am", color = "#1565c0", text = "Shift 2 started — WO-2026-041 active on all lines" },
            });
        }
    }
}

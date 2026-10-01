using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoccerClubPlayerManagement.Data;
using SoccerClubPlayerManagement.Models;

namespace SoccerClubPlayerManagement.Controllers
{
    // Any logged-in user can view the saved lineup; only Coach can change it.
    [Authorize]
    public class LineupController : Controller
    {
        private const int SubstituteBenchSize = 7;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LineupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lineup — read-only pitch view of the currently saved starting XI
        public async Task<IActionResult> Index()
        {
            var lineup = await _context.Lineups
                .Include(l => l.Formation)
                .Include(l => l.Captain)
                .Include(l => l.LineupSlots!)
                    .ThenInclude(ls => ls.FormationSlot)
                .Include(l => l.LineupSlots!)
                    .ThenInclude(ls => ls.Player)
                .Include(l => l.Substitutes!)
                    .ThenInclude(s => s.Player)
                        .ThenInclude(p => p!.Position)
                .OrderByDescending(l => l.LastUpdated)
                .FirstOrDefaultAsync();

            return View(lineup); // null is valid — view handles "no lineup saved yet"
        }

        // GET: Lineup/Edit?formationId=2
        // Changing the formation dropdown reloads this page with a fresh, empty slot set for
        // that formation (existing assignments only carry over when the formation is unchanged).
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Edit(int? formationId)
        {
            var existingLineup = await _context.Lineups
                .Include(l => l.LineupSlots)
                .Include(l => l.Substitutes)
                .OrderByDescending(l => l.LastUpdated)
                .FirstOrDefaultAsync();

            var selectedFormationId = formationId
                ?? existingLineup?.FormationId
                ?? (await _context.Formations.OrderBy(f => f.FormationId).FirstAsync()).FormationId;

            var slots = await _context.FormationSlots
                .Where(fs => fs.FormationId == selectedFormationId)
                .OrderBy(fs => fs.SlotOrder)
                .ToListAsync();

            var sameFormation = existingLineup != null && existingLineup.FormationId == selectedFormationId;

            var existingSubs = existingLineup?.Substitutes?
                .OrderBy(s => s.SubOrder)
                .Select(s => s.PlayerId)
                .ToList() ?? new List<int?>();

            // Pad or trim the saved substitutes list to a fixed bench size for the form.
            var subAssignments = Enumerable.Range(0, SubstituteBenchSize)
                .Select(i => new SubstituteAssignment { PlayerId = i < existingSubs.Count ? existingSubs[i] : null })
                .ToList();

            var vm = new EditLineupViewModel
            {
                LineupId = existingLineup?.LineupId ?? 0,
                FormationId = selectedFormationId,
                CaptainPlayerId = sameFormation ? existingLineup?.CaptainPlayerId : null,
                Slots = slots.Select(s => new LineupSlotAssignment
                {
                    FormationSlotId = s.FormationSlotId,
                    Label = s.Label,
                    X = s.X,
                    Y = s.Y,
                    // Only prefill a saved player if we're still on the same formation the lineup was saved with
                    PlayerId = sameFormation
                        ? existingLineup?.LineupSlots?.FirstOrDefault(ls => ls.FormationSlotId == s.FormationSlotId)?.PlayerId
                        : null
                }).ToList(),
                Substitutes = subAssignments
            };

            await PopulateDropdowns(selectedFormationId);
            return View(vm);
        }

        // POST: Lineup/Save
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Save(EditLineupViewModel vm)
        {
            var startingIds = vm.Slots.Where(s => s.PlayerId.HasValue).Select(s => s.PlayerId!.Value).ToList();
            var subIds = vm.Substitutes.Where(s => s.PlayerId.HasValue).Select(s => s.PlayerId!.Value).ToList();

            // Flag duplicate player picks — within the starting 11, within the bench, or across both.
            var allIds = startingIds.Concat(subIds).ToList();
            if (allIds.Count != allIds.Distinct().Count())
                ModelState.AddModelError(string.Empty, "The same player is assigned to more than one slot (starting XI and/or substitutes) — each player can only appear once.");

            // Captain must be one of the 11 starting players.
            if (vm.CaptainPlayerId.HasValue && !startingIds.Contains(vm.CaptainPlayerId.Value))
                ModelState.AddModelError(string.Empty, "The captain must be one of the starting 11 players.");

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm.FormationId);
                return View("Edit", vm);
            }

            var lineup = vm.LineupId > 0
                ? await _context.Lineups
                    .Include(l => l.LineupSlots)
                    .Include(l => l.Substitutes)
                    .FirstOrDefaultAsync(l => l.LineupId == vm.LineupId)
                : null;

            if (lineup == null)
            {
                lineup = new Lineup { FormationId = vm.FormationId };
                _context.Lineups.Add(lineup);
            }
            else
            {
                lineup.FormationId = vm.FormationId;
                // Formation may have changed — clear old slot assignments and rebuild from scratch.
                _context.LineupSlots.RemoveRange(lineup.LineupSlots!);
                _context.LineupSubstitutes.RemoveRange(lineup.Substitutes!);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            lineup.CaptainPlayerId = vm.CaptainPlayerId;
            lineup.CoachName = currentUser?.FullName;
            lineup.LastUpdated = DateTime.Now;
            lineup.LineupSlots = vm.Slots.Select(s => new LineupSlot
            {
                FormationSlotId = s.FormationSlotId,
                PlayerId = s.PlayerId
            }).ToList();

            // Only keep bench slots that actually have a player picked, numbered by their position in the form.
            lineup.Substitutes = vm.Substitutes
                .Select((s, i) => new { s.PlayerId, Order = i + 1 })
                .Where(x => x.PlayerId.HasValue)
                .Select(x => new LineupSubstitute { PlayerId = x.PlayerId, SubOrder = x.Order })
                .ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int selectedFormationId)
        {
            ViewBag.Formations = new SelectList(
                await _context.Formations.OrderBy(f => f.Name).ToListAsync(), "FormationId", "Name", selectedFormationId);

            // Only active players are eligible for the starting XI/bench — pass the full list so the
            // view can build "FirstName LastName (Position)" labels itself.
            ViewBag.Players = await _context.Players
                .Include(p => p.Position)
                .Where(p => p.IsActive)
                .OrderBy(p => p.FirstName)
                .ToListAsync();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoccerClubPlayerManagement.Data;
using SoccerClubPlayerManagement.Models;
using SoccerClubPlayerManagement.Services;

namespace SoccerClubPlayerManagement.Controllers
{
    // Any logged-in user (Coach or Member) can view; only Coach can modify — enforced per-action below.
    [Authorize]
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorage;

        public PlayersController(ApplicationDbContext context, BlobStorageService blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        // GET: Players?search=&positionId=&traitId=&showInactive=
        public async Task<IActionResult> Index(string? search, int? positionId, int? traitId, bool showInactive = false)
        {
            var query = _context.Players
                .Include(p => p.Position)
                .Include(p => p.PlayerTraits!)
                    .ThenInclude(pt => pt.Trait)
                .AsQueryable();

            if (!showInactive)
                query = query.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search));

            if (positionId.HasValue)
                query = query.Where(p => p.PositionId == positionId.Value);

            if (traitId.HasValue)
                query = query.Where(p => p.PlayerTraits!.Any(pt => pt.TraitId == traitId.Value));

            ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name", positionId);
            ViewBag.Traits = new SelectList(await _context.Traits.ToListAsync(), "TraitId", "Name", traitId);
            ViewBag.Search = search;
            ViewBag.ShowInactive = showInactive;

            return View(await query.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var player = await _context.Players
                .Include(p => p.Position)
                .Include(p => p.PlayerTraits!)
                    .ThenInclude(pt => pt.Trait)
                .FirstOrDefaultAsync(p => p.PlayerId == id);

            if (player == null) return NotFound();
            return View(player);
        }

        // GET: Players/Create
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name");
            return View();
        }

        // POST: Players/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Create(Player player)
        {
            ModelState.Remove(nameof(Player.Position)); // navigation property, not bound from form

            if (ModelState.IsValid)
            {
                if (player.PhotoFile != null)
                    player.PhotoUrl = await _blobStorage.UploadPlayerPhotoAsync(player.PhotoFile);

                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name", player.PositionId);
            return View(player);
        }

        // GET: Players/Edit/5
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name", player.PositionId);
            return View(player);
        }

        // POST: Players/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Edit(int id, Player player)
        {
            if (id != player.PlayerId) return NotFound();

            ModelState.Remove(nameof(Player.Position));

            if (ModelState.IsValid)
            {
                var existing = await _context.Players.FindAsync(id);
                if (existing == null) return NotFound();

                existing.FirstName = player.FirstName;
                existing.LastName = player.LastName;
                existing.DateOfBirth = player.DateOfBirth;
                existing.ContactNumber = player.ContactNumber;
                existing.Email = player.Email;
                existing.PositionId = player.PositionId;
                existing.IsAvailable = player.IsAvailable;

                if (player.PhotoFile != null)
                {
                    // Clean up the old blob before uploading the replacement, so orphaned photos don't accumulate.
                    if (!string.IsNullOrEmpty(existing.PhotoUrl))
                        await _blobStorage.DeletePlayerPhotoAsync(existing.PhotoUrl);

                    existing.PhotoUrl = await _blobStorage.UploadPlayerPhotoAsync(player.PhotoFile);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name", player.PositionId);
            return View(player);
        }

        // POST: Players/Deactivate/5
        // Client requirement: retain player info once they leave the team rather than delete it.
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = false;
            player.IsAvailable = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Players/Reactivate/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Players/RateTraits/5
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> RateTraits(int id)
        {
            var player = await _context.Players
                .Include(p => p.PlayerTraits)
                .FirstOrDefaultAsync(p => p.PlayerId == id);
            if (player == null) return NotFound();

            var allTraits = await _context.Traits
                .Where(t => t.PositionId == null || t.PositionId == player.PositionId)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var vm = new RatePlayerTraitsViewModel
            {
                PlayerId = player.PlayerId,
                PlayerName = $"{player.FirstName} {player.LastName}",
                PreferredFoot = player.PreferredFoot,
                HeightFeet = player.HeightFeet,
                HeightInches = player.HeightInches,
                Traits = allTraits.Select(t => new TraitRatingItem
                {
                    TraitId = t.TraitId,
                    TraitName = t.Name,
                    Rating = player.PlayerTraits?.FirstOrDefault(pt => pt.TraitId == t.TraitId)?.Rating
                }).ToList()
            };

            return View(vm);
        }

        // POST: Players/RateTraits/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> RateTraits(RatePlayerTraitsViewModel vm)
        {
            // Redisplay the form if height is out of range (feet 3-8, inches 0-11)
            if (!ModelState.IsValid) return View(vm);

            var player = await _context.Players.FindAsync(vm.PlayerId);
            if (player == null) return NotFound();

            player.PreferredFoot = vm.PreferredFoot;
            player.HeightFeet = vm.HeightFeet;
            player.HeightInches = vm.HeightInches;

            var existingRatings = await _context.PlayerTraits
                .Where(pt => pt.PlayerId == vm.PlayerId)
                .ToListAsync();

            foreach (var item in vm.Traits)
            {
                var existing = existingRatings.FirstOrDefault(pt => pt.TraitId == item.TraitId);

                if (item.Rating.HasValue && item.Rating.Value > 0)
                {
                    if (existing != null)
                    {
                        existing.Rating = item.Rating.Value;
                    }
                    else
                    {
                        _context.PlayerTraits.Add(new PlayerTrait
                        {
                            PlayerId = vm.PlayerId,
                            TraitId = item.TraitId,
                            Rating = item.Rating.Value
                        });
                    }
                }
                else if (existing != null)
                {
                    // Rating cleared to blank/0 — remove the row rather than storing a meaningless rating.
                    _context.PlayerTraits.Remove(existing);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = vm.PlayerId });
        }
    }
}

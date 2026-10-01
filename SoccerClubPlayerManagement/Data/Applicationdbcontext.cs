using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoccerClubPlayerManagement.Models;

namespace SoccerClubPlayerManagement.Data
{
    // IdentityDbContext<ApplicationUser> adds AspNetUsers, AspNetRoles, etc. alongside our own tables.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Trait> Traits { get; set; } = null!;
        public DbSet<PlayerTrait> PlayerTraits { get; set; } = null!;
        public DbSet<Formation> Formations { get; set; } = null!;
        public DbSet<FormationSlot> FormationSlots { get; set; } = null!;
        public DbSet<Lineup> Lineups { get; set; } = null!;
        public DbSet<LineupSlot> LineupSlots { get; set; } = null!;
        public DbSet<LineupSubstitute> LineupSubstitutes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player -> Position (many-to-one)
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Position)
                .WithMany(pos => pos.Players)
                .HasForeignKey(p => p.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlayerTrait join table (Player <-> Trait many-to-many with Rating payload)
            modelBuilder.Entity<PlayerTrait>()
                .HasOne(pt => pt.Player)
                .WithMany(p => p.PlayerTraits)
                .HasForeignKey(pt => pt.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerTrait>()
                .HasOne(pt => pt.Trait)
                .WithMany(t => t.PlayerTraits)
                .HasForeignKey(pt => pt.TraitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trait -> Position (optional): null means the trait applies to every position
            modelBuilder.Entity<Trait>()
                .HasOne(t => t.Position)
                .WithMany()
                .HasForeignKey(t => t.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate trait assignment per player
            modelBuilder.Entity<PlayerTrait>()
                .HasIndex(pt => new { pt.PlayerId, pt.TraitId })
                .IsUnique();

            // Formation -> FormationSlot (one-to-many)
            modelBuilder.Entity<FormationSlot>()
                .HasOne(fs => fs.Formation)
                .WithMany(f => f.Slots)
                .HasForeignKey(fs => fs.FormationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lineup -> Formation (many lineups could reference a formation, restrict delete)
            modelBuilder.Entity<Lineup>()
                .HasOne(l => l.Formation)
                .WithMany()
                .HasForeignKey(l => l.FormationId)
                .OnDelete(DeleteBehavior.Restrict);

            // LineupSlot -> Lineup (cascade: deleting a lineup removes its slot assignments)
            modelBuilder.Entity<LineupSlot>()
                .HasOne(ls => ls.Lineup)
                .WithMany(l => l.LineupSlots)
                .HasForeignKey(ls => ls.LineupId)
                .OnDelete(DeleteBehavior.Cascade);

            // LineupSlot -> FormationSlot (restrict: a formation slot template shouldn't vanish mid-use)
            modelBuilder.Entity<LineupSlot>()
                .HasOne(ls => ls.FormationSlot)
                .WithMany()
                .HasForeignKey(ls => ls.FormationSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // LineupSlot -> Player (set null on delete: a deactivated/removed player just leaves the slot empty)
            modelBuilder.Entity<LineupSlot>()
                .HasOne(ls => ls.Player)
                .WithMany()
                .HasForeignKey(ls => ls.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Lineup -> Captain (optional; set null if the captain's player row is ever removed)
            modelBuilder.Entity<Lineup>()
                .HasOne(l => l.Captain)
                .WithMany()
                .HasForeignKey(l => l.CaptainPlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // LineupSubstitute -> Lineup (cascade: deleting a lineup removes its bench too)
            modelBuilder.Entity<LineupSubstitute>()
                .HasOne(ls => ls.Lineup)
                .WithMany(l => l.Substitutes)
                .HasForeignKey(ls => ls.LineupId)
                .OnDelete(DeleteBehavior.Cascade);

            // LineupSubstitute -> Player (set null: a removed player just leaves the bench slot empty)
            modelBuilder.Entity<LineupSubstitute>()
                .HasOne(ls => ls.Player)
                .WithMany()
                .HasForeignKey(ls => ls.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed common positions so the dropdown works out of the box
            modelBuilder.Entity<Position>().HasData(
                new Position { PositionId = 1, Name = "Goalkeeper" },
                new Position { PositionId = 2, Name = "Defender" },
                new Position { PositionId = 3, Name = "Midfielder" },
                new Position { PositionId = 4, Name = "Forward" }
            );

            // Seed common traits so coaches can rate players immediately
            modelBuilder.Entity<Trait>().HasData(
                new Trait { TraitId = 1, Name = "Pace" },
                new Trait { TraitId = 2, Name = "Stamina" },
                new Trait { TraitId = 3, Name = "Leadership" },
                new Trait { TraitId = 4, Name = "Finishing" },
                new Trait { TraitId = 5, Name = "Passing" },
                new Trait { TraitId = 6, Name = "Tackling" },
                new Trait { TraitId = 7, Name = "Marking" },
                new Trait { TraitId = 8, Name = "Heading" },
                new Trait { TraitId = 9, Name = "Agility" },
                new Trait { TraitId = 10, Name = "Vision" },
                new Trait { TraitId = 11, Name = "Skill Moves" },
                new Trait { TraitId = 16, Name = "Ball Control" },
                new Trait { TraitId = 12, Name = "Diving", PositionId = 1 },       // Goalkeeper only
                new Trait { TraitId = 13, Name = "Reflexes", PositionId = 1 },     // Goalkeeper only
                new Trait { TraitId = 14, Name = "Handling", PositionId = 1 },     // Goalkeeper only
                new Trait { TraitId = 15, Name = "Distribution", PositionId = 1 }  // Goalkeeper only
            );

            // Seed 3 common formations, each with 11 named/positioned slots for the pitch diagram
            modelBuilder.Entity<Formation>().HasData(
                new Formation { FormationId = 1, Name = "4-4-2" },
                new Formation { FormationId = 2, Name = "4-3-3" },
                new Formation { FormationId = 3, Name = "3-5-2" },
                new Formation { FormationId = 4, Name = "4-2-3-1" }
            );

            modelBuilder.Entity<FormationSlot>().HasData(
                // 4-4-2 (FormationId 1)
                new FormationSlot { FormationSlotId = 1, FormationId = 1, Label = "GK", SlotOrder = 1, X = 50, Y = 95 },
                new FormationSlot { FormationSlotId = 2, FormationId = 1, Label = "LB", SlotOrder = 2, X = 15, Y = 75 },
                new FormationSlot { FormationSlotId = 3, FormationId = 1, Label = "CB", SlotOrder = 3, X = 35, Y = 80 },
                new FormationSlot { FormationSlotId = 4, FormationId = 1, Label = "CB", SlotOrder = 4, X = 65, Y = 80 },
                new FormationSlot { FormationSlotId = 5, FormationId = 1, Label = "RB", SlotOrder = 5, X = 85, Y = 75 },
                new FormationSlot { FormationSlotId = 6, FormationId = 1, Label = "LM", SlotOrder = 6, X = 15, Y = 45 },
                new FormationSlot { FormationSlotId = 7, FormationId = 1, Label = "CM", SlotOrder = 7, X = 35, Y = 50 },
                new FormationSlot { FormationSlotId = 8, FormationId = 1, Label = "CM", SlotOrder = 8, X = 65, Y = 50 },
                new FormationSlot { FormationSlotId = 9, FormationId = 1, Label = "RM", SlotOrder = 9, X = 85, Y = 45 },
                new FormationSlot { FormationSlotId = 10, FormationId = 1, Label = "ST", SlotOrder = 10, X = 35, Y = 15 },
                new FormationSlot { FormationSlotId = 11, FormationId = 1, Label = "ST", SlotOrder = 11, X = 65, Y = 15 },

                // 4-3-3 (FormationId 2)
                new FormationSlot { FormationSlotId = 12, FormationId = 2, Label = "GK", SlotOrder = 1, X = 50, Y = 95 },
                new FormationSlot { FormationSlotId = 13, FormationId = 2, Label = "LB", SlotOrder = 2, X = 15, Y = 75 },
                new FormationSlot { FormationSlotId = 14, FormationId = 2, Label = "CB", SlotOrder = 3, X = 35, Y = 80 },
                new FormationSlot { FormationSlotId = 15, FormationId = 2, Label = "CB", SlotOrder = 4, X = 65, Y = 80 },
                new FormationSlot { FormationSlotId = 16, FormationId = 2, Label = "RB", SlotOrder = 5, X = 85, Y = 75 },
                new FormationSlot { FormationSlotId = 17, FormationId = 2, Label = "CM", SlotOrder = 6, X = 30, Y = 50 },
                new FormationSlot { FormationSlotId = 18, FormationId = 2, Label = "CM", SlotOrder = 7, X = 50, Y = 55 },
                new FormationSlot { FormationSlotId = 19, FormationId = 2, Label = "CM", SlotOrder = 8, X = 70, Y = 50 },
                new FormationSlot { FormationSlotId = 20, FormationId = 2, Label = "LW", SlotOrder = 9, X = 15, Y = 20 },
                new FormationSlot { FormationSlotId = 21, FormationId = 2, Label = "ST", SlotOrder = 10, X = 50, Y = 10 },
                new FormationSlot { FormationSlotId = 22, FormationId = 2, Label = "RW", SlotOrder = 11, X = 85, Y = 20 },

                // 3-5-2 (FormationId 3)
                new FormationSlot { FormationSlotId = 23, FormationId = 3, Label = "GK", SlotOrder = 1, X = 50, Y = 95 },
                new FormationSlot { FormationSlotId = 24, FormationId = 3, Label = "CB", SlotOrder = 2, X = 30, Y = 80 },
                new FormationSlot { FormationSlotId = 25, FormationId = 3, Label = "CB", SlotOrder = 3, X = 50, Y = 85 },
                new FormationSlot { FormationSlotId = 26, FormationId = 3, Label = "CB", SlotOrder = 4, X = 70, Y = 80 },
                new FormationSlot { FormationSlotId = 27, FormationId = 3, Label = "LM", SlotOrder = 5, X = 10, Y = 50 },
                new FormationSlot { FormationSlotId = 28, FormationId = 3, Label = "CM", SlotOrder = 6, X = 35, Y = 55 },
                new FormationSlot { FormationSlotId = 29, FormationId = 3, Label = "CM", SlotOrder = 7, X = 50, Y = 60 },
                new FormationSlot { FormationSlotId = 30, FormationId = 3, Label = "CM", SlotOrder = 8, X = 65, Y = 55 },
                new FormationSlot { FormationSlotId = 31, FormationId = 3, Label = "RM", SlotOrder = 9, X = 90, Y = 50 },
                new FormationSlot { FormationSlotId = 32, FormationId = 3, Label = "ST", SlotOrder = 10, X = 35, Y = 15 },
                new FormationSlot { FormationSlotId = 33, FormationId = 3, Label = "ST", SlotOrder = 11, X = 65, Y = 15 },

                // 4-2-3-1 (FormationId 4)
                new FormationSlot { FormationSlotId = 34, FormationId = 4, Label = "GK", SlotOrder = 1, X = 50, Y = 95 },
                new FormationSlot { FormationSlotId = 35, FormationId = 4, Label = "LB", SlotOrder = 2, X = 15, Y = 75 },
                new FormationSlot { FormationSlotId = 36, FormationId = 4, Label = "CB", SlotOrder = 3, X = 35, Y = 80 },
                new FormationSlot { FormationSlotId = 37, FormationId = 4, Label = "CB", SlotOrder = 4, X = 65, Y = 80 },
                new FormationSlot { FormationSlotId = 38, FormationId = 4, Label = "RB", SlotOrder = 5, X = 85, Y = 75 },
                new FormationSlot { FormationSlotId = 39, FormationId = 4, Label = "CDM", SlotOrder = 6, X = 35, Y = 60 },
                new FormationSlot { FormationSlotId = 40, FormationId = 4, Label = "CDM", SlotOrder = 7, X = 65, Y = 60 },
                new FormationSlot { FormationSlotId = 41, FormationId = 4, Label = "LW", SlotOrder = 8, X = 15, Y = 35 },
                new FormationSlot { FormationSlotId = 42, FormationId = 4, Label = "CAM", SlotOrder = 9, X = 50, Y = 30 },
                new FormationSlot { FormationSlotId = 43, FormationId = 4, Label = "RW", SlotOrder = 10, X = 85, Y = 35 },
                new FormationSlot { FormationSlotId = 44, FormationId = 4, Label = "ST", SlotOrder = 11, X = 50, Y = 10 }
            );
        }
    }
}
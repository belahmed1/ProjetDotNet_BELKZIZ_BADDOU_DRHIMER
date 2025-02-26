using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gauniv.WebServer.Data
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string PayloadPath { get; set; }

        // Categories to which this game belongs.
        public List<Category> Categories { get; set; } = new List<Category>();

        // Users who have purchased this game.
        public List<User> PurchasedBy { get; set; } = new List<User>();
    }
}

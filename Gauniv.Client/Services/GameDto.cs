namespace Gauniv.WebServer.Dtos
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        // We can include this if you want the path to be part of the DTO
        public string PayloadPath { get; set; }

        // If you want to list categories by name or Id
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
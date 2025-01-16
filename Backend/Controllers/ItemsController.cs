using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Data;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private static readonly Dictionary<string, object> _cache = new();
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);

    public ItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        const string cacheKey = "all_items";
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            return Ok(cached);
        }

        var items = await _context.Items.ToListAsync();
        _cache[cacheKey] = items;
        return Ok(items);
    }

    // GET: api/Items/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return item;
    }

    // POST: api/Items
    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (item == null)
            {
                return BadRequest("Item cannot be null");
            }

            if (string.IsNullOrEmpty(item.Title))
            {
                return BadRequest("Title is required");
            }

            _context.Items.Add(item);
            Console.WriteLine($"Attempting to save item: {item.Title}");
            await _context.SaveChangesAsync();
            Console.WriteLine($"Item saved successfully with ID: {item.Id}");

            _cache.Clear(); // Invalidate cache
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            return StatusCode(500, new { 
                message = "Error creating item", 
                error = ex.Message,
                details = ex.InnerException?.Message 
            });
        }
    }

    // PUT: api/Items/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, Item item)
    {
        try
        {
            if (id != item.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingItem = await _context.Items.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Item with ID {id} not found");
            }

            // Update only the fields we want to allow updating
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            _context.Entry(existingItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _cache.Clear(); // Invalidate cache after update

            return Ok(existingItem);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating item: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { 
                message = "Error updating item", 
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }

    // DELETE: api/Items/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            _cache.Clear(); // Clear cache before delete
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting item: {ex.Message}");
            return StatusCode(500, new { 
                message = "Error deleting item", 
                error = ex.Message,
                details = ex.InnerException?.Message 
            });
        }
    }
} 
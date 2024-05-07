using System.Transactions;
using kolokwium1.Model;
using kolokwium1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BooksController: ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    
    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    [HttpGet("/{id}/authors")]
    public async Task<IActionResult> GetAnimal(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
            return NotFound($"Animal with given ID - {id} doesn't exist");

        var animal = await _booksRepository.GetBook(id);
            
        return Ok(animal);
    }
    [HttpPost]
    [Route("with-scope")]
    public async Task<IActionResult> AddAnimal(newBookWithAuthor newBookWithAuthor)
    {

        if (!await _booksRepository.DoesAuthorExist(newBookWithAuthor.IdAuthor))
            return NotFound();
        

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            

            scope.Complete();
        }

        return Created(Request.Path.Value ?? "api/animals", newBookWithAuthor);
    }
}

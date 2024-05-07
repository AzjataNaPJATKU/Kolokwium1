using kolokwium1.Model;

namespace kolokwium1.Repositories;

public interface IBooksRepository
{
    Task<bool> DoesBookExist(int id);
    Task<bool> DoesAuthorExist(int id);
    Task<BooksDTO> GetBook(int id);

    Task AddBookWithAuthor(newBookWithAuthor newBookWithAuthor);
}
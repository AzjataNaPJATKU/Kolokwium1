using System.Data.SqlClient;
using kolokwium1.Model;

namespace kolokwium1.Repositories;

public class BookRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;
    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesAuthorExist(int id)
    {
	    var query = "SELECT 1 FROM Authors WHERE PK = @ID";

	    await using SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);

	    await connection.OpenAsync();

	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }

    public async Task<BooksDTO> GetBook(int id)
    {
        var query = @"SELECT 
							books.PK AS BookID,
							books.title AS bookTitle,
							first_na,
							last_na,
						FROM books
						JOIN books_authors ON books.pk = books_author.FK_book
						JOIN authors ON authors.pk = books_authors.FK_author
						WHERE books.PK = @ID";
	    
	    await using SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);
	    
	    await connection.OpenAsync();

	    var reader = await command.ExecuteReaderAsync();

	    var bookIdOrdinal = reader.GetOrdinal("BookID");
	    var booktitleOrdinal = reader.GetOrdinal("bookTitle");
	    var firstNameOrdinal = reader.GetOrdinal("first_na");
	    var lastNameOrdinal = reader.GetOrdinal("last_na");

	    BooksDTO animalDto = new BooksDTO()
	    {
		    id = reader.GetInt32(bookIdOrdinal),
		    title = reader.GetString(booktitleOrdinal),
		    author = new authorDTO()
		    {
			    firstName = reader.GetString(firstNameOrdinal),
			    lastName = reader.GetString(lastNameOrdinal)
		    }
	    };
	    

	    if (animalDto is null) throw new Exception();
        
        return animalDto;
    }
    
    public async Task AddBookWithAuthor(newBookWithAuthor newBookWithAuthor)
    {
	    var insert = @"INSERT INTO books VALUES(@id, @title);
					   SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@id", newBookWithAuthor.id);
	    command.Parameters.AddWithValue("@title", newBookWithAuthor.title);

	    await connection.OpenAsync();

	    var transaction = await connection.BeginTransactionAsync();
	    command.Transaction = transaction as SqlTransaction;
	    
	    try
	    {
		    var id = await command.ExecuteScalarAsync();
    
		    foreach (var author in newBookWithAuthor.Authors)
		    {
			    command.Parameters.Clear();
			    command.CommandText = "INSERT INTO books_authors VALUES(@ID_book, @id_author)";
			    command.Parameters.AddWithValue("@ID_book", newBookWithAuthor.id);
			    command.Parameters.AddWithValue("@id_author", author.authorid);

			    await command.ExecuteNonQueryAsync();
		    }

		    await transaction.CommitAsync();
	    }
	    catch (Exception)
	    {
		    await transaction.RollbackAsync();
		    throw;
	    }
    }
}
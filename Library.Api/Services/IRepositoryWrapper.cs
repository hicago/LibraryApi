namespace Library.Api.Services
{
    interface IRepositoryWrapper
    {
        IBookRepository Book { get; }
        IAuthorRepository Author { get; }
    }
}

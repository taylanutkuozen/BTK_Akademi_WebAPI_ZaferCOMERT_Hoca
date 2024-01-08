namespace Entities.Exceptions
{
    public sealed class BookNotFoundException :NotFoundException
        /*sealed=kalıtılması,inherit edilmesi mümkğn olmayacak*/
    {
        public BookNotFoundException(int id) : base($"The book with id : {id} could not found")
        {
            
        }
    }
}
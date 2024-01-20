namespace Entities.Exceptions
{
    public class PriceOutOfRangeBadRequestException:BadRequestException 
    {
        public PriceOutOfRangeBadRequestException():base("Maximum price should be less than 350 and greater than 10.")
        {
            
        }
    }
}
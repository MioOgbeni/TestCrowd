namespace TestCrowd.DataAccess.Interface
{
    public interface ICountry :IEntity
    {
        string IdentificationCode { get; set; }
        string Name { get; set; }
    }
}
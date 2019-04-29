namespace TestCrowd.DataAccess.Interface
{
    public interface IApplicationRole : IEntity
    {
        string Name { get; set; }

        string RoleName { get; set; }
    }
}
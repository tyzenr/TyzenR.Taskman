using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITeamManager : IRepository<TeamEntity>
    {
        Task<IList<TeamEntity>> GetByManagerId(Guid userId);  
    }
}

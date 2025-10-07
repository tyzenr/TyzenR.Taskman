using Microsoft.EntityFrameworkCore;
using TyzenR.Account;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class TeamManager : BaseRepository<TeamEntity>, ITeamManager
    {
        private readonly EntityContext entityContext;
        private readonly AccountContext accountContext;
        private readonly IAppInfo appInfo;

        public TeamManager(
            EntityContext entityContext,
            AccountContext accountContext,
            IAppInfo appInfo) : base(entityContext)
        {
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
            this.accountContext = accountContext ?? throw new ApplicationException("Instance is null!");
            this.appInfo = appInfo ?? throw new ApplicationException("Instance is null!");
        }

        public async Task<IList<TeamEntity>> GetByManagerId(Guid userId)
        {
            var result = await Task.Run(() =>
            {
                return entityContext.Teams
                    .Where(t => t.ManagerId == userId)
                    .AsNoTracking()
                    .ToList();
            });

            foreach (var team in result)
            {
                team.Name = accountContext.Users
                    .Where(u => u.Id == team.MemberId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? string.Empty;
            }

            return result;
        }
    }
}

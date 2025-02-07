using Microsoft.EntityFrameworkCore;
using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class TaskManager : BaseRepository<TaskEntity>, ITaskManager
    {
        private EntityContext context;

        public TaskManager(EntityContext context) : base(context)
        {
            this.context = context;
        }

        public Task<(bool, TaskEntity)> CreateTaskAsync(TaskEntity task)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TaskEntity>> GetTasksByUserAsync(UserEntity user)
        {
            var result = await this.context.Tasks
                .Where(t => t.Status == TaskStatusEnum.InProgress)
                .ToListAsync();

            return result;
        }

        public Task<bool> IsValidAsync(TaskEntity task)
        {
            throw new NotImplementedException();
        }
    }
}

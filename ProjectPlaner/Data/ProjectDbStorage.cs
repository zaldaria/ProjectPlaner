using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Models.Entity;
using System.Threading.Tasks;


namespace ProjectPlaner.Data
{
    public class ProjectDbStorage
    {
        private readonly ApplicationDbContext _context;

        public ProjectDbStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE - Создание нового проекта
        public async Task<Project> CreateProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            project.projectId = Guid.NewGuid();
            _context.projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        // READ (Single) - Получение проекта по ID со всеми зависимостями
        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
        {
            return await _context.projects
                .Include(p => p.client)  //  загружаем клиента
                .Include(p => p.tasks)   //  загружаем задачи
                    .ThenInclude(t => t.marks)  // И метки задач
                .Include(p => p.tasks)
                    .ThenInclude(t => t.marker) // И маркеры задач
                .Include(p => p.tasks)
                    .ThenInclude(t => t.status)  // И статусы задач
                .FirstOrDefaultAsync(p => p.projectId == projectId);
        }

        // READ (All) - Получение всех проектов со всеми зависимостями
        public async Task<List<Project>> GetAllprojectsAsync()
        {
            return await _context.projects
                .Include(p => p.client)
                .Include(p => p.tasks)
                    .ThenInclude(t => t.marks)
                .Include(p => p.tasks)
                    .ThenInclude(t => t.marker)
                .Include(p => p.tasks)
                    .ThenInclude(t => t.status)
                .ToListAsync();
        }

        // UPDATE - Обновление проекта
        public async Task<Project> UpdateProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            //_context.Entry(project).State = EntityState.Modified;
            _context.projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        // DELETE - Удаление проекта (с каскадным удалением задач)
        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            var project = await _context.projects
                .Include(p => p.tasks)
                .FirstOrDefaultAsync(p => p.projectId == projectId);

            if (project == null)
                return false;

            _context.projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        // Дополнительные методы

        // Получение проектов по клиенту
        public async Task<List<Project>> GetprojectsByClientAsync(Guid clientId)
        {
            return await _context.projects
                .Include(p => p.client)
                .Include(p => p.tasks)
                .Where(p => p.clientId == clientId)
                .ToListAsync();
        }

        // Добавление задачи к проекту
        public async System.Threading.Tasks.Task AddTaskToProjectAsync(Guid projectId, Models.Entity.Task task)
        {
            var project = await _context.projects
                .Include(p => p.tasks)
                .FirstOrDefaultAsync(p => p.projectId == projectId);

            if (project != null)
            {
                task.projectId = projectId;
                task.taskId = Guid.NewGuid();
                project.tasks.Add(task);
                await _context.SaveChangesAsync();
            }
        }

        // Получение количества задач в проекте
        public async Task<int> GetTaskCountAsync(Guid projectId)
        {
            return await _context.tasks
                .CountAsync(t => t.projectId == projectId);
        }

        // Обновление дедлайна проекта
        public async System.Threading.Tasks.Task UpdateProjectDeadlineAsync(Guid projectId, DateTime newDeadline)
        {
            var project = await _context.projects.FindAsync(projectId);
            if (project != null)
            {
                project.deadline = newDeadline;
                await _context.SaveChangesAsync();
            }
        }
    }
}
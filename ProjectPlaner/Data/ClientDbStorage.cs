using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Models.Entity;

namespace ProjectPlaner.Data
{
    public class ClientDbStorage
    {
        private readonly ApplicationDbContext _context;

        public ClientDbStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE - Создание нового клиента
        public async Task<Client> CreateClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            client.clientId = Guid.NewGuid();
            _context.clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        // READ (Single) - Получение клиента по ID со всеми зависимостями
        public async Task<Client?> GetClientByIdAsync(Guid clientId)
        {
            return await _context.clients
                .Include(c => c.projects)          // Всегда загружаем проекты
                    .ThenInclude(p => p.tasks)     // И задачи каждого проекта
                        .ThenInclude(t => t.marks) // И метки задач
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.marker) // И маркеры задач
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.status)  // И статусы задач
                .FirstOrDefaultAsync(c => c.clientId == clientId);
        }

        // READ (All) - Получение всех клиентов со всеми зависимостями
        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.clients
                .Include(c => c.projects)          // загружаем проекты
                    .ThenInclude(p => p.tasks)     // И задачи каждого проекта
                        .ThenInclude(t => t.marks) // И метки задач
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.marker) // И маркеры задач
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.status)  // И статусы задач
                .ToListAsync();
        }

        // UPDATE - Обновление данных клиента
        public async Task<Client> UpdateClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            //_context.Entry(client).State = EntityState.Modified;
            _context.clients.Update(client);
            await _context.SaveChangesAsync();
            return client;
        }

        // DELETE - Удаление клиента
        public async Task<bool> DeleteClientAsync(Guid clientId)
        {
            var client = await _context.clients
                .Include(c => c.projects)
                .FirstOrDefaultAsync(c => c.clientId == clientId);

            if (client == null)
                return false;

            // Проверка на наличие связанных проектов
            //if (client.projects.Any())
            //    throw new InvalidOperationException("Cannot delete client with existing projects");

            _context.clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        // Дополнительные методы

        // Поиск клиента по email
        public async Task<Client?> FindClientByEmailAsync(string email)
        {
            return await _context.clients
                .FirstOrDefaultAsync(c => c.email == email);
        }

        // Поиск клиента по телефону
        public async Task<Client?> FindClientByPhoneAsync(string phone)
        {
            return await _context.clients
                .FirstOrDefaultAsync(c => c.phone == phone);
        }

        // Получение количества проектов клиента
        public async Task<int> GetProjectsCountAsync(Guid clientId)
        {
            return await _context.projects
                .CountAsync(p => p.clientId == clientId);
        }

        // Обновление контактных данных клиента
        public async System.Threading.Tasks.Task UpdateClientContactInfoAsync(Guid clientId, string phone, string email)
        {
            var client = await _context.clients.FindAsync(clientId);
            if (client == null)
                throw new KeyNotFoundException("Client not found");

            client.phone = phone;
            client.email = email;

            await _context.SaveChangesAsync();
        }
    }
}
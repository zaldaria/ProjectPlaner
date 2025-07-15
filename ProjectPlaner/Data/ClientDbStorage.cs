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

        // CREATE - �������� ������ �������
        public async Task<Client> CreateClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            client.clientId = Guid.NewGuid();
            _context.clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        // READ (Single) - ��������� ������� �� ID �� ����� �������������
        public async Task<Client?> GetClientByIdAsync(Guid clientId)
        {
            return await _context.clients
                .Include(c => c.projects)          // ������ ��������� �������
                    .ThenInclude(p => p.tasks)     // � ������ ������� �������
                        .ThenInclude(t => t.marks) // � ����� �����
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.marker) // � ������� �����
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.status)  // � ������� �����
                .FirstOrDefaultAsync(c => c.clientId == clientId);
        }

        // READ (All) - ��������� ���� �������� �� ����� �������������
        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.clients
                .Include(c => c.projects)          // ��������� �������
                    .ThenInclude(p => p.tasks)     // � ������ ������� �������
                        .ThenInclude(t => t.marks) // � ����� �����
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.marker) // � ������� �����
                .Include(c => c.projects)
                    .ThenInclude(p => p.tasks)
                        .ThenInclude(t => t.status)  // � ������� �����
                .ToListAsync();
        }

        // UPDATE - ���������� ������ �������
        public async Task<Client> UpdateClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            //_context.Entry(client).State = EntityState.Modified;
            _context.clients.Update(client);
            await _context.SaveChangesAsync();
            return client;
        }

        // DELETE - �������� �������
        public async Task<bool> DeleteClientAsync(Guid clientId)
        {
            var client = await _context.clients
                .Include(c => c.projects)
                .FirstOrDefaultAsync(c => c.clientId == clientId);

            if (client == null)
                return false;

            // �������� �� ������� ��������� ��������
            //if (client.projects.Any())
            //    throw new InvalidOperationException("Cannot delete client with existing projects");

            _context.clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        // �������������� ������

        // ����� ������� �� email
        public async Task<Client?> FindClientByEmailAsync(string email)
        {
            return await _context.clients
                .FirstOrDefaultAsync(c => c.email == email);
        }

        // ����� ������� �� ��������
        public async Task<Client?> FindClientByPhoneAsync(string phone)
        {
            return await _context.clients
                .FirstOrDefaultAsync(c => c.phone == phone);
        }

        // ��������� ���������� �������� �������
        public async Task<int> GetProjectsCountAsync(Guid clientId)
        {
            return await _context.projects
                .CountAsync(p => p.clientId == clientId);
        }

        // ���������� ���������� ������ �������
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
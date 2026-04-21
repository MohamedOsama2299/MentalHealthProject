using CBTClinic.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin> GetByIdAsync(int id);
        Task UpdateAsync(Admin admin);
        Task<int> CountAsync(); 
        Task<List<Admin>> GetAllAsync(); 
    }
}
